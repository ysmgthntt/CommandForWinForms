namespace RoutedCommands.WinForms
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
        /// <summary>
        /// Cuts the selected content and places it on the clipboard.
        /// </summary>
        /// <remarks>
        /// Key gesture: Ctrl+X, Shift+Delete
        /// </remarks>
        public static UICommandBase Cut => field ??= new UICommand(nameof(Cut), nameof(Cut), [new KeyGesture(Keys.X, KeyModifiers.Control), new KeyGesture(Keys.Delete, KeyModifiers.Shift)]);

        /// <summary>
        /// Gets the command that copies the selected content to the clipboard.
        /// </summary>
        /// <remarks>
        /// Key gesture: Ctrl+C, Ctrl+Insert
        /// </remarks>
        public static UICommandBase Copy => field ??= new UICommand(nameof(Copy), nameof(Copy), [new KeyGesture(Keys.C, KeyModifiers.Control), new KeyGesture(Keys.Insert, KeyModifiers.Control)]);

        /// <summary>
        /// Gets the command for pasting content.
        /// </summary>
        /// <remarks>
        /// Key gesture: Ctrl+V, Shift+Insert
        /// </remarks>
        public static UICommandBase Paste => field ??= new UICommand(nameof(Paste), nameof(Paste), [new KeyGesture(Keys.V, KeyModifiers.Control), new KeyGesture(Keys.Insert, KeyModifiers.Shift)]);

        /// <summary>
        /// Gets the command associated with the Delete key.
        /// </summary>
        /// <remarks>
        /// Key gesture: Delete
        /// </remarks>
        public static UICommandBase Delete => field ??= new UICommand(nameof(Delete), nameof(Delete), [new KeyGesture(Keys.Delete)]);

        /// <summary>
        /// Represents the command for undoing the most recent action.
        /// </summary>
        /// <remarks>
        /// Key gesture: Ctrl+Z
        /// </remarks>
        public static UICommandBase Undo => field ??= new UICommand(nameof(Undo), nameof(Undo), [new KeyGesture(Keys.Z, KeyModifiers.Control)]);

        /// <summary>
        /// Gets the command that reapplies the most recently undone action.
        /// </summary>
        /// <remarks>
        /// Key gesture: Ctrl+Y
        /// </remarks>
        public static UICommandBase Redo => field ??= new UICommand(nameof(Redo), nameof(Redo), [new KeyGesture(Keys.Y, KeyModifiers.Control)]);

        /// <summary>
        /// Gets the command for finding content.
        /// </summary>
        /// <remarks>
        /// Key gesture: Ctrl+F
        /// </remarks>
        public static UICommandBase Find => field ??= new UICommand(nameof(Find), nameof(Find), [new KeyGesture(Keys.F, KeyModifiers.Control)]);

        /// <summary>
        /// Represents the Replace command.
        /// </summary>
        /// <remarks>
        /// Key gesture: Ctrl+H
        /// </remarks>
        public static UICommandBase Replace => field ??= new UICommand(nameof(Replace), nameof(Replace), [new KeyGesture(Keys.H, KeyModifiers.Control)]);

        /// <summary>
        /// Gets the command that selects all items.
        /// </summary>
        public static UICommandBase SelectAll => field ??= new UICommand(nameof(SelectAll), nameof(SelectAll), [new KeyGesture(Keys.A, KeyModifiers.Control)]);

        /// <summary>
        /// Gets the help command.
        /// </summary>
        /// <remarks>
        /// Key gesture: F1
        /// </remarks>
        public static UICommandBase Help => field ??= new UICommand(nameof(Help), nameof(Help), [new KeyGesture(Keys.F1)]);

        /// <summary>
        /// Gets the command for creating a new item.
        /// </summary>
        /// <remarks>
        /// Key gesture: Ctrl+N
        /// </remarks>
        public static UICommandBase New => field ??= new UICommand(nameof(New), nameof(New), [new KeyGesture(Keys.N, KeyModifiers.Control)]);

        /// <summary>
        /// Gets the command for opening an existing item.
        /// </summary>
        /// <remarks>
        /// Key gesture: Ctrl+O
        /// </remarks>
        public static UICommandBase Open => field ??= new UICommand(nameof(Open), nameof(Open), [new KeyGesture(Keys.O, KeyModifiers.Control)]);

        /// <summary>
        /// Gets the command that closes the current document or window.
        /// </summary>
        public static UICommandBase Close => field ??= new UICommand(nameof(Close), nameof(Close));

        /// <summary>
        /// Gets the command for saving the current document or item.
        /// </summary>
        /// <remarks>
        /// Key gesture: Ctrl+S
        /// </remarks>
        public static UICommandBase Save => field ??= new UICommand(nameof(Save), nameof(Save), [new KeyGesture(Keys.S, KeyModifiers.Control)]);

        /// <summary>
        /// Gets the command for saving the current document under a new name.
        /// </summary>
        public static UICommandBase SaveAs => field ??= new UICommand(nameof(SaveAs), nameof(SaveAs));

        /// <summary>
        /// Gets the command to print a document.
        /// </summary>
        /// <remarks>
        /// Key gesture: Ctrl+P
        /// </remarks>
        public static UICommandBase Print => field ??= new UICommand(nameof(Print), nameof(Print), [new KeyGesture(Keys.P, KeyModifiers.Control)]);

        /// <summary>
        /// Gets the command that cancels printing.
        /// </summary>
        public static UICommandBase CancelPrint => field ??= new UICommand(nameof(CancelPrint), nameof(CancelPrint));

        /// <summary>
        /// Gets the command to display a print preview.
        /// </summary>
        /// <remarks>
        /// Key gesture: Ctrl+F2
        /// </remarks>
        public static UICommandBase PrintPreview => field ??= new UICommand(nameof(PrintPreview), nameof(PrintPreview), [new KeyGesture(Keys.F2, KeyModifiers.Control)]);

        /// <summary>
        /// Gets the Properties command.
        /// </summary>
        /// <remarks>
        /// Key gesture: F4
        /// </remarks>
        public static UICommandBase Properties => field ??= new UICommand(nameof(Properties), nameof(Properties), [new KeyGesture(Keys.F4)]);

        /// <summary>
        /// Displays the context menu.
        /// </summary>
        /// <remarks>
        /// Key gesture: Application key, Shift+F10
        /// </remarks>
        public static UICommandBase ContextMenu => field ??= new UICommand(nameof(ContextMenu), nameof(ContextMenu), [new KeyGesture(Keys.Apps), new KeyGesture(Keys.F10, KeyModifiers.Shift)]);

        /// <summary>
        /// Gets the stop command.
        /// </summary>
        /// <remarks>
        /// Key gesture: Escape
        /// </remarks>
        public static UICommandBase Stop => field ??= new UICommand(nameof(Stop), nameof(Stop), [new KeyGesture(Keys.Escape)]);

        /// <summary>
        /// Gets the command for displaying the correction list.
        /// </summary>
        public static UICommandBase CorrectionList => field ??= new UICommand(nameof(CorrectionList), nameof(CorrectionList));

        /// <summary>
        /// Gets the command that performs no action.
        /// </summary>
        public static UICommandBase NotACommand => field ??= new UICommand(nameof(NotACommand), nameof(NotACommand));
    }
}
