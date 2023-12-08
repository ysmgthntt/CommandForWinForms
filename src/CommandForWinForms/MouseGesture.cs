namespace CommandForWinForms
{
    public sealed class MouseGesture : InputGesture
    {
        public MouseAction MouseAction { get; set; }

        public ModifierKeys Modifiers { get; set; }

        public MouseGesture() { }

        public MouseGesture(MouseAction mouseAction, ModifierKeys modifiers = ModifierKeys.None)
        {
            MouseAction = mouseAction;
            Modifiers = modifiers;
        }

        protected internal override bool Matches(Control target, MouseAction mouseAction, ModifierKeys modifiers)
            => MouseAction != MouseAction.None && mouseAction == MouseAction && ((modifiers & Modifiers) == Modifiers);
    }
}
