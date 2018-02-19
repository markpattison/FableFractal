module App.Types

open Fable.Import

type Msg =
    | MandelbrotClick
    | JuliaClick
    | JuliaMoveClick
    | JuliaChangeSeedClick
    | WheelMsg of React.WheelEvent
    | MouseDownMsg of React.MouseEvent
    | MouseUpMsg of React.MouseEvent
    | MouseMoveMsg of React.MouseEvent
    | MouseLeaveMsg of React.MouseEvent
    | TouchStartMsg of React.TouchEvent
    | TouchEndMsg of React.TouchEvent
    | TouchMoveMsg of React.TouchEvent
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
