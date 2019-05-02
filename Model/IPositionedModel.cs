using UniRx;
using UnityEngine;

namespace Dekuple.Model
{
    /// <summary>
    /// Base for models which require a tracked world position.
    /// </summary>
    public interface IPositionedModel
        : IModel
    {
        /// <summary>
        /// Always in world space.
        /// </summary>
        IReactiveProperty<Vector3> Position { get; }
    }
}