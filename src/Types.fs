module App.Types

open Fable.Import

type Msg =
    | MandelbrotClick
    | JuliaClick
    | WheelMsg of React.WheelEvent
    | MouseDownMsg of React.MouseEvent
    | MouseUpMsg of React.MouseEvent
    | MouseMoveMsg of React.MouseEvent
    | MouseLeaveMsg of React.MouseEvent
    | TouchStartMsg of React.TouchEvent
    | TouchEndMsg of React.TouchEvent
    | TouchMoveMsg of React.TouchEvent
    | RenderMsg

type ScrollType = Left | Right
type FractalType = Mandelbrot | Julia

type Transform =
    | Scrolling of float * float * ScrollType
    | Pinching of float
    | NoTransform

type Model =
    {
        CanvasHeight: float
        Zoom: float
        FractalType: FractalType
        MandelbrotX: float
        MandelbrotY: float
        JuliaX: float
        JuliaY: float
        Now: System.DateTime
        Render: (Model -> unit) option
        Transform: Transform
    }
