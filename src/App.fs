module App.View

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open App.Types
open App.State

importAll "../sass/main.sass"

open Fable.Helpers.React
open Fable.Helpers.React.Props

let root model dispatch =

  div
    []
    [ model.Now.ToString() |> str ]


open Elmish.React
open Elmish.Debug
open Elmish.HMR

let renderer = FractalRenderer.create (Browser.document.getElementById("Fractal"))

Program.mkProgram init (update renderer) root
#if DEBUG
//|> Program.withDebugger
|> Program.withHMR
#endif
|> Program.withReact "Hud"
|> Program.run


