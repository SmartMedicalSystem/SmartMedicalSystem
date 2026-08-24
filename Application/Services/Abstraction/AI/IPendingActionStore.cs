using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.Abstraction.AI
{
    public interface IPendingActionStore
    {
        void Add(string actionId, object action);

        bool TryGet(string actionId, out object? action);

        bool TryRemove(string actionId, out object? action);

        bool Contains(string actionId);
    }
}
