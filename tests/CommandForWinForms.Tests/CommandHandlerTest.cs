﻿#nullable disable

using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;

namespace CommandForWinForms.Tests
{
    public class CommandHandlerTest : IDisposable
    {
        public CommandHandlerTest()
        {
            Monitor.Enter(Locks.SetCommand);
        }

        public void Dispose()
        {
            Monitor.Exit(Locks.SetCommand);
        }

        // static で CanExecuteChanged を持ち続ける。
        private static readonly TestCommand _staticTestCommand = new TestCommand();

        private sealed class IntWrapper
        {
            public int Value;
        }

        private void DisposeTest(Func<IntWrapper, IDisposable> create)
        {
            var intWrapper = new IntWrapper();
            void test(IntWrapper intWrapper)
            {
                var component = create(intWrapper);
                // CanExecuteChanged を解除する。
                component.Dispose();
                GC.ReRegisterForFinalize(component);
            }
            test(intWrapper);
            // イベントが解除されていれば参照がなくなり回収される。
            GC.Collect(2);
            GC.WaitForPendingFinalizers();
            Assert.Equal(1, intWrapper.Value);
        }

        private sealed class DisposeTestButton : Button
        {
            public readonly IntWrapper _intWrapper;

            public DisposeTestButton(IntWrapper intWrapper)
                => _intWrapper = intWrapper;

            ~DisposeTestButton()
                => _intWrapper.Value++;
        }

        private sealed class DisposeTestToolStripItem : ToolStripItem
        {
            public readonly IntWrapper _intWrapper;

            public DisposeTestToolStripItem(IntWrapper intWrapper)
                => _intWrapper = intWrapper;

            ~DisposeTestToolStripItem()
                => _intWrapper.Value++;
        }

        [Fact]
        public void DisposeButtonTest()
        {
            DisposeTest((intWrapper) =>
            {
                var button = new DisposeTestButton(intWrapper);
                button.SetCommand(_staticTestCommand);
                return button;
            });
        }

        [Fact]
        public void DisposeToolStripItemTest()
        {
            DisposeTest((intWrapper) =>
            {
                var item = new DisposeTestToolStripItem(intWrapper);
                item.SetCommand(_staticTestCommand);
                return item;
            });
        }

        private static readonly UICommand _staticTestUICommand = new UICommand("text", "name");

        [Fact]
        public void DisposeButtonNet7Test()
        {
            DisposeTest((intWrapper) =>
            {
                var button = new DisposeTestButton(intWrapper);
                button.Command = _staticTestUICommand;
                return button;
            });
        }

        [Fact]
        public void DisposeToolStripItemNet7Test()
        {
            DisposeTest((intWrapper) =>
            {
                var item = new DisposeTestToolStripItem(intWrapper);
                item.Command = _staticTestUICommand;
                return item;
            });
        }

        [Fact]
        public void AvailableButtonTest()
        {
            using var button = new Button();

            Assert.True(button.Visible);
            Assert.True(button.Enabled);
            bool enabled = false;
            int enabledCount = 0;
            int canExecuteCount = 0;
            button.EnabledChanged += (_, _) =>
            {
                Assert.Equal(enabled, button.Enabled);
                enabledCount++;
            };
            var command = new TestCommand(_ => { }, _ =>
            {
                canExecuteCount++;
                return enabled;
            });
            button.SetCommand(command);

            command.RaiseCanExecuteChanged();
            Assert.Equal(1, enabledCount);
            Assert.Equal(1, canExecuteCount);
            enabled = true;
            command.RaiseCanExecuteChanged();
            Assert.Equal(2, enabledCount);
            Assert.Equal(2, canExecuteCount);
            button.Visible = false;
            command.RaiseCanExecuteChanged();
            Assert.Equal(2, enabledCount);
            Assert.Equal(2, canExecuteCount);
        }

