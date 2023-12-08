using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CommandForWinForms
{
    public sealed class CommandManager
    {
        public CommandManager(Form form)
        {
            ANE.ThrowIfNull(form);

            Initialize(form, Control_GotFocus, Control_KeyDown, Control_ControlAdded);
        }

        private void Initialize(Control control, EventHandler onGotFocus, KeyEventHandler onKeyDown, ControlEventHandler onControlAdded)
        {
            if (control is not (Label or ContainerControl or Panel or GroupBox or Splitter or ProgressBar))
            {
                // TODO: Selectable

                // Enter だとうまくいかないときがある。
                control.GotFocus += onGotFocus;
                control.KeyDown += onKeyDown;
            }
            control.ControlAdded += onControlAdded;

            if (control.Controls is { Count: > 0 } children)
            {
                foreach (Control child in children)
                    Initialize(child, onGotFocus, onKeyDown, onControlAdded);
            }
        }

        private void Control_GotFocus(object? sender, EventArgs e)
        {
            InvalidateRequerySuggested();
        }

        private void Control_KeyDown(object? sender, KeyEventArgs e)
        {
            if (!e.Handled)
                e.SuppressKeyPress = ProcessInput((Control?)sender, e);
        }

        private void Control_ControlAdded(object? sender, ControlEventArgs e)
        {
            var control = e.Control;
            if (control is not null)
                Initialize(control, Control_GotFocus, Control_KeyDown, Control_ControlAdded);
        }

        private static bool ProcessInput(Control? target, KeyEventArgs keyEventArgs)
        {
            while (target is not null)
            {
                if (ControlCommandProperties.TryGetCollection<InputBinding>(target, out var inputBindings))
                {
                    foreach (var inputBinding in inputBindings)
                    {
                        if (inputBinding.Gesture.Matches(target, keyEventArgs))
                        {
                            target.BeginInvoke(ExecuteCommand, inputBinding, target);
                            return true;
                        }
                    }
                }

                if (ControlCommandProperties.TryGetCollection<CommandBinding>(target, out var commandBindings))
                {
                    foreach (var commandBinding in commandBindings)
                    {
                        if (commandBinding.Command is UICommandBase uicommand)
                        {
                            if (uicommand.TryGetInputGestures(out var inputGestures))
                            {
                                foreach (var inputGesture in inputGestures)
                                {
                                    if (inputGesture.Matches(target, keyEventArgs))
                                    {
                                        if (uicommand.ExecuteIfCan(null, target))
                                            return true;

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                target = target.Parent;
            }

            return false;
        }
        private static void ExecuteCommand(ICommandSource commandSource, Control target)
        {
            var command = commandSource.Command;
            var parameter = commandSource.CommandParameter;
            if (command is UICommandBase uicommand)
            {
                uicommand.Execute(parameter, commandSource.CommandTarget ?? target);
            }
            else
            {
                if (command.CanExecute(parameter))
                    command.Execute(parameter);
            }
        }

        private static bool _requerySuggested;

        public static void InvalidateRequerySuggested()
        {
            if (!_requerySuggested)
            {
                var timer = SetTimer(IntPtr.Zero, IntPtr.Zero, 0, _requerySuggestedTimerProc);
                //Debug.WriteLine($"{nameof(SetTimer)}: {timer}");
                _requerySuggested = true;
            }
            else
            {
                //Debug.WriteLine("Requery Suggested");
            }
        }

        private static readonly TimerProc _requerySuggestedTimerProc = RequerySuggestedTimerProc;

        private static void RequerySuggestedTimerProc(IntPtr hWnd, uint uMsg, IntPtr nIDEvent, uint dwTime)
        {
            _requerySuggested = false;
            //Debug.WriteLine($"{nameof(KillTimer)}: {nIDEvent}");
            KillTimer(hWnd, nIDEvent);
            ControlCommandProperties.RaiseCanExecuteChangedAllUICommand();
        }

        private delegate void TimerProc(IntPtr hWnd, uint uMsg, IntPtr nIDEvent, uint dwTime);
        [DllImport("user32.dll", ExactSpelling = true)]
        private static extern IntPtr SetTimer(IntPtr hWnd, IntPtr nIDEvent, uint uElapse, TimerProc lpTimerFunc);
        [DllImport("user32.dll", ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool KillTimer(IntPtr hWnd, IntPtr uIDEvent);
    }
}
