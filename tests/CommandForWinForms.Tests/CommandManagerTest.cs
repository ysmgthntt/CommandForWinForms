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
        public void ProcessInputTest()
        {
            using var form = new ProcessInputTestForm();
            using var control1 = new ProcessInputTestControl() { Name = "control1" };
            using var control2 = new ProcessInputTestControl() { Name = "control2" };

            form.Controls.Add(control1);
            new CommandManager(form);
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
            new CommandManager(form);
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

        //

        private sealed class ProcessInputTestForm : Form
        {
            public bool RaiseKeyDown(Keys keyData)
            {
                var e = new KeyEventArgs(keyData);
                OnKeyDown(e);
                return e.Handled;
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
