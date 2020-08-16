module App.State

open Browser
open Browser.Types
open Elmish
open Fable.Core.JsInterop
open Types

type INormalizedWheel =
    abstract member pixelX: float
    abstract member pixelY: float
    abstract member spinX: float
    abstract member spinY: float

let normalizeWheel : WheelEvent -> INormalizedWheel = importDefault "normalize-wheel"

let renderCommand =
    let sub dispatch =
        window.requestAnimationFrame(fun _ -> dispatch RenderMsg) |> ignore
    Cmd.ofSub sub

let initMandelbrot =
    {
        CanvasHeight = 1.0
        Zoom = 0.314
        FractalType = Mandelbrot
        X = -0.5
        Y = 0.0
        Now = System.DateTime.Now
        Render = None
        Transform = NoTransform
    }

let initJulia =
    {
        CanvasHeight = 1.0
        Zoom = 0.314
        FractalType = Julia ({ SeedX = 0.0; SeedY = 0.0 }, ChangeSeed)
        X = 0.0
        Y = 0.0
        Now = System.DateTime.Now
        Render = None
        Transform = NoTransform
    }

let init result =
    document.addEventListener("gesturestart", (fun e -> e.preventDefault()), true)
    document.addEventListener("gesturechange", (fun e -> e.preventDefault()), true)
    document.addEventListener("gestureend", (fun e -> e.preventDefault()), true)
    document.addEventListener("scroll", (fun e -> e.preventDefault()), true)
    initMandelbrot, renderCommand

let updateForMove x y model =
    match model.Transform with
    | Scrolling (lastScreenX, lastScreenY) ->
        { model with
            X = model.X - (x - lastScreenX) / (model.Zoom * model.CanvasHeight)
            Y = model.Y + (y - lastScreenY) / (model.Zoom * model.CanvasHeight)
            Transform = Scrolling (x, y)
        }, []
    | _ -> model, []

let updateForSeedChange seed x y model =
    match model.Transform with
    | Scrolling (lastScreenX, lastScreenY) ->
        { model with
            FractalType = Julia ( {
                                    SeedX = seed.SeedX - (x - lastScreenX) / (model.Zoom * model.CanvasHeight)
                                    SeedY = seed.SeedY - (y - lastScreenY) / (model.Zoom * model.CanvasHeight)}, ChangeSeed)
            Transform = Scrolling (x, y)
        }, []
    | _ -> model, []

let update msg model =
    match model.FractalType, msg with
    | Julia _, MandelbrotClick _ ->
        { model with
            Zoom = 0.314; FractalType = Mandelbrot; X = -0.5; Y = 0.0
        }, []

    | Mandelbrot, JuliaClick ->
        { model with
            Zoom = 0.314; FractalType = Julia ({ SeedX = 0.0; SeedY = 0.0 }, ChangeSeed); X = 0.0; Y = 0.0
        }, []

    | Julia (seed, _), JuliaMoveClick ->
        { model with FractalType = Julia (seed, Move) }, []
    
    | Julia (seed, _), JuliaChangeSeedClick ->
        { model with FractalType = Julia (seed, ChangeSeed) }, []
    
    | _, WheelMsg we ->
        let zoom = (normalizeWheel we).pixelY / 100.0
        { model with Zoom = model.Zoom * 0.99 ** zoom }, []
    
    | _, MouseDownMsg me when me.button = 0.0 ->
        { model with
            Transform = Scrolling (me.screenX, me.screenY)
        }, []
    
    | _, MouseUpMsg me when me.button = 0.0 -> { model with Transform = NoTransform }, []

    | _, MouseLeaveMsg _ | _, TouchEndMsg _ -> { model with Transform = NoTransform }, []

    | Mandelbrot, MouseMoveMsg me
    | Julia (_, Move), MouseMoveMsg me ->
        updateForMove me.screenX me.screenY model
    
    | Julia (seed, ChangeSeed), MouseMoveMsg me ->
        updateForSeedChange seed me.screenX me.screenY model
    
    | _, TouchStartMsg te when te.touches.Length = 1 ->
        { model with
            Transform = Scrolling (te.touches.[0].clientX, te.touches.[0].clientY)
        }, []
    
    | _, TouchStartMsg te when te.touches.Length = 2 ->
        let dx = te.touches.[1].clientX - te.touches.[0].clientX
        let dy = te.touches.[1].clientY - te.touches.[0].clientY
        let distance = sqrt (dx * dx + dy * dy)
        { model with
            Transform = Pinching distance
        }, []
    
    | Mandelbrot, TouchMoveMsg te
    | Julia (_, Move), TouchMoveMsg te when te.touches.Length = 1 ->
        updateForMove te.touches.[0].screenX te.touches.[0].screenY model
    
    | Julia (seed, ChangeSeed), TouchMoveMsg te when te.touches.Length = 1 ->
        updateForSeedChange seed te.touches.[0].screenX te.touches.[0].screenY model
    
    | Mandelbrot, TouchMoveMsg te
    | Julia _, TouchMoveMsg te when te.touches.Length = 2 ->
        match model.Transform with
        | Pinching lastDistance ->
            let dx = te.touches.[1].clientX - te.touches.[0].clientX
            let dy = te.touches.[1].clientY - te.touches.[0].clientY
            let distance = sqrt (dx * dx + dy * dy)
            { model with
                Zoom = model.Zoom * 0.99 ** (lastDistance - distance)
                Transform = Pinching distance
            }, []
        | _ -> model, []
    
    | _, RenderMsg ->
        match model.Render with
        | None ->
            let holder = document.getElementById("Fractal")
            match holder with
            | null -> model, renderCommand
            | h ->
                let renderer, height = FractalRenderer.create h
                { model with Render = Some renderer; CanvasHeight = float height }, renderCommand
        | Some render ->
            render model
            { model with Now = System.DateTime.Now }, renderCommand
    
    | _ -> model, []
