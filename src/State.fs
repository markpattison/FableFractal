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
        FractalType = Mandelbrot
        JuliaScrolling = Move
        MandelbrotX = -0.5
        MandelbrotY = 0.0
        JuliaX = 0.0
        JuliaY = 0.0
        Now = System.DateTime.Now
        Render = None
        Transform = NoTransform
    }, renderCommand

let update msg model =
    match msg with
    | MandelbrotClick ->
        { model with FractalType = Mandelbrot }, []
    | JuliaClick ->
        { model with FractalType = Julia }, []
    | JuliaMoveClick ->
        { model with JuliaScrolling = Move }, []
    | JuliaChangeSeedClick ->
        { model with JuliaScrolling = ChangeSeed }, []
    | WheelMsg we ->
        let zoom = (normalizeWheel we).pixelY / 100.0
        { model with Zoom = model.Zoom * 0.99 ** zoom }, []
    | MouseDownMsg me when me.button = 0.0 ->
        { model with
            Transform = Scrolling (me.screenX, me.screenY)
        }, []
    | MouseUpMsg me when me.button = 0.0 -> { model with Transform = NoTransform }, []
    | MouseDownMsg _ | MouseUpMsg _ -> model, []
    | MouseLeaveMsg _ | TouchEndMsg _ -> { model with Transform = NoTransform }, []
    | MouseMoveMsg me ->
        match model.Transform, model.FractalType, model.JuliaScrolling with
        | Scrolling (lastScreenX, lastScreenY), Julia, Move ->
            { model with
                JuliaX = model.JuliaX - (me.screenX - lastScreenX) / (model.Zoom * model.CanvasHeight)
                JuliaY = model.JuliaY + (me.screenY - lastScreenY) / (model.Zoom * model.CanvasHeight)
                Transform = Scrolling (me.screenX, me.screenY)
            }, []
        | Scrolling (lastScreenX, lastScreenY), Julia, ChangeSeed
        | Scrolling (lastScreenX, lastScreenY), Mandelbrot, _ ->
            { model with
                MandelbrotX = model.MandelbrotX - (me.screenX - lastScreenX) / (model.Zoom * model.CanvasHeight)
                MandelbrotY = model.MandelbrotY + (me.screenY - lastScreenY) / (model.Zoom * model.CanvasHeight)
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
                MandelbrotX = model.MandelbrotX - (te.touches.[0.0].screenX - lastScreenX) / (model.Zoom * model.CanvasHeight)
                MandelbrotY = model.MandelbrotY + (te.touches.[0.0].screenY - lastScreenY) / (model.Zoom * model.CanvasHeight)
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
