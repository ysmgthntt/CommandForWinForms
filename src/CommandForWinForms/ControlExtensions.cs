namespace CommandForWinForms
{
    public partial class ControlCommandProperties
    {
        extension(Control control)
        {
            /// <summary>
            /// Gets the collection of command bindings associated with this control.
            /// </summary>
            public IList<CommandBinding> CommandBindings
                => CollectionHolder<CommandBinding>.GetOrCreate(control);

            /// <summary>
            /// Gets the collection of input bindings associated with this control.
            /// </summary>
            public IList<InputBinding> InputBindings
                => CollectionHolder<InputBinding>.GetOrCreate(control);
        }
    }
}
