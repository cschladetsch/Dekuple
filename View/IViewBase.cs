namespace Dekuple.View
{
    using UnityEngine;
    using Registry;
    using Agent;
    using Model;

    /// <inheritdoc cref="IEntity" />
    /// <summary>
    /// Common interface for all views
    /// </summary>
    public interface IViewBase
        : IEntity
        , IHasDestroyHandler<IViewBase>
        , IHasRegistry<IViewBase>
    {
        IAgent AgentBase { get; set; }
        GameObject GameObject { get; }
        Transform Transform { get; }
        IModel Model { get; }

        void SetAgent(IAgent agent);
    }

    public interface IView<out TIAgent>
        : IViewBase
        where TIAgent : IAgent
    {
        TIAgent Agent { get; }
    }
}
