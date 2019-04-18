using System.Linq;
using Dekuple.Agent;
using Dekuple.Model;
using Dekuple.View.Impl;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dekuple.View
{
    public abstract class MainBase
        : ViewBase
    {
        public IModelRegistry Models { get; } = new ModelRegistry();
        public IAgentRegistry Agents { get; } = new AgentRegistry();
        public IViewRegistry Views { get; } = new ViewRegistry();

        private static MainBase _instance;

        protected override void Create()
        {
            base.Create();
            if (_instance == null)
                _instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            CreateBindings();
            ResolveBindings();
            SceneManager.sceneLoaded += (x, y) => OnSceneLoaded();
            Agents.Kernel.Root.Resume();
        }

        private void ResolveBindings()
        {
            Models.Resolve();
            Agents.Resolve();
            Views.Resolve();
        }

        protected override void Step()
        {
            Agents.Kernel.Step();
        }

        protected virtual void OnSceneLoaded()
        {
            Views.InjectViewsInScene();
            Models.AddAllSubscriptions();
            Agents.AddAllSubscriptions();
            Views.AddAllSubscriptions();
        }

        public TIView NewEntity<TIView, TIAgent, TIModel>(UnityEngine.Object prefab)
            where TIView : class, IViewBase
            where TIAgent : class, IAgent
            where TIModel : class, IModel
        {
            var view = Views.FromPrefab<TIView>(prefab);
            var model = Models.Get<TIModel>();
            var agent = Agents.Get<TIAgent>(model);
            view.SetAgent(agent);
            return view;
        }

        /// <summary>
        /// Find all Views of type TIView that exist in the scene and give them a model and agent.
        /// </summary>
        protected TView[] BuildEntitiesOfType<TView, TIAgent, TIModel>(params object[] args)
            where TView : Component, IViewBase
            where TIModel : class, IModel
            where TIAgent : class, IAgent
        {
            var objs = FindObjectsOfType<TView>().OrderBy(o => o.name).ToArray();
            foreach (var obj in objs)
            {
                var model = Models.Get<TIModel>(args);
                var agent = Agents.Get<TIAgent>(model);
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
