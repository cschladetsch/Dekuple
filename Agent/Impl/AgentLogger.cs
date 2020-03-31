namespace Dekuple.Agent
{
    using Flow;
    using Flow.Impl;

    /// <summary>
    /// AgentBase for all agents. Provides a custom logger and an ITransient implementation
    /// to be used with Flow library.
    /// </summary>
    public class AgentLogger
        : Transient
        , ILogger
    {
//        public event TransientHandler Completed;

//        public bool Active { get; protected set; }
//        public IKernel Kernel { get; set; }
//        public IFactory New => Kernel.Factory;
//        public IFactory Factory => New;
        public INode Root => Kernel.Root;
    }

    public abstract class AgentLogger<TModel>
        : AgentLogger
        where TModel : Model.IModel
    {
        public Model.IModel BaseModel { get; protected set; }
        public TModel Model { get; protected set; }
    }
}
