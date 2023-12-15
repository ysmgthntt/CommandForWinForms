#nullable disable

using System.ComponentModel;

namespace CommandForWinForms.Tests
{
    public class UICommandBaseTest
    {
        [Fact]
        public void ConstructorTest()
        {
            Assert.Throws<ArgumentNullException>("name", () => new UICommand("", null));
            Assert.Throws<ArgumentException>("name", () => new UICommand("", ""));
            Assert.Throws<ArgumentNullException>("text", () => new UICommand(null, "name"));
        }

        [Fact]
        public void InputGestureDefaultNullTest()
        {
            var command = new UICommand("text", "name", null);
            Assert.NotNull(command.InputGestures);
            Assert.Empty(command.InputGestures);
        }

        [Fact]
        public void InputGestureSetConstructorTest()
        {
            List<InputGesture> inputGestures = [new KeyGesture(Keys.A)];
            var command = new UICommand("text", "name", inputGestures);
            Assert.Equal(inputGestures, command.InputGestures);
        }

        private sealed class TestUICommandOverrideCreateInputGestures : UICommandBase
        {
            public override string Name => "name";
            public override string Text => "text";

            private readonly IList<InputGesture> _inputGestures;
            public TestUICommandOverrideCreateInputGestures(IList<InputGesture> inputGestures)
                => _inputGestures = inputGestures;

            public int CreateCount;

            protected override IList<InputGesture> CreateInputGestures()
            {
                CreateCount++;
                return _inputGestures;
            }
        }

        [Fact]
        public void CreateInputGesturesOverrideNullTest()
        {
            var command = new TestUICommandOverrideCreateInputGestures(null);
            Assert.Equal(0, command.CreateCount);
            var inputGestures = command.InputGestures;
            Assert.NotNull(inputGestures);
            Assert.Equal(1, command.CreateCount);
            Assert.Equal(inputGestures, command.InputGestures);
            Assert.Equal(1, command.CreateCount);
        }

        [Fact]
        public void CreateInputGesturesOverrideTest()
        {
            List<InputGesture> inputGestures = [new KeyGesture(Keys.A)];
            var command = new TestUICommandOverrideCreateInputGestures(inputGestures);
            Assert.Equal(0, command.CreateCount);
            Assert.Equal(inputGestures, command.InputGestures);
            Assert.Equal(1, command.CreateCount);
            Assert.Equal(inputGestures, command.InputGestures);
            Assert.Equal(1, command.CreateCount);
        }

        [Fact]
        public void TryGetInputGestureNullTest()
        {
            var command = new TestUICommandOverrideCreateInputGestures(null);
            Assert.Equal(0, command.CreateCount);
            Assert.False(command.TryGetInputGestures(out var inputGestures));
            Assert.Null(inputGestures);
            Assert.Equal(1, command.CreateCount);
            Assert.False(command.TryGetInputGestures(out inputGestures));
            Assert.Null(inputGestures);
            Assert.Equal(2, command.CreateCount);
        }

        [Fact]
        public void TryGetInputGestureTest()
        {
            List<InputGesture> init = [new KeyGesture(Keys.A)];
            var command = new TestUICommandOverrideCreateInputGestures(init);
            Assert.Equal(0, command.CreateCount);
            Assert.True(command.TryGetInputGestures(out var inputGestures));
            Assert.Equal(init, inputGestures);
            Assert.Equal(1, command.CreateCount);
            Assert.True(command.TryGetInputGestures(out inputGestures));
            Assert.Equal(init, inputGestures);
            Assert.Equal(1, command.CreateCount);
        }
    }

    public class UICommandBaseTest_CommandBindings : IDisposable
    {
        public UICommandBaseTest_CommandBindings()
        {
            Monitor.Enter(Locks.CommandBindings);
        }

        public void Dispose()
        {
            Monitor.Exit(Locks.CommandBindings);
        }

        [Fact]
        public void CanExecuteTest()
        {
            using var control = new Control();
            var command = new UICommand("text", "name");
            var parameter = new object();
            Assert.False(command.CanExecute(parameter, control));

            control.GetCommandBindings().Add(new CommandBinding(command));
            Assert.False(command.CanExecute(parameter, control));

            control.GetCommandBindings().Add(new CommandBinding(command, null, (s, e) =>
            {
                Assert.Equal(command, e.Command);
                Assert.Equal(parameter, e.Parameter);
                Assert.False(e.CanExecute);
                e.CanExecute = true;
            }));
            Assert.True(command.CanExecute(parameter, control));
        }

