#nullable disable

#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.

namespace CommandForWinForms.Tests
{
    public class ControlCommandPropertiesTest : IDisposable
    {
        public ControlCommandPropertiesTest()
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
        public void GetCommandBindingsTest()
        {
            var command = new TestCommand();
            using var control1 = new Control();
            var commandBindings1 = control1.GetCommandBindings();
            Assert.Equal(commandBindings1, control1.GetCommandBindings());
            Assert.Equal(0, control1.GetCommandBindings().Count);

            var commandBinding1 = new CommandBinding(command);
            control1.GetCommandBindings().Add(commandBinding1);
            Assert.Equal(1, control1.GetCommandBindings().Count);
            Assert.Equal(commandBinding1, control1.GetCommandBindings()[0]);

            var commandBinding2 = new CommandBinding(command);
            control1.GetCommandBindings().Add(commandBinding2);
            Assert.Equal(2, control1.GetCommandBindings().Count);
            Assert.Equal(commandBinding1, control1.GetCommandBindings()[0]);
            Assert.Equal(commandBinding2, control1.GetCommandBindings()[1]);


            using var control2 = new Control();
            var commandBindings2 = control2.GetCommandBindings();
            Assert.Equal(commandBindings2, control2.GetCommandBindings());
            Assert.Equal(2, control1.GetCommandBindings().Count);
            Assert.Equal(0, control2.GetCommandBindings().Count);

            var commandBinding3 = new CommandBinding(command);
            control2.GetCommandBindings().Add(commandBinding3);
            Assert.Equal(2, control1.GetCommandBindings().Count);
            Assert.Equal(1, control2.GetCommandBindings().Count);
            Assert.Equal(commandBinding3, control2.GetCommandBindings()[0]);

            control1.GetCommandBindings().Clear();
            Assert.Equal(0, control1.GetCommandBindings().Count);
            Assert.Equal(1, control2.GetCommandBindings().Count);
        }

        [Fact]
        public void GetInputBindingsTest()
        {
            var command = new TestCommand();
            using var control1 = new Control();
            var inputBindings1 = control1.GetInputBindings();
            Assert.Equal(inputBindings1, control1.GetInputBindings());
            Assert.Equal(0, control1.GetInputBindings().Count);

            var inputBinding1 = new KeyBinding(command, Keys.A, ModifierKeys.None);
            control1.GetInputBindings().Add(inputBinding1);
            Assert.Equal(1, control1.GetInputBindings().Count);
            Assert.Equal(inputBinding1, control1.GetInputBindings()[0]);

            var inputBinding2 = new KeyBinding(command, Keys.A, ModifierKeys.None);
            control1.GetInputBindings().Add(inputBinding2);
            Assert.Equal(2, control1.GetInputBindings().Count);
            Assert.Equal(inputBinding1, control1.GetInputBindings()[0]);
            Assert.Equal(inputBinding2, control1.GetInputBindings()[1]);


            using var control2 = new Control();
            var inputBindings2 = control2.GetInputBindings();
            Assert.Equal(inputBindings2, control2.GetInputBindings());
            Assert.Equal(2, control1.GetInputBindings().Count);
            Assert.Equal(0, control2.GetInputBindings().Count);

            var inputBinding3 = new KeyBinding(command, Keys.A, ModifierKeys.None);
            control2.GetInputBindings().Add(inputBinding3);
            Assert.Equal(2, control1.GetInputBindings().Count);
            Assert.Equal(1, control2.GetInputBindings().Count);
            Assert.Equal(inputBinding3, control2.GetInputBindings()[0]);

            control1.GetInputBindings().Clear();
            Assert.Equal(0, control1.GetInputBindings().Count);
            Assert.Equal(1, control2.GetInputBindings().Count);
        }

        [Fact]
        public void CollectionHolderAlreadyDisposedTest()
        {
            var command = new TestCommand();
            using var control = new Control();
            control.Dispose();

            Assert.Throws<ObjectDisposedException>(() => control.GetCommandBindings());
            Assert.Throws<ObjectDisposedException>(() => control.GetInputBindings());
        }

        [Fact]
        public void CollectionHolderDisposeTest()
        {
            var command = new TestCommand();
            using var control = new Control();

            control.GetCommandBindings().Add(new CommandBinding(command));
            control.GetInputBindings().Add(new KeyBinding(command, Keys.A, ModifierKeys.None));

            Assert.True(ControlCommandProperties.TryGetCollection<CommandBinding>(control, out _));
            Assert.True(ControlCommandProperties.TryGetCollection<InputBinding>(control, out _));

            control.Dispose();

            Assert.False(ControlCommandProperties.TryGetCollection<CommandBinding>(control, out _));
            Assert.False(ControlCommandProperties.TryGetCollection<InputBinding>(control, out _));
        }
    }

    public class ControlCommandPropertiesTest_SetCommand : IDisposable
    {
        public ControlCommandPropertiesTest_SetCommand()
        {
            Monitor.Enter(Locks.SetCommand);
        }

        public void Dispose()
        {
            Monitor.Exit(Locks.SetCommand);
        }

        [Fact]
        public void SetCommandButtonAlreadyDisposedTest()
        {
            using var button = new Button();
            button.Dispose();
            Assert.Throws<ObjectDisposedException>(() => button.SetCommand(new TestCommand()));
        }

