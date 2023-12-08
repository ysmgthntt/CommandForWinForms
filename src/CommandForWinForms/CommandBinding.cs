using System.ComponentModel;
using System.Windows.Input;

namespace CommandForWinForms
{
    public class CommandBinding
    {
        private ICommand _command;

        public ICommand Command
        {
            get => _command;
            set
            {
                ANE.ThrowIfNull(value);
                _command = value;
            }
        }

        public event EventHandler<ExecutedEventArgs>? Executed;

        public event EventHandler<CanExecuteEventArgs>? PreviewCanExecute;

        public event EventHandler<CanExecuteEventArgs>? CanExecute;

        public CommandBinding(ICommand command, EventHandler<ExecutedEventArgs>? executed = null, EventHandler<CanExecuteEventArgs>? canExecute = null)
        {
            ANE.ThrowIfNull(command);

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