        [Fact]
        public void CanExecuteHasExecuteTest()
        {
            using var control = new Control();
            var command = new UICommand("text", "name");
            var parameter = new object();

            control.GetCommandBindings().Add(new CommandBinding(command, (_, _) => { }));
            Assert.True(command.CanExecute(parameter, control));

            control.GetCommandBindings()[0].CanExecute += (_, e) => e.CanExecute = false;
            Assert.False(command.CanExecute(parameter, control));
        }

        [Fact]
        public void CanExecuteHandledTest()
        {
            using var parent = new Control();
            using var child = new Control();
            parent.Controls.Add(child);
            var command = new UICommand("text1", "name1");
            parent.GetCommandBindings().Add(new CommandBinding(command, null, (_, e) => e.CanExecute = true));
            child.GetCommandBindings().Add(new CommandBinding(command, null, (_, e) => e.Handled = true));

            Assert.False(command.CanExecute(null, child));
            Assert.True(command.CanExecute(null, parent));
        }

        [Fact]
        public void CanExecuteNestTest()
        {
            using var parent = new Control();
            using var child = new Control();
            parent.Controls.Add(child);

            var command1 = new UICommand("text1", "name1");
            var command2 = new UICommand("text2", "name2");
            var parameter = new object();
            void check(object sender, CanExecuteEventArgs e)
            {
                Assert.Equal(command1, e.Command);
                Assert.Equal(parameter, e.Parameter);
                Assert.False(e.CanExecute);
            }

            parent.GetCommandBindings().Add(new CommandBinding(command1, null, check)); // 0
            parent.GetCommandBindings().Add(new CommandBinding(command2, null, (_, _) => Assert.Fail("command2"))); // 1

            child.GetCommandBindings().Add(new CommandBinding(command1, null, check));
            child.GetCommandBindings().Add(new CommandBinding(command2, null, (_, _) => Assert.Fail("command2")));

            Assert.False(command1.CanExecute(parameter, parent));
            Assert.False(command1.CanExecute(parameter, child));

            parent.GetCommandBindings().Add(new CommandBinding(command1, null
                , (s, e) => { check(s, e); e.CanExecute = true; }));    // 2

            Assert.True(command1.CanExecute(parameter, parent));
            Assert.True(command1.CanExecute(parameter, child));

            child.GetCommandBindings().Add(new CommandBinding(command1, null
                , (s, e) => { check(s, e); e.CanExecute = true; }));
            parent.GetCommandBindings()[0].CanExecute += (s, e) => Assert.Fail("unreachable");

            Assert.True(command1.CanExecute(parameter, child));

            parent.GetCommandBindings().RemoveAt(2);
            parent.GetCommandBindings().RemoveAt(0);

            Assert.False(command1.CanExecute(parameter, parent));
        }

        [Fact]
        public void CanExecutePreviewCanExecuteCancelTest()
        {
            using var form = new Form();
            using var parent = new Control();
            using var child = new Control();
            form.Controls.Add(parent);
            parent.Controls.Add(child);

            var command = new UICommand("text1", "name1");
            bool canExecute = true;
            bool handled = false;
            var topcb = new CommandBinding(command);
            topcb.PreviewCanExecute += (_, e) => { e.CanExecute = canExecute; e.Handled = handled; };
            form.GetCommandBindings().Add(topcb);
            parent.GetCommandBindings().Add(new CommandBinding(command, null, (_, _) => Assert.Fail("canExecute")));
            var bottomcb = new CommandBinding(command);
            int bottomPreviewCanExecuteCount = 0;
            bottomcb.PreviewCanExecute += (_, e) =>
            {
                bottomPreviewCanExecuteCount++;
                e.CanExecute = !canExecute;
            };
            child.GetCommandBindings().Add(bottomcb);

            // Top での PreviewCanExecute true
            Assert.True(command.CanExecute(null, parent));
            Assert.Equal(0, bottomPreviewCanExecuteCount);
            Assert.True(command.CanExecute(null, child));
            Assert.Equal(0, bottomPreviewCanExecuteCount);

            // Bottom での PreviewCanExecute true
            canExecute = false;
            Assert.False(command.CanExecute(null, form));
            Assert.Equal(0, bottomPreviewCanExecuteCount);
            Assert.True(command.CanExecute(null, child));
            Assert.Equal(1, bottomPreviewCanExecuteCount);

            // Top での PreviewCanExecute false
            handled = true;
            Assert.False(command.CanExecute(null, form));
            Assert.Equal(1, bottomPreviewCanExecuteCount);
            Assert.False(command.CanExecute(null, child));
            Assert.Equal(1, bottomPreviewCanExecuteCount);
        }

