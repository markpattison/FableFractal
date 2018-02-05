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
        CanvasHeight = 1.0
        Zoom = 0.314
        OffsetX = -0.5
        OffsetY = 0.0
        Now = System.DateTime.Now
        Render = None
        Scrolling = false
        LastScreenX = 0.0
        LastScreenY = 0.0
    }, renderCommand

let update msg model =
    match msg with
    | WheelMsg we -> { model with Zoom = model.Zoom * 0.99 ** we.deltaY }, []
    | MouseDownMsg me ->
        { model with
            Scrolling = true
            LastScreenX = me.screenX
            LastScreenY = me.screenY
        }, []
    | MouseUpMsg _ | MouseLeaveMsg _ -> { model with Scrolling = false }, []
    | MouseMoveMsg me ->
        if model.Scrolling then
            { model with
                OffsetX = model.OffsetX - (me.screenX - model.LastScreenX) / (model.Zoom * model.CanvasHeight)
                OffsetY = model.OffsetY + (me.screenY - model.LastScreenY) / (model.Zoom * model.CanvasHeight)
                LastScreenX = me.screenX
                LastScreenY = me.screenY
            }, []
        else
            model, []
    | RenderMsg ->
        match model.Render with
        | None ->
            let holder = Browser.document.getElementById("Fractal")
            match holder with
            | null -> model, renderCommand
            | h ->
                let renderer, height = FractalRenderer.create h
                { model with Render = Some renderer; CanvasHeight = float height }, renderCommand
        | Some render ->
            render model
            { model with Now = System.DateTime.Now }, renderCommand
