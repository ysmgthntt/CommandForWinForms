namespace CommandForWinForms.Tests
{
    internal static class Locks
    {
        public static readonly object CommandBindings = new object();
        public static readonly object InputBindings = new object();
        public static readonly object SetCommand = new object();
    }
}
