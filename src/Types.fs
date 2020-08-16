module App.Types

open Browser.Types

type Msg =
    | MandelbrotClick
    | JuliaClick
    | JuliaMoveClick
    | JuliaChangeSeedClick
    | WheelMsg of WheelEvent
    | MouseDownMsg of MouseEvent
    | MouseUpMsg of MouseEvent
    | MouseMoveMsg of MouseEvent
    | MouseLeaveMsg of MouseEvent
    | TouchStartMsg of TouchEvent
    | TouchEndMsg of TouchEvent
    | TouchMoveMsg of TouchEvent
    | RenderMsg

type JuliaSeed = { SeedX: float; SeedY: float }
type JuliaScrolling = Move | ChangeSeed

type FractalType =
    | Mandelbrot
    | Julia of JuliaSeed * JuliaScrolling

type Transform =
    | Scrolling of float * float
    | Pinching of float
    | NoTransform

type Model =
    {
        CanvasHeight: float
        Zoom: float
        FractalType: FractalType
        X: float
        Y: float
        Now: System.DateTime
        Render: (Model -> unit) option
        Transform: Transform
    }
