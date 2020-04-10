namespace Dekuple.Agent
{
    using System;
    using System.Collections.Generic;
    using Flow;
    using Flow.Impl;

    /// <summary>
    /// AgentBase for all agents. Provides a custom logger and an ITransient implementation
    /// to be used with Flow library.
    /// </summary>
    public class AgentLogger
        : Transient
        , IHasSubscriptions
    {
        public INode Root => Kernel.Root;
        
        protected List<IDisposable> _Subscriptions = new List<IDisposable>();

        protected AgentLogger()
        {
            Completed += tr =>
            {
                foreach (var sub in _Subscriptions)
                    sub.Dispose();
            };
        }
        
        public void Add(IDisposable other)
        {
            _Subscriptions.Add(other);
        }
    }

    public abstract class AgentLogger<TModel>
        : AgentLogger
        where TModel : Model.IModel
    {
        public Model.IModel BaseModel { get; protected set; }
        public TModel Model { get; protected set; }
    }
}
