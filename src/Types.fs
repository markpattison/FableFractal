module App.Types

type Msg =
    | UpMsg
    | LeftMsg
    | RightMsg
    | DownMsg
    | ZoomInMsg
    | ZoomOutMsg
    | RenderMsg

type Model =
    {
        Zoom: float
        OffsetX: float
        OffsetY: float
        Now: System.DateTime
    }
