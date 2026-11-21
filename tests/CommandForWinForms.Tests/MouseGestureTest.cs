#nullable disable

namespace CommandForWinForms.Tests
{
    public class MouseGestureTest
    {
        [Fact]
        public void ConstructorTest()
        {
            var target = new MouseGesture();
            Assert.Equal(MouseAction.None, target.MouseAction);
            Assert.Equal(ModifierKeys.None, target.Modifiers);

            target = new MouseGesture(MouseAction.LeftClick);
            Assert.Equal(MouseAction.LeftClick, target.MouseAction);
            Assert.Equal(ModifierKeys.None, target.Modifiers);

            target = new MouseGesture(MouseAction.RightDoubleClick, ModifierKeys.Alt | ModifierKeys.Control);
            Assert.Equal(MouseAction.RightDoubleClick, target.MouseAction);
            Assert.Equal(ModifierKeys.Alt | ModifierKeys.Control, target.Modifiers);
        }

        [Theory]
        [InlineData(MouseAction.LeftClick, MouseAction.RightClick)]
        [InlineData(MouseAction.RightClick, MouseAction.LeftClick)]
        [InlineData(MouseAction.MiddleClick, MouseAction.LeftClick)]
        [InlineData(MouseAction.WheelClick, MouseAction.LeftClick)]
        [InlineData(MouseAction.LeftDoubleClick, MouseAction.LeftClick)]
        [InlineData(MouseAction.RightDoubleClick, MouseAction.LeftClick)]
        [InlineData(MouseAction.MiddleDoubleClick, MouseAction.LeftClick)]
        public void MatchesMouseActionTest(MouseAction mouseAction, MouseAction otherMouseAction)
        {
            var target = new MouseGesture(mouseAction);

            Assert.True(target.Matches(null, mouseAction, ModifierKeys.None));
            Assert.False(target.Matches(null, otherMouseAction, ModifierKeys.None));
            Assert.False(target.Matches(null, MouseAction.None, ModifierKeys.None));
        }

        [Fact]
        public void DoesNotMatchNoneMouseActionTest()
        {
            var target = new MouseGesture();

            Assert.False(target.Matches(null, MouseAction.None, ModifierKeys.None));
            Assert.False(target.Matches(null, MouseAction.LeftClick, ModifierKeys.None));
        }

        [Theory]
        [InlineData(ModifierKeys.None)]
        [InlineData(ModifierKeys.Alt)]
        [InlineData(ModifierKeys.Control)]
        [InlineData(ModifierKeys.Shift)]
        [InlineData(ModifierKeys.Alt | ModifierKeys.Control)]
        [InlineData(ModifierKeys.Alt | ModifierKeys.Shift)]
        [InlineData(ModifierKeys.Control | ModifierKeys.Shift)]
        [InlineData(ModifierKeys.Alt | ModifierKeys.Control | ModifierKeys.Shift)]
        public void MatchesModifierKeysTest(ModifierKeys modifiers)
        {
            const ModifierKeys all = (ModifierKeys.Alt | ModifierKeys.Control | ModifierKeys.Shift);

            var target = new MouseGesture(MouseAction.LeftClick, modifiers);
            Assert.True(target.Matches(null, MouseAction.LeftClick, modifiers));
            if (modifiers != ModifierKeys.None)
                Assert.False(target.Matches(null, MouseAction.LeftClick, ModifierKeys.None));
            if (modifiers != all)
                Assert.False(target.Matches(null, MouseAction.LeftClick, modifiers | all));
            modifiers ^= all;
            Assert.False(target.Matches(null, MouseAction.LeftClick, modifiers));
        }
    }
}
