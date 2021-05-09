using System;

namespace Avalonia
{
    public interface IAvaloniaDependencyResolver
    {
        object GetService(Type t);
    }
}

