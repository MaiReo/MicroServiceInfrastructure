using System;

namespace Core.Utilities
{
    public interface IDisposableModel<out T> : IDisposable
    {
        T Model { get; }
    }
}
