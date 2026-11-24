namespace RoutedCommands.WinForms
{
    /// <summary>
    /// Associates a mouse gesture with a command.
    /// </summary>
    public class MouseBinding : InputBinding
    {
        /// <summary>
        /// Sets the mouse gesture associated with this binding.
        /// </summary>
        public override InputGesture Gesture
        {
            set
            {
                if (value is not MouseGesture)
                    throw new ArgumentException($"Require {nameof(MouseGesture)}", nameof(value));
                base.Gesture = value;
            }
        }

        /// <summary>
        /// Gets or sets the mouse action associated with this binding.
        /// </summary>
        public MouseAction MouseAction
        {
            get => ((MouseGesture)Gesture).MouseAction;
            set
            {
                if (value != MouseAction)
                    base.Gesture = new MouseGesture(value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseBinding"/> class
        /// with the specified command and mouse gesture.
        /// </summary>
        /// <param name="command">The command associated with this binding.</param>
        /// <param name="gesture">The mouse gesture associated with this binding.</param>
        public MouseBinding(ICommand command, MouseGesture gesture)
            : base(command, gesture) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseBinding"/> class
        /// with the specified command and mouse action.
        /// </summary>
        /// <param name="command">The command associated with this binding.</param>
        /// <param name="mouseAction">The mouse action associated with this binding.</param>
        public MouseBinding(ICommand command, MouseAction mouseAction)
            : base(command, new MouseGesture(mouseAction)) { }
    }
}
