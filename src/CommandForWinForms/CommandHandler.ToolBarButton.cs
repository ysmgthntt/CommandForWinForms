#if NETFRAMEWORK
namespace CommandForWinForms
{
    partial class ControlCommandProperties
    {
        public static ICommand? GetCommand(this ToolBarButton button)
        {
            ANE.ThrowIfNull(button);
            return GetCommandCore(button);
        }

        public static void SetCommand(this ToolBarButton button, ICommand? command, object? parameter = null, Control? target = null)
        {
            ANE.ThrowIfNull(button);

            var handler = (command is not null) ? new ToolBarButtonCommandHandler(button, command, parameter, target) : null;
            SetCommandCore(button, handler);
        }

        private sealed class ToolBarButtonCommandHandler : CommandHandler
        {
            private readonly ToolBarButton _button;
            private readonly ToolBar _parent;

            public ToolBarButtonCommandHandler(ToolBarButton button, ICommand command, object? parameter, Control? target)
                : base(command, parameter, target)
            {
                _button = button;
                _parent = button.Parent;

                _parent.ButtonClick += ToolBar_ButtonClick;
                _parent.VisibleChanged += OnCanExecuteChanged;
                _button.Disposed += OnDisposed;
            }

            private void ToolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
            {
                if (e.Button == _button)
                    OnExecute(sender, EventArgs.Empty);
            }

            protected override bool Available => _button.Visible;

            protected override Control? SourceControl => _button.Parent;

            protected override void SetEnabled(bool enabled)
                => _button.Enabled = enabled;

            protected override void DetachComponentEvents()
            {
                _parent.ButtonClick -= ToolBar_ButtonClick;
                _parent.VisibleChanged -= OnCanExecuteChanged;
                _button.Disposed -= OnDisposed;
            }
        }
    }
}
#endif
