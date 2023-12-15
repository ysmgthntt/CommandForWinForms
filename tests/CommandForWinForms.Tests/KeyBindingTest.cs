#nullable disable

namespace CommandForWinForms.Tests
{
    public class KeyBindingTest
    {
        [Fact]
        public void ConstructorTest()
        {
            var command = new TestCommand();
            Assert.Throws<ArgumentNullException>("command", () => new KeyBinding(null, null));
            Assert.Throws<ArgumentNullException>("gesture", () => new KeyBinding(command, null));
            Assert.Throws<ArgumentNullException>("command", () => new KeyBinding(null, Keys.A, ModifierKeys.None));

            var gesture = new KeyGesture(Keys.A, ModifierKeys.Shift);
            var target = new KeyBinding(command, gesture);
            Assert.Equal(command, target.Command);
            Assert.Equal(gesture, target.Gesture);
            Assert.Equal(Keys.A, target.Key);
            Assert.Equal(ModifierKeys.Shift, target.Modifiers);

            target = new KeyBinding(command, Keys.B, ModifierKeys.Alt | ModifierKeys.Control);
            Assert.Equal(command, target.Command);
            Assert.NotNull(target.Gesture);
            Assert.Equal(Keys.B, target.Key);
            Assert.Equal(ModifierKeys.Alt | ModifierKeys.Control, target.Modifiers);
        }

        [Fact]
        public void GestureTest()
        {
            var command = new TestCommand();
            var target = new KeyBinding(command, new KeyGesture(Keys.A));
            Assert.Throws<ArgumentException>("value", () => target.Gesture = null);
            Assert.Throws<ArgumentException>("value", () => target.Gesture = new MouseGesture());

            var gesture = new KeyGesture(Keys.B, ModifierKeys.Shift);
            target.Gesture = gesture;
            Assert.Equal(gesture, target.Gesture);
            Assert.Equal(Keys.B, target.Key);
            Assert.Equal(ModifierKeys.Shift, target.Modifiers);
        }

        [Fact]
        public void KeyTest()
        {
            var command = new TestCommand();
            var gesture = new KeyGesture(Keys.A, ModifierKeys.Shift);
            var target = new KeyBinding(command, gesture);
            target.Key = Keys.B;

            Assert.Equal(Keys.B, target.Key);
            Assert.Equal(ModifierKeys.Shift, target.Modifiers);
        }

        [Fact]
        public void ModifiersTest()
        {
            var command = new TestCommand();
            var gesture = new KeyGesture(Keys.A, ModifierKeys.Shift);
            var target = new KeyBinding(command, gesture);
            target.Modifiers = ModifierKeys.Alt | ModifierKeys.Control;

            Assert.Equal(Keys.A, target.Key);
            Assert.Equal(ModifierKeys.Alt | ModifierKeys.Control, target.Modifiers);
        }
    }
}
