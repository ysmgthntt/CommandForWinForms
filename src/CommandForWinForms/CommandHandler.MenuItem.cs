#if NETFRAMEWORK
namespace CommandForWinForms
{
    partial class ControlCommandProperties
    {
        public static ICommand? GetCommand(this MenuItem menuItem)
        {
            ANE.ThrowIfNull(menuItem);
            return GetCommandCore(menuItem);
        }

        public static void SetCommand(this MenuItem menuItem, ICommand? command, object? parameter = null, Control? target = null)
        {
            ANE.ThrowIfNull(menuItem);

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
