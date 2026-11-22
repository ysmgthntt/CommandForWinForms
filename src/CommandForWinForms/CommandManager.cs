using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CommandForWinForms
{
    /// <summary>
    /// Manages command processing for a form.
    /// </summary>
    /// <remarks>
    /// This class provides static methods to initialize and manage command processing for a form and its controls.
    /// </remarks>
    public static class CommandManager
    {
        /// <summary>
        /// Initializes command handling for a form.
        /// </summary>
        /// <param name="form">The form to initialize.</param>
        public static void Initialize(Form form)
        {
            ArgumentNullException.ThrowIfNull(form);

            Initialize(form, Control_GotFocus, Control_KeyDown, Control_MouseClick, Control_MouseWheel, Control_ControlAdded);
        }

        private static void Initialize(Control control, EventHandler onGotFocus, KeyEventHandler onKeyDown,
            MouseEventHandler onMouseClick, MouseEventHandler onMouseWheel, ControlEventHandler onControlAdded)
        {
            if (IsSelectableControl(control))
            {
                // Enter だとうまくいかないときがある。
                control.GotFocus += onGotFocus;
                control.KeyDown += onKeyDown;
            }
            control.MouseClick += onMouseClick;
            control.MouseDoubleClick += onMouseClick;
            control.MouseWheel += onMouseWheel;
            control.ControlAdded += onControlAdded;

            if (control.Controls is { Count: > 0 } children)
            {
                foreach (Control child in children)
                    Initialize(child, onGotFocus, onKeyDown, onMouseClick, onMouseWheel, onControlAdded);
            }
        }

        private static void Control_GotFocus(object? sender, EventArgs e)
        {
            InvalidateRequerySuggested();
        }

        private static void Control_KeyDown(object? sender, KeyEventArgs e)
        {
            if (!e.Handled)
                e.SuppressKeyPress = ProcessInput((Control?)sender, (target, gesture) => gesture.Matches(target, e), true);
        }

        private static void Control_MouseClick(object? sender, MouseEventArgs e)
        {
            var modifiers = (ModifierKeys)Control.ModifierKeys;
            var mouseAction = e switch
            {
                { Button: MouseButtons.Left, Clicks: 2 } => MouseAction.LeftDoubleClick,
                { Button: MouseButtons.Right, Clicks: 2 } => MouseAction.RightDoubleClick,
                { Button: MouseButtons.Middle, Clicks: 2 } => MouseAction.MiddleDoubleClick,
                { Button: MouseButtons.Left } => MouseAction.LeftClick,
                { Button: MouseButtons.Right } => MouseAction.RightClick,
                { Button: MouseButtons.Middle } => MouseAction.MiddleClick,
                _ => MouseAction.None
            };
            ProcessInput((Control?)sender, (target, gesture) => gesture.Matches(target, mouseAction, modifiers), false);
        }

        private static void Control_MouseWheel(object? sender, MouseEventArgs e)
        {
            var handledMouseEventArgs = (HandledMouseEventArgs)e;
            if (!handledMouseEventArgs.Handled)
            {
                var modifiers = (ModifierKeys)Control.ModifierKeys;
                handledMouseEventArgs.Handled = ProcessInput((Control?)sender, (target, gesture) => gesture.Matches(target, MouseAction.WheelClick, modifiers), false);
            }
        }

        private static void Control_ControlAdded(object? sender, ControlEventArgs e)
        {
            var control = e.Control;
            if (control is not null)
                Initialize(control, Control_GotFocus, Control_KeyDown, Control_MouseClick, Control_MouseWheel, Control_ControlAdded);
        }

        private static bool ProcessInput(Control? target, Func<Control, InputGesture, bool> matches, bool post)
        {
            while (target is not null)
            {
                if (ControlCommandProperties.TryGetCollection<InputBinding>(target, out var inputBindings))
                {
                    foreach (var inputBinding in inputBindings)
                    {
                        if (matches(target, inputBinding.Gesture))
                        {
                            if (post)
                                ((ISynchronizeInvoke)target).BeginInvoke(ExecuteCommand, [inputBinding, target]);
                            else
                                ExecuteCommand(inputBinding, target);
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
                                    if (matches(target, inputGesture))
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

#if NET8_0_OR_GREATER
        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "GetStyle")]
        private static extern bool GetStyle(Control control, ControlStyles flag);
#else
        private static Func<Control, ControlStyles, bool> _getStyleMethod = CreateGetStyleMethod();

        private static Func<Control, ControlStyles, bool> CreateGetStyleMethod()
        {
            var method = typeof(Control).GetMethod("GetStyle", BindingFlags.NonPublic | BindingFlags.Instance)!;
            return (Func<Control, ControlStyles, bool>)Delegate.CreateDelegate(typeof(Func<Control, ControlStyles, bool>), method);
        }
#endif

        private static bool IsSelectableControl(Control control)
        {
            if (control is Label or ContainerControl or Panel or GroupBox or Splitter or ProgressBar)
                return false;
#if NET8_0_OR_GREATER
            return GetStyle(control, ControlStyles.Selectable);
#else
            return _getStyleMethod(control, ControlStyles.Selectable);
#endif
        }

        //

        private static bool _requerySuggested;

        /// <summary>
        /// Schedules a refresh of the enabled state of commands that support requery.
        /// </summary>
        /// <remarks>
        /// Raises the <see cref="UICommandBase.CanExecuteChanged"/> event
        /// for all existing <see cref="UICommandBase"/> instances.
        /// </remarks>
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
            ControlCommandProperties.RaiseCanExecuteChangedAllUICommands();
        }

        private delegate void TimerProc(IntPtr hWnd, uint uMsg, IntPtr nIDEvent, uint dwTime);
        [DllImport("user32.dll", ExactSpelling = true)]
        private static extern IntPtr SetTimer(IntPtr hWnd, IntPtr nIDEvent, uint uElapse, TimerProc lpTimerFunc);
        [DllImport("user32.dll", ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool KillTimer(IntPtr hWnd, IntPtr uIDEvent);
    }
}