        [Fact]
        public void AvailableToolStripItemTest()
        {
            using var parent = new ToolStrip();
            using var button = new ToolStripButton();
            // Visible を true にする。
            parent.Items.Add(button);
            parent.PerformLayout();

            Assert.True(button.Visible);
            Assert.True(button.Enabled);
            bool enabled = false;
            int enabledCount = 0;
            int canExecuteCount = 0;
            button.EnabledChanged += (_, _) =>
            {
                Assert.Equal(enabled, button.Enabled);
                enabledCount++;
            };
            var command = new TestCommand(_ => { }, _ =>
            {
                canExecuteCount++;
                return enabled;
            });
            button.SetCommand(command);

            command.RaiseCanExecuteChanged();
            Assert.Equal(1, enabledCount);
            Assert.Equal(1, canExecuteCount);
            enabled = true;
            command.RaiseCanExecuteChanged();
            Assert.Equal(2, enabledCount);
            Assert.Equal(2, canExecuteCount);
            button.Visible = false;
            command.RaiseCanExecuteChanged();
            Assert.Equal(2, enabledCount);
            Assert.Equal(2, canExecuteCount);
        }

        private void ExecuteTest(Action<ICommand, object> setCommand, Action click)
        {
            object parameter = null;
            int executeCount1 = 0, canExecuteCount1 = 0;
            var command1 = new TestCommand(
                p =>
                {
                    Assert.Equal(parameter, p);
                    executeCount1++;
                },
                p =>
                {
                    Assert.Equal(parameter, p);
                    canExecuteCount1++;
                    return true;
                });

            setCommand(command1, null);
            click();
            Assert.Equal(1, executeCount1);
            Assert.Equal(1, canExecuteCount1);
            executeCount1 = canExecuteCount1 = 0;
            setCommand(command1, null);
            click();
            Assert.Equal(1, executeCount1);
            Assert.Equal(1, canExecuteCount1);

            parameter = new object();
            executeCount1 = canExecuteCount1 = 0;
            setCommand(command1, parameter);
            click();
            Assert.Equal(1, executeCount1);
            Assert.Equal(1, canExecuteCount1);
            executeCount1 = canExecuteCount1 = 0;
            setCommand(command1, parameter);
            click();
            Assert.Equal(1, executeCount1);
            Assert.Equal(1, canExecuteCount1);

            int executeCount2 = 0, canExecuteCount2 = 0;
            var command2 = new TestCommand(
                p =>
                {
                    Assert.Equal(parameter, p);
                    executeCount2++;
                },
                p =>
                {
                    Assert.Equal(parameter, p);
                    canExecuteCount2++;
                    return true;
                });

            executeCount1 = canExecuteCount1 = executeCount2 = canExecuteCount2 = 0;
            setCommand(command2, parameter);
            click();
            Assert.Equal(0, executeCount1);
            Assert.Equal(0, canExecuteCount1);
            Assert.Equal(1, executeCount2);
            Assert.Equal(1, canExecuteCount2);

            parameter = null;
            executeCount1 = canExecuteCount1 = executeCount2 = canExecuteCount2 = 0;
            setCommand(command2, null);
            click();
            Assert.Equal(0, executeCount1);
            Assert.Equal(0, canExecuteCount1);
            Assert.Equal(1, executeCount2);
            Assert.Equal(1, canExecuteCount2);

            executeCount1 = canExecuteCount1 = executeCount2 = canExecuteCount2 = 0;
            setCommand(null, parameter);
            click();
            Assert.Equal(0, executeCount1);
            Assert.Equal(0, canExecuteCount1);
            Assert.Equal(0, executeCount2);
            Assert.Equal(0, canExecuteCount2);
        }

        [Fact]
        public void ExecuteButtonTest()
        {
            using var button = new Button();
            ExecuteTest((command, parameter) => button.SetCommand(command, parameter), button.PerformClick);
        }

