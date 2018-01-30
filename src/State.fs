module App.State

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Fable.Import
open Types

let renderCommand =
    let sub dispatch =
        let f = Browser.FrameRequestCallback (fun _ -> dispatch RenderMsg)
        Browser.window.requestAnimationFrame(f) |> ignore
    Cmd.ofSub sub

let init result =
    {
        Zoom = 0.314
        OffsetX = -0.5
        OffsetY = 0.0
        Now = System.DateTime.Now
    }, renderCommand

let update renderer msg model =
    match msg with
    | Msg.UpMsg -> { model with OffsetY = model.OffsetY + 0.1 / model.Zoom }, []
    | DownMsg -> { model with OffsetY = model.OffsetY - 0.1 / model.Zoom }, []
    | LeftMsg -> { model with OffsetX = model.OffsetX - 0.1 / model.Zoom }, []
    | RightMsg -> { model with OffsetX = model.OffsetX + 0.1 / model.Zoom }, []
    | ZoomInMsg -> { model with Zoom = model.Zoom * 1.1 }, []
    | ZoomOutMsg -> { model with Zoom = model.Zoom / 1.1 }, []
    | RenderMsg ->
        renderer model
        { model with Now = System.DateTime.Now }, renderCommand
