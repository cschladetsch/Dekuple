namespace Dekuple
{
    using System;

    public interface IHasSubscriptions
    {
        void Add(IDisposable other);
        // void Add<T>(T disposable) where T : IDisposable;
    }
}