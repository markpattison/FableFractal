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

let showParams model =
        if model.FractalType = Julia then [
            p [] [ sprintf "X = %.6f" model.JuliaX |> str ]
            p [] [ sprintf "Y = %.6f" model.JuliaY |> str ]
            p [] [ sprintf "Seed X = %.6f" model.MandelbrotX |> str ]
            p [] [ sprintf "Seed Y = %.6f" model.MandelbrotY |> str ]
            ]
        else [
            p [] [ sprintf "X = %.6f" model.MandelbrotX |> str ]
            p [] [ sprintf "Y = %.6f" model.MandelbrotY |> str ]
            ]

let root model dispatch =
    div [] [
        div [] (showParams model)
        button [
            (if model.FractalType = Mandelbrot then ClassName "button is-active" else ClassName "button")
            OnClick (fun _ -> MandelbrotClick |> dispatch)
        ] [ str "Mandelbrot" ]
        button [
            ClassName "button"
            (if model.FractalType = Julia then ClassName "button is-active" else ClassName "button")
            OnClick (fun _ -> JuliaClick |> dispatch)
        ] [ str "Julia" ]
        div [
            Id "Fractal"
            OnWheel (fun e -> (WheelMsg e) |> dispatch; e.preventDefault())
            OnMouseDown (fun e -> (MouseDownMsg e) |> dispatch; e.preventDefault())
            OnMouseUp (fun e -> (MouseUpMsg e) |> dispatch; e.preventDefault())
            OnMouseMove (fun e -> (MouseMoveMsg e) |> dispatch; e.preventDefault())
            OnMouseLeave (fun e -> (MouseLeaveMsg e) |> dispatch; e.preventDefault())
            OnTouchStart (fun e -> (TouchStartMsg e) |> dispatch; e.preventDefault())
            OnTouchMove (fun e -> (TouchMoveMsg e) |> dispatch; e.preventDefault())
            OnTouchEnd (fun e -> (TouchEndMsg e) |> dispatch; e.preventDefault())
            OnTouchCancel (fun e -> (TouchEndMsg e) |> dispatch; e.preventDefault())
            OnContextMenu (fun e -> e.preventDefault())
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


