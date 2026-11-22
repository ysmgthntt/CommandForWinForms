namespace CommandForWinForms
{
    /// <summary>
    /// Represents a keyboard gesture consisting of a key and optional modifier keys.
    /// </summary>
    public sealed class KeyGesture : InputGesture
    {
        /// <summary>
        /// Gets the key associated with this gesture.
        /// </summary>
        public Keys Key { get; }

        /// <summary>
        /// Gets the modifier keys associated with this gesture.
        /// </summary>
        public KeyModifiers Modifiers { get; }

        /// <summary>
        /// Gets the display string representing this gesture.
        /// </summary>
        public string DisplayString { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyGesture"/> class with the specified key.
        /// </summary>
        /// <param name="key">The key associated with this gesture.</param>
        public KeyGesture(Keys key)
        {
            Key = key;
            Modifiers = KeyModifiers.None;
            DisplayString = "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyGesture"/> class with the specified key,
        /// modifier keys, and display string.
        /// </summary>
        /// <param name="key">The key associated with this gesture.</param>
        /// <param name="modifiers">The modifier keys associated with this gesture.</param>
        /// <param name="displayString">The display string representing this gesture.</param>
        public KeyGesture(Keys key, KeyModifiers modifiers, string displayString = "")
        {
            ArgumentNullException.ThrowIfNull(displayString);

            Key = key;
            Modifiers = modifiers;
            DisplayString = displayString;
        }

        /// <inheritdoc/>
        protected internal override bool Matches(Control target, KeyEventArgs keyEventArgs)
            => keyEventArgs.KeyCode == Key && keyEventArgs.Modifiers == (Keys)Modifiers;
    }
}
