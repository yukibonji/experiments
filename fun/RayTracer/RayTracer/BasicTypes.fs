﻿namespace RayTracer

type Color = 
    {
        Red     : float
        Green   : float
        Blue    : float
    }
    static member New red green blue = {Red = unorm red; Green = unorm green; Blue = unorm blue}
    static member Zero = Color.New 0. 0. 0.
    static member (+) (x : Color, y : Color)    = Color.New (x.Red + y.Red) (x.Green + y.Green) (x.Blue + y.Blue)
    static member (*) (x : Color, y : Color)    = Color.New (x.Red * y.Red) (x.Green * y.Green) (x.Blue * y.Blue)
    static member (*) (s : float  , x : Color)  = x.Scale s
    static member (*) (x : Color  , s : float)  = x.Scale s

    member x.Scale t = Color.New (t * x.Red) (t * x.Green) (t * x.Blue)
    member x.Lerp y t = 
        let xx = Vector3.New x.Red x.Green x.Blue
        let yy = Vector3.New y.Red y.Green y.Blue
        let rr = xx.Lerp yy t
        Color.New rr.X rr.Y rr.Z

type Material = 
    {
        Color       : Color
        Opacity     : float
        Diffusion   : float
        Refraction  : float
        Reflection  : float
        Specular    : Color
    }
    static member New color opacity diffusion refraction reflection specular = 
        {
            Color       = color 
            Opacity     = unorm opacity 
            Diffusion   = unorm diffusion 
            Refraction  = unorm refraction 
            Reflection  = unorm reflection
            Specular    = specular
        }


type Surface = Vector2 -> Material

type Ray = 
    {
        Direction   : Vector3
        Origin      : Vector3
    }
    member x.Trace t            = t * x.Direction + x.Origin
    member x.IntersectSphere (center :Vector3) (radius : float) = 
        let v = x.Origin - center
        let vd = v * x.Direction
        let v2 = v.L2
        let r2 = radius * radius

        let discriminant = vd*vd - v2 + r2
        if discriminant < 0. then None
        else
            let root = sqrt discriminant
            let t1 = -vd + root
            let t2 = -vd - root


            if t1 < cutoff || t2 < cutoff then None
            elif t1 < t2 then Some (t1, t2)
            else Some (t2, t1)
    member x.IntersectPlane (normal :Vector3) (offset : float) = 
        let t = -(x.Origin*normal + offset) / (x.Direction*normal)
        if t > cutoff then Some t
        else None
        
    static member FromTo (origin : Vector3) (destination: Vector3) = {Direction = (destination - origin).Normalize; Origin = origin}
    static member DirectionOrigin (direction: Vector3) (origin : Vector3) = {Direction = direction.Normalize; Origin = origin}


[<AutoOpen>]
module BasicTypes =
    let Red     = Color.New 1. 0. 0.
    let Yellow  = Color.New 1. 1. 0.
    let Green   = Color.New 0. 1. 0.
    let Cyan    = Color.New 0. 1. 1.
    let Blue    = Color.New 0. 0. 1.
    let Magenta = Color.New 1. 0. 1.

    let White   = Color.New 1. 1. 1.
    let Black   = Color.New 0. 0. 0.


