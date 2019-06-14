using Dekuple.Model;
using Dekuple.View.Impl;
using UnityEngine;

namespace Dekuple.View
{
    using Agent;
    using Registry;

    /// <inheritdoc />
    /// <summary>
    /// Common registry for all objects that are in the Unity3d scene (or canvas)
    /// </summary>
    public interface IViewRegistry
        : IRegistry<IViewBase>
    {
        /// <summary>
        /// Bind an interface to a singleton and set its agent.
        /// </summary>
        /// <typeparam name="TInterface">The query interface</typeparam>
        /// <typeparam name="TImpl">The Concrete type to create</typeparam>
        /// <param name="single">The prefab OR object instance to bind to</param>
        /// <returns>True if bound</returns>
        bool Bind<TInterface, TImpl>(TImpl single, IAgent agent)
            where TInterface : class, IViewBase where TImpl : TInterface;

        /*
           public static Object Instantiate(Object original);
           public static Object Instantiate(Object original, Transform parent);
           public static Object Instantiate(Object original, Transform parent, bool instantiateInWorldSpace);
           public static Object Instantiate(Object original, Vector3 position, Quaternion rotation);
           public static Object Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent);
         */

        TIView FromPrefab<TIView>(Object prefab)
            where TIView : class, IViewBase;

        TIView FromPrefab<TIView>(Object prefab, Transform parent)
            where TIView : class, IViewBase;

        TIView FromPrefab<TIView>(Object prefab, Transform parent, bool instantiateInWorldSpace)
            where TIView : class, IViewBase;

        TIView FromPrefab<TIView>(Object prefab, Transform parent, bool instantiateInWorldSpace, IAgent agent)
            where TIView : class, IViewBase;

        TIView FromPrefab<TIView, TIAgent>(Object prefab, IRegistry<TIAgent> agents) // DK TODO should TIAgent be IAgent?
            where TIView : class, IViewBase
            where TIAgent : class, IAgent, IHasDestroyHandler<TIAgent>, IHasRegistry<TIAgent>;

        TIView FromPrefab<TIView>(Object prefab, IAgent agent)
            where TIView : class, IViewBase;

        TIView FromPrefab<TIView>(Object prefab, Transform parent, IAgent agent)
            where TIView : class, IViewBase;

        void InjectView(IViewBase view);
        void InjectAllViews();
    }
}
