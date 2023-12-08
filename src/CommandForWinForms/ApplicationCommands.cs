namespace CommandForWinForms
{
    public static class ApplicationCommands
    {
        private static UICommandBase? _close;
        public static UICommandBase Close => _close ??= new CloseCommand();

        private sealed class CloseCommand : UICommandBase
        {
            public override string Name => nameof(Close);
            public override string Text => nameof(Close);
        }

        private static UICommandBase? _print;
        public static UICommandBase Print => _print ??= new PrintCommand();

        private sealed class PrintCommand : UICommandBase
        {
            public override string Name => nameof(Print);
            public override string Text => nameof(Print);
            protected override IList<InputGesture>? CreateInputGestures()
                => [new KeyGesture(Keys.P, ModifierKeys.Control)];
        }

        private static UICommandBase? _printPreview;
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
