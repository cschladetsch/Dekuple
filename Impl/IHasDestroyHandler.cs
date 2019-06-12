using System;

namespace Dekuple
{
    public interface IHasDestroyHandler
    {
        /// <summary>
        /// Agents (and other things) can have injected fields or properties.
        /// These cannot be used during construction, so we use `Create` to
        /// be a point of entry that can work with injected and other dependencies.
        ///
        /// Execution order is Create, Begin, AddSubscriptions
        /// </summary>
        bool AddSubscriptions(); // TODO Move to a more appropriate spot

        void Destroy();
    }

    public interface IHasDestroyHandler<out T>
        : IHasDestroyHandler
    {
        event Action<T> OnDestroyed;
    }
}
