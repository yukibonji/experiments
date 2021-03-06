﻿open System
open System.Drawing
open System.Threading
open System.Windows.Forms

open SharpDX

module Common =
  open SharpDX.Mathematics.Interop
  open System.Diagnostics

  let frameCount    = 2

  let minz          = -7
  let maxz          = 7
  let alphaz        = true

  let minDelay      = 25.F
  let delayVar      = 15.F

  let distance      = 300.0F

  let viewPos       = Vector4 (0.F, distance*1.5F, distance*4.F, 1.F)
  let lightningPos  = Vector4 (-1.F*distance, -1.F*distance, 3.F*distance, 1.F)

  let background    = Color4(0.1F, 0.1F, 0.1F, 1.F)
//  let background    = Color4(1.F, 1.F, 1.F, 1.F)

  let random        = Random 19740531

  let randomVector3 () =
    let v = Vector3 ( random.NextFloat (-1.F, 1.F)
                    , random.NextFloat (-1.F, 1.F)
                    , random.NextFloat (-1.F, 1.F)
                    )

    v.Normalize ()

    v

  let clock         =
    let f  = Stopwatch.Frequency |> float32
    let sw = Stopwatch ()
    sw.Start ()
    fun () ->
      float32 sw.ElapsedMilliseconds / 1000.0F

  let dispose nm (it : IDisposable) =
    try
      if it <> null then it.Dispose ()
    with
    | e -> printfn "Failed to dispose %s" nm


  let rtrue         = RawBool true
  let rfalse        = RawBool false

  let inline (!?) (x:^a) : ^b = ((^a or ^b) : (static member op_Implicit : ^a -> ^b) x)

  let rviewPortf  (v : Viewport)  : RawViewportF  = !? v
  let rrectangle  (r : Rectangle) : RawRectangle  = !? r
  let rcolor4     (c : Color4)    : RawColor4     = !? c

open Common

type ViewState  ( viewPos       : Vector4
                , lightningPos  : Vector4
                , world         : Matrix
                , worldViewProj : Matrix
                , timestamp     : Vector4
                ) =
  struct
    member x.ViewPos        = viewPos
    member x.LightningPos   = lightningPos
    member x.World          = world
    member x.WorldViewProj  = worldViewProj
    member x.Timestamp      = timestamp
  end

type Vertex (position : Vector3, normal : Vector3, color : Vector4) =
  struct
    member x.Position = position
    member x.Normal   = normal
    member x.Color    = color

    override x.ToString () =
      sprintf "V: %A, %A, %A" position normal color
  end

type InstanceVertex (position : Vector3, direction : Vector3, rotation : Vector3, delay : Vector3, color : Vector4) =
  struct
    member x.Position   = position
    member x.Direction  = direction
    member x.Rotation   = rotation
    member x.Delay      = delay
    member x.Color      = color

    override x.ToString () =
      sprintf "IV: %A, %A, %A, %A, %A" position direction rotation delay color
  end

