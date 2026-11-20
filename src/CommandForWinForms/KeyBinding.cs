namespace CommandForWinForms
{
    /// <summary>
    /// Associates a keyboard gesture with a command.
    /// </summary>
    public class KeyBinding : InputBinding
    {
        /// <summary>
        /// Sets the keyboard gesture associated with this binding.
        /// </summary>
        public override InputGesture Gesture
        {
            set
            {
                if (value is not KeyGesture)
                    throw new ArgumentException($"Require {nameof(KeyGesture)}", nameof(value));
                base.Gesture = value;
            }
        }

        /// <summary>
        /// Gets or sets the key associated with this binding.
        /// </summary>
        public Keys Key
        {
            get => ((KeyGesture)Gesture).Key;
            set
            {
                if (value != Key)
                    base.Gesture = new KeyGesture(value, Modifiers);
            }
        }

        /// <summary>
        /// Gets or sets the modifier keys associated with this binding.
        /// </summary>
        public ModifierKeys Modifiers
        {
            get => ((KeyGesture)Gesture).Modifiers;
            set
            {
                if (value != Modifiers)
                    base.Gesture = new KeyGesture(Key, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyBinding"/> class
        /// with the specified command and key gesture.
        /// </summary>
        /// <param name="command">The command associated with this binding.</param>
        /// <param name="gesture">The key gesture associated with this binding.</param>
        public KeyBinding(ICommand command, KeyGesture gesture)
            : base(command, gesture) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyBinding"/> class
        /// with the specified command, key, and modifier keys.
        /// </summary>
        /// <param name="command">The command associated with this binding.</param>
        /// <param name="key">The key associated with this binding.</param>
        /// <param name="modifiers">The modifier keys associated with this binding.</param>
        public KeyBinding(ICommand command, Keys key, ModifierKeys modifiers)
            : base(command, new KeyGesture(key, modifiers)) { }
    }
}
