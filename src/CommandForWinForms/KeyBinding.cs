using System.Windows.Input;

namespace CommandForWinForms
{
    public class KeyBinding : InputBinding
    {
        public override InputGesture Gesture
        {
            set
            {
                if (value is not KeyGesture)
                    throw new ArgumentException($"Require {nameof(KeyGesture)}", nameof(value));
                base.Gesture = value;
            }
        }

        public Keys Key
        {
            get => ((KeyGesture)Gesture).Key;
            set
            {
                if (value != Key)
                    base.Gesture = new KeyGesture(value, Modifiers);
            }
        }


        public ModifierKeys Modifiers
        {
            get => ((KeyGesture)Gesture).Modifiers;
            set
            {
                if (value != Modifiers)
                    base.Gesture = new KeyGesture(Key, value);
            }
        }

        public KeyBinding(ICommand command, KeyGesture gesture)
            : base(command, gesture) { }

        public KeyBinding(ICommand command, Keys key, ModifierKeys modifiers)
            : base(command, new KeyGesture(key, modifiers)) { }
    }
}
