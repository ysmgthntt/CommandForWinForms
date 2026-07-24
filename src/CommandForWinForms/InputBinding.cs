namespace CommandForWinForms
{
    public class InputBinding : ICommandSource
    {
        public ICommand Command { get; set; }

        public object? CommandParameter { get; set; }

        public Control? CommandTarget { get; set; }

        private InputGesture _gesture;

        public virtual InputGesture Gesture
        {
            get => _gesture;
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                _gesture = value;
            }
        }

        public InputBinding(ICommand command, InputGesture gesture)
        {
            ArgumentNullException.ThrowIfNull(command);
            ArgumentNullException.ThrowIfNull(gesture);

            Command = command;
            _gesture = gesture;
        }
    }
}
