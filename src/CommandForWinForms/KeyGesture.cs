namespace CommandForWinForms
{
    public sealed class KeyGesture : InputGesture
    {
        public Keys Key { get; }

        public ModifierKeys Modifiers { get; }

        public string DisplayString { get; }

        public KeyGesture(Keys key)
        {
            Key = key;
            Modifiers = ModifierKeys.None;
            DisplayString = "";
        }

        public KeyGesture(Keys key, ModifierKeys modifiers, string displayString = "")
        {
            ANE.ThrowIfNull(displayString);

            Key = key;
            Modifiers = modifiers;
            DisplayString = displayString;
        }

        protected internal override bool Matches(Control target, KeyEventArgs keyEventArgs)
            => keyEventArgs.KeyCode == Key && keyEventArgs.Modifiers == (Keys)Modifiers;
    }
}
