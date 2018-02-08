module App.Types

open Fable.Import

type Msg =
    | WheelMsg of React.WheelEvent
    | MouseDownMsg of React.MouseEvent
    | MouseUpMsg of React.MouseEvent
    | MouseMoveMsg of React.MouseEvent
    | MouseLeaveMsg of React.MouseEvent
    | TouchStartMsg of React.TouchEvent
    | TouchEndMsg of React.TouchEvent
    | TouchMoveMsg of React.TouchEvent
    | RenderMsg

type Transform =
    | Scrolling of float * float
    | Pinching of float
    | NoTransform

type Model =
    {
        CanvasHeight: float
        Zoom: float
        OffsetX: float
        OffsetY: float
        Now: System.DateTime
        Render: (Model -> unit) option
        Transform: Transform
    }
