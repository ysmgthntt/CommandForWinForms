namespace CommandForWinForms
{
    public class MouseBinding : InputBinding
    {
        public override InputGesture Gesture
        {
            set
            {
                if (value is not MouseGesture)
                    throw new ArgumentException($"Require {nameof(MouseGesture)}", nameof(value));
                base.Gesture = value;
            }
        }

        public MouseAction MouseAction
        {
            get => ((MouseGesture)Gesture).MouseAction;
            set
            {
                if (value != MouseAction)
                    base.Gesture = new MouseGesture(value);
            }
        }

        public MouseBinding(ICommand command, MouseGesture gesture)
            : base(command, gesture) { }

        public MouseBinding(ICommand command, MouseAction mouseAction)
            : base(command, new MouseGesture(mouseAction)) { }
    }
}
