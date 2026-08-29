using Application.Services.Abstraction.AI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.AI
{
    public class PendingActionStore : IPendingActionStore
    {
        private readonly ConcurrentDictionary<string, object> _actions = new();

        public void Add(string actionId, object action)
        {
            if (string.IsNullOrWhiteSpace(actionId))
                throw new ArgumentException(
                    "Action ID cannot be empty.",
                    nameof(actionId));

            ArgumentNullException.ThrowIfNull(action);

            _actions[actionId] = action;
        }

        public bool TryGet(string actionId, out object? action)
        {
            return _actions.TryGetValue(actionId, out action);
        }

        public bool TryRemove(string actionId, out object? action)
        {
            return _actions.TryRemove(actionId, out action);
        }

        public bool Contains(string actionId)
        {
            return _actions.ContainsKey(actionId);
        }
    }
}
