using System.Windows.Input;

namespace CommandForWinForms
{
    public sealed class ExecutedEventArgs : EventArgs
    {
        public ICommand Command { get; }
        public object? Parameter { get; }

        internal ExecutedEventArgs(ICommand command, object? parameter)
        {
            ANE.ThrowIfNull(command);

            Command = command;
            Parameter = parameter;
        }
    }
}
