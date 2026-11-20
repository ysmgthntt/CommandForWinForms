namespace CommandForWinForms
{
    partial class ControlCommandProperties
    {
        /// <summary>
        /// Gets the command associated with a button.
        /// </summary>
        /// <param name="button">The button whose command is returned.</param>
        /// <returns>The associated command, or <see langword="null"/> if none is assigned.</returns>
        public static ICommand? GetCommand(this ButtonBase button)
        {
            ArgumentNullException.ThrowIfNull(button);
            return GetCommandCore(button);
        }

        /// <summary>
        /// Associates a command with a button and updates its enabled state as the command changes.
        /// </summary>
        /// <param name="button">The button associated with the command.</param>
        /// <param name="command">The command to associate, or <see langword="null"/> to remove the association.</param>
        /// <param name="parameter">The parameter passed to the command.</param>
        /// <param name="target">The command target, or <see langword="null"/> to use the active control.</param>
        public static void SetCommand(this ButtonBase button, ICommand? command, object? parameter = null, Control? target = null)
        {
            ArgumentNullException.ThrowIfNull(button);
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
