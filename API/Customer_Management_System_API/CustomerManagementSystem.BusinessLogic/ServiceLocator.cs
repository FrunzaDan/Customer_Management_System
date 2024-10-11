using System;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerManagementSystem.BusinessLogic;

public static class ServiceLocator
{
    public static IServiceProvider? Instance { get; private set; }

    public static void SetLocatorProvider(IServiceProvider serviceProvider)
    {
        Instance = serviceProvider;
    }

    public static T GetService<T>() where T : class
    {
        return Instance?.GetService<T>() ?? throw new InvalidOperationException("Service provider not initialized.");
    }
}
