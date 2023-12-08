using System.Windows.Input;

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
                ANE.ThrowIfNull(value);
                _gesture = value;
            }
        }

        public InputBinding(ICommand command, InputGesture gesture)
        {
            ANE.ThrowIfNull(command);
            ANE.ThrowIfNull(gesture);

            Command = command;
            _gesture = gesture;
        }
    }
}
