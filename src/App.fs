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

let iconButton iconName action dispatch =
    let x = span [ ClassName "icon" ] [ i [ ClassName iconName ] [] ]
    let y = [  ]
    div []
        [
            a
                [ ClassName "button"; OnClick (fun _ -> action |> dispatch) ]
                [ span [ ClassName "icon" ] [ i [ ClassName iconName ] [] ] ]
        ]

let controls dispatch =
    div []
        [
            table []
                [
                    tbody []
                        [
                            tr []
                                [
                                    td [] []
                                    td [] [ iconButton "fa fa-arrow-circle-up" UpMsg dispatch ]
                                    td [] []
                                    td [] [ iconButton "fa fa-search-plus" ZoomInMsg dispatch ]
                                ]
                            tr []
                                [
                                    td [] [ iconButton "fa fa-arrow-circle-left" LeftMsg dispatch ]
                                    td [] []
                                    td [] [ iconButton "fa fa-arrow-circle-right" RightMsg dispatch ]
                                ]
                            tr []
                                [
                                    td [] []
                                    td [] [ iconButton "fa fa-arrow-circle-down" DownMsg dispatch ]
                                    td [] []
                                    td [] [ iconButton "fa fa-search-minus" ZoomOutMsg dispatch ]
                                ]
                        ]
                ]
        ]

let root model dispatch =
    div []
        [
            model.Now.ToString() |> str
            controls dispatch
        ]

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


