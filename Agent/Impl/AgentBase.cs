namespace Dekuple.Agent
{
    using System;
    using System.Collections.Generic;
    using UniRx;
    using Utility;
    using Registry;
    using Model;

    /// <summary>
    /// Common for all agents that manage models in the system.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public abstract class AgentBase<TModel>
        : AgentLogger
        , IAgent<TModel>
        where TModel : class, IModel
    {
        private bool _addCalled;
        public event Action<IAgent> OnDestroyed;
        public IRegistry<IAgent> Registry { get; set; }
        public Guid Id { get; /*private*/ set; }

        public IModel BaseModel { get; }

        public TModel Model => BaseModel as TModel;
        public IReadOnlyReactiveProperty<IOwner> Owner => Model?.Owner;

        public virtual bool IsValid
        {
            get
            {
                if (Id == Guid.Empty) return false;
                if (Registry == null) return false;
                return BaseModel != null && Model.IsValid;
            }
        }

        protected AgentBase(TModel model)
        {
            if (model == null)
            {
                Error("Model cannot be null");
                return;
            }

            Assert.IsNotNull(model);
            BaseModel = model;
        }

        public bool SameOwner(IEntity other)
        {
            if (other == null)return Owner.Value == null;

            return other.Owner.Value == Owner.Value;
        }

        public void SetOwner(IOwner owner)
        {
            Model.SetOwner(owner);
        }

        public bool SameOwner(IOwned other)
        {
            if (other == null)
                return Owner.Value == null;

            return other.Owner.Value == Owner.Value;
        }

        // DK TODO Move to Chess2
        //public virtual void StartGame()
        //{
        //    Assert.IsFalse(_started);
        //    _started = true;
        //}

        //public virtual void EndGame()
        //{
        //    _started = false;
        //}

        public virtual bool AddSubscriptions()
        {
            return !this.EarlyOut(ref _addCalled, $"{this} has already had AddSubscriptions called. Aborting.");
        }

        public virtual void Destroy()
        {
            Complete();

            foreach (var disposable in _Subscriptions)
                disposable.Dispose();

            _Subscriptions.Clear();

            Model?.Destroy();
            this.OnDestroyed?.Invoke(this);
        }
    }
}
