#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.

namespace CommandForWinForms.Tests
{
    public class ControlExtensionsTest : IDisposable
    {
        public ControlExtensionsTest()
        {
            Monitor.Enter(Locks.CommandBindings);
            Monitor.Enter(Locks.InputBindings);
        }

        public void Dispose()
        {
            Monitor.Exit(Locks.InputBindings);
            Monitor.Exit(Locks.CommandBindings);
        }

        [Fact]
        public void CommandBindingsTest()
        {
            var command = new TestCommand();
            using var control1 = new Control();
            var commandBindings1 = control1.CommandBindings;
            Assert.Equal(commandBindings1, control1.CommandBindings);
            Assert.Equal(0, control1.CommandBindings.Count);

            var commandBinding1 = new CommandBinding(command);
            control1.CommandBindings.Add(commandBinding1);
            Assert.Equal(1, control1.CommandBindings.Count);
            Assert.Equal(commandBinding1, control1.CommandBindings[0]);

            var commandBinding2 = new CommandBinding(command);
            control1.CommandBindings.Add(commandBinding2);
            Assert.Equal(2, control1.CommandBindings.Count);
            Assert.Equal(commandBinding1, control1.CommandBindings[0]);
            Assert.Equal(commandBinding2, control1.CommandBindings[1]);


            using var control2 = new Control();
            var commandBindings2 = control2.CommandBindings;
            Assert.Equal(commandBindings2, control2.CommandBindings);
            Assert.Equal(2, control1.CommandBindings.Count);
            Assert.Equal(0, control2.CommandBindings.Count);

            var commandBinding3 = new CommandBinding(command);
            control2.CommandBindings.Add(commandBinding3);
            Assert.Equal(2, control1.CommandBindings.Count);
            Assert.Equal(1, control2.CommandBindings.Count);
            Assert.Equal(commandBinding3, control2.CommandBindings[0]);

            control1.CommandBindings.Clear();
            Assert.Equal(0, control1.CommandBindings.Count);
            Assert.Equal(1, control2.CommandBindings.Count);
        }

        [Fact]
        public void InputBindingsTest()
        {
            var command = new TestCommand();
            using var control1 = new Control();
            var inputBindings1 = control1.InputBindings;
            Assert.Equal(inputBindings1, control1.InputBindings);
            Assert.Equal(0, control1.InputBindings.Count);

            var inputBinding1 = new KeyBinding(command, Keys.A, ModifierKeys.None);
            control1.InputBindings.Add(inputBinding1);
            Assert.Equal(1, control1.InputBindings.Count);
            Assert.Equal(inputBinding1, control1.InputBindings[0]);

            var inputBinding2 = new KeyBinding(command, Keys.A, ModifierKeys.None);
            control1.InputBindings.Add(inputBinding2);
            Assert.Equal(2, control1.InputBindings.Count);
            Assert.Equal(inputBinding1, control1.InputBindings[0]);
            Assert.Equal(inputBinding2, control1.InputBindings[1]);


            using var control2 = new Control();
            var inputBindings2 = control2.InputBindings;
            Assert.Equal(inputBindings2, control2.InputBindings);
            Assert.Equal(2, control1.InputBindings.Count);
            Assert.Equal(0, control2.InputBindings.Count);

            var inputBinding3 = new KeyBinding(command, Keys.A, ModifierKeys.None);
            control2.InputBindings.Add(inputBinding3);
            Assert.Equal(2, control1.InputBindings.Count);
            Assert.Equal(1, control2.InputBindings.Count);
            Assert.Equal(inputBinding3, control2.InputBindings[0]);

            control1.InputBindings.Clear();
            Assert.Equal(0, control1.InputBindings.Count);
            Assert.Equal(1, control2.InputBindings.Count);
        }

        [Fact]
        public void CollectionHolderAlreadyDisposedTest()
        {
            var command = new TestCommand();
            using var control = new Control();
            control.Dispose();

            Assert.Throws<ObjectDisposedException>(() => control.CommandBindings);
            Assert.Throws<ObjectDisposedException>(() => control.InputBindings);
        }

        [Fact]
        public void CollectionHolderDisposeTest()
        {
            var command = new TestCommand();
            using var control = new Control();

            control.CommandBindings.Add(new CommandBinding(command));
            control.InputBindings.Add(new KeyBinding(command, Keys.A, ModifierKeys.None));

            Assert.True(ControlCommandProperties.TryGetCollection<CommandBinding>(control, out _));
            Assert.True(ControlCommandProperties.TryGetCollection<InputBinding>(control, out _));

            control.Dispose();

            Assert.False(ControlCommandProperties.TryGetCollection<CommandBinding>(control, out _));
            Assert.False(ControlCommandProperties.TryGetCollection<InputBinding>(control, out _));
        }

        [Fact]
        public void CollectionHolderInstanceTest()
        {
            using var control = new Control();
            Assert.Equal(control.GetCommandBindings(), control.CommandBindings);
            Assert.Equal(control.GetInputBindings(), control.InputBindings);
        }
    }
}