        [Fact]
        public void ExecuteTest()
        {
            using var control = new TestControl();
            var command = new UICommand("text", "name");
            var parameter = new object();
            int execute = 0;
            control.GetCommandBindings().Add(new CommandBinding(command, (s, e) =>
            {
                Assert.Equal(command, e.Command);
                Assert.Equal(parameter, e.Parameter);
                execute++;
            }));
            command.Execute(parameter, control);
            Assert.Equal(1, execute);

            control.GetCommandBindings()[0].CanExecute += (_, e) => e.CanExecute = false;
            command.Execute(parameter, control);
            Assert.Equal(1, execute);

            // CanExecute が true でないと実行されない。
            control.GetCommandBindings()[0].PreviewCanExecute += (_, e) => e.CanExecute = true;
            command.Execute(parameter, control);
            Assert.Equal(1, execute);
        }

        [Fact]
        public void ExecuteHandledTest()
        {
            using var parent = new TestControl();
            using var child = new TestControl();
            parent.Controls.Add(child);
            var command = new UICommand("text", "name");
            int parentExecute = 0, childExecute = 0;
            parent.GetCommandBindings().Add(new CommandBinding(command, (_, _) => parentExecute++, (_, e) => e.CanExecute = true));
            child.GetCommandBindings().Add(new CommandBinding(command, (_, _) => childExecute++, (_, e) => e.Handled = true));

            // Handled されても、上の CommandBinding が実行される。
            command.Execute(null, child);
            Assert.Equal(1, parentExecute);
            Assert.Equal(0, childExecute);
            command.Execute(null, parent);
            Assert.Equal(2, parentExecute);
            Assert.Equal(0, childExecute);
        }

        [Fact]
        public void ExecuteNestTest()
        {
            using var parent = new TestControl();
            using var child = new TestControl();
            parent.Controls.Add(child);

            var command1 = new UICommand("text1", "name1");
            var command2 = new UICommand("text2", "name2");
            var parameter = new object();
            void check(object sender, ExecutedEventArgs e)
            {
                Assert.Equal(command1, e.Command);
                Assert.Equal(parameter, e.Parameter);
            }

            parent.GetCommandBindings().Add(new CommandBinding(command1, null, (_, e) => e.CanExecute = true));
            parent.GetCommandBindings().Add(new CommandBinding(command2, (_, _) => Assert.Fail("command2"), (_, _) => Assert.Fail("command2")));

            child.GetCommandBindings().Add(new CommandBinding(command1, null, (_, e) => e.CanExecute = true));
            child.GetCommandBindings().Add(new CommandBinding(command2, (_, _) => Assert.Fail("command2"), (_, _) => Assert.Fail("command2")));

            command1.Execute(parameter, parent);
            command1.Execute(parameter, child);

            int parentExecuteCount = 0;
            parent.GetCommandBindings().Add(new CommandBinding(command1, (s, e) => { check(s, e); parentExecuteCount++; }));

            command1.Execute(parameter, parent);
            Assert.Equal(1, parentExecuteCount);
            command1.Execute(parameter, child);
            Assert.Equal(2, parentExecuteCount);

            int childExecuteCount = 0;
            child.GetCommandBindings().Add(new CommandBinding(command1, (s, e) => { check(s, e); childExecuteCount++; }));

            command1.Execute(parameter, parent);
            Assert.Equal(3, parentExecuteCount);
            Assert.Equal(0, childExecuteCount);
            command1.Execute(parameter, child);
            Assert.Equal(3, parentExecuteCount);
            Assert.Equal(1, childExecuteCount);

            child.GetCommandBindings()[2].CanExecute += (_, e) => e.CanExecute = false;
            command1.Execute(parameter, child);
            Assert.Equal(4, parentExecuteCount);
            Assert.Equal(1, childExecuteCount);
        }

