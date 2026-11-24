using System.ComponentModel;

namespace RoutedCommands.WinForms
{
    /// <summary>
    /// Binds a command to the events that determine and perform its execution.
    /// </summary>
    /// <remarks>
    /// A command binding can handle the command's execution and determine whether the command can be executed.
    /// It can also handle preview events before the command's execution availability is evaluated.
    /// </remarks>
    public class CommandBinding
    {
        private ICommand _command;

        /// <summary>
        /// Gets or sets the command associated with this binding.
        /// </summary>
        public ICommand Command
        {
            get => _command;
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                _command = value;
            }
        }

        /// <summary>
        /// Occurs when the bound command is executed.
        /// </summary>
        public event EventHandler<ExecutedEventArgs>? Executed;

        /// <summary>
        /// Occurs before the regular can-execute query is raised.
        /// </summary>
        public event EventHandler<CanExecuteEventArgs>? PreviewCanExecute;

        /// <summary>
        /// Occurs when the bound command's ability to execute is queried.
        /// </summary>
        public event EventHandler<CanExecuteEventArgs>? CanExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBinding"/> class.
        /// </summary>
        /// <param name="command">The command associated with this binding.</param>
        /// <param name="executed">The event handler for the <see cref="Executed"/> event.</param>
        /// <param name="canExecute">The event handler for the <see cref="CanExecute"/> event.</param>
        public CommandBinding(ICommand command, EventHandler<ExecutedEventArgs>? executed = null, EventHandler<CanExecuteEventArgs>? canExecute = null)
        {
            ArgumentNullException.ThrowIfNull(command);

            _command = command;
            Executed = executed;
            CanExecute = canExecute;
        }

        internal bool OnPreviewCanExecute(object sender, object? parameter, ref CanExecuteEventArgs? e)
        {
            if (PreviewCanExecute is null)
                return false;

            e ??= new CanExecuteEventArgs(_command, parameter);
            PreviewCanExecute(sender, e);
            return e.CanExecute;
        }

        internal bool OnCanExecute(object sender, object? parameter, ref CanExecuteEventArgs? e)
        {
            if (CanExecute is null)
                return (Executed is not null);

            e ??= new CanExecuteEventArgs(_command, parameter);
            CanExecute(sender, e);
            return e.CanExecute;
        }

        internal bool OnExecuted(ISynchronizeInvoke sender, object? parameter, ref CanExecuteEventArgs? e)
        {
            if (Executed is not null)
            {
                bool canExecute = true;
                if (CanExecute is not null)
                {
                    if (e is null)
                        e = new CanExecuteEventArgs(_command, parameter);
                    else
                        e.CanExecute = false;
                    CanExecute(sender, e);
                    canExecute = e.CanExecute;
                }
                if (canExecute)
                {
                    sender.BeginInvoke(Executed, [sender, new ExecutedEventArgs(_command, parameter)]);
                    return true;
                }
            }
            return false;
        }
    }
}
