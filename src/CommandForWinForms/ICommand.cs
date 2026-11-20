#if NET35
namespace CommandForWinForms
{
    /// <summary>
    /// Defines a command that can be executed and whose execution availability can be determined.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Occurs when changes occur that affect whether the command can be executed.
        /// </summary>
        event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Determines whether the command can be executed with the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter passed to the command.</param>
        /// <returns>
        /// <see langword="true"/> if the command can be executed; otherwise, <see langword="false"/>.
        /// </returns>
        bool CanExecute(object? parameter);

        /// <summary>
        /// Executes the command with the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter passed to the command.</param>
        void Execute(object? parameter);
    }
}
#endif