        [Fact]
        public void SetCommandToolStripItemAlreadyDisposedTest()
        {
            using var parent = new ToolStrip();
            using var item = new ToolStripButton();
            parent.Items.Add(item);
            item.Dispose();
            Assert.Throws<ObjectDisposedException>(() => item.SetCommand(new TestCommand()));
        }

        [Fact]
        public void GetSetCommandButtonNullTest()
        {
            Assert.Throws<ArgumentNullException>("button", () => ControlCommandProperties.GetCommand((ButtonBase)null));
            Assert.Throws<ArgumentNullException>("button", () => ControlCommandProperties.SetCommand((ButtonBase)null, null));
        }

        [Fact]
        public void GetSetCommandToolStripItemNullTest()
        {
            Assert.Throws<ArgumentNullException>("item", () => ControlCommandProperties.GetCommand((ToolStripItem)null));
            Assert.Throws<ArgumentNullException>("item", () => ControlCommandProperties.SetCommand((ToolStripItem)null, null));
        }

        [Fact]
        public void GetSetCommandButtonTest()
        {
            using var button1 = new Button();
            using var button2 = new Button();

            Assert.Null(button1.GetCommand());
            Assert.Null(button2.GetCommand());

            var command1 = new TestCommand();
            button1.SetCommand(command1);

            Assert.Equal(command1, button1.GetCommand());
            Assert.Null(button2.GetCommand());

            var command2 = new TestCommand();
            button2.SetCommand(null);
            button2.SetCommand(command2);

            Assert.Equal(command1, button1.GetCommand());
            Assert.Equal(command2, button2.GetCommand());

            button1.SetCommand(command2);

            Assert.Equal(command2, button1.GetCommand());
            Assert.Equal(command2, button2.GetCommand());

            button2.SetCommand(null);

            Assert.Equal(command2, button1.GetCommand());
            Assert.Null(button2.GetCommand());

            button1.SetCommand(null);
            button2.SetCommand(null);

            Assert.Null(button1.GetCommand());
            Assert.Null(button2.GetCommand());
        }

        [Fact]
        public void GetSetCommandToolStripItemTest()
        {
            using var button1 = new ToolStripButton();
            using var button2 = new ToolStripButton();

            Assert.Null(button1.GetCommand());
            Assert.Null(button2.GetCommand());

            var command1 = new TestCommand();
            button1.SetCommand(command1);

            Assert.Equal(command1, button1.GetCommand());
            Assert.Null(button2.GetCommand());

            var command2 = new TestCommand();
            button2.SetCommand(null);
            button2.SetCommand(command2);

            Assert.Equal(command1, button1.GetCommand());
            Assert.Equal(command2, button2.GetCommand());

            button1.SetCommand(command2);

            Assert.Equal(command2, button1.GetCommand());
            Assert.Equal(command2, button2.GetCommand());

            button2.SetCommand(null);

            Assert.Equal(command2, button1.GetCommand());
            Assert.Null(button2.GetCommand());

            button1.SetCommand(null);
            button2.SetCommand(null);

            Assert.Null(button1.GetCommand());
            Assert.Null(button2.GetCommand());
        }
    }

    public class ControlCommandPropertiesTest_UICommand : IDisposable
    {
        public ControlCommandPropertiesTest_UICommand()
        {
            Monitor.Enter(Locks.CommandBindings);
            Monitor.Enter(Locks.SetCommand);
        }

        public void Dispose()
        {
            Monitor.Exit(Locks.SetCommand);
            Monitor.Exit(Locks.CommandBindings);
        }

        [Fact]
        public void RaiseCanExecuteAllUICommandsTest()
        {
            using var button1 = new Button();
            using var button2 = new Button();
            using var button3 = new Button();
            using var button4 = new Button();
            using var button5 = new Button();
            using var button6 = new Button();
            using var parent = new Control();
            parent.Controls.AddRange([button1, button2, button3, button4, button5, button6]);

            var command1 = new UICommand("command1", "command1");
            var command2 = new UICommand("command2", "command2");
            var command3 = new TestCommand(_ => Assert.Fail("not UICommand"), _ => { Assert.Fail("not UICommand"); return false; });

            int canExecute1 = 0, canExecute2 = 0;
            parent.GetCommandBindings().Add(new CommandBinding(command1, null, (_, _) => canExecute1++));
            parent.GetCommandBindings().Add(new CommandBinding(command2, null, (_, _) => canExecute2++));

            button1.SetCommand(command1, null, parent);
            button2.SetCommand(command2, null, parent);
            button3.SetCommand(command3, null, parent);
            button4.SetCommand(command1, null, parent);
            button5.SetCommand(command2, null, parent);
            button6.SetCommand(command3, null, parent);

            ControlCommandProperties.RaiseCanExecuteChangedAllUICommands();
            Assert.Equal(1, canExecute1);
            Assert.Equal(1, canExecute2);
            ControlCommandProperties.RaiseCanExecuteChangedAllUICommands();
            Assert.Equal(2, canExecute1);
            Assert.Equal(2, canExecute2);

            button2.SetCommand(command3);
            ControlCommandProperties.RaiseCanExecuteChangedAllUICommands();
            Assert.Equal(3, canExecute1);
            Assert.Equal(3, canExecute2);
            ControlCommandProperties.RaiseCanExecuteChangedAllUICommands();
            Assert.Equal(4, canExecute1);
            Assert.Equal(4, canExecute2);
        }
    }
}
