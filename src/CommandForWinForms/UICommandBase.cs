using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace CommandForWinForms
{
    /// <summary>
    /// Provides the base class for commands that can be associated with user interface elements.
    /// </summary>
    /// <remarks>
    /// A command can define its name, display text, and input gestures.
    /// It also provides methods to determine whether the command can be executed
    /// and to execute the command against a specified target control.
    /// </remarks>
    public abstract class UICommandBase : ICommand
    {
        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the display text of the command.
        /// </summary>
        public abstract string Text { get; }

        private IList<InputGesture>? _inputGestures;

        /// <summary>
        /// Gets the collection of input gestures associated with this command.
        /// </summary>
        /// <remarks>
        /// If no input gestures were specified during initialization, <see cref="CreateInputGestures"/> is called
        /// the first time this property is accessed. If that method returns <see langword="null"/>,
        /// an empty collection is used.
        /// </remarks>
        public IList<InputGesture> InputGestures => _inputGestures ??= (CreateInputGestures() ?? []);

        /// <summary>
        /// Creates the default collection of input gestures for the command.
        /// </summary>
        /// <returns>
        /// The default input gestures, or <see langword="null"/> if no default input gestures are defined.
        /// </returns>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="UICommandBase"/> class.
        /// </summary>
        protected UICommandBase() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UICommandBase"/> class
        /// with the specified input gestures.
        /// </summary>
        /// <param name="inputGestures">
        /// The input gestures associated with the command.
        /// If <see langword="null"/>, the default input gestures are created when <see cref="InputGestures"/> is first accessed.
        /// </param>
        protected UICommandBase(IList<InputGesture>? inputGestures)
        {
            _inputGestures = inputGestures;
        }

        // ICommand

        private EventHandler? _canExecuteChanged;

        /// <summary>
        /// Occurs when changes occur that affect whether the command can be executed.
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add
            {
                _canExecuteChanged += value;
                if (value?.Target is IComponent component)
                    component.Disposed += (s, e) => _canExecuteChanged -= value;
            }
            remove
            {
                _canExecuteChanged -= value;
            }
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

        /// <summary>
        /// Determines whether the command can be executed with the specified parameter and target control.
        /// </summary>
        /// <param name="parameter">The parameter passed to the command.</param>
        /// <param name="target">The target control, or <see langword="null"/> to use the active control.</param>
        /// <returns>
        /// <see langword="true"/> if the command can be executed; otherwise, <see langword="false"/>.
        /// </returns>
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
            if (_canExecuteChanged is not null)
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

        /// <summary>
        /// Executes the command with the specified parameter and target control.
        /// </summary>
        /// <param name="parameter">The parameter passed to the command.</param>
        /// <param name="target">The target control, or <see langword="null"/> to use the active control.</param>
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
