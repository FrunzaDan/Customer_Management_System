using Microsoft.Extensions.DependencyInjection;

namespace CustomerManagementSystem.BusinessLogic;

public static class ServiceLocator
{
    private static readonly object Lock = new();
    private static IServiceProvider? _instance;

    private static IServiceProvider Instance
    {
        get => _instance ?? throw new InvalidOperationException("Service provider not initialized.");
        set
        {
            lock (Lock)
            {
                if (_instance != null)
                    throw new InvalidOperationException(
                        "Service provider has already been set and cannot be modified.");
                _instance = value;
            }
        }
    }

    public static void SetLocatorProvider(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        Instance = serviceProvider;
    }

    public static T GetService<T>() where T : class
    {
        return Instance.GetService<T>() ??
               throw new InvalidOperationException($"Service of type {typeof(T).Name} not found.");
    }
}