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
    {
        /// <summary>
        /// If true, this model has already been prepared.
        /// </summary>
        bool Prepared { get; }
    }
}
