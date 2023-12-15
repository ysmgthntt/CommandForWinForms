#nullable disable

using System.ComponentModel;
using System.Windows.Input;

namespace CommandForWinForms.Tests
{
    public class CommandBindingTest
    {
        [Fact]
        public void ConstructorTest()
        {
            Assert.Throws<ArgumentNullException>("command", () => new CommandBinding(null));
            var command = new TestCommand();
            var target = new CommandBinding(command);
            Assert.Equal(command, target.Command);
        }

        [Fact]
        public void CommandTest()
        {
            var target = new CommandBinding(new TestCommand());
            Assert.Throws<ArgumentNullException>("value", () => target.Command = null);
        }

        [Fact]
        public void OnExecutedNoHandlerTest()
        {
            var target = new CommandBinding(new TestCommand());
            var sender = new TestSender();
            CanExecuteEventArgs ceea = null;
            Assert.False(target.OnExecuted(sender, null, ref ceea));
            Assert.Null(ceea);
        }

        [Fact]
        public void OnExecutedNoHandlerCanExecuteTest()
        {
            var target = new CommandBinding(new TestCommand(), null, (_, e) => e.CanExecute = true);
            var sender = new TestSender();
            CanExecuteEventArgs ceea = null;
            Assert.False(target.OnExecuted(sender, null, ref ceea));
            Assert.Null(ceea);
        }

        [Fact]
        public void OnExecutedConstructorTest()
        {
            var command = new TestCommand();
            var sender = new TestSender();
            var parameter = new object();
            var checker = new OnExecutedChecker(sender, command, parameter);
            var target = new CommandBinding(command, checker.OnExecuted);
            CanExecuteEventArgs ceea = null;
            Assert.True(target.OnExecuted(sender, parameter, ref ceea));
            Assert.Null(ceea);
            Assert.Equal(1, checker.ExecutedCount);
        }

        [Fact]
        public void OnExecutedEventTest()
        {
            var command = new TestCommand();
            var sender = new TestSender();
            var parameter = new object();
            var checker = new OnExecutedChecker(sender, command, parameter);
            var target = new CommandBinding(command);
            target.Executed += checker.OnExecuted;
            CanExecuteEventArgs ceea = null;
            Assert.True(target.OnExecuted(sender, parameter, ref ceea));
            Assert.Null(ceea);
            Assert.Equal(1, checker.ExecutedCount);
        }

        [Fact]
        public void OnExecutedConstructorAndEventTest()
        {
            var command = new TestCommand();
            var sender = new TestSender();
            var parameter = new object();
            var checker1 = new OnExecutedChecker(sender, command, parameter);
            var checker2 = new OnExecutedChecker(sender, command, parameter);
            var target = new CommandBinding(command, checker1.OnExecuted);
            target.Executed += checker2.OnExecuted;
            CanExecuteEventArgs ceea = null;
            Assert.True(target.OnExecuted(sender, parameter, ref ceea));
            Assert.Null(ceea);
            Assert.Equal(1, checker1.ExecutedCount);
            Assert.Equal(1, checker2.ExecutedCount);
        }

        [Fact]
        public void OnExecutedCanExecuteTrueTest()
        {
            var command = new TestCommand();
            var sender = new TestSender();
            var parameter = new object();
            var checker = new OnExecutedChecker(sender, command, parameter);
            var target = new CommandBinding(command, checker.OnExecuted, (_, e) => e.CanExecute = true);
            CanExecuteEventArgs ceea = null;
            Assert.True(target.OnExecuted(sender, parameter, ref ceea));
            Assert.NotNull(ceea);
            Assert.Equal(1, checker.ExecutedCount);
        }

        [Fact]
        public void OnExecutedCanExecuteFalseTest()
        {
            var command = new TestCommand();
            var sender = new TestSender();
            var parameter = new object();
            var checker = new OnExecutedChecker(sender, command, parameter);
            var target = new CommandBinding(command, checker.OnExecuted, (_, e) => e.CanExecute = false);
            CanExecuteEventArgs ceea = null;
            Assert.False(target.OnExecuted(sender, parameter, ref ceea));
            Assert.NotNull(ceea);
            Assert.Equal(0, checker.ExecutedCount);
        }

