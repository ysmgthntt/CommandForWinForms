#nullable disable

namespace CommandForWinForms.Tests
{
    public class InputBindingTest
    {
        [Fact]
        public void ConstructorTest()
        {
            var command = new TestCommand();
            Assert.Throws<ArgumentNullException>("command", () => new InputBinding(null, null));
            Assert.Throws<ArgumentNullException>("gesture", () => new InputBinding(command, null));

            var gesture = new KeyGesture(Keys.A);
            var target = new InputBinding(command, gesture);
            Assert.Equal(command, target.Command);
            Assert.Equal(gesture, target.Gesture);
            Assert.Null(target.CommandParameter);
            Assert.Null(target.CommandTarget);
        }

        [Fact]
        public void CommandTest()
        {
            var target = new InputBinding(new TestCommand(), new KeyGesture(Keys.A));
            //Assert.Throws<ArgumentNullException>("value", () => target.Command = null);
            var command = new TestCommand();
            target.Command = command;
            Assert.Equal(command, target.Command);
        }

        [Fact]
        public void GestureTest()
        {
            var target = new InputBinding(new TestCommand(), new KeyGesture(Keys.A));
            Assert.Throws<ArgumentNullException>("value", () => target.Gesture = null);
            var gesture = new KeyGesture(Keys.B);
            target.Gesture = gesture;
            Assert.Equal(gesture, target.Gesture);
        }

        [Fact]
        public void CommandParameterTest()
        {
            var target = new InputBinding(new TestCommand(), new KeyGesture(Keys.A));
            Assert.Null(target.CommandParameter);
            target.CommandParameter = null;
            var param = new object();
            target.CommandParameter = param;
            Assert.Equal(param, target.CommandParameter);
        }

        [Fact]
        public void CommandTargetTest()
        {
            var target = new InputBinding(new TestCommand(), new KeyGesture(Keys.A));
            Assert.Null(target.CommandTarget);
            target.CommandTarget = null;
            using var control = new Control();
            target.CommandTarget = control;
            Assert.Equal(control, target.CommandTarget);
        }
    }
}