type CommandList(device : Direct3D12.Device, pipelineState : Direct3D12.PipelineState) =
  let listType   = Direct3D12.CommandListType.Direct
  let allocator  = device.CreateCommandAllocator listType
  let queue      = device.CreateCommandQueue (Direct3D12.CommandQueueDescription listType)
  let list       =
    let cl = device.CreateCommandList (listType, allocator, pipelineState)
    cl.Close () // Opened in recording state, close it.
    cl

  interface IDisposable with
    member x.Dispose () =
      dispose "queue"     queue
      dispose "allocator" allocator

  member x.Execute (a : Direct3D12.GraphicsCommandList -> 'T) : 'T =
    allocator.Reset ()

    list.Reset (allocator, pipelineState)

    let v =
      try
        a list
      finally
        list.Close ()

    queue.ExecuteCommandList list

    v

  member x.Queue = queue

type UploadConstantBuffer<'T when 'T : struct and 'T : (new : unit -> 'T) and 'T :> ValueType> (device : Direct3D12.Device, initial : 'T) =
  let mutable data = initial
  let size = Utilities.SizeOf<'T> ()

  let heap =
    let dhd = Direct3D12.DescriptorHeapDescription  ( DescriptorCount = 1
                                                    , Flags           = Direct3D12.DescriptorHeapFlags.ShaderVisible
                                                    , Type            = Direct3D12.DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView
                                                    )

    device.CreateDescriptorHeap dhd


  let resource =
    let hp  = Direct3D12.HeapProperties Direct3D12.HeapType.Upload
    let hf  = Direct3D12.HeapFlags.None
    let rd  = Direct3D12.ResourceDescription.Buffer (int64 size)
    let rs  = Direct3D12.ResourceStates.GenericRead

    device.CreateCommittedResource (hp, hf, rd, rs)

  let view =
    let cbvd = Direct3D12.ConstantBufferViewDescription ( BufferLocation  = resource.GPUVirtualAddress
                                                        , SizeInBytes     = ((size + 0xFF) &&& ~~~0xFF)
                                                        )

    device.CreateConstantBufferView (Nullable cbvd, heap.CPUDescriptorHandleForHeapStart)

  let updateConstantBuffer () =
    let ptr = resource.Map 0

    try
      Utilities.Write (ptr, &data)
    finally
      resource.Unmap 0

  do
    updateConstantBuffer ()

  interface IDisposable with
    member x.Dispose () =
      dispose "resource"  resource
      dispose "heap"      heap

  member x.Heap = heap

  member x.Data
    with  get ()  = data
    and   set v   = data <- v; updateConstantBuffer ()

type UploadVertexBuffer<'T when 'T : struct and 'T : (new : unit -> 'T) and 'T :> ValueType> (device : Direct3D12.Device, initial : 'T []) =
  let data = initial

  let size = Utilities.SizeOf data

  let resource =
    let hp  = Direct3D12.HeapProperties Direct3D12.HeapType.Upload
    let hf  = Direct3D12.HeapFlags.None
    let rd  = Direct3D12.ResourceDescription.Buffer (int64 size)
    let rs  = Direct3D12.ResourceStates.GenericRead

    device.CreateCommittedResource (hp, hf, rd, rs)

  let view =
    let vbv = Direct3D12.VertexBufferView ( BufferLocation  = resource.GPUVirtualAddress
                                          , StrideInBytes   = Utilities.SizeOf<'T> ()
                                          , SizeInBytes     = size
                                          )

    vbv

  let updateVertexBuffer () =
    let ptr = resource.Map 0

    try
      Utilities.Write (ptr, data, 0, data.Length) |> ignore
    finally
      resource.Unmap 0

  do
    updateVertexBuffer ()

  interface IDisposable with
    member x.Dispose () =
      dispose "resource"  resource

  member x.Data                   = data
  member x.Length                 = data.Length
  member x.Resource               = resource
  member x.Size                   = size
  member x.UpdateVertexBuffer ()  = updateVertexBuffer ()
  member x.View                   = view

type DefaultVertexBuffer<'T when 'T : struct and 'T : (new : unit -> 'T) and 'T :> ValueType> (device : Direct3D12.Device, commandList: CommandList, initial : UploadVertexBuffer<'T>) =
  let length  = initial.Length
  let size    = initial.Size

  let resource =
    let hp  = Direct3D12.HeapProperties Direct3D12.HeapType.Default
    let hf  = Direct3D12.HeapFlags.None
    let rd  = Direct3D12.ResourceDescription.Buffer (int64 size)
    let rs  = Direct3D12.ResourceStates.GenericRead

    let d   = device.CreateCommittedResource (hp, hf, rd, rs)

    commandList.Execute <| fun commandList ->
      commandList.CopyResource (d, initial.Resource)

    d


  let view =
    let vbv = Direct3D12.VertexBufferView ( BufferLocation  = resource.GPUVirtualAddress
                                          , StrideInBytes   = Utilities.SizeOf<'T> ()
                                          , SizeInBytes     = size
                                          )

    vbv

  interface IDisposable with
    member x.Dispose () =
      dispose "resource"  resource

  member x.Length = length
  member x.Size   = size
  member x.View   = view

