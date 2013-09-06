﻿open System
open System.Windows
open System.Windows.Controls
open System.Windows.Media
open System.Windows.Media.Imaging
open System.Windows.Threading

[<AutoOpen>]
module Util =
    let sign d = if d < 0. then -1. else 1.
    let pi = Math.PI
    let pi2 = 2. * pi

    let degree2rad d = d / pi2
    let rad2degree r = r * pi2

    let clamp x min max = 
        if x < min then min
        elif x > max then max
        else x

    let norm x = clamp x -1. 1.
    let unorm x = clamp x 0. 1.

    let dispatch (d : Dispatcher) (a : unit -> unit) = 
        let a' = Action a
        ignore <| d.BeginInvoke (DispatcherPriority.ApplicationIdle, a')
        
    let asByte d = byte ((unorm d) * 255.)

type Color = 
    {
        Red     : float
        Green   : float
        Blue    : float
    }
    static member New red green blue = {Red = unorm red; Green = unorm green; Blue = unorm blue}
    member x.Dim t = Color.New (t * x.Red) (t * x.Green) (t * x.Blue)

    


type Material = 
    {
        Color       : Color
        Opacity     : float
        Diffusion   : float
        Refraction  : float
        Reflection  : float
    }
    static member New color opacity diffusion refraction reflection = {Color = color; Opacity = unorm opacity; Diffusion = unorm diffusion; Refraction = unorm refraction; Reflection = unorm reflection}

type Vector2 = 
    {
        X   : float
        Y   : float
    }


    static member New x y = {X = x; Y = y}
    static member Zero = Vector2.New 0. 0.
    static member (+) (x : Vector2, y : Vector2) = Vector2.New (x.X + y.X) (x.Y + y.Y)
    static member (-) (x : Vector2, y : Vector2) = Vector2.New (x.X - y.X) (x.Y - y.Y)
    static member (*) (x : Vector2, y : Vector2) = x.X * y.X + x.Y * y.Y

    member x.Scale s    = Vector2.New (s * x.X) (s * x.Y)
    member x.Normalize  = x.Scale (1. / x.L1)
    member x.L2         = x * x
    member x.L1         = sqrt x.L2
    member x.Min y      = Vector2.New (min x.X y.X) (min x.Y y.Y)
    member x.Max y      = Vector2.New (max x.X y.X) (max x.Y y.Y)

type Vector3 = 
    {
        X   : float
        Y   : float
        Z   : float
    }


    static member New x y z = {X = x; Y = y; Z = z}
    static member Zero = Vector3.New 0. 0. 0.
    static member (+) (x : Vector3, y : Vector3) = Vector3.New (x.X + y.X) (x.Y + y.Y) (x.Z + y.Z)
    static member (-) (x : Vector3, y : Vector3) = Vector3.New (x.X - y.X) (x.Y - y.Y) (x.Z - y.Z)
    static member (*) (x : Vector3, y : Vector3) = x.X * y.X + x.Y * y.Y + x.Z * y.Z
    static member ( *+* ) (x : Vector3, y : Vector3) = Vector3.New (x.Y * y.Z - x.Z * y.Y) (x.Z * y.Y - x.Y*y.Z) (x.X * y.Y - x.Y * y.X)

    member x.Scale s    = Vector3.New (s * x.X) (s * x.Y) (s * x.Z)
    member x.Normalize  = x.Scale (1. / x.L1)
    member x.L2         = x * x
    member x.L1         = sqrt x.L2
    member x.Min y      = Vector3.New (min x.X y.X) (min x.Y y.Y) (min x.Z y.Z)
    member x.Max y      = Vector3.New (max x.X y.X) (max x.Y y.Y) (max x.Z y.Z)

type ViewPort = 
    {
        Center          : Vector3
        Normal          : Vector3
        Axis0           : Vector3
        Axis1           : Vector3
        Width           : float
        Height          : float
        Corner0         : Vector3
        Corner1         : Vector3
        Corner2         : Vector3
        Corner3         : Vector3
    }

    static member New (c :Vector3) (normal : Vector3) (up : Vector3) width height = 
        let xaxis = (up *+* normal).Normalize
        let yaxis = (up *+* xaxis).Normalize

        let halfx       = xaxis.Scale (width / 2.)
        let halfy       = yaxis.Scale (height / 2.)

        {
            Center  = c
            Normal  = normal.Normalize
            Axis0   = xaxis
            Axis1   = yaxis
            Width   = width
            Height  = height
            Corner0 = c - halfx - halfy
            Corner1 = c + halfx - halfy
            Corner2 = c + halfx + halfy
            Corner3 = c - halfx + halfy
        }


type Surface = Vector2 -> Material

type Intersection =
    {
        Normal      : Vector3
        Point       : Vector3
        Material    : Material
    }
    static member New normal point material = {Normal = normal; Point = point; Material = material}

type Light = 
    {
        Color   : Color
        Origin  : Vector3
    }
    static member New color origin = {Color = color; Origin = origin}

type Ray = 
    {
        Direction   : Vector3
        Origin      : Vector3
    }
    member x.Trace t = (x.Direction.Scale t) + x.Origin
    static member New (direction : Vector3) (origin : Vector3) = {Direction = direction.Normalize; Origin = origin}


[<AbstractClass>]
type Shape (surface: Surface) = 
    member x.Surface with get () = surface
    member x.AsShape with get () = x
    abstract Intersect  : Ray -> Intersection option


type Sphere (surface: Surface, center : Vector3, radius : float) =
    inherit Shape (surface)

    member x.NormalAndMaterial p =
        let n' = (p - center) 
        let n = n'.Normalize
        let y' = Vector3.New n.X center.Y n.Z
        let y = (sign (n.Y - center.Y)) * acos ((y' * n) / (y'.L1 * n.L1)) / pi2 + 0.5
        let x' = Vector3.New n.X center.Y center.Z
        let x = (sign (n.Z - center.Z)) * acos ((x' * y') / (x'.L1 * y'.L1)) / pi2 + 0.5

        let m = base.Surface <| Vector2.New x y

        n,m

    override x.Intersect r = 
        let v = r.Origin - center
        let _2vd = 2. * (v * r.Direction)
        let v2 = v.L2
        let r2 = radius * radius

        let discriminant = _2vd + v2 - r2
        if discriminant < 0. then None
        else
            let root = sqrt discriminant
            let t1 = (_2vd + root) / 2.
            let t2 = (_2vd - root) / 2.

            if t1 <= 0. && t2 <= 0. then None
            elif t1 > 0. && t1 < t2 then 
                let p = r.Trace t1
                let n,m = x.NormalAndMaterial p
                Some <| Intersection.New n p m
            else 
                let p = r.Trace t2
                let n,m = x.NormalAndMaterial p
                Some <| Intersection.New n p m

type Plane (surface: Surface, offset : float, normal : Vector3)=
    inherit Shape (surface)

    let N = normal.Normalize

    let X = 
        
        let result = 
            if N.X <> 0. then Vector3.New (offset / N.X) 0. 0.
            elif N.Y <> 0. then Vector3.New 0. (offset / N.Y) 0.
            elif N.Z <> 0. then Vector3.New 0. 0. (offset / N.Z)
            else Vector3.Zero
        result.Normalize

    let Y = N *+* X

    override x.Intersect r = 
        let t = -(r.Origin*N + offset) / (r.Direction*normal)

        if t <= 0. then None
        else
            let p = r.Trace t

            let c = Vector2.New (X * p) (Y * p)

            let m = base.Surface c

            Some <| Intersection.New N p m

let White   = Color.New 1. 1. 1.
let Red     = Color.New 1. 0. 0.
let Green   = Color.New 0. 1. 0.
let Blue    = Color.New 0. 0. 1.
let Black   = Color.New 0. 0. 0.
let Matte c = Material.New c 1. 1. 0. 0.

let UniformSurface (material : Material) : Surface = fun v -> material

let trace (ray : Ray) (world : Shape[]) (lights : Light[]) (ambientLight : Color) =
    ambientLight

[<EntryPoint>]
[<STAThread>]
let main argv = 

    let lights = 
       [|
            Light.New White (Vector3.New 2. 2. 2.)
       |]

    let world = 
        [|
            Plane(UniformSurface <| Matte Blue, 0., Vector3.New 0. 1. 0.).AsShape
            Sphere(UniformSurface <| Matte Red, Vector3.New 1. 1. 1., 1.).AsShape
        |]

    let ambientLight = White.Dim 0.75

    let eye         = Vector3.New 5. 1. 1.
    let at          = Vector3.New 0. 0. 0.
    let clipDistance= 1.
    let clipNormal  = (at - eye).Normalize
    let clipUp      = Vector3.New 0. 1. 0.
    let clipCenter  = eye + clipNormal.Scale clipDistance
    let fov         = degree2rad 90.


   
    let window = new Window()
    window.MinWidth <- 640.
    window.MinHeight <- 400.


    use loaded = window.Loaded.Subscribe (fun v -> 
        let width   = window.Width
        let height  = window.Height
        let ratio   = width / height

        let wb = new WriteableBitmap(int width, int height, 96., 96., PixelFormats.Bgr32, null)

        let iwidth = wb.PixelWidth
        let iheight = wb.PixelHeight

        let viewPortDim = clipDistance / tan (fov / 2.)
        let viewPortWidth, viewPortHeight = 
            if width > height then
                viewPortDim, viewPortDim / ratio
            else
                viewPortDim * ratio, viewPortDim

        let viewPort = ViewPort.New clipCenter clipNormal clipUp viewPortWidth viewPortHeight

        let tracer = 
            async {
                
                let row = [| for i in 0..iheight - 1 -> Black|]

                for x in 0..iwidth - 1 do
                    for y in 0..iheight - 1 do
                        let vp = viewPort.Corner0 + viewPort.Axis0.Scale (viewPort.Width * float x / width) + viewPort.Axis1.Scale (viewPort.Height * float y / height)
                        let ray = Ray.New (vp - eye) eye
                        row.[y] <- trace ray world lights ambientLight
    
                    dispatch window.Dispatcher (fun () -> 
                        let pixels = 
                            [| for i in row do
                                yield asByte i.Blue
                                yield asByte i.Green
                                yield asByte i.Red 
                                yield byte 255
                            |]
                        wb.WritePixels (Int32Rect (x, 0, 1, iheight), pixels, 4, 0)
                        )

                return ()
            }

        Async.Start tracer

        let i = new Image ()
        i.Source <- wb
        window.Content <- i
        )

    let result = window.ShowDialog()

    0
