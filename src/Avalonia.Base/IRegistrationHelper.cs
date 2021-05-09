using System;

#pragma warning disable CS1591 // Enable me later

namespace Avalonia
{
    public interface IRegistrationHelper<TService>
    {
        AvaloniaLocator ToConstant<TImpl>(TImpl constant) where TImpl : TService;
        AvaloniaLocator ToFunc<TImlp>(Func<TImlp> func) where TImlp : TService;
        AvaloniaLocator ToLazy<TImlp>(Func<TImlp> func) where TImlp : TService;
        AvaloniaLocator ToSingleton<TImpl>() where TImpl : class, TService, new();
        AvaloniaLocator ToTransient<TImpl>() where TImpl : class, TService, new();
    }

}

