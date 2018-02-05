module App.View

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open App.Types
open App.State

importAll "./sass/main.sass"

open Fable.Helpers.React
open Fable.Helpers.React.Props

let root model dispatch =
    div [] [
        p [] [ sprintf "X = %.6f" model.OffsetX |> str ]
        p [] [ sprintf "Y = %.6f" model.OffsetY |> str ]
        p [] [ sprintf "Zoom = %.6f" model.Zoom |> str ]
        div [
            Id "Fractal"
            OnWheel (fun we -> (WheelMsg we) |> dispatch)
            OnMouseDown (fun me -> (MouseDownMsg me) |> dispatch)
            OnMouseUp (fun me -> (MouseUpMsg me) |> dispatch)
            OnMouseMove (fun me -> (MouseMoveMsg me) |> dispatch)
            OnMouseLeave (fun me -> (MouseLeaveMsg me) |> dispatch)
        ] []
    ]

open Elmish.React
open Elmish.Debug
open Elmish.HMR

//let renderer = FractalRenderer.create (Browser.document.getElementById("Fractal"))

Program.mkProgram init update root

#if DEBUG
//|> Program.withDebugger
|> Program.withHMR
#endif
|> Program.withReact "FableFractal"
|> Program.run


