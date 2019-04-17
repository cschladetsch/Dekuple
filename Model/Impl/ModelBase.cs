using System;
using UniRx;

// event not used
#pragma warning disable 67

namespace Dekuple.Model
{
    using Registry;

    ///  <summary>
    ///  Common for all Models.
    ///  Models are created from a Registry, have an OnDestroyed event, and are persistent by default.
    ///  </summary>
    public abstract class ModelBase
        : Flow.Impl.Logger
        , IModel
    {
        public event Action<IModel> OnDestroyed;

        public bool Prepared { get; protected set; }
        public IRegistry<IModel> Registry { get; set; }
        public string Name { get; set; }
        public Guid Id { get; /*private*/ set; }
        public IReadOnlyReactiveProperty<IOwner> Owner => _owner;

        private bool _destroyed;
        private readonly ReactiveProperty<IOwner> _owner;
        private bool _prepared;

        public virtual bool IsValid
        {
            get
            {
                if (Registry == null) return false;
                return Id != Guid.Empty;
            }
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

        public virtual void Create()
        {
        }

        public virtual void Begin()
        {
            Assert.IsFalse(_prepared);
            if (_prepared)
            {
                Error($"{this} has already been prepared");
                return;
            }
            _prepared = true;
        }

        public virtual void AddSubscriptions()
        {
        }

        public virtual void Destroy()
        {
            Verbose(40, $"Destroy {this}");
            if (_destroyed)
                return;

            _destroyed = true;

            OnDestroyed?.Invoke(this);
            Id = Guid.Empty;
        }

        public void SetOwner(IOwner owner)
        {
            if (Owner.Value == owner)
                return;

            Verbose(30, $"{this} changes ownership from {Owner.Value} to {owner}");
            _owner.Value = owner;
        }

        protected void NotImplemented(string text)
        {
            Error($"Not {text} implemented");
        }
    }
}

//static class ModelExt
//{
//    public static T AddTo<T>(this T disposable, Dekuple.Model.IModel model)
//        where T : IDisposable
//    {
//        model.Destroyed.Subscribe(m => disposable.Dispose());
//        return disposable;
//    }
//}
