// event not used
#pragma warning disable 67

namespace Dekuple.Model
{
    using System;
    using System.Collections.Generic;
    using Utility;
    using UniRx;
    using Registry;

    ///  <summary>
    ///  Common for all Models.
    ///  Models are created from a Registry, have an OnDestroyed event, and are persistent by default.
    ///  </summary>
    public abstract class ModelBase
        : Flow.Impl.Logger
        , IModel
    {
        /// <summary>
        /// When loading a Unity Scene, the AsyncOperation loads the scene by 0.9F, the last .1F is for activation
        /// </summary>
        public const float LoadCompletionProgress = 0.9F;

        public event Action<IModel> OnDestroyed;

        public bool Prepared { get; protected set; }

        public IRegistry<IModel> Registry { get; set; }
        public string Name { get; set; }
        public Guid Id { get; /*private*/ set; }
        public IReadOnlyReactiveProperty<IOwner> Owner => _owner;

        protected List<IDisposable> _Subscriptions => _subscriptions ?? (_subscriptions = new List<IDisposable>());

        private List<IDisposable> _subscriptions;
        private bool _destroyed;
        private readonly ReactiveProperty<IOwner> _owner;
        private bool _prepared;
        private bool _addCalled;

        public virtual bool IsValid
        {
            get
            {
                if (Registry == null) return false;
                return Id != Guid.Empty;
            }
        }

        public virtual void PrepareModels()
        {
        }

        public bool SameOwner(IEntity other)
        {
            if (other == null)
                return Owner.Value == null;
            return other.Owner.Value == Owner.Value;
        }

        protected ModelBase()
        {
            LogSubject = this;
            LogPrefix = GetType().Name;
            Verbosity = Parameters.DefaultLogVerbosity;
            ShowStack = Parameters.DefaultShowTraceStack;
            ShowSource = Parameters.DefaultShowTraceSource;
        }

        protected ModelBase(IOwner owner)
            : this()
        {
            _owner = new ReactiveProperty<IOwner>(owner);
        }

        public bool SameOwner(IOwned other)
        {
            if (other == null)
                return Owner.Value == null;
            return ReferenceEquals(other.Owner.Value, Owner.Value);
        }

        public virtual bool AddSubscriptions()
        {
            return !this.EarlyOut(ref _addCalled, $"{this} has already had AddSubscriptions called. Aborting.");
        }

        public virtual void Destroy()
        {
            Verbose(40, $"Destroy {this}");
            if (_destroyed)
                return;

            _destroyed = true;

            foreach (var disposable in _Subscriptions)
                disposable.Dispose();

            _Subscriptions.Clear();

            OnDestroyed?.Invoke(this);
            Id = Guid.Empty;
        }

        public void SetOwner(IOwner owner)
        {
            if (Owner.Value == owner)
                return;

            //Verbose(30, $"{this} changes ownership from {Owner.Value} to {owner}"); TODO calculates string interpolation even when not used?
            _owner.Value = owner;
        }

        protected void NotImplemented(string text)
        {
            Error($"Not {text} implemented");
        }

        public void Add(IDisposable other)
        {
            _Subscriptions.Add(other);
        }
    }
}

