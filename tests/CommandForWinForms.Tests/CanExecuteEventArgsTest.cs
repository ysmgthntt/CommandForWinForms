#nullable disable

namespace CommandForWinForms.Tests
{
    public class CanExecuteEventArgsTest
    {
        [Fact]
        public void ConstructorTest()
        {
            Assert.Throws<ArgumentNullException>("command", () => new CanExecuteEventArgs(null, null));

            var command = new TestCommand();
            var target = new CanExecuteEventArgs(command, null);
            Assert.Equal(command, target.Command);
            Assert.Null(target.Parameter);
            Assert.False(target.CanExecute);
            Assert.False(target.Handled);

            var param = new object();
            target = new CanExecuteEventArgs(command, param);
            Assert.Equal(command, target.Command);
            Assert.Equal(param, target.Parameter);
            Assert.False(target.CanExecute);
            Assert.False(target.Handled);
        }

        [Fact]
        public void CanExecuteTest()
        {
            var target = new CanExecuteEventArgs(new TestCommand(), null);
            Assert.False(target.CanExecute);
            target.CanExecute = true;
            Assert.True(target.CanExecute);
        }

        [Fact]
        public void HandledTest()
        {
            var target = new CanExecuteEventArgs(new TestCommand(), null);
            Assert.False(target.Handled);
            target.Handled = true;
            Assert.True(target.Handled);
        }
    }
}
