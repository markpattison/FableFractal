module WebGLHelper

open Fable.Core.JsInterop
open Fable.Import

// Shorthand
type GL = Browser.WebGLRenderingContext

let getWebGLContext (canvas: Browser.HTMLCanvasElement) = 
    let getContext ctxString =
        canvas.getContext(ctxString, createObj [ "premultipliedAlpha" ==> false ]) |> unbox<Browser.WebGLRenderingContext>

    let webgl = getContext "webgl"
  
    // If we have webgl = null in JS then try to get experimental-webgl
    // Edge and webkit use experimental-webgl
    if not (unbox webgl) then
        getContext "experimental-webgl"
    else
        webgl

let createShaderProgram (gl:GL) vertex fragment =
    let vertexShader = gl.createShader(gl.VERTEX_SHADER)
    gl.shaderSource(vertexShader, vertex)
    gl.compileShader(vertexShader)

    let fragShader = gl.createShader(gl.FRAGMENT_SHADER)
    gl.shaderSource(fragShader, fragment)
    gl.compileShader(fragShader)

    let program = gl.createProgram()
    gl.attachShader(program, vertexShader)
    gl.attachShader(program, fragShader)
    gl.linkProgram(program)

    program

let createAttributeLocation (gl : GL) program name =
    let attributeLocation = gl.getAttribLocation(program, name)
    gl.enableVertexAttribArray(attributeLocation)

    attributeLocation

let createBuffer (items : float[]) (gl:GL) =
    let buffer = gl.createBuffer()

    gl.bindBuffer(gl.ARRAY_BUFFER, buffer)
    gl.bufferData(gl.ARRAY_BUFFER, (createNew JS.Float32Array items) |> unbox, gl.STATIC_DRAW)

    buffer

let clear (gl:GL) (width, height) =
    gl.clearColor(1.0, 1.0, 1.0, 1.0)

    gl.blendFunc(gl.SRC_ALPHA, gl.ONE_MINUS_SRC_ALPHA)
    //gl.enable(gl.DEPTH_TEST)
    gl.enable(gl.BLEND)

    gl.viewport(0., 0., width, height)
    gl.clear(float (int gl.COLOR_BUFFER_BIT ||| int gl.DEPTH_BUFFER_BIT))
