using System;
using System.Collections;
using System.Linq;
using Flow;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Dekuple.View.Impl
{
    using Agent;
    using Model;

    public abstract class MainBase
        : ViewBase
    {
        public IModelRegistry Models { get; } = new ModelRegistry();
        public IAgentRegistry Agents { get; } = new AgentRegistry();
        public IViewRegistry Views { get; } = new ViewRegistry();

        private static MainBase _instance;

        protected override bool Create()
        {
            if (!base.Create())
                return false;

            if (_instance == null)
                _instance = this;
            else
            {
                Warn($"Duplicate instance of type {GetType().Name}. Destroying new instance.");
                Destroy(gameObject);
                return false;
            }
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            CreateBindings();
            ResolveBindings();
            SceneManager.sceneLoaded += OnSceneLoaded;
            Agents.Kernel.Root.Resume();

            return true;
        }

        private void ResolveBindings()
        {
            Models.Resolve();
            Agents.Resolve();
            Views.Resolve();
        }

        protected override void Step()
        {
            base.Step();
            Agents.Kernel.Step();
        }

        protected void ResolveScene()
        {
            Views.InjectAllViews();
            Models.AddSubscriptionsInScene();
            Agents.AddSubscriptionsInScene();
            Views.AddSubscriptionsInScene();
        }

        protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            IEnumerator LateSceneLoad(IGenerator self)
            {
                yield return null;
                ResolveScene();
            }

            var kernel = Agents.Kernel;
            kernel.Factory.Coroutine(LateSceneLoad).AddTo(kernel.Root);
        }

        public TIView NewEntity<TIView, TIModel>(Object prefab)
            where TIView : class, IViewBase
            where TIModel : class, IModel
        {
            var view = Views.FromPrefab<TIView>(prefab);
            var model = Models.Get<TIModel>();
            view.SetModel(model);
            return view;
        }

        public TIView NewEntity<TIView, TIAgent, TIModel>(Object prefab)
            where TIView : class, IViewBase
            where TIAgent : class, IAgent
            where TIModel : class, IModel
        {
            return Views.FromPrefab<TIView>(prefab, Agents.Get<TIAgent>(Models.Get<TIModel>()));
        }

        protected TView BuildEntityAsSingle<TView, TIView, TIAgent, TIModel>(params object[] modelArgs)
            where TView : Component, TIView
            where TIView : class, IViewBase
            where TIModel : class, IModel
            where TIAgent : class, IAgent
        {
            var obj = FindObjectOfType<TView>();

            if (obj == null)
            {
                Verbose(15, $"Failed to find instance of type {typeof(TView).Name}.");
                return null;
            }

            if (obj.AgentBase != null)
                return obj;

            Views.Bind<TIView, TView>(obj);

            var model = Models.Get<TIModel>(modelArgs);
            Assert.IsNotNull(model);

            var agent = Agents.Get<TIAgent>(model);
            Assert.IsNotNull(agent);

            obj.SetAgent(agent);
            obj.SetModel(model);

            return obj;
        }

        /// <summary>
        /// Find all Views of type TIView that exist in the scene and give them a model and agent.
        /// </summary>
        /// <param name="modelArgs"> Arguments passed to the model's constructor. </param>
        protected TView[] BuildEntitiesOfType<TView, TIView, TIAgent, TIModel>(params object[] modelArgs)
            where TView : Component, TIView
            where TIView : class, IViewBase
            where TIModel : class, IModel
            where TIAgent : class, IAgent
        {
            var objs = FindObjectsOfType<TView>().OrderBy(o => o.name).ToArray();
            foreach (var obj in objs)
            {
                if (obj.AgentBase != null)
                    continue;

                Views.Bind<TIView, TView>(obj, false);

                var model = Models.Get<TIModel>(modelArgs);
                Assert.IsNotNull(model);

                var agent = Agents.Get<TIAgent>(model);
                Assert.IsNotNull(agent);

                obj.SetAgent(agent);
                obj.SetModel(model);
            }
            return objs;
        }

        /// <summary>
        /// Add registry bindings in here using Registry.Bind
        /// </summary>
        protected abstract void CreateBindings();
    }
}
