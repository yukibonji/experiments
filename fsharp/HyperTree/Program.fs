﻿open System
open System.Drawing
open System.Reflection
open System.Threading
open System.Windows.Forms

open SharpDX

module Common =
  open SharpDX.Mathematics.Interop
  open System.Diagnostics

  let frameCount    = 2

  let background    = Color4(0.1F, 0.1F, 0.1F, 1.F)
//  let background    = Color4(1.F, 1.F, 1.F, 1.F)

  let random        = Random 19740531

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

type ViewState  ( offset        : Vector4
                , world         : Matrix
                , worldViewProj : Matrix
                , timestamp     : Vector4
                ) =
  struct
    member x.Offset         = offset
    member x.World          = world
    member x.WorldViewProj  = worldViewProj
    member x.Timestamp      = timestamp
  end

type Vertex (position : Vector3) =
  struct
    member x.Position = position

    override x.ToString () =
      sprintf "V: %A" position
  end

type InstanceVertex (color : Vector4, position : Vector2, parent : Vector2, size : Vector3) =
  struct
    member x.Color      = color
    member x.Position   = position
    member x.Parent     = parent
    member x.Size       = size

    override x.ToString () =
      sprintf "IV: %A, %A, %A" color position size
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

[<RequireQualifiedAccess>]
type Trie<'Key, 'Value when 'Key : comparison> =
  | Node  of 'Value list*Map<'Key, Trie<'Key, 'Value>>

  static member Empty = Node (List.empty, Map.empty)

  member x.Update (ks : _ []) u =
    let e = Trie<'Key, 'Value>.Empty
    let rec loop t i =
      let (Node (vs, m)) = t
      if i < ks.Length then
        let k = ks.[i]
        let nt = 
          match Map.tryFind k m with
          | Some nt -> loop nt (i + 1)
          | None    -> loop e  (i + 1)
        let nm = Map.add k nt m
        Node (vs, nm)
      else
        Node (u vs, m)
    loop x 0

  member x.Lookup (ks : _ []) =
    let rec loop t i =
      let (Node (vs, m)) = t
      if i < ks.Length then
        let k = ks.[0]
        match Map.tryFind k m with
        | Some nt -> loop nt (i + 1)
        | None    -> List.empty
      else
        vs
    loop x 0

type DeviceIndependent () =
  let background   = background |> rcolor4

  let boxVertices  =
    [|
      // The box
      let w   = 4
      let h   = 4
      let dx  = 1.F / float32 w
      let dy  = 1.F / float32 h

      let v x y = Vertex (Vector3 (x, y, 0.F))
      for y = 0 to (h - 1) do
        for x = 0 to (w - 1) do
//          x     y    
          let l = -0.5F + float32 x*dx
          let t = -0.5F + float32 y*dy
          let r = l + dx
          let b = t + dy
          yield v l t
          yield v l b
          yield v r b
          yield v l t
          yield v r b
          yield v r t

      // The line
      let w   = 20
      let h   = 1
      let dx  = 1.F / float32 w
      let dy  = 1.F / float32 h

      let v x y = Vertex (Vector3 (x, y, 1.F))
      for y = 0 to (h - 1) do
        for x = 0 to (w - 1) do
//          x     y    
          let l = 0.F + float32 x*dx
          let t = -0.5F + float32 y*dy
          let r = l + dx
          let b = t + dy
          yield v l t
          yield v l b
          yield v r b
          yield v l t
          yield v r b
          yield v r t
    |]

  let instanceVertices =
    let split s = if s <> null then (s : string).Split '.' else [||]
    let types = 
      AppDomain.CurrentDomain.GetAssemblies ()
      |> Array.collect  (fun a -> a.GetTypes ())
      |> Array.filter   (fun t -> t.IsNested |> not)
      |> Array.map      (fun t -> split t.Namespace, t )
   
    printfn "Types: %d" types.Length

    let trie = 
      let folder (s : Trie<_, _>) (ns, t) = s.Update ns (fun ts -> t::ts)
      types |> Array.fold folder Trie<_, _>.Empty