        [Fact]
        public void OnExecutedCanExecuteResetTest()
        {
            var command = new TestCommand();
            var sender = new TestSender();
            var parameter = new object();
            var checker = new OnExecutedChecker(sender, command, parameter);
            var target = new CommandBinding(command, checker.OnExecuted, (_, e) => { });
            CanExecuteEventArgs ceea = new CanExecuteEventArgs(command, parameter);
            ceea.CanExecute = true;
            Assert.False(target.OnExecuted(sender, parameter, ref ceea));
            Assert.NotNull(ceea);
            Assert.False(ceea.CanExecute);
            Assert.Equal(0, checker.ExecutedCount);
        }

        [Fact]
        public void OnCanExecuteNoHandlerTest()
        {
            var target = new CommandBinding(new TestCommand());
            var sender = new TestSender();
            CanExecuteEventArgs ceea = null;
            Assert.False(target.OnCanExecute(sender, null, ref ceea));
            Assert.Null(ceea);
        }

        [Fact]
        public void OnCanExecuteNoHandlerConstructorExecutedTest()
        {
            var target = new CommandBinding(new TestCommand(), (_, _) => { });
            var sender = new TestSender();
            CanExecuteEventArgs ceea = null;
            Assert.True(target.OnCanExecute(sender, null, ref ceea));
            Assert.Null(ceea);
        }

        [Fact]
        public void OnCanExecuteNoHandlerEventExecutedTest()
        {
            var target = new CommandBinding(new TestCommand());
            target.Executed += (_, _) => { };
            var sender = new TestSender();
            CanExecuteEventArgs ceea = null;
            Assert.True(target.OnCanExecute(sender, null, ref ceea));
            Assert.Null(ceea);
        }

        [Fact]
        public void OnCanExecuteConstructorTest()
        {
            var command = new TestCommand();
            var sender = new TestSender();
            var parameter = new object();
            var checker = new OnCanExecuteChecker(sender, command, parameter, false, true);
            var target = new CommandBinding(command, null, checker.OnCanExecute);
            CanExecuteEventArgs ceea = null;
            Assert.True(target.OnCanExecute(sender, parameter, ref ceea));
            Assert.NotNull(ceea);
            Assert.True(ceea.CanExecute);
            Assert.Equal(1, checker.ExecutedCount);
        }

        [Fact]
        public void OnCanExecuteEventTest()
        {
            var command = new TestCommand();
            var sender = new TestSender();
            var parameter = new object();
            var checker = new OnCanExecuteChecker(sender, command, parameter, false, true);
            var target = new CommandBinding(command);
            target.CanExecute += checker.OnCanExecute;
            CanExecuteEventArgs ceea = null;
            Assert.True(target.OnCanExecute(sender, parameter, ref ceea));
            Assert.NotNull(ceea);
            Assert.True(ceea.CanExecute);
            Assert.Equal(1, checker.ExecutedCount);
        }

        [Fact]
        public void OnCanExecuteConstructorAndEventTest()
        {
            var command = new TestCommand();
            var sender = new TestSender();
            var parameter = new object();
            var checker1 = new OnCanExecuteChecker(sender, command, parameter, false, true);
            var checker2 = new OnCanExecuteChecker(sender, command, parameter, true, true);
            var target = new CommandBinding(command, null, checker1.OnCanExecute);
            target.CanExecute += checker2.OnCanExecute;
            CanExecuteEventArgs ceea = null;
            Assert.True(target.OnCanExecute(sender, parameter, ref ceea));
            Assert.NotNull(ceea);
            Assert.True(ceea.CanExecute);
            Assert.Equal(1, checker1.ExecutedCount);
            Assert.Equal(1, checker2.ExecutedCount);
        }

