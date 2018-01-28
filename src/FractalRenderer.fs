module FractalRenderer

open System
open Fable.Import
open Fable.Core.JsInterop
open WebGLHelper
open App.Types
open Fable.Core

let myVertex = """
    attribute vec4 aVertexPosition;
    attribute vec4 aVertexColour;

    varying lowp vec4 vColour;

    void main() {
      gl_Position = aVertexPosition;
      vColour = aVertexColour;
    }
"""

let myFragment = """
    varying lowp vec4 vColour;

    void main(void) {
        gl_FragColor = vColour;
    }
"""

let initBuffers gl =
    let positions =
        createBuffer
            [|
                 0.5; -0.7;
                 0.0;  0.7;
                -0.5; -0.7
            |] gl
    let colours =
        createBuffer
            [|
                1.0;  0.0; 0.0; 1.0;
                0.0;  1.0; 0.0; 1.0;
                0.0;  0.0; 1.0; 1.0
            |] gl
    positions, colours

let create (holder : Browser.Element) =

    let canvas = Browser.document.createElement_canvas()
    let width = 640
    let height = 256

    canvas.width <- float width
    canvas.height <- float height
    canvas.style.width <- width.ToString() + "px"
    canvas.style.height <- height.ToString() + "px"

    holder.appendChild(canvas) |> ignore

    let context = getWebGLContext canvas

    let program = createShaderProgram context myVertex myFragment

    let positionBuffer, colourBuffer = initBuffers context
    let vertexPositionAttribute = createAttributeLocation context program "aVertexPosition"
    let vertexColourAttribute = createAttributeLocation context program "aVertexColour"

    let draw () =
        context.useProgram(program)
        context.bindBuffer(context.ARRAY_BUFFER, positionBuffer)
        context.vertexAttribPointer(vertexPositionAttribute, 2., context.FLOAT, false, 0., 0.)
        context.bindBuffer(context.ARRAY_BUFFER, colourBuffer)
        context.vertexAttribPointer(vertexColourAttribute, 4., context.FLOAT, false, 0., 0.)
        context.drawArrays (context.TRIANGLE_STRIP, 0., 3.)

    let clear = clear context

    // Try not to use "context" after this point, bind a function above.

    let imageLoadCanvas = Browser.document.createElement_canvas()
    let imageLoadCanvasContext = imageLoadCanvas.getContext_2d()

    let created = DateTime.Now
    let mutable last = DateTime.Now

    let render model =
        match model with
        | model when model.Now <> last ->
            last <- model.Now

            let resolution = canvas.width, canvas.height

            clear resolution

            draw()

        | _ -> ignore()
    
    render
