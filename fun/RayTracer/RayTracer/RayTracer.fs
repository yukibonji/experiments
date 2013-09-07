﻿namespace RayTracer

type Color = 
    {
        Red     : float
        Green   : float
        Blue    : float
    }
    static member New red green blue = {Red = unorm red; Green = unorm green; Blue = unorm blue}
    static member Zero = Color.New 0. 0. 0.
    static member (+) (x : Color, y : Color) = Color.New (x.Red + y.Red) (x.Green + y.Green) (x.Blue + y.Blue)
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

type Surface = Vector2 -> Material

type Ray = 
    {
        Direction   : Vector3
        Origin      : Vector3
    }
    member x.Trace t = (x.Direction.Scale t) + x.Origin
    static member New (origin : Vector3) (destination: Vector3) = {Direction = (destination - origin).Normalize; Origin = origin}

type Intersection =
    {
        Ray         : Ray
        Distance    : float
        Normal      : Vector3
        Point       : Vector3
        Material    : Material
    }
    static member New ray distance normal point material = {Ray = ray; Distance = distance; Normal = normal; Point = point; Material = material}

type LightSource = 
    {
        Color   : Color
        Origin  : Vector3
    }
    static member New color origin = {Color = color; Origin = origin}

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
                Some <| Intersection.New r t1 n p m
            else 
                let p = r.Trace t2
                let n,m = x.NormalAndMaterial p
                Some <| Intersection.New r t2 n p m

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

            Some <| Intersection.New r t N p m

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

    static member New (eye :Vector3) (at : Vector3) (up : Vector3) (clipDistance : float) (fov : double) (ratio : float) = 
        let clipNormal  = (at - eye).Normalize
        let clipCenter  = eye + clipNormal.Scale clipDistance

        let width       = clipDistance / tan (fov / 2.)
        let height      = width * ratio

        let xaxis = (up *+* clipNormal).Normalize
        let yaxis = (up *+* xaxis).Normalize

        let halfx       = xaxis.Scale (width / 2.)
        let halfy       = yaxis.Scale (height / 2.)

        {
            Center  = clipCenter
            Normal  = clipNormal
            Axis0   = xaxis
            Axis1   = yaxis
            Width   = width
            Height  = height
            Corner0 = clipCenter - halfx - halfy
            Corner1 = clipCenter + halfx - halfy
            Corner2 = clipCenter + halfx + halfy
            Corner3 = clipCenter - halfx + halfy
        }


[<AutoOpen>]
module RayTracerUtil =

    let UniformSurface (material : Material) : Surface = fun v -> material

    let Matte c = Material.New c 1. 1. 0. 0.

    let Diffusion (i : Intersection) (shapes : Shape[]) (lights : LightSource[]) (ambientLight : Color) =
        if i.Material.Opacity > 0. && i.Material.Diffusion > 0. then
            let isShapeBlockingLight (light : LightSource) (shape : Shape) = 
                let lightRay = Ray.New i.Point light.Origin
                let intersection = shape.Intersect lightRay
                match intersection with
                |   Some _  ->  true
                |   _       ->  false
            let isLightVisible (light : LightSource) = 
                let someShapeAreBlockLight = 
                    shapes
                    |> Array.exists (isShapeBlockingLight light)
                not someShapeAreBlockLight

            let visibleLights = 
                lights 
                |> Array.filter isLightVisible

            let illumination (light : LightSource) = 
                let direction = (light.Origin - i.Point).Normalize
                let c = direction * i.Normal
                light.Color.Dim (c * i.Material.Diffusion)

            let sumOfIllumination =
                visibleLights
                |>  Array.map illumination
                |>  Array.sum

            sumOfIllumination
        else
            i.Material.Color

            


    let rec TraceImpl (ray : Ray) (shapes : Shape[]) (lights : LightSource[]) (ambientLight : Color) = 
        
        let mutable closestIntersection : Intersection option = None
        for shape in shapes do
            let intersection = shape.Intersect ray
            closestIntersection <- 
                match intersection, closestIntersection with
                |   Some i, Some ci when i.Distance < ci.Distance 
                                    -> Some i
                |   Some i, _       -> Some i
                |   _               -> closestIntersection

        match closestIntersection with
            |   Some i      ->
                let diffusion = Diffusion i shapes lights ambientLight

                diffusion.Dim i.Material.Diffusion
            |   _           -> ambientLight
        
    let Trace (ray : Ray) (world : Shape[]) (lights : LightSource[]) (ambientLight : Color) =
        TraceImpl ray world lights ambientLight

