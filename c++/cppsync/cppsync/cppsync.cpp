#include "stdafx.h"

#include <functional>
#include <stdio.h>
#include <string>
#include <type_traits>

struct empty
{

};

enum class cancel_reason
{
    unknown  = 0x0001   ,
};

struct async_context
{

};

template<typename TValue>
using on_value      = std::function<void (TValue &&)>           ;
using on_error      = std::function<void (std::exception const &)>;
using on_cancelled  = std::function<void (cancel_reason)>       ;

template<typename TValue>
using async             = std::function<void (
    async_context       &       ,
    on_value<TValue>    const & ,
    on_error            const & ,
    on_cancelled        const & )>;


template<typename TA, typename TB>
using binder    = std::function<async<TB> (TA &&)>;

namespace details
{
    template<typename T>
    struct deduce_async_type;
    
    template<typename T>
    struct deduce_async_type<async<T>>
    {
        using type = T;
    };


    template<typename T>
    struct is_async
    {
        enum
        {
            value   = false ,
        };
    };
    
    template<typename T>
    struct is_async<async<T>>
    {
        enum
        {
            value   = true ,
        };
    };


    template<typename TA, typename TBinder>
    struct deduce_binder_return_type
    {
        static TA       get_a ();
        static TBinder  get_binder ();

        using type          = decltype (get_binder () (get_a ())) ;
        static_assert (is_async<type>::value, "Return type must be async");
//        using async_type    = typename std::remove_reference<deduce_async_type<type>>::type       ;
    };
}

template<typename TValue>
async<TValue> async_return (TValue && value)
{
    return
        [iv = std::forward<TValue> (value)]
        (async_context & ctx, on_value<TValue> const & ov, on_error const & oe, on_cancelled const & oc) mutable
        {
            ov (std::move (iv));
        };
}

template<typename TA, typename TBinder>
typename details::deduce_binder_return_type<TA, TBinder>::type async_bind (async<TA> a, TBinder && binder)
{
    return
        [ia = std::move (a), ibinder = std::forward<TBinder> (binder)]
        (async_context & ctx, on_value<TA> const & ov, on_error const & oe, on_cancelled const & oc) mutable
        {
            auto fov = [&ctx, &ov, &oe, &oc, iibinder = std::move (ibinder)] (TA && v)
            {
                auto b = iibinder (std::move (v));
                b (ctx, ov, oe, oc);
            };

            ia (ctx, fov, oe, oc);
        };
}

int main ()
{
    auto a = async_return (std::string ("Test"));
    auto b = async_bind (a, [] (auto a) { return async_return ("Testing_" + a); });

    async_context ctx;

    auto on_string = [] (std::string && v)
    {
        printf ("Value: %s\n", v.c_str ());
    };

    auto on_error = [] (std::exception const & e)
    {
        printf ("Exception: %s\n", e.what ());
    };

    auto on_cancelled = [] (cancel_reason cr)
    {
        printf ("Cancelled: %d\n", cr);
    };

    b (ctx, on_string, on_error, on_cancelled);

    return 0;
}