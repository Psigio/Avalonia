using System;
using System.Collections.Generic;

#pragma warning disable CS1591 // Enable me later

namespace Avalonia
{
    public partial class AvaloniaLocator : IAvaloniaLocator
    {
        private readonly IAvaloniaDependencyResolver _parentScope;
        public static IAvaloniaDependencyResolver Current { get; set; }
        public static IAvaloniaLocator CurrentMutable { get; set; }
        private readonly Dictionary<Type, Func<object>> _registry = new Dictionary<Type, Func<object>>();

        static AvaloniaLocator()
        {
            Current = CurrentMutable = new AvaloniaLocator();
        }

        public AvaloniaLocator()
        {

        }

        public AvaloniaLocator(IAvaloniaDependencyResolver parentScope)
        {
            _parentScope = parentScope;
        }

        public object GetService(Type t)
        {
            Func<object> rv;
            return _registry.TryGetValue(t, out rv) ? rv() : _parentScope?.GetService(t);
        }

        public class RegistrationHelper<TService> : IRegistrationHelper<TService>
        {
            private readonly AvaloniaLocator _locator;

            public RegistrationHelper(AvaloniaLocator locator)
            {
                _locator = locator;
            }

            public AvaloniaLocator ToConstant<TImpl>(TImpl constant) where TImpl : TService
            {
                _locator._registry[typeof(TService)] = () => constant;
                return _locator;
            }

            public AvaloniaLocator ToFunc<TImlp>(Func<TImlp> func) where TImlp : TService
            {
                _locator._registry[typeof(TService)] = () => func();
                return _locator;
            }

            public AvaloniaLocator ToLazy<TImlp>(Func<TImlp> func) where TImlp : TService
            {
                var constructed = false;
                TImlp instance = default;
                _locator._registry[typeof(TService)] = () =>
               {
                   if (!constructed)
                   {
                       instance = func();
                       constructed = true;
                   }

                   return instance;
               };
                return _locator;
            }

            public AvaloniaLocator ToSingleton<TImpl>() where TImpl : class, TService, new()
            {
                TImpl instance = null;
                return ToFunc(() => instance ?? (instance = new TImpl()));
            }

            public AvaloniaLocator ToTransient<TImpl>() where TImpl : class, TService, new() => ToFunc(() => new TImpl());
        }

        public IRegistrationHelper<T> Bind<T>() => new RegistrationHelper<T>(this);

        public IAvaloniaLocator BindToSelf<T>(T constant)
            => Bind<T>().ToConstant(constant);

        public IAvaloniaLocator BindToSelfSingleton<T>() where T : class, new() => Bind<T>().ToSingleton<T>();

        class ResolverDisposable : IDisposable
        {
            private readonly IAvaloniaDependencyResolver _resolver;
            private readonly IAvaloniaLocator _mutable;

            public ResolverDisposable(IAvaloniaDependencyResolver resolver, IAvaloniaLocator mutable)
            {
                _resolver = resolver;
                _mutable = mutable;
            }

            public void Dispose()
            {
                Current = _resolver;
                CurrentMutable = _mutable;
            }
        }

        public static IDisposable EnterScope()
        {
            var d = new ResolverDisposable(Current, CurrentMutable);
            Current = CurrentMutable = new AvaloniaLocator(Current);
            return d;
        }
    }

    public static class LocatorExtensions
    {
        public static T GetService<T>(this IAvaloniaDependencyResolver resolver)
        {
            return (T)resolver.GetService(typeof(T));
        }
    }
}