        [Fact]
        public void ExecuteToolStripItemTest()
        {
            using var button = new ToolStripButton();
            ExecuteTest((command, parameter) => button.SetCommand(command, parameter), button.PerformClick);
        }
    }

    public class CommandHandlerTest_UICommand : IDisposable
    {
        public CommandHandlerTest_UICommand()
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
        public void ActiveControlTest()
        {
            using var parentContainer = new UserControl();
            using var parent = new TestControl();
            using var childContainer = new UserControl();
            using var child = new TestControl();
            using var child2 = new TestControl();
            using var button = new Button();
            // 先に Dispose しないと表示される。
            using (var form = new TestFrom())
            {
                form.Controls.Add(parentContainer);
                parentContainer.Controls.Add(parent);
                parent.Controls.Add(childContainer);
                childContainer.Controls.AddRange([child, child2]);
                form.Controls.Add(button);

                // form
                //   button
                //   parentContainer
                //     parent (CommandBinding)
                //       childContainer
                //         child (CommandBinding)
                //         child2

                var command = new UICommand("text1", "name1");
                var parameter = new object();
                void onExecuted(object sender, ExecutedEventArgs e)
                {
                    Assert.Equal(command, e.Command);
                    Assert.Equal(parameter, e.Parameter);
                }
                void onCanExecute(object sender, CanExecuteEventArgs e)
                {
                    Assert.Equal(command, e.Command);
                    Assert.Equal(parameter, e.Parameter);
                    Assert.False(e.CanExecute);
                    e.CanExecute = true;
                }

                int parentExecuted = 0, childExecuted = 0;
                int parentCanExecute = 0, childCanExecute = 0;
                parent.GetCommandBindings().Add(new CommandBinding(command, (s, e) => { onExecuted(s, e); parentExecuted++; }, (s, e) => { onCanExecute(s, e); parentCanExecute++; }));
                child.GetCommandBindings().Add(new CommandBinding(command, (s, e) => { onExecuted(s, e); childExecuted++; }, (s, e) => { onCanExecute(s, e); childCanExecute++; }));

                button.SetCommand(command, parameter);

                parent.Select();
                command.RaiseCanExecuteChanged();
                Assert.Equal(1, parentCanExecute);
                Assert.Equal(0, parentExecuted);
                Assert.Equal(0, childCanExecute);
                Assert.Equal(0, childExecuted);
                parentExecuted = childExecuted = parentCanExecute = childCanExecute = 0;
                button.PerformClick();
                Assert.Equal(1, parentCanExecute);
                Assert.Equal(1, parentExecuted);
                Assert.Equal(0, childCanExecute);
                Assert.Equal(0, childExecuted);

                child.Select();
                parentExecuted = childExecuted = parentCanExecute = childCanExecute = 0;
                command.RaiseCanExecuteChanged();
                Assert.Equal(0, parentCanExecute);
                Assert.Equal(0, parentExecuted);
                Assert.Equal(1, childCanExecute);
                Assert.Equal(0, childExecuted);
                parentExecuted = childExecuted = parentCanExecute = childCanExecute = 0;
                button.PerformClick();
                Assert.Equal(0, parentCanExecute);
                Assert.Equal(0, parentExecuted);
                Assert.Equal(1, childCanExecute);
                Assert.Equal(1, childExecuted);

                child2.Select();
                parentExecuted = childExecuted = parentCanExecute = childCanExecute = 0;
                command.RaiseCanExecuteChanged();
                Assert.Equal(1, parentCanExecute);
                Assert.Equal(0, parentExecuted);
                Assert.Equal(0, childCanExecute);
                Assert.Equal(0, childExecuted);
                parentExecuted = childExecuted = parentCanExecute = childCanExecute = 0;
                button.PerformClick();
                Assert.Equal(1, parentCanExecute);
                Assert.Equal(1, parentExecuted);
                Assert.Equal(0, childCanExecute);
                Assert.Equal(0, childExecuted);
            }
        }

