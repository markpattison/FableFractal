module Home.FractalRenderer

open System
open Fable.Import
open Fable.Core.JsInterop
open WebGLHelper
open Types
open Fable.Core

let create subscribe dispatch spriteLoadInfos (holder : Browser.Element) =

    let canvas = Browser.document.createElement_canvas()
    let width = 640
    let height = 256

    canvas.width <- float width
    canvas.height <- float height
    canvas.style.width <- width.ToString() + "px"
    canvas.style.height <- height.ToString() + "px"

    holder.appendChild(canvas) |> ignore

    let context : Browser.WebGLRenderingContext = 
      // Helper to get the webgl context
      let getContext ctxString =
        canvas.getContext(ctxString, createObj [ "premultipliedAlpha" ==> false ]) |> unbox<Browser.WebGLRenderingContext>

      let webgl = getContext "webgl"
  
      // If we have wegl = null in JS then try to get experimental-wegl
      // Edge and webkit use experimental-webgl
      if not (unbox webgl) then
        getContext "experimental-webgl"
      else // Else give back the context
        webgl

    let positionBuffer = createSpritePositionBuffer context
    let textureBuffer = createSpriteTextureBuffer context
    let defaultSpriteUniformSetter = createSpriteUniformSetter context positionBuffer textureBuffer

    let defaultRender =
        let program = createShaderProgram context defaultVertex defaultFragment
        context.useProgram(program)

        let spriteUniformSetter = defaultSpriteUniformSetter program

        fun spriteUniform ->
            context.useProgram(program)
            spriteUniformSetter spriteUniform
            drawSprite context


    let clear = clear context
    let createTexture = createTexture context

    // Try not to use "context" after this point, bind a function above.

    let imageLoadCanvas = Browser.document.createElement_canvas()
    let imageLoadCanvasContext = imageLoadCanvas.getContext_2d()

    let created = DateTime.Now;
    let mutable last = DateTime.Now

    subscribe <|
    function
    | model when model.Now <> last ->
        last <- model.Now

        let resolution = canvas.width, canvas.height

        clear (fst resolution) (snd resolution)

        //model.Items
        //|> List.map(fun item ->
        //    match getSprite item.Name with
        //    | Some sprite ->
        //        sprite.Layers
        //        |> List.map(fun (level, mode, texture) ->
        //            item, (sprite.Width, sprite.Height), level, mode, texture
        //        )
        //    | _ ->
        //        [])
        //|> List.concat
        //|> List.sortWith(fun (aItem, _, aLevel, _, _) (bItem, _, bLevel, _, _) ->
        //    if aItem.Y > bItem.Y then 1
        //    elif aItem.Y < bItem.Y then -1
        //    else
        //        if aItem.X > bItem.X then 1
        //        elif aItem.X < bItem.X then -1
        //        else
        //            if aLevel > bLevel then 1
        //            elif aLevel < bLevel then -1
        //            else 0
        //)
        //|> List.iter(fun (item, (width, height), _, mode, texture) ->
        //    let size = float width, float height
        //    let position = float item.X * scale, float item.Y * scale
        //    let spriteUniformValues = resolution, position, size, texture

        //    if item.X = fst model.Highlight && item.Y = snd model.Highlight then
        //        highlightRender spriteUniformValues now
        //    else
        //        match mode with
        //        | RenderMode.Default ->
        //            defaultRender spriteUniformValues

        //        | RenderMode.Ripple ->
        //            rippleRender spriteUniformValues now

        //)

    | _ -> ignore()
