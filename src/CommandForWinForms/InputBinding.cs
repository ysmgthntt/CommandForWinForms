namespace RoutedCommands.WinForms
{
    /// <summary>
    /// Associates an input gesture with a command.
    /// </summary>
    /// <remarks>
    /// When the specified input gesture is recognized, the associated command is invoked.
    /// </remarks>
    public class InputBinding : ICommandSource
    {
        /// <summary>
        /// Gets or sets the command associated with this binding.
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// Gets or sets the parameter passed to the command.
        /// </summary>
        public object? CommandParameter { get; set; }

        /// <summary>
        /// Gets or sets the target control of the command.
        /// </summary>
        /// <remarks>
        /// If <see langword="null"/>, the control to which this input binding is associated is used.
        /// </remarks>
        public Control? CommandTarget { get; set; }

        private InputGesture _gesture;

        /// <summary>
        /// Gets or sets the input gesture associated with this binding.
        /// </summary>
        public virtual InputGesture Gesture
        {
            get => _gesture;
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                _gesture = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputBinding"/> class
        /// with the specified command and input gesture.
        /// </summary>
        /// <param name="command">The command associated with this binding.</param>
        /// <param name="gesture">The input gesture associated with this binding.</param>
        public InputBinding(ICommand command, InputGesture gesture)
        {
            ArgumentNullException.ThrowIfNull(command);
            ArgumentNullException.ThrowIfNull(gesture);

            Command = command;
            _gesture = gesture;
        }
    }
}
