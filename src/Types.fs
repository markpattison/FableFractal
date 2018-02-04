module App.Types

open Fable.Import

type Msg =
    | UpMsg
    | LeftMsg
    | RightMsg
    | DownMsg
    | ZoomInMsg
    | ZoomOutMsg
    | WheelMsg of React.WheelEvent
    | RenderMsg

type Model =
    {
        Zoom: float
        OffsetX: float
        OffsetY: float
        Now: System.DateTime
        Render: (Model -> unit) option
    }
