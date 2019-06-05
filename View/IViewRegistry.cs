using UnityEngine;

namespace Dekuple.View
{
    using Agent;
    using Model;
    using Registry;

    /// <inheritdoc />
    /// <summary>
    /// Common registry for all objects that are in the Unity3d scene (or canvas)
    /// </summary>
    public interface IViewRegistry
        : IRegistry<IViewBase>
    {
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

        TIView FromPrefab<TIView, TIAgent>(Object prefab, IRegistry<TIAgent> agents)
            where TIView : class, IViewBase
            where TIAgent : class, IAgent, IHasDestroyHandler<TIAgent>, IHasRegistry<TIAgent>;

        TIView FromPrefab<TIView>(Object prefab, IAgent agent)
            where TIView : class, IViewBase;

        TIView FromPrefab<TIView>(Object prefab, Transform parent, IAgent agent)
            where TIView : class, IViewBase;

        void InjectView(IViewBase view);
        void InjectViewsInScene();
    }
}
