#if NETFRAMEWORK
namespace RoutedCommands.WinForms
{
    partial class ControlCommandProperties
    {
        /// <summary>
        /// Gets the command associated with a menu item.
        /// </summary>
        /// <param name="menuItem">The menu item whose command is returned.</param>
        /// <returns>The associated command, or <see langword="null"/> if none is assigned.</returns>
        public static ICommand? GetCommand(this MenuItem menuItem)
        {
            ArgumentNullException.ThrowIfNull(menuItem);
            return GetCommandCore(menuItem);
        }

        /// <summary>
        /// Associates a command with a menu item and updates its enabled state as the command changes.
        /// </summary>
        /// <param name="menuItem">The menu item associated with the command.</param>
        /// <param name="command">The command to associate, or <see langword="null"/> to remove the association.</param>
        /// <param name="parameter">The parameter passed to the command.</param>
        /// <param name="target">The command target, or <see langword="null"/> to use the active control.</param>
        public static void SetCommand(this MenuItem menuItem, ICommand? command, object? parameter = null, Control? target = null)
        {
            ArgumentNullException.ThrowIfNull(menuItem);

            var handler = (command is not null) ? new MenuItemCommandHandler(menuItem, command, parameter, target) : null;
            SetCommandCore(menuItem, handler);
        }

        private sealed class MenuItemCommandHandler : CommandHandler
        {
            private readonly MenuItem _menuItem;

            public MenuItemCommandHandler(MenuItem menuItem, ICommand command, object? parameter, Control? target)
                : base(command, parameter, target)
            {
                _menuItem = menuItem;

                _menuItem.Click += OnExecute;
                _menuItem.Disposed += MenuItem_Disposed;
            }

            // VisibleChanged がないため制御しない。
            //protected override bool Available => _menuItem.Visible;

            public override void SetEnabled(bool enabled)
                => _menuItem.Enabled = enabled;

            private bool _disposing;

            private void MenuItem_Disposed(object sender, EventArgs e)
            {
                _disposing = true;
                base.OnDisposed(sender, e);
            }

            protected override void DetachComponentEvents()
            {
                // エラーになる。
                if (!_disposing)
                    _menuItem.Click -= OnExecute;
                _menuItem.Disposed -= OnDisposed;
            }
        }
    }
}
#endif
