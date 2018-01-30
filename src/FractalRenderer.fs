module FractalRenderer

open System
open Fable.Import
open Fable.Core.JsInterop
open WebGLHelper
open App.Types
open Fable.Core

let myVertex = """
    precision highp float;
    precision highp int;
    
    attribute vec4 aVertexPosition;
    attribute vec2 aTextureCoord;

    varying vec2 vTextureCoord;

    void main() {
      gl_Position = aVertexPosition;
      vTextureCoord = aTextureCoord;
    }
"""

let myFragment = """
    precision highp float;
    precision highp int;

    uniform float uWidthOverHeight;
    uniform float uZoom;
    uniform vec2 uOffset;

    varying vec2 vTextureCoord;

    vec2 calculatePosition(vec2 inputCoords, float zoom, float widthOverHeight, vec2 offset)
    {
        return (inputCoords - 0.5) * vec2(widthOverHeight, 1.0) / zoom + offset;
    }

    vec4 applyColourMap(float x)
    {
        return vec4(sin(x * 4.0), sin (x * 5.0), sin (x * 6.0), 1.0);
    }

    vec2 complexMultiply(vec2 a, vec2 b)
    {
        return vec2(a.x * b.x - a.y * b.y, a.x * b.y + a.y * b.x);
    }

    vec2 complexSquare(vec2 z)
    {
        return complexMultiply(z, z);
    }

    vec2 complexCube(vec2 z)
    {
        return complexMultiply(z, complexSquare(z));
    }

    vec2 f(vec2 z, vec2 offset)
    {
        return complexSquare(z) + offset;
    }

    float pixelResult(vec2 z, vec2 offset)
    {
        float result = 0.0;
        vec2 zsq = z * z;

        int iterations = 0;

        for (int i = 0; i < 128; i++)
        {
            iterations = i;
            if (zsq.x + zsq.y > 49.0)
            {
                break;
            }
            z = f(z, offset);
            zsq = z * z;
        }

        if (iterations == 127)
        {
            result = 0.0;
        }
        else
        {
            result = float(iterations) + (log(2.0 * log(7.0)) - log(log(zsq.x + zsq.y))) / log(2.0);
            result = log(result * 0.4) / log(128.0);
        }

        return result;
    }

    void main(void)
    {
        vec2 z = calculatePosition(vTextureCoord, uZoom, uWidthOverHeight, uOffset);
        float result = pixelResult(z, z);
        gl_FragColor = applyColourMap(result);
    }
"""

let initBuffers gl =
    let positions =
        createBuffer
            [|
                 -1.0; -1.0;
                  1.0; -1.0;
                 -1.0;  1.0;
                  1.0;  1.0
            |] gl
    let textureCoords =
        createBuffer
            [|
                0.0; 0.0;
                1.0; 0.0;
                0.0; 1.0;
                1.0; 1.0
            |] gl
    positions, textureCoords

let create (holder : Browser.Element) =

    let canvas = Browser.document.createElement_canvas()
    let width = 640
    let height = 480

    canvas.width <- float width
    canvas.height <- float height
    canvas.style.width <- width.ToString() + "px"
    canvas.style.height <- height.ToString() + "px"

    holder.appendChild(canvas) |> ignore

    let context = getWebGLContext canvas

    let program = createShaderProgram context myVertex myFragment

    let positionBuffer, colourBuffer = initBuffers context
    let vertexPositionAttribute = createAttributeLocation context program "aVertexPosition"
    let textureCoordAttribute = createAttributeLocation context program "aTextureCoord"
    let widthOverHeightUniform = createUniformLocation context program "uWidthOverHeight"
    let zoomUniform = createUniformLocation context program "uZoom"
    let offsetUniform = createUniformLocation context program "uOffset"

    let draw widthOverHeight zoom x y =
        context.useProgram(program)

        context.bindBuffer(context.ARRAY_BUFFER, positionBuffer)
        context.vertexAttribPointer(vertexPositionAttribute, 2.0, context.FLOAT, false, 0.0, 0.0)
        context.bindBuffer(context.ARRAY_BUFFER, colourBuffer)
        context.vertexAttribPointer(textureCoordAttribute, 2.0, context.FLOAT, false, 0.0, 0.0)

        context.uniform1f(widthOverHeightUniform, widthOverHeight)
        context.uniform1f(zoomUniform, zoom)
        context.uniform2f(offsetUniform, x, y)

        context.drawArrays (context.TRIANGLE_STRIP, 0., 4.0)

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
            let widthOverHeight = if canvas.height = 0.0 then 1.0 else canvas.width / canvas.height
            clear resolution

            draw widthOverHeight model.Zoom model.OffsetX model.OffsetY

        | _ -> ignore()
    
    render
