using System;
using System.Collections.Generic;

namespace Dekuple.Registry
{
    /// <summary>
    /// Root interface for any registry
    /// </summary>
    public interface IRegistry
        : IPrintable
    {
        IEnumerable<IHasDestroyHandler> Instances { get; }
        int NumInstances { get; }
        bool Has(Guid id);
        bool Resolve();
        bool HasInjector(Type type);
        bool HasInjector<T>();
        void AddSubscriptionsInScene();
    }

    /// <inheritdoc />
    /// <summary>
    /// Interface for a Registry that uses instances that implement
    /// a given interface.
    /// </summary>
    /// <typeparam name="TBase"></typeparam>
    public interface IRegistry<TBase>
        : IRegistry
        where TBase
            : class
            , IHasId
            , IHasDestroyHandler<TBase>
    {
        new IEnumerable<TBase> Instances { get; }

        bool Has(TBase instance);
        TBase Get(Guid id);

        // bind an interface to an implementation, which can be abstract
        bool Bind<TInterface, TImpl>()
            where TInterface : TBase where TImpl : TInterface;

        /// <summary>
        /// Bind an interface to a singleton
        /// </summary>
        /// <typeparam name="TInterface">The query interface</typeparam>
        /// <typeparam name="TImpl">The Concrete type to create</typeparam>
        /// <param name="single">The prefab OR object instance to bind to</param>
        /// <returns>True if bound</returns>
        bool Bind<TInterface, TImpl>(TImpl single)
            where TInterface : TBase where TImpl : TInterface;

        // make a new instance given interface
        TIBase Get<TIBase>(params object[] args)
            where TIBase : class, TBase, IHasRegistry<TBase>, IHasDestroyHandler<TBase>;

        /// <summary>
        /// Perform all dependency injections manually. This is useful for
        /// not objects created using Registry.New. Such as Unit3d Components.
        /// </summary>
        TBase Inject<TIFace>(TBase model);

        TBase Inject(Type type, TBase model);

        /// <summary>
        /// Adds an Id, sets the Registry and adds destroy handler.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        TBase Prepare(TBase model);

        /// <summary>
        /// Internally used by Injection inner-class. Do not touch.
        /// </summary>
        TBase Inject(TBase model, Inject inject, Type iface, TBase single);
    }
}
