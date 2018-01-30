module App.Types

type Msg =
    | Render

type Model =
    {
        Zoom: float
        OffsetX: float
        OffsetY: float
        Now: System.DateTime
    }