type DeviceIndependent () =
  let background   = background |> rcolor4

  let boxVertices  =
    let fn = -Vector3.UnitZ
    let bn = Vector3.UnitZ
    let tn = -Vector3.UnitY
    let on = Vector3.UnitY
    let ln = -Vector3.UnitX
    let rn = Vector3.UnitX
    let v x y z n i = Vertex (Vector3 (x, y, z), n, Vector4 (i, i, i, 1.F))
    [|
//       x     y     z    n   i
      v -1.0F -1.0F -1.0F fn  1.0F // Front
      v -1.0F  1.0F -1.0F fn  1.0F
      v  1.0F  1.0F -1.0F fn  1.0F
      v -1.0F -1.0F -1.0F fn  1.0F
      v  1.0F  1.0F -1.0F fn  1.0F
      v  1.0F -1.0F -1.0F fn  1.0F

      v -1.0F -1.0F  1.0F bn 1.0F // Back
      v  1.0F  1.0F  1.0F bn 1.0F
      v -1.0F  1.0F  1.0F bn 1.0F
      v -1.0F -1.0F  1.0F bn 1.0F
      v  1.0F -1.0F  1.0F bn 1.0F
      v  1.0F  1.0F  1.0F bn 1.0F

      v -1.0F -1.0F -1.0F tn 0.5F // Top
      v  1.0F -1.0F  1.0F tn 0.5F
      v -1.0F -1.0F  1.0F tn 0.5F
      v -1.0F -1.0F -1.0F tn 0.5F
      v  1.0F -1.0F -1.0F tn 0.5F
      v  1.0F -1.0F  1.0F tn 0.5F

      v -1.0F  1.0F -1.0F on 0.5F // Bottom
      v -1.0F  1.0F  1.0F on 0.5F
      v  1.0F  1.0F  1.0F on 0.5F
      v -1.0F  1.0F -1.0F on 0.5F
      v  1.0F  1.0F  1.0F on 0.5F
      v  1.0F  1.0F -1.0F on 0.5F

      v -1.0F -1.0F -1.0F ln 0.75F // Left
      v -1.0F -1.0F  1.0F ln 0.75F
      v -1.0F  1.0F  1.0F ln 0.75F
      v -1.0F -1.0F -1.0F ln 0.75F
      v -1.0F  1.0F  1.0F ln 0.75F
      v -1.0F  1.0F -1.0F ln 0.75F

      v  1.0F -1.0F -1.0F rn 0.75F // Right
      v  1.0F  1.0F  1.0F rn 0.75F
      v  1.0F -1.0F  1.0F rn 0.75F
      v  1.0F -1.0F -1.0F rn 0.75F
      v  1.0F  1.0F -1.0F rn 0.75F
      v  1.0F  1.0F  1.0F rn 0.75F
    |]

#if DD
  let instanceVertices =
    let depth     = 4
    let boxCount  = pown 20 depth
    let maxSide   = (pown 3.F depth)
    let maxDist   = sqrt (3.F * (pown (maxSide / 2.F) 2))

    let v x y z c =
      let v     = Vector3 (x, y, z)
      let s     = Vector3 2.F
      let ratio = v.Length () / maxDist
      let delay = minDelay + random.NextFloat(0.F, 1.F) * ratio * delayVar
      InstanceVertex  ( s * Vector3 (x, y, z)
                      , s * s * randomVector3 ()
                      , randomVector3 ()
                      , delay * Vector3.UnitX
                      , c
                      )

    let ra = ResizeArray<InstanceVertex> boxCount

    let rec menger_cube x y z i =
      if i > 0 then
        let d = pown 3.F (i - 1)
        let i = i - 1
        for xx = -1 to 1 do
          for yy = -1 to 1 do
            for zz = -1 to 1 do
              let a = abs xx + abs yy + abs zz
              if a > 1 then
                let xx = x + d*float32 xx
                let yy = y + d*float32 yy
                let zz = z + d*float32 zz
                menger_cube xx yy zz i
      else
        let c c = 
          let c = abs c / (maxSide / 2.F)
          c*c*c
        let cc = Vector4 (c x, c y, c z, 1.F)
        ra.Add (v x y z cc)

    menger_cube 0.F 0.F 0.F depth

    let vs = ra.ToArray ()

    printfn "Instance count: %d" vs.Length

    vs
