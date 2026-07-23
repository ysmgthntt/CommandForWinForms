namespace CommandForWinForms
{
    public partial class ControlCommandProperties
    {
        extension(Control control)
        {
            public IList<CommandBinding> CommandBindings
                => CollectionHolder<CommandBinding>.GetOrCreate(control);

            public IList<InputBinding> InputBindings
                => CollectionHolder<InputBinding>.GetOrCreate(control);
        }
    }
}
