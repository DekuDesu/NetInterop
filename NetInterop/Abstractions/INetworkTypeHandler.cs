using System;

namespace NetInterop
{
    public interface INetworkTypeHandler
    {
        INetworkType<T> GetNetworkType<T>(INetPtr<T> id);
        INetworkType<T> GetNetworkType<T>();
        INetworkType GetAmbiguousNetworkType(INetPtr id);

        [System.Obsolete("Use RegisterType<T> instead for type safety if available. This is remains for niche compatibility.")]
        ushort RegisterType(Type type);
        [System.Obsolete("Use RegisterType<T> instead for type safety if available. This is remains for niche compatibility.")]
        ushort RegisterType(Type type, ushort explicitId);
        [System.Obsolete("Use RegisterType<T> instead for type safety if available. This is remains for niche compatibility.")]
        ushort RegisterType(Type type, ushort explicitId, object instantiator);


        ushort RegisterType<T>(ushort explicitId);
        ushort RegisterType<T>(Func<T> instantiator);
        ushort RegisterType<T>(Action<T> disposer);
        ushort RegisterType<T>(Func<T> instantiator, Action<T> disposer);
        ushort RegisterType<T>(ushort explicitId, Func<T> instantiator, Action<T> disposer);

        void Clear();
    }
}