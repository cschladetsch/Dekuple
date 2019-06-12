using System;
using UniRx;

namespace Dekuple
{
    public interface IHasDestroyHandler
    {
        /// <summary>
        /// Agents (and other things) can have injected fields or properties.
        /// These cannot be used during construction, so we use `Create` to
        /// be a point of entry that can work with injected and other dependencies.
        /// </summary>
        void AddSubscriptions();

        void Destroy();
    }

    public interface IHasDestroyHandler<out T>
        : IHasDestroyHandler
    {
        event Action<T> OnDestroyed;
    }
}
