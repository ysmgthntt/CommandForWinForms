namespace CommandForWinForms
{
    /// <summary>
    /// Represents a command with a display text and a name.
    /// </summary>
    public sealed class UICommand : UICommandBase
    {
        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        public override string Name { get; }

        /// <summary>
        /// Gets the display text of the command.
        /// </summary>
        public override string Text { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UICommand"/> class.
        /// </summary>
        /// <param name="text">The display text of the command.</param>
        /// <param name="name">The name of the command.</param>
        /// <param name="inputGestures">The input gestures associated with the command.</param>
        public UICommand(string text, string name, IList<InputGesture>? inputGestures = null)
            : base(inputGestures)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(name);
            ArgumentNullException.ThrowIfNull(text);

            Name = name;
            Text = text;
        }
    }
}
