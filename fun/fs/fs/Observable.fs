﻿namespace FolderSize

open System
open System.Diagnostics
open System.Threading
open System.Windows.Forms


module ObservableEx = 
    
    type Observer<'T> =
        {
            OnNext      : 'T -> unit
            OnCompleted : unit -> unit
            OnError     : exn -> unit
        }

        interface IObserver<'T> with
            member this.OnNext t =
                this.OnNext t

            member this.OnCompleted() =
                this.OnCompleted()

            member this.OnError err =
                this.OnError err

        static member New onNext onComplete onError =
            {
                OnNext      = onNext
                OnCompleted = onComplete
                OnError     = onError
            } :> IObserver<'T>

    type Observable<'T> =
        {
            OnSubscribe : IObserver<'T> -> IDisposable
        }

        interface IObservable<'T> with

            member this.Subscribe o =
                this.OnSubscribe o

        member this.Subscribe (onNext: 'T -> unit) (onComplete: unit -> unit) (onError: exn -> unit) =
            let observer = Observer<_>.New onNext onComplete onError
            this.OnSubscribe observer

        static member New onSubscribe = { OnSubscribe = onSubscribe } :> IObservable<'T>

    let terminator onNext onComplete onError (o : IObservable<'T>) = 
        let obs = Observer<_>.New   onNext
                                    onComplete
                                    onError
        o.Subscribe obs

    let deref (o : IObservable<'T option>) : IObservable<'T> =
        Observable<_>.New <| 
            fun observer -> 
                let obs = Observer<_>.New   (fun v  -> match v with 
                                                       | Some vv -> observer.OnNext vv 
                                                       | _ -> ()
                                            )
                                            (fun () -> observer.OnCompleted ())
                                            (fun exn-> observer.OnError exn)

                o.Subscribe obs

    let fold (f : 'U -> 'T -> 'U) (s : 'U) (o : IObservable<'T>) : IObservable<'U> = 
        Observable<_>.New <| 
            fun observer -> 
                let state = ref s

                let obs = Observer<_>.New   (fun v  -> state := f !state v)
                                            (fun () -> observer.OnNext !state; observer.OnCompleted ())
                                            (fun exn-> observer.OnError exn)

                o.Subscribe obs

    let asyncFold (timeout : int) (f : 'U -> 'T -> 'U) (s : 'U) (o : IObservable<'T>) = 
        Observable<_>.New <| 
            fun observer -> 
                let state = ref s

                let actionProcessor = ActionProcessorWithTimeOut (int64 timeout) (fun () -> observer.OnNext !state)

                let obs = Observer<_>.New   (fun v  -> actionProcessor.Post <| fun () -> state := f !state v)
                                            (fun () -> actionProcessor.Post <| fun () -> observer.OnNext !state; observer.OnCompleted ())
                                            (fun exn-> actionProcessor.Post <| fun () -> observer.OnError exn)

                actionProcessor <?+?> o.Subscribe obs

    let async (o : IObservable<'T>) = 
        Observable<_>.New <| 
            fun observer -> 
                let actionProcessor = ActionProcessor ()

                let obs = Observer<_>.New   (fun v  -> actionProcessor.Post <| fun () -> observer.OnNext v)
                                            (fun () -> actionProcessor.Post <| fun () -> observer.OnCompleted ())
                                            (fun exn-> actionProcessor.Post <| fun () -> observer.OnError exn)

                actionProcessor <?+?> o.Subscribe obs

    let asyncTerminator onNext onComplete onError (o : IObservable<'T>) = 
        let actionProcessor = ActionProcessor ()

        let obs = Observer<_>.New   (fun v  -> actionProcessor.Post <| fun () -> onNext v)
                                    (fun () -> actionProcessor.Post <| fun () -> onComplete ())
                                    (fun exn-> actionProcessor.Post <| fun () -> onError exn)

        actionProcessor <?+?> o.Subscribe obs

    let dispatch (c : Control) (o : IObservable<'T>) : IObservable<'T> = 
        let dispatcher = DispatchAction c
        Observable<_>.New <| 
            fun observer -> 
                let obs = Observer<_>.New   (fun v  -> dispatcher <| fun () -> observer.OnNext v)
                                            (fun () -> dispatcher <| fun () -> observer.OnCompleted ())
                                            (fun exn-> dispatcher <| fun () -> observer.OnError exn)
                o.Subscribe obs
                                        

type IObservableSource<'T> = 
    inherit IObservable<'T>
    inherit IDisposable

    abstract member Start       : unit -> unit
    abstract member Next        : 'T -> unit 
    abstract member Completed   : unit -> unit
    abstract member Error       : Exception -> unit

type ObservableSource<'TPayload, 'T>(onStart : IObservableSource<'T> -> 'TPayload, ?onCompleted : 'TPayload -> unit, ?onError : Exception -> unit) =

    [<Literal>]
    let Idle        = 0
    [<Literal>]
    let Running     = 1
    [<Literal>]
    let Finished    = 2

    let state           = ref Idle
    let mutable payload = None
    let key             = ref 0
    let subscriptions   : Map<int, IObserver<'T>> ref = ref Map.empty 

    let oncompleted =   match onCompleted with
                        | Some v    -> v
                        | _         -> fun _ -> ()

    let onerror =   match onError with
                    | Some v    -> v
                    | _         -> fun e -> Debug.Fail <| sprintf "ObservableSource caught exception: %A" e

    let isrunning () = !key = Running

    member this.Start () =  payload <- Some <| onStart this
                            state := Running

    member this.Next v =
        if isrunning () then
            let subs = !subscriptions
            for kv in subs do
                try
                    kv.Value.OnNext v
                with
                | e ->  onerror e

    member this.Completed () =
        let f = Interlocked.Exchange (state, Finished)
        if f = Running then
            try
                let subs = Interlocked.Exchange (subscriptions, Map.empty)
                for kv in subs do
                    try
                        kv.Value.OnCompleted ()
                    with
                    | e ->  onerror e
            finally
                oncompleted payload.Value   // In order to enter running state payload has to be initialized
            

    member this.Error err =
        if isrunning () then
            let subs = !subscriptions
            for kv in subs do
                try
                    kv.Value.OnError err
                with
                | e ->  onerror e

    interface IObservableSource<'T> with
        member this.Start ()        = this.Start ()
        member this.Next v          = this.Next v
        member this.Completed ()    = this.Completed ()
        member this.Error e         = this.Error e
        member this.Subscribe obs   = 
            let k = Interlocked.Increment key
            
            CompareAndExchange (fun m -> Map.add k obs m) subscriptions

            OnExit <| fun () -> CompareAndExchange (fun m -> Map.remove k m) subscriptions

    interface IDisposable with
        member this.Dispose () = TryRun this.Completed

