#nullable disable

namespace CommandForWinForms.Tests
{
    public class MouseBindingTest
    {
        [Fact]
        public void ConstructorTest()
        {
            var command = new TestCommand();
            Assert.Throws<ArgumentNullException>("command", () => new MouseBinding(null, (MouseGesture)null));
            Assert.Throws<ArgumentNullException>("gesture", () => new MouseBinding(command, (MouseGesture)null));
            Assert.Throws<ArgumentNullException>("command", () => new MouseBinding(null, MouseAction.LeftClick));

            var gesture = new MouseGesture(MouseAction.LeftClick, KeyModifiers.Shift);
            var target = new MouseBinding(command, gesture);
            Assert.Equal(command, target.Command);
            Assert.Equal(gesture, target.Gesture);
            Assert.Equal(MouseAction.LeftClick, target.MouseAction);

            target = new MouseBinding(command, MouseAction.RightDoubleClick);
            Assert.Equal(command, target.Command);
            Assert.NotNull(target.Gesture);
            Assert.Equal(MouseAction.RightDoubleClick, target.MouseAction);
        }

        [Fact]
        public void GestureTest()
        {
            var command = new TestCommand();
            var target = new MouseBinding(command, new MouseGesture(MouseAction.LeftClick));
            Assert.Throws<ArgumentException>("value", () => target.Gesture = null);
            Assert.Throws<ArgumentException>("value", () => target.Gesture = new KeyGesture(Keys.A));

            var gesture = new MouseGesture(MouseAction.MiddleDoubleClick, KeyModifiers.Control);
            target.Gesture = gesture;
            Assert.Equal(gesture, target.Gesture);
            Assert.Equal(MouseAction.MiddleDoubleClick, target.MouseAction);
        }

        [Fact]
        public void MouseActionTest()
        {
            var target = new MouseBinding(new TestCommand(), MouseAction.LeftClick);
            target.MouseAction = MouseAction.MiddleClick;

            Assert.Equal(MouseAction.MiddleClick, target.MouseAction);
            Assert.Equal(MouseAction.MiddleClick, ((MouseGesture)target.Gesture).MouseAction);
        }
    }
}
