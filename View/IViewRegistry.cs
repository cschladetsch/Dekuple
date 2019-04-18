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
        TIView FromPrefab<TIView>(Object prefab)
            where TIView : class, IViewBase;

        TIView FromPrefab<TIView>(Object prefab, Transform parent)
            where TIView : class, IViewBase;

        TIView FromPrefab<TIView, TIAgent>(Object prefab, IRegistry<TIAgent> agents)
            where TIView : class, IViewBase
            where TIAgent : class, IAgent, IHasDestroyHandler<TIAgent>, IHasRegistry<TIAgent>;

        //TIView FromPrefab<TIView, TIAgent, TIModel>(Object prefab, IRegistry<TIModel> models,
        //    IRegistry<TIAgent> agents = null)
        //    where TIView : class, IViewBase
        //    where TIAgent : class, IAgent, IHasDestroyHandler<IAgent>, IHasRegistry<IAgent>
        //    where TIModel : class, IModel, IHasDestroyHandler<IModel>, IHasRegistry<IModel>;

        //TIView FromPrefab<TIView, TIAgent, TModel>(
        //    IViewBase owner, Object prefab, TModel model)
        //    where TIView : class, IViewBase
        //    where TIAgent : class, IAgent, IHasDestroyHandler<IAgent>
        //    where TModel : IModel;
        void InjectView(IViewBase view);
        void InjectViewsInScene();

        TIView FromPrefab<TIView>(Object prefab, IAgent agent)
            where TIView : class, IViewBase;

        TIView FromPrefab<TIView, TIAgent, TIModel>(Object prefab, IRegistry<TIModel> models, IRegistry<TIAgent> agents = null)
            where TIView : class, IViewBase
            where TIAgent : class, IAgent, IHasDestroyHandler<TIAgent>, IHasRegistry<TIAgent>
            where TIModel : class, IModel, IHasDestroyHandler<TIModel>, IHasRegistry<TIModel>;
    }
}
