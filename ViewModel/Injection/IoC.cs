namespace DoenaSoft.FolderSize.ViewModel.Injection;

public static class IoC
{
    private const string Default = "Default";

    private static readonly object _threadLock;

    private static readonly Dictionary<InjectionKey, object> _container;

    static IoC()
    {
        _container = new Dictionary<InjectionKey, object>();
        _threadLock = new object();
    }

    public static void RegisterSingleton<T>(T instance)
    {
        var key = GetKey<T>(Default);

        lock (_threadLock)
        {
            if (GetInstances<T>().Any())
            {
                throw new ArgumentException($"There is an object of this type '{typeof(T).FullName}' already!");
            }

            _container.Add(key, instance);
        }
    }

    public static void RegisterSingleton<T>() where T : new() => RegisterSingleton(new T());

    public static IEnumerable<T> ResolveAll<T>()
    {
        lock (_threadLock)
        {
            var instances = GetInstances<T>();

            return instances;
        }
    }

    public static void RegisterInstance<T>(T instance) => RegisterInstance(instance, Default);

    public static void RegisterInstance<T>(T instance, string id)
    {
        var key = GetKey<T>(id);

        lock (_threadLock)
        {
            _container.Add(key, instance);
        }
    }

    public static T Resolve<T>()
    {
        lock (_threadLock)
        {
            if (TryResolve<T>(Default, out var instance))
            {
                return instance;
            }
            else
            {
                throw new Exception($"IoC could not resolve type '{typeof(T).FullName}'");
            }
        }
    }

    public static T Resolve<T>(string id)
    {
        lock (_threadLock)
        {
            if (TryResolve<T>(id, out var instance))
            {
                return instance;
            }
            else
            {
                throw new Exception($"IoC could not resolve type '{typeof(T).FullName}' with Id '{id}'");
            }
        }
    }

    public static bool TryResolve<T>(out T instance)
    {
        lock (_threadLock)
        {
            var success = TryResolve(Default, out instance);

            return success;
        }
    }

    public static bool TryResolve<T>(string id, out T instance)
    {
        var key = GetKey<T>(id);

        lock (_threadLock)
        {
            if (_container.TryGetValue(key, out var temp))
            {
                instance = (T)temp;

                return true;
            }
            else
            {
                instance = default;

                return false;
            }
        }
    }

    public static void Remove<T>() => Remove<T>(Default);

    public static void Remove<T>(string id)
    {
        var key = GetKey<T>(id);

        lock (_threadLock)
        {
            _container.Remove(key);
        }
    }

    private static InjectionKey GetKey<T>(string id) => new InjectionKey(typeof(T), id);

    private static List<T> GetInstances<T>()
    {
        var matches = _container.Where(kvp => kvp.Key.Type.Equals(typeof(T))).ToList();

        var objects = matches.Select(kvp => kvp.Value).ToList();

        var instances = objects.Cast<T>().ToList();

        return instances;
    }
}