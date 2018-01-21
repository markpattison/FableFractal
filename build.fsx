// include Fake libs
#r "packages/FAKE/tools/FakeLib.dll"

open System

open Fake
open Fake.FileUtils
open Fake.NpmHelper

// Filesets
let fableDirectory = "src"
let fableReference = !! (fableDirectory + "/*.fsproj") |> Seq.exactlyOne

let dotnetcliVersion = "2.1.3"
let mutable dotnetExePath = "dotnet"

// Targets
Target "InstallDotNetCore" (fun _ ->
    dotnetExePath <- DotNetCli.InstallDotNetSDK dotnetcliVersion
)

Target "Restore" (fun _ ->
    [ fableReference ]
    |> Seq.iter (fun proj -> DotNetCli.Restore (fun p -> { p with Project = proj; ToolPath = dotnetExePath }))
)

Target "NpmInstall" (fun _ ->
    Npm (fun p ->
        { p with Command = Install Standard; WorkingDirectory = fableDirectory })
)

Target "BuildFable" (fun _ ->
    fableReference
    |> (fun proj -> DotNetCli.RunCommand (fun p -> { p with WorkingDir = fableDirectory; ToolPath = dotnetExePath }) ("fable npm-build " + proj))
)

Target "RunFable" (fun _ ->
    fableReference
    |> (fun proj -> DotNetCli.RunCommand (fun p -> { p with WorkingDir = fableDirectory; ToolPath = dotnetExePath }) ("fable npm-start " + proj))
)

// Build order
"InstallDotNetCore"
    ==> "Restore"
    ==> "NpmInstall"

"NpmInstall" ==> "BuildFable"
"NpmInstall" ==> "RunFable"

// start build
RunTargetOrDefault "BuildFable"
