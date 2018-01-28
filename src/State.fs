module App.State

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Fable.Import
open Types

let renderCommand =
    let sub dispatch =
        let f = Browser.FrameRequestCallback (fun _ -> dispatch Render)
        Browser.window.requestAnimationFrame(f) |> ignore
    Cmd.ofSub sub

let init result =
  { Now = System.DateTime.Now }, renderCommand

let update renderer msg model =
  match msg with
  | Render ->
    renderer model
    { model with Now = System.DateTime.Now }, renderCommand
