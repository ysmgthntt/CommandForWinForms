namespace CommandForWinForms
{
    /// <summary>
    /// Defines the source object that invokes the command.
    /// </summary>
    public interface ICommandSource
    {
        /// <summary>
        /// Gets the command that is executed when the command source is invoked.
        /// </summary>
        ICommand Command { get; }

        /// <summary>
        /// Gets the parameter passed to the command.
        /// </summary>
        object? CommandParameter { get; }

        /// <summary>
        /// Gets the target control of the command.
        /// </summary>
        /// <remarks>
        /// <see cref="CommandTarget"/> is only valid when then <see cref="Command"/> property is <see cref="UICommandBase"/>.
        /// Otherwise, <see cref="CommandTarget"/> is ignored.
        /// When <see cref="UICommandBase"/> is used, the <see cref="CommandTarget"/> property is the target for the
        /// <see cref="CommandBinding.Executed"/> and <see cref="CommandBinding.CanExecute"/> events.
        /// If <see langword="null"/>, the currently active control is targeted.
        /// </remarks>
        Control? CommandTarget { get; }
    }
}
