namespace Avalonia
{
    public interface IAvaloniaLocator : Avalonia.IAvaloniaDependencyResolver
    {
        IRegistrationHelper<T> Bind<T>();

        IAvaloniaLocator BindToSelf<T>(T constant);

        IAvaloniaLocator BindToSelfSingleton<T>()
            where T : class, new();
    }
}
