namespace CommandForWinForms
{
    partial class ControlCommandProperties
    {
        public static ICommand? GetCommand(this ToolStripItem item)
        {
            ANE.ThrowIfNull(item);
            return GetCommandCore(item);
        }

        public static void SetCommand(this ToolStripItem item, ICommand? command, object? parameter = null, Control? target = null)
        {
            ANE.ThrowIfNull(item);
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

            protected override void SetEnabled(bool enabled)
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
