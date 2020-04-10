using System;

namespace Dekuple.Model
{
    using Registry;

    /// <inheritdoc cref="IEntity" />
    /// <summary>
    /// Base for all persistent models.
    /// </summary>
    public interface IModel
        : Flow.ILogger
        , IEntity
        , IHasDestroyHandler<IModel>
        , IHasRegistry<IModel>
        , IHasSubscriptions
    {
        /// <summary>
        /// If true, this model has already been prepared.
        /// </summary>
        bool Prepared { get; }

        void PrepareModels();
    }
}
