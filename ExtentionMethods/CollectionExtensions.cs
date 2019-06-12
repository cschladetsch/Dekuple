using System.Collections.Generic;
using Dekuple.Model;

namespace Dekuple
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Add an element with a destroy handler to the collection and remove it from the list when it is destroyed.
        /// </summary>
        /// <typeparam name="T">The type of the object you are adding to the collection.</typeparam>
        /// <param name="coll">The collection to add and remove the element from.</param>
        /// <param name="val">The element to add and remove reactively.</param>
        public static void AddReactive<T>(this ICollection<T> coll, T val)
            where T : class, IHasDestroyHandler<IModel>
        {
            coll.Add(val);

            void Remove(IHasDestroyHandler<IModel> tr)
            {
                val.OnDestroyed -= Remove;  // remove dangling reference
                coll.Remove(val);
            }

            val.OnDestroyed += Remove;
        }
    }
}
