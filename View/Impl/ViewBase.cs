using System;
using System.Collections.Generic;

using UnityEngine;

using CoLib;
using Dekuple.Utility;
using UniRx;

namespace Dekuple.View.Impl
{
    using Registry;
    using Model;
    using Agent;

    /// <inheritdoc cref="LoggingBehavior" />
    /// <summary>
    /// Common for all Views in the game. This is to replace MonoBehavior and make it more rational, as well as to
    /// conform with Flow.ITransient.
    /// </summary>
    public abstract class ViewBase
        : LoggingBehavior
        , IViewBase
    {
        public Guid Id { get; set; }
        public IRegistry<IViewBase> Registry { get; set; }
        public IViewRegistry ViewRegistry => Registry as IViewRegistry;
        public IReadOnlyReactiveProperty<IOwner> Owner => AgentBase?.Owner ?? Model?.Owner;
        public event Action<IViewBase> OnDestroyed;
        public IAgent AgentBase { get; set; }
        public IViewBase OwnerView { get; set; }
        public IModel OwnerModel => Owner?.Value as IModel;
        public GameObject GameObject => gameObject;
        public Transform Transform => gameObject.transform;
        public IModel Model => AgentBase?.BaseModel ?? _localModel;

        // lazy create because most views won't need a queue or audio source
        protected CommandQueue _Queue => _queue ?? (_queue = new CommandQueue());
        protected List<IDisposable> _Subscriptions => _subscriptions ?? (_subscriptions = new List<IDisposable>());
        protected AudioSource _AudioSource
        {
            get
            {
                _audioSource = GetComponent<AudioSource>();
                return _audioSource ? _audioSource : (_audioSource = GameObject.AddComponent<AudioSource>());
            }
        }

        private bool _paused;
        private bool _createCalled;
        private bool _beginCalled;
        private bool _addCalled;
        private bool _destroyed;
        private float _localTime;
        private CommandQueue _queue;
        private AudioSource _audioSource;
        private IModel _localModel;
        private List<IDisposable> _subscriptions;

        public virtual bool IsValid
        {
            get
            {
                if (Id == Guid.Empty) return false;
                if (Registry == null) return false;
                if (ViewRegistry == null) return false;
                if (GameObject == null) return false;
                if (AgentBase == null) return false;
                return AgentBase.IsValid && AgentBase.BaseModel.IsValid;
            }
        }

        public bool SameOwner(IEntity other)
        {
            if (other == null)
                return Owner.Value == null;

            return other.Owner.Value == Owner.Value;
        }

        public void SetModel(IModel model)
        {
            _localModel = model;
            model.OnDestroyed += o => Destroy();
        }

        public void SetAgent(IAgent agent)
        {
            AgentBase = agent;
        }

        public void SetOwner(IOwner owner)
        {
            //Verbose(20, $"New owner of {this} is {owner}");

            Model?.SetOwner(owner);
        }

        private void Awake()
            => Create();

        private void Start()
            => Begin();

        private void Update()
        {
            if (_paused)
                return;

            Step();
            _localTime += Time.deltaTime;
        }

        /// <remarks>
        /// Unity initialisation methods are called via reflection.
        /// Dekuple overrides them so they can be used more consistently using
        /// virtual functions and overrides.
        /// </remarks>
        protected virtual bool Create()
        {
            return !this.EarlyOut(ref _createCalled, $"{this} has already had AddSubscriptions called. Aborting.");
        }

        /// <remarks>
        /// Unity initialisation methods are called via reflection.
        /// Dekuple overrides them so they can be used more consistently using
        /// virtual functions and overrides.
        /// </remarks>
        protected virtual bool Begin()
        {
            return !this.EarlyOut(ref _beginCalled, $"{this} has already had Begin called. Aborting.");
        }

        public virtual bool AddSubscriptions()
        {
            if (this.EarlyOut(ref _addCalled, $"{this} has already had AddSubscriptions called. Aborting."))
                return false;

            BindTransformComponents();
            return true;
        }

        private void BindTransformComponents()
        {
            if (Model is IPositionedModel positionedModel)
                positionedModel.Position.DistinctUntilChanged().Subscribe(pos => Transform.position = pos);
            if (Model is IScaledModel localScaledModel)
                localScaledModel.LocalScale.DistinctUntilChanged().Subscribe(scale => Transform.localScale = scale);
            if (Model is IRotatedModel rotatedModel)
                rotatedModel.Rotation.DistinctUntilChanged().Subscribe(rot => Transform.rotation = rot);
        }

        /// <remarks>
        /// Unity initialisation methods are called via reflection.
        /// Dekuple overrides them so they can be used more consistently using
        /// virtual functions and overrides.
        /// </remarks>
        protected virtual void Step()
        {
            _queue?.Update(Time.deltaTime);
        }

        public void Pause(bool pause = true)
        {
            _paused = pause;
        }

        public float LifeTime()
        {
            return _localTime;
        }

        public bool SameOwner(IOwned other)
        {
            return ReferenceEquals(Owner.Value, other);
        }

        private void OnDestroy() => Destroy();

        public virtual void Destroy()
        {
            if (_destroyed)
                return;

            _destroyed = true;

            foreach (var disposable in _Subscriptions)
                disposable.Dispose();

            _Subscriptions.Clear();

            AgentBase?.Destroy();
            if (AgentBase == null)
                _localModel?.Destroy();
            OnDestroyed?.Invoke(this);
            UnityEngine.Object.Destroy(GameObject);
        }

        public override string ToString()
        {
            return $"View {name} of type {GetType().Name}, Id={Id}";
        }
    }

    public class ViewBase<TIAgent>
        : ViewBase
        , IView<TIAgent>
        where TIAgent
            : class, IAgent
    {
        public TIAgent Agent => AgentBase as TIAgent;

        // !NOTE! To override this, you ***must*** declare the typed signature
        // in the overridden interface. Otherwise it will fall back to this
        // default implementation. This is a trap you will fall into, so
        // sorry you had to read this comment after debugging for 5 minutes.
        //
        // Specifically, it is not enough to just override this in an
        // implementation. The signature must also be in the in the interface.
        //
        // Unsure if this is a bug in C# or intended behavior.
        //
        // !!NOTE!! Update 2019 with .Net 4.7 - this seems to've been fixed?
        //public virtual void SetAgent(IViewBase player, TIAgent agent)
        //{
        //    base.SetAgent(player, agent);
        //}
    }
}
