using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

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

        private static ICommand? GetCommandCore(IComponent component)
        {
            if (_commands is null || !_commands.TryGetValue(component, out var commandHandler))
                return null;
            return commandHandler?.Command;
        }

        private static void SetCommandCore(IComponent component, CommandHandler? commandHandler)
        {
            if (_commands is null)
            {
                if (commandHandler is null)
                    return;
                _commands = [];
                component.Disposed += _commands.Key_Disposed;
            }
            else
            {
                if (_commands.TryGetValue(component, out var old))
                {
                    if (old is not null)
                    {
                        old.DetachEvents();
                        old.SetEnabled(true);
                    }
                }
                else
                {
                    if (commandHandler is null)
                        return;
                    component.Disposed += _commands.Key_Disposed;
                }
            }
            _commands[component] = commandHandler;
            _allUICommands = null;
            commandHandler?.UpdateCanExecute();
        }

        private static HashSet<UICommandBase>? _allUICommands;

        internal static void RaiseCanExecuteChangedAllUICommands()
        {
            if (_allUICommands is not null)
            {
                foreach (var uicommand in _allUICommands)
                    uicommand.RaiseCanExecuteChanged();
            }
            else if (_commands is not null)
            {
                var raised = new HashSet<UICommandBase>();
                foreach (var commandHandler in _commands.Values)
                {
                    if (commandHandler?.Command is UICommandBase uicommand)
                    {
                        if (raised.Add(uicommand))
                            uicommand.RaiseCanExecuteChanged();
                    }
                }
                _allUICommands = raised;
            }
        }
    }
}