//    printfn "%A" trie

    let fromPolar r a = Vector2 (r * cos a |> float32, r * sin a |> float32)

    let tau   = 2. * Math.PI

    let size  = 0.04

    let v x y px py i =
      let i   = i % 6 + 1
      let c b = if i &&& (1 <<< b) <> 0 then 1.F else 0.F
      InstanceVertex  ( Vector4 (c 2, c 1, c 0, 1.F)
                      , Vector2 (x, y)
                      , Vector2 (px, py)
                      , Vector3 (size |> float32, size |> float32, size / 4. |> float32)
                      )

    let ra = ResizeArray 16

    let generateTree md trie =
      let rec oloop f t r px py d (Trie.Node (vs, cs)) =
        let cs  = cs |> Seq.toArray
        let aa  = (t - f) / float cs.Length
        let m   = size * sqrt 2.
        let ir  = max m (m * float cs.Length / (t - f) - r)
        let r   = r + ir
        let rec iloop i =
          if i < cs.Length then
            let kv  = cs.[i]
            let c   = kv.Value
            let ff  = f + aa * float i
            let tt  = ff + aa
            let c   = fromPolar r (ff + (tt - ff) / 2.)
            ra.Add <| v c.X c.Y px py d
            oloop ff tt r c.X c.Y (d + 1) kv.Value
            iloop (i + 1)
        if d < md then iloop 0
      oloop 0. tau 0. 0.F 0.F 0 trie

    generateTree 1000 trie

    let vs = ra.ToArray ()
    printfn "Instance count: %d" vs.Length

    vs

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
        ie "COLOR"      1 DXGI.Format.R32G32B32A32_Float  0       1 Direct3D12.InputClassification.PerInstanceData  1
        ie "TEXCOORD"   1 DXGI.Format.R32G32_Float        aligned 1 Direct3D12.InputClassification.PerInstanceData  1
        ie "TEXCOORD"   2 DXGI.Format.R32G32_Float        aligned 1 Direct3D12.InputClassification.PerInstanceData  1
        ie "TEXCOORD"   3 DXGI.Format.R32G32B32A32_Float  aligned 1 Direct3D12.InputClassification.PerInstanceData  1
      |]
    let mutable rsd  = Direct3D12.RasterizerStateDescription.Default ()
    rsd.IsMultisampleEnabled  <- rtrue
    rsd.IsAntialiasedLineEnabled <- rtrue
    let gpsd = Direct3D12.GraphicsPipelineStateDescription( InputLayout           = Direct3D12.InputLayoutDescription ies
                                                          , RootSignature         = rootSignature
                                                          , VertexShader          = vertexShader
                                                          , PixelShader           = pixelShader
                                                          , RasterizerState       = rsd
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

  let createHeap dc tp  =
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

  member val Offset = Vector2.Zero with get, set

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
    let view          = Matrix.LookAtLH (Vector3 (0.F, 0.F, -5.F), Vector3.Zero, Vector3.Zero - Vector3.UnitY)
    let proj          = Matrix.OrthoLH (2.F*aspectRatio, 2.F, -10.F, 10.F)
    let world         = Matrix.Identity
//    let worldViewProj = world * view * proj
//    let worldViewProj = Matrix.Identity
    let worldViewProj = proj

    let offset        = Vector4 (x.Offset.X, x.Offset.Y, 0.F, 1.F)

    viewState.Data <- ViewState (offset, world, worldViewProj, Vector4 timestamp)

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

  let mutable speed   = Vector2.Zero
  let mutable offset  = Vector2.Zero

  let mutable before  = clock ()

  let keyUp (a : KeyEventArgs)  =
    let ns =
      match a.KeyCode with
      | Keys.Left   -> speed + Vector2.UnitX  |> Some
      | Keys.Right  -> speed - Vector2.UnitX  |> Some
      | Keys.Up     -> speed - Vector2.UnitY  |> Some
      | Keys.Down   -> speed + Vector2.UnitY  |> Some
      | Keys.Space  -> Vector2.Zero           |> Some
      | _           -> None

    match ns with
    | Some ns -> speed <- ns
    | None    -> ()
    

  do
    rf.SizeChanged.Add      reinitialize
    rf.HandleCreated.Add    reinitialize
    rf.HandleDestroyed.Add  uninitialize
    rf.KeyUp.Add            keyUp

  interface IDisposable with
    member x.Dispose () =
      uninitialize ()

  member x.Initialize () =
    ()

  member x.Update () =
    let timestamp           = clock ()
    let diff                = timestamp - before
    offset                  <- offset + diff*speed
    deviceDependent.Offset  <- offset
    deviceDependent.Update timestamp
    before <- timestamp

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