        [Fact]
        public void ExecutePreviewCanExecuteTest()
        {
            using var control = new TestControl();
            var command = new UICommand("text", "name");
            var parameter = new object();
            int execute = 0;
            var cb = new CommandBinding(command, (_, e) => execute++);
            // 呼ばれない。
            cb.PreviewCanExecute += (_, e) => Assert.Fail("PreviewCanExecute");
            control.GetCommandBindings().Add(cb);

            command.Execute(parameter, control);
            Assert.Equal(1, execute);
        }

        [Fact]
        public void ExecuteIfCanTest()
        {
            using var control = new TestControl();
            var command = new UICommand("text", "name");
            var parameter = new object();
            int execute = 0;
            control.GetCommandBindings().Add(new CommandBinding(command, (s, e) =>
            {
                Assert.Equal(command, e.Command);
                Assert.Equal(parameter, e.Parameter);
                execute++;
            }));

            Assert.True(command.ExecuteIfCan(parameter, control));
            Assert.Equal(1, execute);

            control.GetCommandBindings()[0].CanExecute += (_, e) => e.CanExecute = false;
            Assert.False(command.ExecuteIfCan(parameter, control));
            Assert.Equal(1, execute);

            // CanExecute が true でないと実行されない。
            control.GetCommandBindings()[0].PreviewCanExecute += (_, e) => e.CanExecute = true;
            Assert.False(command.ExecuteIfCan(parameter, control));
            Assert.Equal(1, execute);
        }

        [Fact]
        public void ExecuteIfCanNoHandlerTest()
        {
            using var control = new TestControl();
            var command = new UICommand("text", "name");
            var parameter = new object();
            control.GetCommandBindings().Add(new CommandBinding(command, null, (_, e) => e.CanExecute = true));
            Assert.False(command.ExecuteIfCan(parameter, control));
        }

        [Fact]
        public void ExecuteIfCanHandledTest()
        {
            using var parent = new TestControl();
            using var child = new TestControl();
            parent.Controls.Add(child);
            var command = new UICommand("text", "name");
            int parentExecute = 0, childExecute = 0;
            parent.GetCommandBindings().Add(new CommandBinding(command, (_, _) => parentExecute++, (_, e) => e.CanExecute = true));
            child.GetCommandBindings().Add(new CommandBinding(command, (_, _) => childExecute++, (_, e) => e.Handled = true));

            // Handled されても、上の CommandBinding が実行される。
            Assert.True(command.ExecuteIfCan(null, child));
            Assert.Equal(1, parentExecute);
            Assert.Equal(0, childExecute);
            Assert.True(command.ExecuteIfCan(null, parent));
            Assert.Equal(2, parentExecute);
            Assert.Equal(0, childExecute);
        }

        [Fact]
        public void ExecuteIfCanNestTest()
        {
            using var parent = new TestControl();
            using var child = new TestControl();
            parent.Controls.Add(child);

            var command1 = new UICommand("text1", "name1");
            var command2 = new UICommand("text2", "name2");
            var parameter = new object();

            bool parentCanExecute = false, childCanExecute = false;
            int parentExecute = 0, childExecute = 0;

            parent.GetCommandBindings().Add(new CommandBinding(command1, (_, _) => parentExecute++, (_, e) => e.CanExecute = parentCanExecute));
            parent.GetCommandBindings().Add(new CommandBinding(command2, null, (_, _) => Assert.Fail("command2")));

            child.GetCommandBindings().Add(new CommandBinding(command1, (_, _) => childExecute++, (_, e) => e.CanExecute = childCanExecute));
            child.GetCommandBindings().Add(new CommandBinding(command2, null, (_, _) => Assert.Fail("command2")));

            Assert.False(command1.ExecuteIfCan(parameter, parent));
            Assert.Equal(0, parentExecute);
            Assert.Equal(0, childExecute);
            Assert.False(command1.ExecuteIfCan(parameter, child));
            Assert.Equal(0, parentExecute);
            Assert.Equal(0, childExecute);

            parentCanExecute = true;
            parentExecute = childExecute = 0;
            Assert.True(command1.ExecuteIfCan(parameter, parent));
            Assert.Equal(1, parentExecute);
            Assert.Equal(0, childExecute);
            parentExecute = childExecute = 0;
            Assert.True(command1.ExecuteIfCan(parameter, child));
            Assert.Equal(1, parentExecute);
            Assert.Equal(0, childExecute);

            parentCanExecute = false;
            childCanExecute = true;
            parentExecute = childExecute = 0;
            Assert.False(command1.ExecuteIfCan(parameter, parent));
            Assert.Equal(0, parentExecute);
            Assert.Equal(0, childExecute);
            parentExecute = childExecute = 0;
            Assert.True(command1.ExecuteIfCan(parameter, child));
            Assert.Equal(0, parentExecute);
            Assert.Equal(1, childExecute);
        }

