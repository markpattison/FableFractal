#r "paket: groupref Build //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.IO.Globbing.Operators
open Fake.DotNet

// Filesets

let fableDirectory = "src"
let fableReference = !! (fableDirectory + "/*.fsproj") |> Seq.exactlyOne

let dotnetcliVersion = "2.1.300"

let install = lazy DotNet.install (fun p ->
    { p with Version = DotNet.CliVersion.Version dotnetcliVersion })

let inline dotnet arg = DotNet.Options.lift install.Value arg

let inline withWorkDir wd =
    DotNet.Options.lift install.Value
    >> DotNet.Options.withWorkingDirectory wd

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
Target.create "BuildFable" (fun _ ->
    [ fableReference ]
    |> Seq.iter (fun proj ->
        DotNet.exec (withWorkDir fableDirectory) "fable npm-build" proj |> ignore))

Target.description "Building Fable for local run"
Target.create "RunFable" (fun _ ->
    [ fableReference ]
    |> Seq.iter (fun proj ->
        DotNet.exec (withWorkDir fableDirectory) "fable npm-start" proj |> ignore))

// Build order

open Fake.Core.TargetOperators

"Clean"
    ==> "Restore"
    ==> "NpmInstall"

"NpmInstall" ==> "BuildFable"
"NpmInstall" ==> "RunFable"

// Start build

Target.runOrDefault "BuildFable"
