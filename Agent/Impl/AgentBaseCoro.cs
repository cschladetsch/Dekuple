using System;
using System.Collections;

using Flow;

namespace Dekuple.Agent
{
    using Model;

    /// <inheritdoc />
    /// <summary>
    /// Base for agents that perform actions over time.
    /// </summary>
    /// <typeparam name="TModel">The model that this Agent represents.</typeparam>
    public abstract class AgentBaseCoro<TModel>
        : AgentBase<TModel>
        where TModel
            : class
            , IModel
    {
        protected INode _Node
        {
            get
            {
                if (_node != null)
                    return _node;
                _node = New.Node().Named(Name);
                Root.Add(_node);
                return _node;
            }
        }

        private INode _node;

        protected AgentBaseCoro(TModel model)
            : base(model)
        {
        }

        protected IGenerator NewCoro(Func<IGenerator, IEnumerator> fun)
        {
            var coro = New.Coroutine(fun);
            _Node.Add(coro);
            return coro;
        }

        protected IGenerator NewCoro<T0>(Func<IGenerator, T0, IEnumerator> fun, T0 t0)
        {
            var coro = New.Coroutine(fun, t0);
            _Node.Add(coro);
            return coro;
        }

        protected IGenerator NewCoro<T0, T1>(Func<IGenerator, T0, T1, IEnumerator> fun, T0 t0, T1 t1)
        {
            var coro = New.Coroutine(fun, t0, t1);
            _Node.Add(coro);
            return coro;
        }
    }
}
