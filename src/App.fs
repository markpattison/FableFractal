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
            OnWheel (fun e -> (WheelMsg e) |> dispatch)
            OnMouseDown (fun e -> (MouseDownMsg e) |> dispatch)
            OnMouseUp (fun e -> (MouseUpMsg e) |> dispatch)
            OnMouseMove (fun e -> (MouseMoveMsg e) |> dispatch)
            OnMouseLeave (fun e -> (MouseLeaveMsg e) |> dispatch)
            OnTouchStart (fun e -> (TouchStartMsg e) |> dispatch)
            OnTouchMove (fun e -> (TouchMoveMsg e) |> dispatch)
            OnTouchEnd (fun e -> (TouchEndMsg e) |> dispatch)
            OnTouchCancel (fun e -> (TouchEndMsg e) |> dispatch)
        ] []
    ]

open Elmish.React
open Elmish.Debug
open Elmish.HMR

Program.mkProgram init update root

#if DEBUG
//|> Program.withDebugger
|> Program.withHMR
#endif
|> Program.withReact "FableFractal"
|> Program.run


