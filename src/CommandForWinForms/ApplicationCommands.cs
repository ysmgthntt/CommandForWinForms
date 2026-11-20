namespace CommandForWinForms
{
    /// <summary>
    /// Provides commonly used commands for standard application operations.
    /// </summary>
    /// <remarks>
    /// These commands are provided for convenience and do not include default command handlers.
    /// Applications must implement the command behavior using <see cref="CommandBinding"/>.
    /// </remarks>
    public static class ApplicationCommands
    {
        private static UICommandBase? _close;

        /// <summary>
        /// Gets the command that closes the current document or window.
        /// </summary>
        public static UICommandBase Close => _close ??= new CloseCommand();

        private sealed class CloseCommand : UICommandBase
        {
            public override string Name => nameof(Close);
            public override string Text => nameof(Close);
        }

        private static UICommandBase? _print;

        /// <summary>
        /// Gets the command to print a document.
        /// </summary>
        /// <remarks>
        /// Key gesture: Ctrl+P, UI text: Print
        /// </remarks>
        public static UICommandBase Print => _print ??= new PrintCommand();

        private sealed class PrintCommand : UICommandBase
        {
            public override string Name => nameof(Print);
            public override string Text => nameof(Print);
            protected override IList<InputGesture>? CreateInputGestures()
                => [new KeyGesture(Keys.P, ModifierKeys.Control)];
        }

        private static UICommandBase? _printPreview;

        /// <summary>
        /// Gets the command to display a print preview.
        /// </summary>
        /// <remarks>
        /// Key gesture: Ctrl+F2, UI text: PrintPreview
        /// </remarks>
        public static UICommandBase PrintPreview => _printPreview ??= new PrintPreviewCommand();

        private sealed class PrintPreviewCommand : UICommandBase
        {
            public override string Name => nameof(PrintPreview);
            public override string Text => nameof(PrintPreview);
            protected override IList<InputGesture>? CreateInputGestures()
                => [new KeyGesture(Keys.F2, ModifierKeys.Control)];
        }
    }
}
