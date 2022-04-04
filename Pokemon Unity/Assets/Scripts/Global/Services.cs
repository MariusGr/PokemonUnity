public static class Services
{

    public static T Get<T>() where T : IService
    {
        return ServiceEntry<T>.instance;
    }

    public static void Register<T>(T service) where T : IService
    {
        ServiceEntry<T>.instance = service;
    }

    public static void Unregister<T>(bool destroy = true) where T : IService
    {
        var instance = ServiceEntry<T>.instance;
        ServiceEntry<T>.instance = default(T);
        if (instance != null && destroy && instance is UnityEngine.Object)
            UnityEngine.Object.Destroy(instance as UnityEngine.Object);
    }

    private static class ServiceEntry<T> where T : IService
    {
        public static T instance;

    }

}