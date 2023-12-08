using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace CommandForWinForms
{
    public static partial class ControlCommandProperties
    {
        private static class CollectionHolder<T>
        {
            public static Dictionary<Control, IList<T>>? Collections;

            public static IList<T> GetOrCreate(Control control)
            {
                ANE.ThrowIfNull(control);
                if (control.IsDisposed)
                    ThrowObjectDisposedException(control.Name);

                IList<T>? collection = null;
                if (Collections is null)
                {
                    Collections = [];
                    collection = new List<T>();
                    Collections[control] = collection;
                    control.Disposed += Collections.Key_Disposed;
                }
                else if (!Collections.TryGetValue(control, out collection))
                {
                    collection = new List<T>();
                    Collections[control] = collection;
                    control.Disposed += Collections.Key_Disposed;
                }
                return collection;
            }
        }

        public static IList<CommandBinding> GetCommandBindings(this Control control)
            => CollectionHolder<CommandBinding>.GetOrCreate(control);

        public static IList<InputBinding> GetInputBindings(this Control control)
            => CollectionHolder<InputBinding>.GetOrCreate(control);

        internal static bool TryGetCollection<T>(Control control, [NotNullWhen(true)] out IList<T>? collection)
        {
            var collections = CollectionHolder<T>.Collections;
            if (collections is null)
            {
                collection = null;
                return false;
            }
            return collections.TryGetValue(control, out collection);
        }

        private static void Key_Disposed<K, V>(this Dictionary<K, V> dictionary, object? sender, EventArgs e)
            where K : notnull
        {
            if (sender is K key)
                dictionary.Remove(key);
        }

        private static void ThrowObjectDisposedException(string? name)
            => throw new ObjectDisposedException(name);

        // Command

        private static Dictionary<IComponent, CommandHandler?>? _commands;

        private static ICommand? GetCommandCore(IComponent source)
        {
            ANE.ThrowIfNull(source);
            if (_commands is null || !_commands.TryGetValue(source, out var commandHandler))
                return null;
            return commandHandler?.Command;
        }

        private static void SetCommandCore(IComponent source, CommandHandler? commandHandler)
        {
            if (_commands is null)
            {
                if (commandHandler is null)
                    return;
                _commands = [];
                source.Disposed += _commands.Key_Disposed;
            }
            else
            {
                if (_commands.TryGetValue(source, out var old))
                {
                    old?.DetachEvents();
                }
                else
                {
                    if (commandHandler is null)
                        return;
                    source.Disposed += _commands.Key_Disposed;
                }
            }
            _commands[source] = commandHandler;
        }

        internal static void RaiseCanExecuteChangedAllUICommand()
        {
            if (_commands is not null)
            {
                // TODO: DistinctedCommands
                var raised = new HashSet<UICommandBase>();
                foreach (var commandHandler in _commands.Values)
                {
                    if (commandHandler?.Command is UICommandBase uicommand)
                    {
                        if (raised.Contains(uicommand))
                            continue;

                        uicommand.RaiseCanExecuteChanged();
                        raised.Add(uicommand);
                    }
                }
            }
        }
    }
}
