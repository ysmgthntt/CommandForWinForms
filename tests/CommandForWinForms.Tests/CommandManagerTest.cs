#nullable disable

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CommandForWinForms.Tests
{
    public class CommandManagerTest : IDisposable
    {
        public CommandManagerTest()
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
        public void InitializeSubscribesToKeyDownOnlyForSelectableControls()
        {
            using var form = new Form();
            using var selectableControl = new ProcessInputTestControl(true);
            using var nonSelectableControl = new ProcessInputTestControl(false);
            using var selectablePanel = new SelectablePanel();
            form.Controls.AddRange([selectableControl, nonSelectableControl, selectablePanel]);

            var selectableCommand = new UICommand("Selectable", "Selectable", [new KeyGesture(Keys.A)]);
            var nonSelectableCommand = new UICommand("NonSelectable", "NonSelectable", [new KeyGesture(Keys.B)]);
            var panelCommand = new UICommand("Panel", "Panel", [new KeyGesture(Keys.C)]);
            int selectableExecutionCount = 0;
            int nonSelectableExecutionCount = 0;
            int panelExecutionCount = 0;
            selectableControl.GetCommandBindings().Add(new CommandBinding(selectableCommand, (_, _) => selectableExecutionCount++));
            nonSelectableControl.GetCommandBindings().Add(new CommandBinding(nonSelectableCommand, (_, _) => nonSelectableExecutionCount++));
            selectablePanel.GetCommandBindings().Add(new CommandBinding(panelCommand, (_, _) => panelExecutionCount++));

            CommandManager.Initialize(form);

            Assert.True(selectableControl.RaiseKeyDown(Keys.A));
            Assert.Equal(1, selectableExecutionCount);

            Assert.False(nonSelectableControl.RaiseKeyDown(Keys.B));
            Assert.Equal(0, nonSelectableExecutionCount);

            Assert.False(selectablePanel.RaiseKeyDown(Keys.C));
            Assert.Equal(0, panelExecutionCount);
        }

        [Fact]
        public void ProcessInputTest()
        {
            using var form = new ProcessInputTestForm();
            using var control1 = new ProcessInputTestControl() { Name = "control1" };
            using var control2 = new ProcessInputTestControl() { Name = "control2" };

            form.Controls.Add(control1);
            CommandManager.Initialize(form);
            control1.Controls.Add(control2);

            Assert.False(control2.RaiseKeyDown(Keys.A));

            bool inputBinding = false;
            var parameter = new object();
            var commandA1 = new UICommand("A1", "A1", [new KeyGesture(Keys.A)]);
            var commandA2 = new UICommand("A2", "A2", [new KeyGesture(Keys.A)]);
            var commandB1 = new UICommand("B1", "B1", [new KeyGesture(Keys.B)]);
            var commandB2 = new UICommand("B2", "B2", [new KeyGesture(Keys.B)]);
            var commandC = new UICommand("C", "C", [new KeyGesture(Keys.C)]);

            int control1CommandBindingA1Count = 0;
            int control1CommandBindingA2Count = 0;
            int control1CommandBindingB1Count = 0;
            int control1CommandBindingB2Count = 0;
            int control2CommandBindingA1Count = 0;
            int control2CommandBindingA2Count = 0;
            int control2CommandBindingB2Count = 0;

            void ResetCount()
            {
                control1CommandBindingA1Count = 0;
                control1CommandBindingA2Count = 0;
                control1CommandBindingB1Count = 0;
                control1CommandBindingB2Count = 0;
                control2CommandBindingA1Count = 0;
                control2CommandBindingA2Count = 0;
                control2CommandBindingB2Count = 0;
            }

            // 親
            // CommandBindings

            control1.GetCommandBindings().Add(new CommandBinding(commandA1, (s, e) =>
            {
                Assert.Equal(commandA1, e.Command);
                Assert.Equal(inputBinding ? parameter : null, e.Parameter);
                control1CommandBindingA1Count++;
            }));
            control1.GetCommandBindings().Add(new CommandBinding(commandA2, (s, e) =>
            {
                Assert.Equal(commandA2, e.Command);
                Assert.Equal(inputBinding ? parameter : null, e.Parameter);
                control1CommandBindingA2Count++;
            }));
            control1.GetCommandBindings().Add(new CommandBinding(commandB1, (s, e) =>
            {
                Assert.Equal(commandB1, e.Command);
                Assert.Equal(inputBinding ? parameter : null, e.Parameter);
                control1CommandBindingB1Count++;
            }));
            control1.GetCommandBindings().Add(new CommandBinding(commandB2, (s, e) =>
            {
                Assert.Equal(commandB2, e.Command);
                Assert.Equal(inputBinding ? parameter : null, e.Parameter);
                control1CommandBindingB2Count++;
            }));
            control1.GetCommandBindings().Add(new CommandBinding(commandC));

            ResetCount();
            Assert.True(control2.RaiseKeyDown(Keys.A));
            // control1 の CommandBinding の commandA1
            Assert.Equal(1, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);
            ResetCount();
            Assert.True(control2.RaiseKeyDown(Keys.B));
            // control1 の CommandBinding の commandB1
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(1, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);
            ResetCount();
            Assert.False(control2.RaiseKeyDown(Keys.C));
            // 対象なし。 CommandBinding は実行できない場合対象とならない。
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);

            // InputBindings

            inputBinding = true;
            control1.GetInputBindings().Add(new KeyBinding(commandA2, Keys.C, ModifierKeys.None) { CommandParameter = parameter });
            control1.GetInputBindings().Add(new KeyBinding(commandA1, Keys.C, ModifierKeys.None) { CommandParameter = parameter });
            control1.GetInputBindings().Add(new KeyBinding(commandB2, Keys.A, ModifierKeys.None) { CommandParameter = parameter });
            control1.GetInputBindings().Add(new KeyBinding(commandB1, Keys.A, ModifierKeys.None) { CommandParameter = parameter });
            control1.GetInputBindings().Add(new KeyBinding(commandC, Keys.B, ModifierKeys.None) { CommandParameter = parameter });

            ResetCount();
            Assert.True(control2.RaiseKeyDown(Keys.A));
            // control1 の InputBinding の commandB2
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(1, control1CommandBindingB2Count);
            ResetCount();
            Assert.True(control2.RaiseKeyDown(Keys.B));
            // control1 の InputBinding の commandC InputBinding は実行できなくても対象となる。
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);
            ResetCount();
            Assert.True(control2.RaiseKeyDown(Keys.C));
            // control1 の InputBinding の commandA2
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(1, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);

            // 子
            // CommandBindings

            inputBinding = false;
            control2.GetCommandBindings().Add(new CommandBinding(commandA1, (s, e) =>
            {
                Assert.Equal(commandA1, e.Command);
                Assert.Equal(inputBinding ? parameter : null, e.Parameter);
                control2CommandBindingA1Count++;
            }));
            control2.GetCommandBindings().Add(new CommandBinding(commandA2, (s, e) =>
            {
                Assert.Equal(commandA2, e.Command);
                Assert.Equal(inputBinding ? parameter : null, e.Parameter);
                control2CommandBindingA2Count++;
            }));
            control2.GetCommandBindings().Add(new CommandBinding(commandB1));
            control2.GetCommandBindings().Add(new CommandBinding(commandB2, (s, e) =>
            {
                Assert.Equal(commandB2, e.Command);
                Assert.Equal(inputBinding ? parameter : null, e.Parameter);
                control2CommandBindingB2Count++;
            }));
            control2.GetCommandBindings().Add(new CommandBinding(commandC));

            ResetCount();
            Assert.True(control2.RaiseKeyDown(Keys.A));
            // control2 の CommandBinding の commandA1
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);
            Assert.Equal(1, control2CommandBindingA1Count);
            Assert.Equal(0, control2CommandBindingA2Count);
            Assert.Equal(0, control2CommandBindingB2Count);
            ResetCount();
            Assert.True(control2.RaiseKeyDown(Keys.B));
            // control2 の CommandBinding の commandB1 → control1 の CommandBinding の commandB1
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(1, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);
            Assert.Equal(0, control2CommandBindingA1Count);
            Assert.Equal(0, control2CommandBindingA2Count);
            Assert.Equal(0, control2CommandBindingB2Count);
            ResetCount();
            inputBinding = true;
            Assert.True(control2.RaiseKeyDown(Keys.C));
            // control1 の InputBinding の commandA2 CommandBinding は実行できない場合対象とならない。
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(1, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);
            Assert.Equal(0, control2CommandBindingA1Count);
            Assert.Equal(0, control2CommandBindingA2Count);
            Assert.Equal(0, control2CommandBindingB2Count);

            // InputBindings

            inputBinding = true;
            control2.GetInputBindings().Add(new KeyBinding(commandA2, Keys.C, ModifierKeys.None) { CommandParameter = parameter });
            control2.GetInputBindings().Add(new KeyBinding(commandA1, Keys.C, ModifierKeys.None) { CommandParameter = parameter });
            control2.GetInputBindings().Add(new KeyBinding(commandB1, Keys.A, ModifierKeys.None) { CommandParameter = parameter });
            control2.GetInputBindings().Add(new KeyBinding(commandB2, Keys.A, ModifierKeys.None) { CommandParameter = parameter });
            control2.GetInputBindings().Add(new KeyBinding(commandC, Keys.B, ModifierKeys.None) { CommandParameter = parameter });

            ResetCount();
            Assert.True(control2.RaiseKeyDown(Keys.A));
            // control2 の InputBinding の commandB1
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(1, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);
            Assert.Equal(0, control2CommandBindingA1Count);
            Assert.Equal(0, control2CommandBindingA2Count);
            Assert.Equal(0, control2CommandBindingB2Count);
            ResetCount();
            Assert.True(control2.RaiseKeyDown(Keys.B));
            // control2 の InputBinding の commandC InputBinding は実行できなくても対象となる。
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);
            Assert.Equal(0, control2CommandBindingA1Count);
            Assert.Equal(0, control2CommandBindingA2Count);
            Assert.Equal(0, control2CommandBindingB2Count);
            ResetCount();
            Assert.True(control2.RaiseKeyDown(Keys.C));
            // control2 の InputBinding の commandA2
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);
            Assert.Equal(0, control2CommandBindingA1Count);
            Assert.Equal(1, control2CommandBindingA2Count);
            Assert.Equal(0, control2CommandBindingB2Count);

            // 他

            ResetCount();
            Assert.True(control2.RaiseKeyDown(Keys.A, true));
            // Handled == true の場合、処理されない。
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);
            Assert.Equal(0, control2CommandBindingA1Count);
            Assert.Equal(0, control2CommandBindingA2Count);
            Assert.Equal(0, control2CommandBindingB2Count);

            ResetCount();
            Assert.False(form.RaiseKeyDown(Keys.A));
            // from は対象なし。
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);
            Assert.Equal(0, control2CommandBindingA1Count);
            Assert.Equal(0, control2CommandBindingA2Count);
            Assert.Equal(0, control2CommandBindingB2Count);

            ResetCount();
            Assert.True(control1.RaiseKeyDown(Keys.A));
            // control1 の InputBindng の commandB2
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(1, control1CommandBindingB2Count);
            Assert.Equal(0, control2CommandBindingA1Count);
            Assert.Equal(0, control2CommandBindingA2Count);
            Assert.Equal(0, control2CommandBindingB2Count);
            ResetCount();
            Assert.True(control1.RaiseKeyDown(Keys.B));
            // control1 の InputBinding の commandC InputBinding は実行できなくても対象となる。
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);
            Assert.Equal(0, control2CommandBindingA1Count);
            Assert.Equal(0, control2CommandBindingA2Count);
            Assert.Equal(0, control2CommandBindingB2Count);
            ResetCount();
            Assert.True(control1.RaiseKeyDown(Keys.C));
            // control1 の InputBinding の commandA2
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(1, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);
            Assert.Equal(0, control2CommandBindingA1Count);
            Assert.Equal(0, control2CommandBindingA2Count);
            Assert.Equal(0, control2CommandBindingB2Count);
        }

        [Fact]
        public void ProcessInputSimpleCommandTest()
        {
            using var form = new Form();
            using var control = new ProcessInputTestControl();
            form.Controls.Add(control);
            CommandManager.Initialize(form);
            var parameter = new object();
            int executeCount = 0;
            int canExecuteCount = 0;
            var command = new TestCommand(p =>
            {
                Assert.Equal(parameter, p);
                executeCount++;
            },
            p =>
            {
                Assert.Equal(parameter, p);
                canExecuteCount++;
                return true;
            });
            control.GetInputBindings().Add(new KeyBinding(command, Keys.A, ModifierKeys.None) { CommandParameter = parameter });

            control.RaiseKeyDown(Keys.A);
            Assert.Equal(1, executeCount);
            Assert.Equal(1, canExecuteCount);
        }

        [Fact]
        public void ProcessMouseClickTest()
        {
            using var form = new ProcessInputTestForm();
            using var control1 = new ProcessInputTestControl() { Name = "control1" };
            using var control2 = new ProcessInputTestControl() { Name = "control2" };

            form.Controls.Add(control1);
            CommandManager.Initialize(form);
            control1.Controls.Add(control2);

            Assert.False(control2.RaiseMouseWheel(1));

            bool inputBinding = false;
            var parameter = new object();
            var commandA1 = new UICommand("A1", "A1", [new MouseGesture(MouseAction.WheelClick)]);
            var commandA2 = new UICommand("A2", "A2", [new MouseGesture(MouseAction.WheelClick)]);
            var commandB1 = new UICommand("B1", "B1", [new MouseGesture(MouseAction.LeftClick)]);
            var commandB2 = new UICommand("B2", "B2", [new MouseGesture(MouseAction.LeftClick)]);
            var commandC = new UICommand("C", "C", [new MouseGesture(MouseAction.RightClick)]);

            int control1CommandBindingA1Count = 0;
            int control1CommandBindingA2Count = 0;
            int control1CommandBindingB1Count = 0;
            int control1CommandBindingB2Count = 0;
            int control2CommandBindingA1Count = 0;
            int control2CommandBindingA2Count = 0;
            int control2CommandBindingB2Count = 0;

            void ResetCount()
            {
                control1CommandBindingA1Count = 0;
                control1CommandBindingA2Count = 0;
                control1CommandBindingB1Count = 0;
                control1CommandBindingB2Count = 0;
                control2CommandBindingA1Count = 0;
                control2CommandBindingA2Count = 0;
                control2CommandBindingB2Count = 0;
            }

            // 親
            // CommandBindings

            control1.GetCommandBindings().Add(new CommandBinding(commandA1, (s, e) =>
            {
                Assert.Equal(commandA1, e.Command);
                Assert.Equal(inputBinding ? parameter : null, e.Parameter);
                control1CommandBindingA1Count++;
            }));
            control1.GetCommandBindings().Add(new CommandBinding(commandA2, (s, e) =>
            {
                Assert.Equal(commandA2, e.Command);
                Assert.Equal(inputBinding ? parameter : null, e.Parameter);
                control1CommandBindingA2Count++;
            }));
            control1.GetCommandBindings().Add(new CommandBinding(commandB1, (s, e) =>
            {
                Assert.Equal(commandB1, e.Command);
                Assert.Equal(inputBinding ? parameter : null, e.Parameter);
                control1CommandBindingB1Count++;
            }));
            control1.GetCommandBindings().Add(new CommandBinding(commandB2, (s, e) =>
            {
                Assert.Equal(commandB2, e.Command);
                Assert.Equal(inputBinding ? parameter : null, e.Parameter);
                control1CommandBindingB2Count++;
            }));
            control1.GetCommandBindings().Add(new CommandBinding(commandC));

            ResetCount();
            Assert.True(control2.RaiseMouseWheel(1));
            // control1 の CommandBinding の commandA1
            Assert.Equal(1, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);
            ResetCount();
            control2.RaiseMouseClick(MouseButtons.Left, 1);
            // control1 の CommandBinding の commandB1
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(1, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);
            ResetCount();
            control2.RaiseMouseClick(MouseButtons.Right, 1);
            // 対象なし。 CommandBinding は実行できない場合対象とならない。
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);

            // InputBindings

            inputBinding = true;
            control1.GetInputBindings().Add(new MouseBinding(commandA2, MouseAction.RightClick) { CommandParameter = parameter });
            control1.GetInputBindings().Add(new MouseBinding(commandA1, MouseAction.RightClick) { CommandParameter = parameter });
            control1.GetInputBindings().Add(new MouseBinding(commandB2, MouseAction.WheelClick) { CommandParameter = parameter });
            control1.GetInputBindings().Add(new MouseBinding(commandB1, MouseAction.WheelClick) { CommandParameter = parameter });
            control1.GetInputBindings().Add(new MouseBinding(commandC, MouseAction.LeftClick) { CommandParameter = parameter });

            ResetCount();
            Assert.True(control2.RaiseMouseWheel(1));
            // control1 の InputBinding の commandB2
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(1, control1CommandBindingB2Count);
            ResetCount();
            control2.RaiseMouseClick(MouseButtons.Left, 1);
            // control1 の InputBinding の commandC InputBinding は実行できなくても対象となる。
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);
            ResetCount();
            control2.RaiseMouseClick(MouseButtons.Right, 1);
            // control1 の InputBinding の commandA2
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(1, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);

            // 子
            // CommandBindings

            inputBinding = false;
            control2.GetCommandBindings().Add(new CommandBinding(commandA1, (s, e) =>
            {
                Assert.Equal(commandA1, e.Command);
                Assert.Equal(inputBinding ? parameter : null, e.Parameter);
                control2CommandBindingA1Count++;
            }));
            control2.GetCommandBindings().Add(new CommandBinding(commandA2, (s, e) =>
            {
                Assert.Equal(commandA2, e.Command);
                Assert.Equal(inputBinding ? parameter : null, e.Parameter);
                control2CommandBindingA2Count++;
            }));
            control2.GetCommandBindings().Add(new CommandBinding(commandB1));
            control2.GetCommandBindings().Add(new CommandBinding(commandB2, (s, e) =>
            {
                Assert.Equal(commandB2, e.Command);
                Assert.Equal(inputBinding ? parameter : null, e.Parameter);
                control2CommandBindingB2Count++;
            }));
            control2.GetCommandBindings().Add(new CommandBinding(commandC));

            ResetCount();
            Assert.True(control2.RaiseMouseWheel(1));
            // control2 の CommandBinding の commandA1
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);
            Assert.Equal(1, control2CommandBindingA1Count);
            Assert.Equal(0, control2CommandBindingA2Count);
            Assert.Equal(0, control2CommandBindingB2Count);
            ResetCount();
            control2.RaiseMouseClick(MouseButtons.Left, 1);
            // control2 の CommandBinding の commandB1 → control1 の CommandBinding の commandB1
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(1, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);
            Assert.Equal(0, control2CommandBindingA1Count);
            Assert.Equal(0, control2CommandBindingA2Count);
            Assert.Equal(0, control2CommandBindingB2Count);
            ResetCount();
            inputBinding = true;
            control2.RaiseMouseClick(MouseButtons.Right, 1);
            // control1 の InputBinding の commandA2 CommandBinding は実行できない場合対象とならない。
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(1, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);
            Assert.Equal(0, control2CommandBindingA1Count);
            Assert.Equal(0, control2CommandBindingA2Count);
            Assert.Equal(0, control2CommandBindingB2Count);

            // InputBindings

            inputBinding = true;
            control2.GetInputBindings().Add(new MouseBinding(commandA2, MouseAction.RightClick) { CommandParameter = parameter });
            control2.GetInputBindings().Add(new MouseBinding(commandA1, MouseAction.RightClick) { CommandParameter = parameter });
            control2.GetInputBindings().Add(new MouseBinding(commandB1, MouseAction.WheelClick) { CommandParameter = parameter });
            control2.GetInputBindings().Add(new MouseBinding(commandB2, MouseAction.WheelClick) { CommandParameter = parameter });
            control2.GetInputBindings().Add(new MouseBinding(commandC, MouseAction.LeftClick) { CommandParameter = parameter });

            ResetCount();
            Assert.True(control2.RaiseMouseWheel(1));
            // control2 の InputBinding の commandB1
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(1, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);
            Assert.Equal(0, control2CommandBindingA1Count);
            Assert.Equal(0, control2CommandBindingA2Count);
            Assert.Equal(0, control2CommandBindingB2Count);
            ResetCount();
            control2.RaiseMouseClick(MouseButtons.Left, 1);
            // control2 の InputBinding の commandC InputBinding は実行できなくても対象となる。
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);
            Assert.Equal(0, control2CommandBindingA1Count);
            Assert.Equal(0, control2CommandBindingA2Count);
            Assert.Equal(0, control2CommandBindingB2Count);
            ResetCount();
            control2.RaiseMouseClick(MouseButtons.Right, 1);
            // control2 の InputBinding の commandA2
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);
            Assert.Equal(0, control2CommandBindingA1Count);
            Assert.Equal(1, control2CommandBindingA2Count);
            Assert.Equal(0, control2CommandBindingB2Count);

            // 他

            ResetCount();
            Assert.True(control2.RaiseMouseWheel(1, true));
            // Handled == true の場合、処理されない。
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);
            Assert.Equal(0, control2CommandBindingA1Count);
            Assert.Equal(0, control2CommandBindingA2Count);
            Assert.Equal(0, control2CommandBindingB2Count);

            ResetCount();
            form.RaiseMouseClick(MouseButtons.Left, 1);
            // from は対象なし。
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);
            Assert.Equal(0, control2CommandBindingA1Count);
            Assert.Equal(0, control2CommandBindingA2Count);
            Assert.Equal(0, control2CommandBindingB2Count);

            ResetCount();
            Assert.True(control1.RaiseMouseWheel(1));
            // control1 の InputBindng の commandB2
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(1, control1CommandBindingB2Count);
            Assert.Equal(0, control2CommandBindingA1Count);
            Assert.Equal(0, control2CommandBindingA2Count);
            Assert.Equal(0, control2CommandBindingB2Count);
            ResetCount();
            foreach (var b in control1.GetInputBindings().Where(b => b.Gesture is MouseGesture { MouseAction: MouseAction.WheelClick }))
                b.Command = commandC;
            Assert.True(control1.RaiseMouseWheel(1));
            // control1 の InputBinding の commandC InputBinding は実行できなくても対象となる。
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(0, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);
            Assert.Equal(0, control2CommandBindingA1Count);
            Assert.Equal(0, control2CommandBindingA2Count);
            Assert.Equal(0, control2CommandBindingB2Count);
            ResetCount();
            control1.RaiseMouseClick(MouseButtons.Right, 1);
            // control1 の InputBinding の commandA2
            Assert.Equal(0, control1CommandBindingA1Count);
            Assert.Equal(1, control1CommandBindingA2Count);
            Assert.Equal(0, control1CommandBindingB1Count);
            Assert.Equal(0, control1CommandBindingB2Count);
            Assert.Equal(0, control2CommandBindingA1Count);
            Assert.Equal(0, control2CommandBindingA2Count);
            Assert.Equal(0, control2CommandBindingB2Count);
        }

        [Theory]
        [InlineData(MouseButtons.Left, 1, MouseAction.LeftClick)]
        [InlineData(MouseButtons.Right, 1, MouseAction.RightClick)]
        [InlineData(MouseButtons.Middle, 1, MouseAction.MiddleClick)]
        [InlineData(MouseButtons.Left, 2, MouseAction.LeftDoubleClick)]
        [InlineData(MouseButtons.Right, 2, MouseAction.RightDoubleClick)]
        [InlineData(MouseButtons.Middle, 2, MouseAction.MiddleDoubleClick)]
        [InlineData(MouseButtons.Left, 3, MouseAction.LeftClick)]
        [InlineData(MouseButtons.Right, 3, MouseAction.RightClick)]
        [InlineData(MouseButtons.Middle, 3, MouseAction.MiddleClick)]
        [InlineData(MouseButtons.None, 1, MouseAction.None)]
        [InlineData(MouseButtons.XButton1, 1, MouseAction.None)]
        [InlineData(MouseButtons.XButton2, 1, MouseAction.None)]
        public void ProcessMouseClickInputBindingTest(MouseButtons button, int clicks, MouseAction mouseAction)
        {
            using var form = new Form();
            using var parent = new ProcessInputTestControl();
            using var child = new ProcessInputTestControl();
            form.Controls.Add(parent);
            parent.Controls.Add(child);
            CommandManager.Initialize(form);

            var parameter = new object();
            int executeCount = 0;
            var command = new TestCommand(p =>
            {
                Assert.Same(parameter, p);
                executeCount++;
            }, _ => true);
            parent.GetInputBindings().Add(new MouseBinding(command, mouseAction) { CommandParameter = parameter });

            child.RaiseMouseClick(MouseButtons.None, 0);
            Assert.Equal(0, executeCount);

            child.RaiseMouseClick(button, clicks);
            Assert.Equal(mouseAction == MouseAction.None ? 0 : 1, executeCount);
        }

        [Fact]
        public void ProcessMouseWheelInputBindingTest()
        {
            using var form = new Form();
            using var control = new ProcessInputTestControl();
            form.Controls.Add(control);
            CommandManager.Initialize(form);

            int executeCount = 0;
            var command = new TestCommand(_ => executeCount++, _ => true);
            control.GetInputBindings().Add(new MouseBinding(command, MouseAction.WheelClick));

            Assert.True(control.RaiseMouseWheel(120));
            Assert.Equal(1, executeCount);

            control.GetInputBindings().Clear();
            control.GetInputBindings().Add(new MouseBinding(command, MouseAction.LeftClick));
            Assert.False(control.RaiseMouseWheel(120));
            Assert.Equal(1, executeCount);
        }

        [Fact]
        public void ProcessMouseClickCommandBindingTest()
        {
            using var form = new Form();
            using var control = new ProcessInputTestControl();
            form.Controls.Add(control);
            CommandManager.Initialize(form);

            int executeCount = 0;
            var command = new UICommand("Mouse", "Mouse", [new MouseGesture(MouseAction.RightDoubleClick)]);
            control.GetCommandBindings().Add(new CommandBinding(command, (_, _) => executeCount++));

            control.RaiseMouseClick(MouseButtons.Left, 2);
            Assert.Equal(0, executeCount);

            control.RaiseMouseClick(MouseButtons.Right, 2);
            Assert.Equal(1, executeCount);
        }

        //

        private sealed class ProcessInputTestForm : Form
        {
            public bool RaiseKeyDown(Keys keyData)
            {
                var e = new KeyEventArgs(keyData);
                OnKeyDown(e);
                return e.Handled;
            }

            public void RaiseMouseClick(MouseButtons button, int clicks)
            {
                var e = new MouseEventArgs(button, clicks, 0, 0, 0);
                if (clicks == 2)
                    OnMouseDoubleClick(e);
                else
                    OnMouseClick(e);
            }
        }

        private sealed class ProcessInputTestControl : Control, ISynchronizeInvoke
        {
            public bool RaiseKeyDown(Keys keyData, bool handled = false)
            {
                var e = new KeyEventArgs(keyData);
                e.Handled = handled;
                OnKeyDown(e);
                return e.Handled;
            }

            public void RaiseMouseClick(MouseButtons button, int clicks)
            {
                var e = new MouseEventArgs(button, clicks, 0, 0, 0);
                if (clicks == 2)
                    OnMouseDoubleClick(e);
                else
                    OnMouseClick(e);
            }

            public bool RaiseMouseWheel(int delta, bool handled = false)
            {
                var e = new HandledMouseEventArgs(MouseButtons.None, 0, 0, 0, delta, handled);
                OnMouseWheel(e);
                return e.Handled;
            }

            IAsyncResult ISynchronizeInvoke.BeginInvoke(Delegate method, object[] args)
            {
                method.DynamicInvoke(args);
                return null;
            }

            public ProcessInputTestControl() { }

            public ProcessInputTestControl(bool selectable)
            {
                SetStyle(ControlStyles.Selectable, selectable);
            }
        }

        private sealed class SelectablePanel : Panel, ISynchronizeInvoke
        {
            public SelectablePanel()
            {
                SetStyle(ControlStyles.Selectable, true);
            }

            public bool RaiseKeyDown(Keys keyData)
            {
                var e = new KeyEventArgs(keyData);
                OnKeyDown(e);
                return e.Handled;
            }

            // メッセージループがないため、自分で呼び出す。
            IAsyncResult ISynchronizeInvoke.BeginInvoke(Delegate method, object[] args)
            {
                method.DynamicInvoke(args);
                return null;
            }
        }
    }

    public class CommandManagerTest_SetCommand
    {
        [Fact]
        public void InvalidateRequerySuggestedTest()
        {
            lock (Locks.SetCommand)
            {
                var command1 = new UICommand("command1", "command1");
                int canExecuteChangedCount1 = 0;
                command1.CanExecuteChanged += (sender, args) =>
                {
                    canExecuteChangedCount1++;
                };
                using var button1 = new Button();
                button1.SetCommand(command1);

                CommandManager.InvalidateRequerySuggested();
                CommandManager.InvalidateRequerySuggested();
                Assert.Equal(0, canExecuteChangedCount1);
                DoEvents();
                Assert.Equal(1, canExecuteChangedCount1);
                CommandManager.InvalidateRequerySuggested();
                CommandManager.InvalidateRequerySuggested();
                Assert.Equal(1, canExecuteChangedCount1);
                DoEvents();
                Assert.Equal(2, canExecuteChangedCount1);

                var command2 = new UICommand("command2", "command2");
                int canExecuteChangedCount2 = 0;
                command2.CanExecuteChanged += (sender, args) =>
                {
                    canExecuteChangedCount2++;
                };
                using var button2 = new Button();
                button2.SetCommand(command2);

                CommandManager.InvalidateRequerySuggested();
                DoEvents();
                Assert.Equal(3, canExecuteChangedCount1);
                Assert.Equal(1, canExecuteChangedCount2);
                CommandManager.InvalidateRequerySuggested();
                DoEvents();
                Assert.Equal(4, canExecuteChangedCount1);
                Assert.Equal(2, canExecuteChangedCount2);
            }
        }

        private static void DoEvents()
        {
            //const uint WM_TIMER = 0x0113;
            MSG msg = default;
            while (GetMessage(out msg, IntPtr.Zero, 0, 0) != 0)
            {
                TranslateMessage(ref msg);
                DispatchMessage(ref msg);

                if (PeekMessage(out msg, IntPtr.Zero, 0, 0, 0) == 0)
                    break;
            }
        }


        [DllImport("user32.dll")]
        private static extern int GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
        [DllImport("user32.dll")]
        private static extern int PeekMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);
        [DllImport("user32.dll")]
        private static extern bool TranslateMessage([In] ref MSG lpMsg);
        [DllImport("user32.dll")]
        private static extern IntPtr DispatchMessage([In] ref MSG lpmsg);

        [StructLayout(LayoutKind.Sequential)]
        private struct MSG
        {
            public IntPtr hwnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public Point pt;
        }
    }
}
