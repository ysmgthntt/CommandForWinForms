namespace RoutedCommands.WinForms
{
    /// <summary>
    /// Represents a mouse gesture consisting of a mouse action and optional modifier keys.
    /// </summary>
    public sealed class MouseGesture : InputGesture
    {
        /// <summary>
        /// Gets or sets the mouse action associated with this gesture.
        /// </summary>
        public MouseAction MouseAction { get; set; }

        /// <summary>
        /// Gets or sets the modifier keys associated with this gesture.
        /// </summary>
        public KeyModifiers Modifiers { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseGesture"/> class.
        /// </summary>
        public MouseGesture() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseGesture"/> class
        /// with the specified mouse action and modifier keys.
        /// </summary>
        /// <param name="mouseAction">The mouse action associated with this gesture.</param>
        /// <param name="modifiers">The modifier keys associated with this gesture.</param>
        public MouseGesture(MouseAction mouseAction, KeyModifiers modifiers = KeyModifiers.None)
        {
            MouseAction = mouseAction;
            Modifiers = modifiers;
        }

        /// <inheritdoc/>
        protected internal override bool Matches(Control target, MouseAction mouseAction, KeyModifiers modifiers)
            => MouseAction != MouseAction.None && mouseAction == MouseAction && modifiers == Modifiers;
    }
}
