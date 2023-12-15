using System.Runtime.CompilerServices;

namespace CommandForWinForms
{
    internal sealed class WeakEventHandler
    {
        private readonly ConditionalWeakTable<object, object> _weakTable = new();
        private readonly List<WeakReference<EventHandler>> _eventHandlers = new();

        public void AddHandler(EventHandler? value)
        {
            if (value is not null)
            {
                var target = value.Target ?? this;
                if (_weakTable.TryGetValue(target, out var handler))
                {
                    if (handler is List<EventHandler> list)
                    {
                        list.Add(value);
                    }
                    else
                    {
                        list = [(EventHandler)handler, value];
                        _weakTable.Remove(target);
                        _weakTable.Add(target, list);
                    }
                }
                else
                {
                    _weakTable.Add(target, value);
                }
                _eventHandlers.Add(new WeakReference<EventHandler>(value));
            }
        }

        public void RemoveHandler(EventHandler? value)
        {
            if (value is not null)
            {
                var target = value.Target ?? this;
                if (_weakTable.TryGetValue(target, out var handler))
                {
                    if (handler is List<EventHandler> list)
                    {
                        list.Remove(value);
                        if (list.Count == 0)
                            _weakTable.Remove(target);
                    }
                    else
                    {
                        _weakTable.Remove(target);
                    }
                }
                for (int i = 0; i < _eventHandlers.Count; i++)
                {
                    if (_eventHandlers[i].TryGetTarget(out var eventHandler) && eventHandler == value)
                    {
                        _eventHandlers.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public bool IsEmpty => _eventHandlers.Count == 0;

        public void Invoke(object sender, EventArgs e)
        {
            for (int i = 0; i < _eventHandlers.Count; i++)
            {
                if (_eventHandlers[i].TryGetTarget(out var handler))
                    handler(sender, e);
                else
                    _eventHandlers.RemoveAt(i--);
            }
        }
    }
}
