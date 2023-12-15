#nullable disable

namespace CommandForWinForms.Tests
{
    public class ExecutedEventArgsTest
    {
        [Fact]
        public void ConstructorTest()
        {
            Assert.Throws<ArgumentNullException>("command", () => new ExecutedEventArgs(null, null));

            var command = new TestCommand();
            var target = new ExecutedEventArgs(command, null);
            Assert.Equal(command, target.Command);
            Assert.Null(target.Parameter);

            var param = new object();
            target = new ExecutedEventArgs(command, param);
            Assert.Equal(command, target.Command);
            Assert.Equal(param, target.Parameter);
        }
    }
}
