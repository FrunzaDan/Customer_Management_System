using Microsoft.Extensions.DependencyInjection;

namespace CustomerManagementSystem.BusinessLogic
{
    public static class ServiceLocator
    {
        private static readonly object Lock = new();
        private static IServiceProvider? _instance;

        private static IServiceProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException("Service provider not initialized.");
                }
                return _instance;
            }
            set
            {
                lock (Lock)
                {
                    if (_instance != null)
                    {
                        throw new InvalidOperationException("Service provider has already been set and cannot be modified.");
                    }

                    _instance = value ?? throw new ArgumentNullException(nameof(value), "Service provider cannot be null.");
                }
            }
        }

        public static void SetLocatorProvider(IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider);
            Instance = serviceProvider;
        }

        public static T GetServiceFromServiceProvider<T>() where T : class
        {
            var service = Instance.GetService<T>();
            return service ?? throw new InvalidOperationException($"Service of type {typeof(T).Name} not found.");
        }
    }
}