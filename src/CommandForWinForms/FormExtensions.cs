namespace CommandForWinForms
{
    internal static class FormExtensions
    {
        public static Control? GetActiveControl(this Form form)
        {
            Control? activeControl = null;
            IContainerControl? container = form;
            do
            {
                activeControl = container.ActiveControl;
                if (activeControl == container)
                    break;
                container = activeControl as IContainerControl;
            }
            while (container is not null);
            return activeControl;
        }
    }
}
