module WebGLHelper

open Fable.Core.JsInterop
open Fable.Import

// Shorthand
type GL = Browser.WebGLRenderingContext

let createShaderProgram (gl:GL) vertex fragment =
    let vertexShader = gl.createShader(gl.VERTEX_SHADER);
    gl.shaderSource(vertexShader, vertex);
    gl.compileShader(vertexShader);

    let fragShader = gl.createShader(gl.FRAGMENT_SHADER);
    gl.shaderSource(fragShader, fragment);
    gl.compileShader(fragShader);

    let program = gl.createProgram()
    gl.attachShader(program, vertexShader);
    gl.attachShader(program, fragShader);
    gl.linkProgram(program);

    program

let createAttributeLocation (gl : GL) program name =
    let attributeLocation = gl.getAttribLocation(program, name)
    gl.enableVertexAttribArray(attributeLocation)

    attributeLocation

let createBuffer (items : float[]) (gl:GL) =
    let buffer = gl.createBuffer();

    gl.bindBuffer(gl.ARRAY_BUFFER, buffer);
    gl.bufferData(gl.ARRAY_BUFFER, (createNew JS.Float32Array items) |> unbox, gl.STATIC_DRAW)

    buffer

let clear (gl:GL) width height =
    gl.clearColor(1.0, 1.0, 1.0, 1.0);

    gl.blendFunc(gl.SRC_ALPHA, gl.ONE_MINUS_SRC_ALPHA);
    //gl.enable(gl.DEPTH_TEST);
    gl.enable(gl.BLEND);

    gl.viewport(0., 0., width, height);
    gl.clear(float (int gl.COLOR_BUFFER_BIT ||| int gl.DEPTH_BUFFER_BIT));

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
    