using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace CommandForWinForms
{
    public abstract class UICommandBase : ICommand
    {
        public abstract string Name { get; }

        public abstract string Text { get; }

        private IList<InputGesture>? _inputGestures;

        public IList<InputGesture> InputGestures => _inputGestures ??= (CreateInputGestures() ?? []);

        protected virtual IList<InputGesture>? CreateInputGestures() => null;

        internal bool TryGetInputGestures([NotNullWhen(true)] out IList<InputGesture>? inputGestures)
        {
            if (_inputGestures is not null)
            {
                inputGestures = _inputGestures;
                return true;
            }
            inputGestures = CreateInputGestures();
            if (inputGestures is not null)
            {
                _inputGestures = inputGestures;
                return true;
            }
            return false;
        }

        protected UICommandBase() { }

        protected UICommandBase(IList<InputGesture>? inputGestures)
        {
            _inputGestures = inputGestures;
        }

        // ICommand

        private readonly WeakEventHandler _canExecuteChanged = new();
        public event EventHandler? CanExecuteChanged
        {
            add => _canExecuteChanged.AddHandler(value);
            remove => _canExecuteChanged.RemoveHandler(value);
        }

        bool ICommand.CanExecute(object? parameter)
            => CanExecute(parameter, null);

        void ICommand.Execute(object? parameter)
            => Execute(parameter, null);

        //

        // cache
        private bool _inRaiseCanExecuteChanged;
        private object? _lastCanExecuteParameter;
        private Control? _lastCanExecuteTarget;
        private bool _lastCanExecuteResult;

        public bool CanExecute(object? parameter, Control? target)
        {
            target ??= GetActiveControl();

            if (target is not null)
            {
                if (_inRaiseCanExecuteChanged)
                {
                    if (parameter == _lastCanExecuteParameter && target == _lastCanExecuteTarget)
                        return _lastCanExecuteResult;

                    _lastCanExecuteResult = CanExecuteCore(parameter, target);
                    _lastCanExecuteParameter = parameter;
                    _lastCanExecuteTarget = target;
                    return _lastCanExecuteResult;
                }

                return CanExecuteCore(parameter, target);
            }

            return false;
        }

        private bool CanExecuteCore(object? parameter, Control target)
        {
            var targetCommandBindings = GetTargetCommandBindings(target);

            if (targetCommandBindings.Count > 0)
            {
                CanExecuteEventArgs? e = null;

                // Preview
                for (int i = targetCommandBindings.Count - 1; i >= 0; i--)
                {
                    var ccb = targetCommandBindings[i];
                    if (ccb.CommandBinding.OnPreviewCanExecute(ccb.Control, parameter, ref e))
                        return true;
                    if (e is { Handled: true })
                        return false;
                }

                for (int i = 0; i < targetCommandBindings.Count; i++)
                {
                    var ccb = targetCommandBindings[i];
                    if (ccb.CommandBinding.OnCanExecute(ccb.Control, parameter, ref e))
                        return true;
                    if (e is { Handled: true })
                        return false;
                }
            }

            return false;
        }

        internal void RaiseCanExecuteChanged()
        {
            if (!_canExecuteChanged.IsEmpty)
            {
                _inRaiseCanExecuteChanged = true;
                _lastCanExecuteParameter = null;
                _lastCanExecuteTarget = null;
                try
                {
                    _canExecuteChanged.Invoke(this, EventArgs.Empty);
                }
                finally
                {
                    _inRaiseCanExecuteChanged = false;
                    _lastCanExecuteParameter = null;
                    _lastCanExecuteTarget = null;
                }
            }
        }

        public void Execute(object? parameter, Control? target)
        {
            target ??= GetActiveControl();
            CanExecuteEventArgs? e = null;

            while (target is not null)
            {
                if (ControlCommandProperties.TryGetCollection<CommandBinding>(target, out var commandBindings))
                {
                    foreach (var commandBinding in commandBindings)
                    {
                        if (commandBinding.Command == this)
                        {
                            if (commandBinding.OnExecuted(target, parameter, ref e))
                                return;
                        }
                    }
                }
                target = target.Parent;
            }
        }

        internal bool ExecuteIfCan(object? parameter, Control? target)
        {
            target ??= GetActiveControl();
            if (target is not null)
            {
                var targetCommandBindings = GetTargetCommandBindings(target);
                if (targetCommandBindings.Count > 0)
                {
                    CanExecuteEventArgs? e = null;

                    // Preview
                    for (int i = targetCommandBindings.Count - 1; i >= 0; i--)
                    {
                        var ccb = targetCommandBindings[i];
                        if (ccb.CommandBinding.OnPreviewCanExecute(ccb.Control, parameter, ref e))
                            break;
                        if (e is { Handled: true })
                            return false;
                    }

                    for (int i = 0; i < targetCommandBindings.Count; i++)
                    {
                        var ccb = targetCommandBindings[i];
                        if (ccb.CommandBinding.OnExecuted(ccb.Control, parameter, ref e))
                            return true;
                    }
                }
            }

            return false;
        }

        private static Control? GetActiveControl()
            => Form.ActiveForm?.GetActiveControl();

        private List<ControlAndCommandBinding> GetTargetCommandBindings(Control target)
        {
            var targetCommandBindings = new List<ControlAndCommandBinding>();
            do
            {
                if (ControlCommandProperties.TryGetCollection<CommandBinding>(target, out var commandBindings))
                {
                    foreach (var commandBinding in commandBindings)
                    {
                        if (commandBinding.Command == this)
                            targetCommandBindings.Add(new ControlAndCommandBinding(target, commandBinding));
                    }
                }
                target = target.Parent!;
            }
            while (target is not null);

            return targetCommandBindings;
        }

        private readonly struct ControlAndCommandBinding(Control control, CommandBinding commandBinding)
        {
            public readonly Control Control = control;
            public readonly CommandBinding CommandBinding = commandBinding;
        }
    }
}
