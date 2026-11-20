namespace CommandForWinForms
{
    /// <summary>
    /// Specifies modifier keys that can be combined with an input gesture.
    /// </summary>
    [Flags]
    public enum ModifierKeys
    {
        /// <summary>
        /// No modifier keys.
        /// </summary>
        None = Keys.None,

        /// <summary>
        /// The Alt key.
        /// </summary>
        Alt = Keys.Alt,

        /// <summary>
        /// The Control key.
        /// </summary>
        Control = Keys.Control,

        /// <summary>
        /// The Shift key.
        /// </summary>
        Shift = Keys.Shift,
        //Windows,
    }
}
