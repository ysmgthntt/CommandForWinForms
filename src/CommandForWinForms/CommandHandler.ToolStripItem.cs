namespace RoutedCommands.WinForms
{
    partial class ControlCommandProperties
    {
        /// <summary>
        /// Gets the command associated with a tool strip item.
        /// </summary>
        /// <param name="item">The tool strip item whose command is returned.</param>
        /// <returns>The associated command, or <see langword="null"/> if none is assigned.</returns>
        public static ICommand? GetCommand(this ToolStripItem item)
        {
            ArgumentNullException.ThrowIfNull(item);
            return GetCommandCore(item);
        }

        /// <summary>
        /// Associates a command with a tool strip item and updates its enabled state as the command changes.
        /// </summary>
        /// <param name="item">The tool strip item associated with the command.</param>
        /// <param name="command">The command to associate, or <see langword="null"/> to remove the association.</param>
        /// <param name="parameter">The parameter passed to the command.</param>
        /// <param name="target">The command target, or <see langword="null"/> to use the active control.</param>
        public static void SetCommand(this ToolStripItem item, ICommand? command, object? parameter = null, Control? target = null)
        {
            ArgumentNullException.ThrowIfNull(item);
            if (item.IsDisposed)
                ThrowObjectDisposedException(item.Name);

            var handler = (command is not null) ? new ToolStripItemCommandHandler(item, command, parameter, target) : null;
            SetCommandCore(item, handler);
        }

        private sealed class ToolStripItemCommandHandler : CommandHandler
        {
            private readonly ToolStripItem _item;

            public ToolStripItemCommandHandler(ToolStripItem item, ICommand command, object? parameter, Control? target)
                : base(command, parameter, target)
            {
                _item = item;

                _item.Click += OnExecute;
                _item.VisibleChanged += OnCanExecuteChanged;
                _item.Disposed += OnDisposed;
            }

            protected override bool Available => _item.Visible;

            protected override Control? SourceControl => _item.Owner;

            public override void SetEnabled(bool enabled)
                => _item.Enabled = enabled;

            protected override void DetachComponentEvents()
            {
                _item.Click -= OnExecute;
                _item.VisibleChanged -= OnCanExecuteChanged;
                _item.Disposed -= OnDisposed;
            }
        }
    }
}
