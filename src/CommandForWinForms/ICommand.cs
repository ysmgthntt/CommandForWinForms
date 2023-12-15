#if NET35
namespace CommandForWinForms
{
    public interface ICommand
    {
        event EventHandler? CanExecuteChanged;
        bool CanExecute(object? parameter);
        void Execute(object? parameter);
    }
}
#endif
