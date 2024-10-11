using System;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerManagementSystem.BusinessLogic
{
    public static class ServiceLocator
    {
        private static IServiceProvider? _instance;

        public static IServiceProvider Instance
        {
            get => _instance ?? throw new InvalidOperationException("Service provider not initialized.");
            set => _instance = value;
        }

        public static void SetLocatorProvider(IServiceProvider serviceProvider)
        {
            Instance = serviceProvider;
        }

        public static T GetService<T>() where T : class
        {
            return Instance?.GetService<T>() ?? throw new InvalidOperationException("Service provider not initialized.");
        }
    }
}