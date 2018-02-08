module App.State

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Types

type INormalizedWheel =
    abstract member pixelX: float
    abstract member pixelY: float
    abstract member spinX: float
    abstract member spinY: float

let normalizeWheel : React.WheelEvent -> INormalizedWheel = importDefault "normalize-wheel"

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
        Transform = NoTransform
    }, renderCommand

let update msg model =
    match msg with
    | WheelMsg we ->
        let zoom = (normalizeWheel we).pixelY / 100.0
        { model with Zoom = model.Zoom * 0.99 ** zoom }, []
    | MouseDownMsg me ->
        { model with
            Transform = Scrolling (me.screenX, me.screenY)
        }, []
    | MouseUpMsg _ | MouseLeaveMsg _ | TouchEndMsg _ -> { model with Transform = NoTransform }, []
    | MouseMoveMsg me ->
        match model.Transform with
        | Scrolling (lastScreenX, lastScreenY) ->
            { model with
                OffsetX = model.OffsetX - (me.screenX - lastScreenX) / (model.Zoom * model.CanvasHeight)
                OffsetY = model.OffsetY + (me.screenY - lastScreenY) / (model.Zoom * model.CanvasHeight)
                Transform = Scrolling (me.screenX, me.screenY)
            }, []
        | _ -> model, []
    | TouchStartMsg te ->
        match te.touches.length with
        | 1.0 ->
            { model with
                Transform = Scrolling (te.touches.[0.0].clientX, te.touches.[0.0].clientY)
            }, []
        | 2.0 ->
            let dx = te.touches.[1.0].clientX - te.touches.[0.0].clientX
            let dy = te.touches.[1.0].clientY - te.touches.[0.0].clientY
            let distance = sqrt (dx * dx + dy * dy)
            { model with
                Transform = Pinching distance
            }, []
        | _ -> model, []
    | TouchMoveMsg te ->
        match model.Transform, te.touches.length with
        | Scrolling (lastScreenX, lastScreenY), 1.0 ->
            { model with
                OffsetX = model.OffsetX - (te.touches.[0.0].screenX - lastScreenX) / (model.Zoom * model.CanvasHeight)
                OffsetY = model.OffsetY + (te.touches.[0.0].screenY - lastScreenY) / (model.Zoom * model.CanvasHeight)
                Transform = Scrolling (te.touches.[0.0].screenX, te.touches.[0.0].screenY)
            }, []
        | Pinching lastDistance, 2.0 ->
            let dx = te.touches.[1.0].clientX - te.touches.[0.0].clientX
            let dy = te.touches.[1.0].clientY - te.touches.[0.0].clientY
            let distance = sqrt (dx * dx + dy * dy)
            { model with
                Zoom = model.Zoom * 0.99 ** (lastDistance - distance)
                Transform = Pinching distance
            }, []
        | _ -> model, []
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
