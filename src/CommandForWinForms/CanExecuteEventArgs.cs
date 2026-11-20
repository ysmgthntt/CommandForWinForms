using System.ComponentModel;

namespace CommandForWinForms
{
    /// <summary>
    /// Provides data for events that determine whether a command can be executed.
    /// </summary>
    public sealed class CanExecuteEventArgs : HandledEventArgs
    {
        /// <summary>
        /// Gets the command associated with the event.
        /// </summary>
        public ICommand Command { get; }

        /// <summary>
        /// Gets the parameter passed to the command.
        /// </summary>
        public object? Parameter { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the command can be executed.
        /// </summary>
        public bool CanExecute { get; set; }

        internal CanExecuteEventArgs(ICommand command, object? parameter)
        {
            ArgumentNullException.ThrowIfNull(command);

            Command = command;
            Parameter = parameter;
        }
    }
}