#else
  let instanceVertices  =
    use bmp   = new Bitmap ("img.png")

    let h     = bmp.Height
    let w     = bmp.Width
    let d     = maxz - minz + 1

    let trs   = Drawing.Color.FromArgb (0,0,0,0)
    let ins   = Drawing.Color.FromArgb (1,0,0,0)

    let pixels3 = Array3D.init w h d (fun x y z ->
      let c   = bmp.GetPixel (x, y)
      let i   = max (max c.R c.G) c.B
      let rz  = float d * float i / 255.0 |> round |> int
      let tz  = rz
      if (not alphaz || z <= tz) && c.A = 255uy then
        c
      else
        trs
      )

    let isVisible x y z =
      let c = pixels3.[x,y,z]
      c.A > 0uy

    for x = 1 to (w - 2) do
      for y = 1 to (h - 2) do
        for z = 1 to (d - 2) do
          let c = pixels3.[x,y,z]
          let nc=
            if c.A > 0uy then
              let isInside =
                true
                &&  isVisible (x - 1) y z
                &&  isVisible (x + 1) y z
                &&  isVisible x (y - 1) z
                &&  isVisible x (y + 1) z
                &&  isVisible x y (z - 1)
                &&  isVisible x y (z + 1)
              if isInside then
                ins
              else
                c
            else
              c
          pixels3.[x,y,z] <- nc

    let pixels =
      [|
        for x = 0 to w - 1 do
          for y = 0 to h - 1 do
            for z = 0 to (d - 1) do
              let c = pixels3.[x,y,z]
              if (c.A = 255uy) then
                yield struct (x - w / 2 |> float32, y - h / 2 |> float32, z + minz |> float32, c)
      |]

    let maxDist =
      pixels
      |> Seq.map (fun struct (x, y, _, _) -> sqrt (x*x + y*y))
      |> Seq.max

    let min = Vector3 minDelay

    let m c = float32 c / 255.0F

    let s   = Vector3 2.F

    let v x y z (c : Drawing.Color) =
      let ratio = sqrt (x*x + y*y) / maxDist
      let delay = min + random.NextFloat(0.F, 1.F) * ratio * delayVar
      InstanceVertex  ( s * Vector3 (x, y, z)
                      , s * s * randomVector3 ()
                      , randomVector3 ()
                      , delay * Vector3.UnitX
                      , Vector4 (m c.R, m c.G, m c.B, m c.A)
                      )


    let vs =
      [|
        for struct (x, y, z, c) in pixels do
            yield v x y z c
      |]

    printfn "No of instances: %d" vs.Length

    vs
#endif
  do
    GC.Collect (2, GCCollectionMode.Forced)

  member x.Background       = background
  member x.BoxVertices      = boxVertices
  member x.InstanceVertices = instanceVertices


[<AllowNullLiteral>]
type DeviceDependent (dd : DeviceIndependent, rf : Windows.RenderForm) =

  let size              = rf.ClientSize
  let width             = size.Width
  let height            = size.Height
  let widthf            = width  |> float32
  let heightf           = height |> float32
  let aspectRatio       = widthf / heightf

  let viewPort          = Viewport  (Width = width, Height = height, MaxDepth = 1.0F, MinDepth = -1.0F)
  let scissorRect       = Rectangle (Width = width, Height = height)

  let device            = new Direct3D12.Device (null, Direct3D.FeatureLevel.Level_11_0)

  let fence             = device.CreateFence (0L, Direct3D12.FenceFlags.None)
  let fenceEvent        = new AutoResetEvent false
  let mutable fenceGen  = 0L

  let rootSignature     =
    let rps =
      [|
        Direct3D12.RootParameter  ( Direct3D12.ShaderVisibility.Vertex
                                  , Direct3D12.DescriptorRange  ( RangeType                         = Direct3D12.DescriptorRangeType.ConstantBufferView
                                                                , BaseShaderRegister                = 0
                                                                , OffsetInDescriptorsFromTableStart = Int32.MinValue
                                                                , DescriptorCount                   = 1
                                                                )
                                  )
      |]
    let rsd = Direct3D12.RootSignatureDescription ( Direct3D12.RootSignatureFlags.AllowInputAssemblerInputLayout
                                                  , rps
                                                  )
    use ser = rsd.Serialize ()
    let dp  = DataPointer (ser.BufferPointer, int ser.BufferSize)

    device.CreateRootSignature dp

  let shader main compiler =
