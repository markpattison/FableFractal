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

open Elmish.HMR

let showParams model =
    match model.FractalType with
    | Julia (seed, _) ->
        [
            p [] [ sprintf "X = %.6f" model.X |> str ]
            p [] [ sprintf "Y = %.6f" model.Y |> str ]
            p [] [ sprintf "Zoom = %.6f" model.Zoom |> str ]
            p [] [ sprintf "Seed X = %.6f" seed.SeedX |> str ]
            p [] [ sprintf "Seed Y = %.6f" seed.SeedY |> str ]
        ]
    | Mandelbrot ->
        [
            p [] [ sprintf "X = %.6f" model.X |> str ]
            p [] [ sprintf "Y = %.6f" model.Y |> str ]
            p [] [ sprintf "Zoom = %.6f" model.Zoom |> str ]
        ]

let showButtons model dispatch =
    div [] [
        div [ ClassName "field has-addons" ] [
            button [
                (match model.FractalType with
                    | Mandelbrot -> ClassName "button is-primary is-selected"
                    | Julia _ -> ClassName "button")
                OnClick (fun _ -> MandelbrotClick |> dispatch)
            ] [ str "Mandelbrot" ]
            button [
                (match model.FractalType with
                    | Mandelbrot -> ClassName "button"
                    | Julia _ -> ClassName "button is-primary is-selected")
                OnClick (fun _ -> JuliaClick |> dispatch)
            ] [ str "Julia" ]
        ]
        div [] [
            match model.FractalType with
            | Julia (_, scrollType) ->
                yield button [
                    (match scrollType with
                        | Move -> ClassName "button is-primary is-selected"
                        | ChangeSeed -> ClassName "button")
                    OnClick (fun _ -> JuliaMoveClick |> dispatch)
                ] [ str "Move" ]
                yield button [
                    (match scrollType with
                        | Move -> ClassName "button"
                        | ChangeSeed -> ClassName "button is-primary is-selected")
                    OnClick (fun _ -> JuliaChangeSeedClick |> dispatch)
                ] [ str "ChangeSeed" ]
            | _ -> ()
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
        OnWheel (fun e -> e.preventDefault(); (WheelMsg e) |> dispatch)
        OnMouseDown (fun e -> e.preventDefault(); (MouseDownMsg e) |> dispatch)
        OnMouseUp (fun e -> e.preventDefault(); (MouseUpMsg e) |> dispatch)
        OnMouseMove (fun e -> e.preventDefault(); (MouseMoveMsg e) |> dispatch)
        OnMouseLeave (fun e -> e.preventDefault(); (MouseLeaveMsg e) |> dispatch)
        OnTouchStart (fun e -> e.preventDefault(); (TouchStartMsg e) |> dispatch)
        OnTouchMove (fun e -> e.preventDefault(); (TouchMoveMsg e) |> dispatch)
        OnTouchEnd (fun e -> e.preventDefault(); (TouchEndMsg e) |> dispatch)
        OnTouchCancel (fun e -> e.preventDefault(); (TouchEndMsg e) |> dispatch)
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
#endif
|> Program.withReact "FableFractal"
|> Program.run


