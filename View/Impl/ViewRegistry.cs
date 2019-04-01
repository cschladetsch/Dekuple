using UnityEngine;

namespace Dekuple.View.Impl
{
    using Agent;
    using Model;
    using Registry;

    /// <inheritdoc cref="Registry" />
    /// <summary>
    /// A registry of views. A view is a unity3d-space representation of
    /// and Agent which in turn has a Model.
    /// This is similar to the MVVM/MVC pattern, but extended so that the
    /// 'ViewController' or 'Controller' is an Agent, which has behavior
    /// defined over time since each Agent is a node in a Flow.Kernel
    /// process graph.
    ///
    /// Of course, you can use your own ViewRegistry that doesn't use
    /// Dekuple.IViewBase. But this is a good start and suitable for
    /// most use-cases.
    /// </summary>
    public class ViewRegistry
        : Registry<IViewBase>
        , IViewRegistry
    {
        public override IViewBase Prepare(IViewBase view)
        {
            return base.Prepare(view);
        }

        public void InjectAllGameObjects()
        {
            foreach (var view in Object.FindObjectsOfType<ViewBase>())
                InjectGameObject(view);
        }

        /// <summary>
        /// Inject and prepare GameObjects that reside in the scene
        /// that have not been previously bound using Bind&lt;T&gt;
        /// </summary>
        /// <param name="view"></param>
        public void InjectGameObject(IViewBase view)
        {
            var injections = new Injections(this, view.GetType());
            injections.Inject(view);
            Prepare(view);
        }

        public override bool Bind<TInterface, TImpl>(TImpl single)
        {
            Assert.IsNotNull(single);
            var inScene = single.GameObject.scene.IsValid();
            if (!inScene)
            {
                single = Object.Instantiate(single.GameObject).GetComponent<TImpl>();
            }
            return base.Bind<TInterface, TImpl>(single);
        }

        public TIView FromPrefab<TIView>(Object prefab)
            where TIView : class, IViewBase
        {
            Assert.IsNotNull(prefab);
            var view = Object.Instantiate(prefab) as TIView;
            Assert.IsNotNull(view);
            return Prepare(Prepare(typeof(TIView), view)) as TIView;
        }

        public TIView FromPrefab<TIView, TIAgent>(Object prefab, IRegistry<TIAgent> agents)
            where TIView : class, IViewBase
            where TIAgent : class, IAgent, IHasDestroyHandler<TIAgent>, IHasRegistry<TIAgent>
        {
            var view = FromPrefab<TIView>(prefab);
            Assert.IsNotNull(view);
            var agent = agents.Get<TIAgent>();
            view.SetAgent(agent);
            Assert.IsTrue(view.IsValid);
            return view;
        }

        public TIView FromPrefab<TIView, TIAgent, TIModel>(Object prefab, IRegistry<TIModel> models, IRegistry<TIAgent> agents = null)
            where TIView : class, IViewBase
            where TIAgent : class, IAgent, IHasDestroyHandler<TIAgent>, IHasRegistry<TIAgent>
            where TIModel : class, IModel, IHasDestroyHandler<TIModel>, IHasRegistry<TIModel>
        {
            var view = FromPrefab<TIView>(prefab);
            Assert.IsNotNull(view);
            var model = models.Get<TIModel>();
            if (agents != null)
            {
                var agent = agents.Get<TIAgent>(model);
                view.SetAgent(agent);
            }
            view.SetModel(model);
            Assert.IsTrue(view.IsValid);
            return view;
        }

        [System.Obsolete("This is a remnant from first usage in Chess2")]
        public TIView FromPrefab<TIView, TIAgent, TModel>(IViewBase viewBase, Object prefab, TModel model)
            where TIView : class , IViewBase
            where TIAgent : class, IAgent, IHasDestroyHandler<IAgent>
            where TModel : IModel
        {
            var view = FromPrefab<TIView>(prefab);
            Assert.IsNotNull(view);
            var agent = viewBase.AgentBase.Registry.Get<TIAgent>(model);
            view.SetAgent(agent);
            view.SetModel(model);
            Assert.IsTrue(view.IsValid);
            return view;
        }
    }
}
