namespace CommandForWinForms
{
    partial class ControlCommandProperties
    {
        public static ICommand? GetCommand(this ButtonBase button)
        {
            ANE.ThrowIfNull(button);
            return GetCommandCore(button);
        }

        public static void SetCommand(this ButtonBase button, ICommand? command, object? parameter = null, Control? target = null)
        {
            ANE.ThrowIfNull(button);
            if (button.IsDisposed)
                ThrowObjectDisposedException(button.Name);

            var handler = (command is not null) ? new ButtonCommandHandler(button, command, parameter, target) : null;
            SetCommandCore(button, handler);
        }

        private sealed class ButtonCommandHandler : CommandHandler
        {
            private readonly ButtonBase _button;

            public ButtonCommandHandler(ButtonBase button, ICommand command, object? parameter, Control? target)
                : base(command, parameter, target)
            {
                _button = button;

                _button.Click += OnExecute;
                _button.VisibleChanged += OnCanExecuteChanged;
                _button.Disposed += OnDisposed;
            }

            protected override bool Available => _button.Visible;

            protected override Control? SourceControl => _button;

            public override void SetEnabled(bool enabled)
                => _button.Enabled = enabled;

            protected override void DetachComponentEvents()
            {
                _button.Click -= OnExecute;
                _button.VisibleChanged -= OnCanExecuteChanged;
                _button.Disposed -= OnDisposed;
            }
        }
    }
}