        [Fact]
        public void ExecuteIfCanPreviewCanExecuteCancelTest()
        {
            using var form = new Form();
            using var parent = new TestControl();
            using var child = new TestControl();
            form.Controls.Add(parent);
            parent.Controls.Add(child);

            var command = new UICommand("text1", "name1");
            bool canExecute = true;
            bool handled = false;
            int topExecute = 0, parentExecute = 0, childExecute = 0;
            var topcb = new CommandBinding(command, (_, _) => topExecute++);
            topcb.PreviewCanExecute += (_, e) => { e.CanExecute = canExecute; e.Handled = handled; };
            form.GetCommandBindings().Add(topcb);
            parent.GetCommandBindings().Add(new CommandBinding(command, (_, _) => parentExecute++));
            var bottomcb = new CommandBinding(command, (_, _) => childExecute++);
            int bottomPreviewCanExecuteCount = 0;
            bottomcb.PreviewCanExecute += (_, e) =>
            {
                bottomPreviewCanExecuteCount++;
                e.CanExecute = !canExecute;
            };
            child.GetCommandBindings().Add(bottomcb);

            // Top での PreviewCanExecute true
            Assert.True(command.ExecuteIfCan(null, parent));
            Assert.Equal(0, bottomPreviewCanExecuteCount);
            Assert.Equal(0, topExecute);
            Assert.Equal(1, parentExecute);
            Assert.Equal(0, childExecute);
            topExecute = parentExecute = childExecute = bottomPreviewCanExecuteCount = 0;
            Assert.True(command.ExecuteIfCan(null, child));
            Assert.Equal(0, bottomPreviewCanExecuteCount);
            Assert.Equal(0, topExecute);
            Assert.Equal(0, parentExecute);
            Assert.Equal(1, childExecute);

            // Bottom での PreviewCanExecute true
            canExecute = false;
            topExecute = parentExecute = childExecute = bottomPreviewCanExecuteCount = 0;
            Assert.True(command.ExecuteIfCan(null, parent));
            Assert.Equal(0, bottomPreviewCanExecuteCount);
            Assert.Equal(0, topExecute);
            Assert.Equal(1, parentExecute);
            Assert.Equal(0, childExecute);
            topExecute = parentExecute = childExecute = bottomPreviewCanExecuteCount = 0;
            Assert.True(command.ExecuteIfCan(null, child));
            Assert.Equal(1, bottomPreviewCanExecuteCount);
            Assert.Equal(0, topExecute);
            Assert.Equal(0, parentExecute);
            Assert.Equal(1, childExecute);

            // Top での PreviewCanExecute false
            handled = true;
            topExecute = parentExecute = childExecute = bottomPreviewCanExecuteCount = 0;
            Assert.False(command.ExecuteIfCan(null, parent));
            Assert.Equal(0, bottomPreviewCanExecuteCount);
            Assert.Equal(0, topExecute);
            Assert.Equal(0, parentExecute);
            Assert.Equal(0, childExecute);
            topExecute = parentExecute = childExecute = bottomPreviewCanExecuteCount = 0;
            Assert.False(command.ExecuteIfCan(null, child));
            Assert.Equal(0, bottomPreviewCanExecuteCount);
            Assert.Equal(0, topExecute);
            Assert.Equal(0, parentExecute);
            Assert.Equal(0, childExecute);
        }

