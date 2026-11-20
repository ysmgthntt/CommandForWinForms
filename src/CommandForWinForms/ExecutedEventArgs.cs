namespace CommandForWinForms
{
    /// <summary>
    /// Provides data for a command execution event.
    /// </summary>
    public sealed class ExecutedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the command that was executed.
        /// </summary>
        public ICommand Command { get; }

        /// <summary>
        /// Gets the parameter supplied to the command.
        /// </summary>
        public object? Parameter { get; }

        internal ExecutedEventArgs(ICommand command, object? parameter)
        {
            ArgumentNullException.ThrowIfNull(command);

            Command = command;
            Parameter = parameter;
        }
    }
}
