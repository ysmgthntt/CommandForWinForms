using System.Windows.Input;

namespace CommandForWinForms
{
    partial class ControlCommandProperties
    {
        private abstract class CommandHandler
        {
            public readonly ICommand Command;
            private readonly object? _parameter;
            private readonly Control? _target;

            public CommandHandler(ICommand command, object? parameter, Control? target)
            {
                Command = command;
                _parameter = parameter;
                _target = target;

                Command.CanExecuteChanged += OnCanExecuteChanged;
            }

            protected virtual bool Available => true;

            protected virtual Control? SourceControl => null;

            private Control? GetTarget() => _target ?? SourceControl?.FindForm()?.GetActiveControl();

            protected abstract void SetEnabled(bool enabled);

            protected void OnCanExecuteChanged(object? sender, EventArgs e)
            {
                if (!Available)
                    return;

                bool enabled;
                if (Command is UICommandBase uicommand)
                    enabled = uicommand.CanExecute(_parameter, GetTarget());
                else
                    enabled = Command.CanExecute(_parameter);

                SetEnabled(enabled);
            }

            protected void OnExecute(object? sender, EventArgs e)
            {
                if (Command is UICommandBase uicommand)
                {
                    uicommand.ExecuteIfCan(_parameter, GetTarget());
                }
                else
                {
                    if (Command.CanExecute(_parameter))
                        Command.Execute(_parameter);
                }
            }

            protected void OnDisposed(object? sender, EventArgs e)
            {
                DetachEvents();
            }

            public void DetachEvents()
            {
                Command.CanExecuteChanged -= OnCanExecuteChanged;
                DetachComponentEvents();
            }

            protected abstract void DetachComponentEvents();
        }
    }
}