#if DEBUG
    let flags = D3DCompiler.ShaderFlags.Debug
#else
    let flags = D3DCompiler.ShaderFlags.None
#endif
    use cff = SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile ("shaders.hlsl", main, compiler, flags)
    use bc  = cff.Bytecode
    if bc = null then
      printfn "Shader error: %s" cff.Message
      failwith "Failed to compile shader"
    Direct3D12.ShaderBytecode bc.Data

  let vertexShader      = shader "VSMain" "vs_5_0"
  let pixelShader       = shader "PSMain" "ps_5_0"

  let pipelineState     =
    let aligned = Direct3D12.InputElement.AppendAligned
    let ie name index format offset slot slotClass stepRate =
      Direct3D12.InputElement (name, index, format, offset, slot, slotClass, stepRate)
    let ies =
      [|
        ie "POSITION"   0 DXGI.Format.R32G32B32_Float     0       0 Direct3D12.InputClassification.PerVertexData    0
        ie "NORMAL"     0 DXGI.Format.R32G32B32_Float     aligned 0 Direct3D12.InputClassification.PerVertexData    0
        ie "COLOR"      0 DXGI.Format.R32G32B32A32_Float  aligned 0 Direct3D12.InputClassification.PerVertexData    0
        ie "TEXCOORD"   0 DXGI.Format.R32G32B32_Float     0       1 Direct3D12.InputClassification.PerInstanceData  1
        ie "TEXCOORD"   1 DXGI.Format.R32G32B32_Float     aligned 1 Direct3D12.InputClassification.PerInstanceData  1
        ie "TEXCOORD"   2 DXGI.Format.R32G32B32_Float     aligned 1 Direct3D12.InputClassification.PerInstanceData  1
        ie "TEXCOORD"   3 DXGI.Format.R32G32B32_Float     aligned 1 Direct3D12.InputClassification.PerInstanceData  1
        ie "COLOR"      1 DXGI.Format.R32G32B32A32_Float  aligned 1 Direct3D12.InputClassification.PerInstanceData  1
      |]
    let gpsd = Direct3D12.GraphicsPipelineStateDescription( InputLayout           = Direct3D12.InputLayoutDescription ies
                                                          , RootSignature         = rootSignature
                                                          , VertexShader          = vertexShader
                                                          , PixelShader           = pixelShader
                                                          , RasterizerState       = Direct3D12.RasterizerStateDescription.Default ()
                                                          , BlendState            = Direct3D12.BlendStateDescription.Default ()
                                                          , DepthStencilFormat    = DXGI.Format.D32_Float
                                                          , DepthStencilState     = Direct3D12.DepthStencilStateDescription.Default ()
                                                          , SampleMask            = Int32.MaxValue
                                                          , PrimitiveTopologyType = Direct3D12.PrimitiveTopologyType.Triangle
                                                          , RenderTargetCount     = 1
                                                          , Flags                 = Direct3D12.PipelineStateFlags.None
                                                          , SampleDescription     = DXGI.SampleDescription (1, 0)
                                                          , StreamOutput          = Direct3D12.StreamOutputDescription ()
                                                          )

    gpsd.RenderTargetFormats.[0] <- DXGI.Format.R8G8B8A8_UNorm

    device.CreateGraphicsPipelineState gpsd

  let commandList       = new CommandList (device, pipelineState)

  let swapChain         =
    use f   = new DXGI.Factory4 ()
    let scd = DXGI.SwapChainDescription ( BufferCount       = frameCount
                                        , ModeDescription   = DXGI.ModeDescription (width, height, DXGI.Rational (60, 1), DXGI.Format.R8G8B8A8_UNorm)
                                        , Usage             = DXGI.Usage.RenderTargetOutput
                                        , SwapEffect        = DXGI.SwapEffect.FlipDiscard
                                        , OutputHandle      = rf.Handle
                                        // , Flags          = DXGI.SwapChainsFlags.None
                                        , SampleDescription = DXGI.SampleDescription (1, 0)
                                        , IsWindowed        = rtrue
                                        )

    use sw  = new DXGI.SwapChain (f, commandList.Queue, scd)
    let sw3 = sw.QueryInterface<DXGI.SwapChain3> ()

    sw3

  let createHeap dc tp=
    let dhd = Direct3D12.DescriptorHeapDescription  ( DescriptorCount = dc
                                                    , Flags           = Direct3D12.DescriptorHeapFlags.None
                                                    , Type            = tp
                                                    )

    device.CreateDescriptorHeap dhd

  let renderTargetHeap  = createHeap frameCount  Direct3D12.DescriptorHeapType.RenderTargetView

  let renderTargets     =
    let ds  = device.GetDescriptorHandleIncrementSize Direct3D12.DescriptorHeapType.RenderTargetView
    let rts = Array.zeroCreate frameCount

    let rec loop offset i =
      if i < frameCount then
        rts.[i] <- swapChain.GetBackBuffer<Direct3D12.Resource> i
        device.CreateRenderTargetView (rts.[i], Nullable(), offset)
        loop (offset + ds) (i + 1)
    loop renderTargetHeap.CPUDescriptorHandleForHeapStart 0

    rts

  let depthHeap         = createHeap 1 Direct3D12.DescriptorHeapType.DepthStencilView

  let depthBuffer       =
    let hp  = Direct3D12.HeapProperties Direct3D12.HeapType.Default
    let hf  = Direct3D12.HeapFlags.None
    let rd  = Direct3D12.ResourceDescription.Texture2D (DXGI.Format.D32_Float, int64 width, height, int16 1, int16 0, 1, 0, Direct3D12.ResourceFlags.AllowDepthStencil)
    let rs  = Direct3D12.ResourceStates.DepthWrite
    let db  = device.CreateCommittedResource (hp, hf, rd, rs)

    db

  let depthStencilView  =
    let dsvd  = Direct3D12.DepthStencilViewDescription  ( Format    = DXGI.Format.D32_Float
                                                        , Dimension = Direct3D12.DepthStencilViewDimension.Texture2D
                                                        , Flags     = Direct3D12.DepthStencilViewFlags.None
                                                        )

    let dsv   = device.CreateDepthStencilView (depthBuffer, Nullable dsvd, depthHeap.CPUDescriptorHandleForHeapStart)

    dsv

  let viewState         = new UploadConstantBuffer<_> (device, ViewState ())

  // TODO: Dispose these resources after transferred
  let uploadBox         = new UploadVertexBuffer<_> (device, dd.BoxVertices)
  let uploadInstance    = new UploadVertexBuffer<_> (device, dd.InstanceVertices)

  let defaultBox        = new DefaultVertexBuffer<_> (device, commandList, uploadBox)
  let defaultInstance   = new DefaultVertexBuffer<_> (device, commandList, uploadInstance)

  let populateCommandList (commandList : Direct3D12.GraphicsCommandList) =
    commandList.SetGraphicsRootSignature  rootSignature
    commandList.SetViewport               (rviewPortf viewPort)
    commandList.SetScissorRectangles      [|rrectangle scissorRect|]

    commandList.SetDescriptorHeaps (1, [| viewState.Heap |]);
    commandList.SetGraphicsRootDescriptorTable (0, viewState.Heap.GPUDescriptorHandleForHeapStart)

    let frameIndex  = swapChain.CurrentBackBufferIndex

    commandList.ResourceBarrierTransition (renderTargets.[frameIndex], Direct3D12.ResourceStates.Present, Direct3D12.ResourceStates.RenderTarget)

    let ds          = device.GetDescriptorHandleIncrementSize Direct3D12.DescriptorHeapType.RenderTargetView
    let rtvOffset   = renderTargetHeap.CPUDescriptorHandleForHeapStart + frameIndex*ds
    let depthOffset = depthHeap.CPUDescriptorHandleForHeapStart

    commandList.SetRenderTargets (Nullable rtvOffset, Nullable depthOffset)
    commandList.ClearRenderTargetView (rtvOffset, dd.Background, 0, null)
    commandList.ClearDepthStencilView (depthOffset, Direct3D12.ClearFlags.FlagsDepth, 1.0F, 0uy, 0, null)

    commandList.PrimitiveTopology <- Direct3D.PrimitiveTopology.TriangleList
    commandList.SetVertexBuffers (0, [|defaultBox.View; defaultInstance.View|], 2)
    commandList.DrawInstanced (defaultBox.Length, defaultInstance.Length, 0, 0)

    commandList.ResourceBarrierTransition (renderTargets.[frameIndex], Direct3D12.ResourceStates.RenderTarget, Direct3D12.ResourceStates.Present)

  let waitForPreviousFrame () =
    // TODO: Find other way to await frame
    let localFenceGen = fenceGen
    fenceGen          <-fenceGen + 1L
    commandList.Queue.Signal (fence, localFenceGen)

    while (fence.CompletedValue < localFenceGen) do
      fence.SetEventOnCompletion (localFenceGen, fenceEvent.SafeWaitHandle.DangerousGetHandle ())
      fenceEvent.WaitOne () |> ignore

  member x.Render () =
    try
      commandList.Execute populateCommandList

      swapChain.Present (1, DXGI.PresentFlags.None) |> ignore

      waitForPreviousFrame ()
    with
    | e ->
      let result = device.DeviceRemovedReason
      printfn "Device removed: %A" result
      reraise ()

  member x.Update (timestamp : float32) =
    let viewPosDist   = viewPos.Length ()
    let view          = Matrix.LookAtLH (Vector3 (viewPos.X, viewPos.Y, viewPos.Z), Vector3.Zero, Vector3.Zero - Vector3.UnitY)
    let proj          = Matrix.PerspectiveFovLH (float32 Math.PI / 4.0F, aspectRatio, 0.1F, 2.F*viewPosDist)
    let world         = Matrix.RotationY ((timestamp - minDelay - delayVar) / 12.F)
