namespace CommandForWinForms
{
    public sealed class UICommand : UICommandBase
    {
        public override string Name { get; }

        public override string Text { get; }

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
