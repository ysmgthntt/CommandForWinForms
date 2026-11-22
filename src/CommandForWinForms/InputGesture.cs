namespace CommandForWinForms
{
    /// <summary>
    /// Represents an input gesture used to recognize user input associated with a command.
    /// </summary>
    /// <remarks>
    /// Derive from this class to define custom input gestures by overriding the appropriate
    /// <see cref="Matches(Control, KeyEventArgs)"/> or
    /// <see cref="Matches(Control, MouseAction, KeyModifiers)"/> method.
    /// </remarks>
    public abstract class InputGesture
    {
        /// <summary>
        /// Determines whether the specified keyboard input matches this gesture.
        /// </summary>
        /// <param name="target">The control that received the keyboard input.</param>
        /// <param name="keyEventArgs">The keyboard event data.</param>
        /// <returns>
        /// <see langword="true"/> if the input matches this gesture; otherwise, <see langword="false"/>.
        /// </returns>
        protected internal virtual bool Matches(Control target, KeyEventArgs keyEventArgs) => false;

        /// <summary>
        /// Determines whether the specified mouse input matches this gesture.
        /// </summary>
        /// <param name="target">The control that received the mouse input.</param>
        /// <param name="mouseAction">The mouse action.</param>
        /// <param name="modifiers">The modifier keys pressed during the mouse action.</param>
        /// <returns>
        /// <see langword="true"/> if the input matches this gesture; otherwise, <see langword="false"/>.
        /// </returns>
        protected internal virtual bool Matches(Control target, MouseAction mouseAction, KeyModifiers modifiers) => false;
    }
}
