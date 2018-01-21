module Home.State

open Elmish
open Types

let init () : Model * Cmd<Msg> =
  { Now = System.DateTime.Now }, []

let update msg model : Model * Cmd<Msg> =
  match msg with
  | Noop -> model, []
