namespace RoutedCommands.WinForms
{
    /// <summary>
    /// Specifies the type of mouse action associated with a mouse gesture.
    /// </summary>
    public enum MouseAction
    {
        /// <summary>
        /// No mouse action.
        /// </summary>
        None,

        /// <summary>
        /// A left mouse button click.
        /// </summary>
        LeftClick,

        /// <summary>
        /// A right mouse button click.
        /// </summary>
        RightClick,

        /// <summary>
        /// A middle mouse button click.
        /// </summary>
        MiddleClick,

        /// <summary>
        /// A mouse wheel button click.
        /// </summary>
        WheelClick,

        /// <summary>
        /// A left mouse button double-click.
        /// </summary>
        LeftDoubleClick,

        /// <summary>
        /// A right mouse button double-click.
        /// </summary>
        RightDoubleClick,

        /// <summary>
        /// A middle mouse button double-click.
        /// </summary>
        MiddleDoubleClick,
    }
}
