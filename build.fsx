#r "paket: groupref Build //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.IO.Globbing.Operators
open Fake.DotNet

// Filesets

let fableDirectory = "src"
let fableReference = !! (fableDirectory + "/*.fsproj") |> Seq.exactlyOne

let dotnetcliVersion = "2.2.203"

let install = lazy DotNet.install (fun p ->
    { p with Version = DotNet.CliVersion.Version dotnetcliVersion })

let inline dotnet arg = DotNet.Options.lift install.Value arg

let inline withWorkDir wd =
    DotNet.Options.lift install.Value
    >> DotNet.Options.withWorkingDirectory wd

let npxTool =
    match ProcessUtils.tryFindFileOnPath "npx.cmd" with
    | Some t -> t
    | None -> failwith "npx not found"

let runTool cmd args workingDir =
    let arguments = args |> String.split ' ' |> Arguments.OfArgs
    Command.RawCommand (cmd, arguments)
    |> CreateProcess.fromCommand
    |> CreateProcess.withWorkingDirectory workingDir
    |> CreateProcess.ensureExitCode
    |> Proc.run
    |> ignore

// Targets

Target.description "Cleaning directories"
Target.create "Clean" (fun _ ->
    [ fableReference ]
    |> Seq.iter (fun proj ->
        let result = DotNet.exec dotnet "clean" proj
        if not result.OK then failwithf "dotnet clean failed with code %i" result.ExitCode))

Target.description "Restoring .NET dependencies"
Target.create "Restore" (fun _ ->
    [ fableReference ]
    |> Seq.iter (fun proj -> DotNet.restore dotnet proj))

Target.description "Restoring Node dependencies"
Target.create "NpmInstall" (fun _ ->
    Fake.JavaScript.Npm.install (fun p -> { p with WorkingDirectory = fableDirectory }))

Target.description "Building Fable for production"
Target.create "Build" (fun _ ->
    runTool npxTool "webpack-cli --config webpack.config.js -p" fableDirectory)

Target.description "Building Fable for local run"
Target.create "Run" (fun _ ->
    runTool npxTool "webpack-dev-server --config webpack.config.js" fableDirectory)

// Build order

open Fake.Core.TargetOperators

"Clean"
    ==> "Restore"
    ==> "NpmInstall"

"NpmInstall" ==> "Build"
"NpmInstall" ==> "Run"

// Start build

Target.runOrDefault "Build"
