using System.ComponentModel;

namespace CommandForWinForms
{
    public sealed class CanExecuteEventArgs : HandledEventArgs
    {
        public ICommand Command { get; }
        public object? Parameter { get; }
        public bool CanExecute { get; set; }

        internal CanExecuteEventArgs(ICommand command, object? parameter)
        {
            ANE.ThrowIfNull(command);

            Command = command;
            Parameter = parameter;
        }
    }
}