        [Fact]
        public void CanExecuteChangedTest()
        {
            using var control = new Control();
            var command = new UICommand("text", "name");
            var parameter = new object();

            bool canExecute = false;
            int canExecuteCount = 0;
            control.GetCommandBindings().Add(new CommandBinding(command, null, (_, e) =>
            {
                e.CanExecute = canExecute;
                canExecuteCount++;
            }));

            int canExecuteChangedCount1 = 0;
            command.CanExecuteChanged += (s, e) =>
            {
                Assert.Equal(command, s);
                Assert.Equal(canExecute, command.CanExecute(parameter, control));   // 1
                Assert.Equal(canExecute, command.CanExecute(parameter, control));
                canExecuteChangedCount1++;
            };
            int canExecuteChangedCount2 = 0;
            command.CanExecuteChanged += (s, e) =>
            {
                Assert.Equal(command, s);
                Assert.Equal(canExecute, command.CanExecute(parameter, control));
                Assert.Equal(canExecute, command.CanExecute(parameter, control));
                Assert.Equal(canExecute, command.CanExecute(null, control));        // 2
                Assert.Equal(canExecute, command.CanExecute(null, control));
                using var control2 = new Control();
                Assert.False(command.CanExecute(null, control2));
                Assert.False(command.CanExecute(null, control2));
                Assert.Equal(canExecute, command.CanExecute(parameter, control));   // 3
                canExecuteChangedCount2++;
            };

            command.RaiseCanExecuteChanged();
            Assert.Equal(3, canExecuteCount);
            Assert.Equal(1, canExecuteChangedCount1);
            Assert.Equal(1, canExecuteChangedCount2);
            canExecute = false;
            command.RaiseCanExecuteChanged();
            Assert.Equal(6, canExecuteCount);
            Assert.Equal(2, canExecuteChangedCount1);
            Assert.Equal(2, canExecuteChangedCount2);
        }

        [Fact]
        public void PreviewCanExecuteTest()
        {
            using var control = new Control();
            var command = new UICommand("text", "name");
            var parameter = new object();

            int canExecuteCount = 0;
            var cb = new CommandBinding(command, null, (s, e) => canExecuteCount++);
            bool canExecute = true;
            bool handled = false;
            cb.PreviewCanExecute += (s, e) =>
            {
                Assert.Equal(command, e.Command);
                Assert.Equal(parameter, e.Parameter);
                Assert.False(e.CanExecute);
                e.CanExecute = canExecute;
                e.Handled = handled;
            };
            control.GetCommandBindings().Add(cb);

            Assert.True(command.CanExecute(parameter, control));
            Assert.Equal(0, canExecuteCount);
            canExecute = false;
            Assert.False(command.CanExecute(parameter, control));
            Assert.Equal(1, canExecuteCount);
            handled = true;
            Assert.False(command.CanExecute(parameter, control));
            Assert.Equal(1, canExecuteCount);
        }