        public static IEnumerable<object[]> OnCanExecutePatternTestData()
            => [
                [(bool?)null, false, (bool?)null, false, (bool?)null, false],
                [(bool?)null, false, (bool?)false, false, (bool?)false, false],
                [(bool?)null, false, (bool?)true, true, (bool?)null, true],
                [(bool?)null, false, (bool?)null, false, (bool?)true, true],
                //
                [(bool?)false, false, (bool?)null, false, (bool?)null, false],
                [(bool?)false, false, (bool?)false, false, (bool?)false, false],
                [(bool?)false, false, (bool?)true, true, (bool?)null, true],
                [(bool?)false, false, (bool?)null, false, (bool?)true, true],
                //
                [(bool?)true, true, (bool?)null, true, (bool?)null, true],
                [(bool?)true, true, (bool?)false, false, (bool?)false, false],
                [(bool?)true, true, (bool?)true, true, (bool?)null, true],
                [(bool?)true, true, (bool?)null, true, (bool?)true, true],
            ];

        [Theory]
        [MemberData(nameof(OnCanExecutePatternTestData))]
        public void OnCanExecutePatternTest(bool? defaultCanExecute, bool expected1, bool? canExecute1, bool expected2, bool? canExecute2, bool onCanExecuteResult)
        {
            var command = new TestCommand();
            var sender = new TestSender();
            var parameter = new object();
            var checker1 = new OnCanExecuteChecker(sender, command, parameter, expected1, canExecute1);
            var checker2 = new OnCanExecuteChecker(sender, command, parameter, expected2, canExecute2);
            var target = new CommandBinding(command, null, checker1.OnCanExecute);
            target.CanExecute += checker2.OnCanExecute;
            CanExecuteEventArgs ceea = null;
            if (defaultCanExecute.HasValue)
            {
                ceea = new CanExecuteEventArgs(command, parameter);
                ceea.CanExecute = defaultCanExecute.Value;
            }
            Assert.Equal(onCanExecuteResult, target.OnCanExecute(sender, parameter, ref ceea));
            Assert.NotNull(ceea);
            Assert.Equal(1, checker1.ExecutedCount);
            Assert.Equal(1, checker2.ExecutedCount);
        }

        //

        private sealed class TestSender : ISynchronizeInvoke
        {
            public bool InvokeRequired => false;

            public IAsyncResult BeginInvoke(Delegate method, object[] args)
            {
                method.DynamicInvoke(args);
                return null;
            }

            public object EndInvoke(IAsyncResult result)
                => null;

            public object Invoke(Delegate method, object[] args)
                => method.DynamicInvoke(args);
        }

#nullable enable

        private sealed class OnExecutedChecker
        {
            private readonly object _sender;
            private readonly ICommand _command;
            private readonly object _parameter;

            public OnExecutedChecker(object sender, ICommand command, object parameter)
            {
                _sender = sender;
                _command = command;
                _parameter = parameter;
            }

            public int ExecutedCount { get; private set; }

            public void OnExecuted(object? sender, ExecutedEventArgs e)
            {
                Assert.Equal(_sender, sender);
                Assert.NotNull(e);
                Assert.Equal(_command, e.Command);
                Assert.Equal(_parameter, e.Parameter);

                ExecutedCount++;
            }
        }

        private sealed class OnCanExecuteChecker
        {
            private readonly object _sender;
            private readonly ICommand _command;
            private readonly object _parameter;

            private readonly bool _expected;
            private readonly bool? _canExecute;

            public OnCanExecuteChecker(object sender, ICommand command, object parameter, bool expected, bool? canExecute)
            {
                _sender = sender;
                _command = command;
                _parameter = parameter;

                _expected = expected;
                _canExecute = canExecute;
            }

            public int ExecutedCount { get; private set; }

            public void OnCanExecute(object? sender, CanExecuteEventArgs e)
            {
                Assert.Equal(_sender, sender);
                Assert.NotNull(e);
                Assert.Equal(_command, e.Command);
                Assert.Equal(_parameter, e.Parameter);
                Assert.Equal(_expected, e.CanExecute);

                ExecutedCount++;
                if (_canExecute.HasValue)
                    e.CanExecute = _canExecute.Value;
            }
        }
    }
}
