namespace CommandForWinForms
{
    public interface ICommandSource
    {
        ICommand Command { get; }
        object? CommandParameter { get; }
        Control? CommandTarget { get; }
    }
}