//    let world         = Matrix.Identity
    let worldViewProj = world * view * proj


    viewState.Data <- ViewState (viewPos, lightningPos, world, worldViewProj, Vector4 timestamp)

  interface IDisposable with
    member x.Dispose () =
      dispose "defaultInstance"     defaultInstance
      dispose "defaultBox"          defaultBox
      dispose "uploadInstance"      uploadInstance
      dispose "uploadBox"           uploadBox
      dispose "viewState"           viewState
      dispose "depthBuffer"         depthBuffer
      dispose "depthHeap"           depthHeap
      for rt in renderTargets do
        dispose "renderTarget"      rt
      dispose "renderTargetHeap"    renderTargetHeap
      dispose "swapChain"           swapChain
      dispose "fenceEvent"          fenceEvent
      dispose "fence"               fence
      dispose "commandList"         commandList
      dispose "pipelineState"       pipelineState
      dispose "rootSignature"       rootSignature
      dispose "device"              device

type App (rf : Windows.RenderForm) =
  let deviceIndependent       = DeviceIndependent ()
  let mutable deviceDependent = null : DeviceDependent

  let uninitialize _ =
    printfn "uninitialize"
    dispose "deviceDependent" deviceDependent
    deviceDependent <- null

  let reinitialize _ =
    printfn "reinitialize"
    dispose "deviceDependent" deviceDependent
    deviceDependent <- null
    deviceDependent <- new DeviceDependent (deviceIndependent, rf)

  do
    rf.SizeChanged.Add      reinitialize
    rf.HandleCreated.Add    reinitialize
    rf.HandleDestroyed.Add  uninitialize

  interface IDisposable with
    member x.Dispose () =
      uninitialize ()

  member x.Initialize () =
    ()

  member x.Update () =
    let timestamp = clock ()
    deviceDependent.Update timestamp
    ()

  member x.Render () =
    deviceDependent.Render ()
    ()

open System.Diagnostics

[<EntryPoint>]
[<STAThread>]
let main argv =
  try
    Environment.CurrentDirectory <- AppDomain.CurrentDomain.BaseDirectory

#if DEBUG
    // Enable the D3D12 debug layer.
    Direct3D12.DebugInterface.Get().EnableDebugLayer ()
#endif


    use rf  = new Windows.RenderForm (Width = 1920, Height = 1200, StartPosition = FormStartPosition.CenterScreen)

    use app = new App (rf)

    rf.Show ()

    app.Initialize ()

    use rl = new Windows.RenderLoop (rf)

    let rec loop () =
      if rl.NextFrame () then
        app.Update ()
        app.Render ()
        loop ()

    loop ()

    0
  with
  | e ->
    printfn "Exception: %s" e.Message
    999
