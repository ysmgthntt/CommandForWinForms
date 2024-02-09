using System.Windows.Input;

namespace CommandForWinForms.Tests
{
    internal sealed class TestCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly Action<object?>? _execute;
        private readonly Func<object?, bool>? _canExecute;

        public TestCommand() { }

        public TestCommand(Action<object?> execute, Func<object?, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            if (_canExecute is null)
                return false;
            return _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            if (_execute is null)
                throw new InvalidOperationException();
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
            => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
