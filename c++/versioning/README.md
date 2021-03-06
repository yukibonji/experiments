C++ versioning with namespaces
==============================

I have a challenge.

1. To provide an API that is binary backwards compatible
    *   A new binary should be usable by an application compiled against an older version of the API
2. Different versions of the API should be separated using namespaces
    * Such v1,v2 and so on
3. Minimize the workload maintaing backwards compatible interfaces


One _easy_ way forward would be to increment the whole API whenever we introduce a breaking change

```cpp
namespace my_api
{
    namespace v1
    {
        // The whole v1 API
    }
}

namespace my_api
{
    namespace v2
    {
        // The whole v2 API
    }
}

```


The drawback with this approach is that this creates a big workload when we make a small breaking change
in one part of the API that isn't related to the other components. I then need to create a new version of
all non-updated components as well and build adapters between different versions. It quickly becomes
extremely cumbersome.

The API consists of different components that aren't directly related (but belongs to the same API).
I like to version these independently.

```cpp
namespace my_api
{
    namespace common
    {
        namespace v1
        {
            // common types
        }
        namespace v2
        {
            // common types
        }
    }
}

namespace my_api
{
    namespace component_1
    {
        namespace v1
        {
            // component 1 API
        }
        namespace v2
        {
            // component 1 API
        }
    }
}

namespace my_api
{
    namespace component_2
    {
        namespace v1
        {
            // component 2 API
        }
        namespace v2
        {
            // component 2 API
        }
    }
}

```

However this creates a bit of a burden on the users as they mostly likely would just want to use the latest
or a specific release. With this approach without some mitigating code the users would have very carefully
cherry-pick what versions to use. Not good.


An idea I had then is to create special api versions using namespaces

```cpp

namespace my_api
{
    namespace v1
    {
        using namespace my_api::common:v1           ;
        using namespace my_api::component_1:v1      ;
        using namespace my_api::component_2:v1      ;
    }
    namespace v2
    {
        using namespace my_api::common:v1           ;
        using namespace my_api::component_1:v2      ;
        using namespace my_api::component_2:v1      ;
    }
    namespace v3
    {
        using namespace my_api::common:v2           ;
        using namespace my_api::component_1:v2      ;
        using namespace my_api::component_2:v2      ;
    }
}

````

Then the users would use using aliases or other techniques to use the preferred API.

My testing indicates that this seems to work but I am not sure if the using namespace technique
inside a namespace is c++ kosher.

I am also wondering if there's a really simple solution which I fail to see (I do have some constraints
from my environment which limits what I am allowed to do).

Any ideas?


