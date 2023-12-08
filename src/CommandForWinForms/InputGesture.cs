namespace CommandForWinForms
{
    public abstract class InputGesture
    {
        protected internal virtual bool Matches(Control target, KeyEventArgs keyEventArgs) => false;

        protected internal virtual bool Matches(Control target, MouseAction mouseAction, ModifierKeys modifiers) => false;
    }
}
