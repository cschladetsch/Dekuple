namespace Dekuple.Agent
{
    using System;
    using System.Collections;
    using Flow;
    using Model;

    /// <summary>
    /// Base for agents that perform actions over time.
    /// </summary>
    /// <typeparam name="TModel">The model that this Agent represents</typeparam>
    public abstract class AgentBaseCoro<TModel>
        : AgentBase<TModel>
        where TModel
            : class
            , IModel
    {
        private INode _node;

        protected INode _Node
        {
            get
            {
                if (_node != null)
                    return _node;
                return _node = Factory.Node().Named(Name).AddTo(Root);
            }
        }

        protected AgentBaseCoro(TModel model)
            : base(model)
        {
        }

        /// <summary>
        /// Adds a new coroutine to the root and starts it immediately.
        /// </summary>
        /// <param name="fun"></param>
        /// <returns></returns>
        protected IGenerator Run(Func<IGenerator, IEnumerator> fun)
        {
            var coro = Factory.Coroutine(fun);
            _Node.Add(coro);
            return coro;
        }

        protected IGenerator Run<T0>(Func<IGenerator, T0, IEnumerator> fun, T0 t0)
        {
            var coro = Factory.Coroutine(fun, t0);
            _Node.Add(coro);
            return coro;
        }

        protected IGenerator Run<T0, T1>(Func<IGenerator, T0, T1, IEnumerator> fun, T0 t0, T1 t1)
        {
            var coro = Factory.Coroutine(fun, t0, t1);
            _Node.Add(coro);
            return coro;
        }
    }
}