        [Fact]
        public void PreviewCanExecuteNestTest()
        {
            using var parent = new Control();
            using var child = new Control();
            parent.Controls.Add(child);

            var command1 = new UICommand("text1", "name1");
            var command2 = new UICommand("text2", "name2");
            var parameter = new object();

            int canExecuteCount = 0;
            var cbParentCommand11 = new CommandBinding(command1, (_, _) => Assert.Fail("execute"), (_, _) => Assert.Fail("canExecute"));
            var cbParentCommand12 = new CommandBinding(command1, (_, _) => Assert.Fail("execute"), (_, _) => Assert.Fail("canExecute"));
            var cbParentCommand21 = new CommandBinding(command2, (_, _) => Assert.Fail("execute"), (_, _) => Assert.Fail("canExecute"));
            var cbParentCommand22 = new CommandBinding(command2, (_, _) => Assert.Fail("execute"), (_, _) => Assert.Fail("canExecute"));
            var cbChildCommand11 = new CommandBinding(command1, (_, _) => Assert.Fail("execute"), (_, e) => { canExecuteCount++; e.Handled = true; });
            var cbChildCommand12 = new CommandBinding(command1, (_, _) => Assert.Fail("execute"), (_, _) => Assert.Fail("canExecute"));
            var cbChildCommand21 = new CommandBinding(command2, (_, _) => Assert.Fail("execute"), (_, _) => Assert.Fail("canExecute"));
            var cbChildCommand22 = new CommandBinding(command2, (_, _) => Assert.Fail("execute"), (_, _) => Assert.Fail("canExecute"));

            parent.GetCommandBindings().Add(cbParentCommand11);
            parent.GetCommandBindings().Add(cbParentCommand12);
            parent.GetCommandBindings().Add(cbParentCommand21);
            parent.GetCommandBindings().Add(cbParentCommand22);
            child.GetCommandBindings().Add(cbChildCommand11);
            child.GetCommandBindings().Add(cbChildCommand12);
            child.GetCommandBindings().Add(cbChildCommand21);
            child.GetCommandBindings().Add(cbChildCommand22);

            bool handledTest = false;
            int count = 0, previewCanExecuteCount = 0;
            CommandBinding targetcb = null;
            void OnPreviewCanExecute(object s, CanExecuteEventArgs e, CommandBinding commandBinding)
            {
                Assert.False(e.CanExecute);
                Assert.Equal(command1, e.Command);
                Assert.Equal(parameter, e.Parameter);

                if (count == previewCanExecuteCount)
                {
                    if (handledTest)
                        e.Handled = true;
                    else
                        e.CanExecute = true;
                    targetcb = commandBinding;
                }
                previewCanExecuteCount++;
            }

            cbParentCommand11.PreviewCanExecute += (s, e) => OnPreviewCanExecute(s, e, cbParentCommand11);
            cbParentCommand12.PreviewCanExecute += (s, e) => OnPreviewCanExecute(s, e, cbParentCommand12);
            cbChildCommand11.PreviewCanExecute += (s, e) => OnPreviewCanExecute(s, e, cbChildCommand11);
            cbChildCommand12.PreviewCanExecute += (s, e) => OnPreviewCanExecute(s, e, cbChildCommand12);
            cbParentCommand21.PreviewCanExecute += (_, _) => Assert.Fail("command2");
            cbParentCommand22.PreviewCanExecute += (_, _) => Assert.Fail("command2");
            cbChildCommand21.PreviewCanExecute += (_, _) => Assert.Fail("command2");
            cbChildCommand22.PreviewCanExecute += (_, _) => Assert.Fail("command2");

            Assert.True(command1.CanExecute(parameter, child));
            Assert.Equal(1, previewCanExecuteCount);
            Assert.Equal(cbParentCommand12, targetcb);
            count = 1;
            previewCanExecuteCount = 0;
            Assert.True(command1.CanExecute(parameter, child));
            Assert.Equal(2, previewCanExecuteCount);
            Assert.Equal(cbParentCommand11, targetcb);
            count = 2;
            previewCanExecuteCount = 0;
            Assert.True(command1.CanExecute(parameter, child));
            Assert.Equal(3, previewCanExecuteCount);
            Assert.Equal(cbChildCommand12, targetcb);
            count = 3;
            previewCanExecuteCount = 0;
            Assert.True(command1.CanExecute(parameter, child));
            Assert.Equal(4, previewCanExecuteCount);
            Assert.Equal(cbChildCommand11, targetcb);
            count = 4;
            previewCanExecuteCount = 0;
            Assert.Equal(0, canExecuteCount);
            Assert.False(command1.CanExecute(parameter, child));
            Assert.Equal(4, previewCanExecuteCount);
            Assert.Equal(cbChildCommand11, targetcb);
            Assert.Equal(1, canExecuteCount);

            handledTest = true;
            canExecuteCount = 0;
            count = 0;
            previewCanExecuteCount = 0;
            Assert.False(command1.CanExecute(parameter, child));
            Assert.Equal(1, previewCanExecuteCount);
            Assert.Equal(cbParentCommand12, targetcb);
            count = 1;
            previewCanExecuteCount = 0;
            Assert.False(command1.CanExecute(parameter, child));
            Assert.Equal(2, previewCanExecuteCount);
            Assert.Equal(cbParentCommand11, targetcb);
            count = 2;
            previewCanExecuteCount = 0;
            Assert.False(command1.CanExecute(parameter, child));
            Assert.Equal(3, previewCanExecuteCount);
            Assert.Equal(cbChildCommand12, targetcb);
            count = 3;
            previewCanExecuteCount = 0;
            Assert.False(command1.CanExecute(parameter, child));
            Assert.Equal(4, previewCanExecuteCount);
            Assert.Equal(cbChildCommand11, targetcb);
            count = 4;
            previewCanExecuteCount = 0;
            Assert.Equal(0, canExecuteCount);
            Assert.False(command1.CanExecute(parameter, child));
            Assert.Equal(4, previewCanExecuteCount);
            Assert.Equal(cbChildCommand11, targetcb);
            Assert.Equal(1, canExecuteCount);
        }

        //

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