        [Fact]
        public void TargetTest()
        {
            using var parentContainer = new UserControl() { Name = "parentContainer" };
            using var parent = new TestControl() { Name = "parent" };
            using var selected = new TestControl() { Name = "selected" };
            using var childContainer = new UserControl() { Name = "childContainer" };
            using var child = new TestControl() { Name = "child" };
            using var target = new TestControl() { Name = "target" };
            using var button = new Button();
            using var buttonWithTarget = new Button();
            // 先に Dispose しないと表示される。
            using (var form = new TestFrom())
            {
                form.Controls.AddRange([parentContainer, button, buttonWithTarget]);
                parentContainer.Controls.Add(parent);
                parent.Controls.Add(childContainer);
                childContainer.Controls.AddRange([child, selected]);
                child.Controls.Add(target);

                // form
                //   button
                //   buttonWithTarget
                //   parentContainer
                //     parent (CommandBinding)
                //       childContainer
                //         selected
                //         child (CommandBinding)
                //           target

                var command = new UICommand("text1", "name1");
                var parameter = new object();
                void onExecuted(object sender, ExecutedEventArgs e)
                {
                    Assert.Equal(command, e.Command);
                    Assert.Equal(parameter, e.Parameter);
                }
                void onCanExecute(object sender, CanExecuteEventArgs e)
                {
                    Assert.Equal(command, e.Command);
                    Assert.Equal(parameter, e.Parameter);
                    Assert.False(e.CanExecute);
                    e.CanExecute = true;
                }

                int parentExecuted = 0, childExecuted = 0;
                int parentCanExecute = 0, childCanExecute = 0;
                parent.GetCommandBindings().Add(new CommandBinding(command, (s, e) => { onExecuted(s, e); parentExecuted++; }, (s, e) => { onCanExecute(s, e); parentCanExecute++; }));
                child.GetCommandBindings().Add(new CommandBinding(command, (s, e) => { onExecuted(s, e); childExecuted++; }, (s, e) => { onCanExecute(s, e); childCanExecute++; }));

                button.SetCommand(command, parameter);
                buttonWithTarget.SetCommand(command, parameter, target);

                selected.Select();
                Assert.Equal(selected, form.GetActiveControl());

                button.PerformClick();
                Assert.Equal(1, parentCanExecute);
                Assert.Equal(1, parentExecuted);
                Assert.Equal(0, childCanExecute);
                Assert.Equal(0, childExecuted);

                parentExecuted = childExecuted = parentCanExecute = childCanExecute = 0;
                buttonWithTarget.PerformClick();
                Assert.Equal(0, parentCanExecute);
                Assert.Equal(0, parentExecuted);
                Assert.Equal(1, childCanExecute);
                Assert.Equal(1, childExecuted);

                parentExecuted = childExecuted = parentCanExecute = childCanExecute = 0;
                // button と buttonWithTarget 両方の OnCanExecuteChanged が呼ばれる。
                command.RaiseCanExecuteChanged();
                Assert.Equal(1, parentCanExecute);
                Assert.Equal(0, parentExecuted);
                Assert.Equal(1, childCanExecute);
                Assert.Equal(0, childExecuted);
            }
        }

        //

        private sealed class TestFrom : Form
        {
            public TestFrom()
            {
                const int STATE_VISIBLE = 0x00000002;
                var mi = GetType().GetMethod("SetState", BindingFlags.NonPublic | BindingFlags.Instance);
                mi?.Invoke(this, [STATE_VISIBLE, true]);
            }

            protected override void CreateHandle()
            {
                Assert.Fail("TestFrom_CreateHandle");
            }
        }

        private sealed class TestControl : Control, ISynchronizeInvoke
        {
            IAsyncResult ISynchronizeInvoke.BeginInvoke(Delegate method, object[] args)
            {
                method.DynamicInvoke(args);
                return null;
            }
        }
    }
}
