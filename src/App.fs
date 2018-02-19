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
    match model.FractalType with
    | Julia ->
        [
            p [] [ sprintf "X = %.6f" model.JuliaX |> str ]
            p [] [ sprintf "Y = %.6f" model.JuliaY |> str ]
            p [] [ sprintf "Seed X = %.6f" model.MandelbrotX |> str ]
            p [] [ sprintf "Seed Y = %.6f" model.MandelbrotY |> str ]
        ]
    | Mandelbrot ->
        [
            p [] [ sprintf "X = %.6f" model.MandelbrotX |> str ]
            p [] [ sprintf "Y = %.6f" model.MandelbrotY |> str ]
        ]

let showButtons model dispatch =
    div [] [
        div [ ClassName "field has-addons" ] [
            button [
                (match model.FractalType with
                    | Mandelbrot -> ClassName "button is-primary is-selected"
                    | Julia -> ClassName "button")
                OnClick (fun _ -> MandelbrotClick |> dispatch)
            ] [ str "Mandelbrot" ]
            button [
                (match model.FractalType with
                    | Mandelbrot -> ClassName "button"
                    | Julia -> ClassName "button is-primary is-selected")
                OnClick (fun _ -> JuliaClick |> dispatch)
            ] [ str "Julia" ]
        ]
        div [] [
            if model.FractalType = Julia then
                yield! [
                    button [
                        (match model.FractalType, model.JuliaScrolling with
                            | Julia, Move -> ClassName "button is-primary is-selected"
                            | Julia, ChangeSeed -> ClassName "button"
                            | Mandelbrot, _ -> ClassName "button is-disabled")
                        OnClick (fun _ -> JuliaMoveClick |> dispatch)
                        ] [ str "Move" ]
                    button [
                        (match model.FractalType, model.JuliaScrolling with
                            | Julia, Move -> ClassName "button"
                            | Julia, ChangeSeed -> ClassName "button is-primary is-selected"
                            | Mandelbrot, _ -> ClassName "button is-disabled")
                        OnClick (fun _ -> JuliaChangeSeedClick |> dispatch)
                ] [ str "ChangeSeed" ]
            ]
        ]
    ]

let hud model dispatch =
    div [ ClassName "columns" ] [
        div [ ClassName "column" ] (showParams model)
        div [ ClassName "column" ] [ showButtons model dispatch ]
    ]

let fractalCanvas dispatch =
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
    ] []

let root model dispatch =
    div [] [
        hud model dispatch
        fractalCanvas dispatch
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


