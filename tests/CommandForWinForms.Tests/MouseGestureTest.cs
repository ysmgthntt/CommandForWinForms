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
            Assert.Equal(KeyModifiers.None, target.Modifiers);

            target = new MouseGesture(MouseAction.LeftClick);
            Assert.Equal(MouseAction.LeftClick, target.MouseAction);
            Assert.Equal(KeyModifiers.None, target.Modifiers);

            target = new MouseGesture(MouseAction.RightDoubleClick, KeyModifiers.Alt | KeyModifiers.Control);
            Assert.Equal(MouseAction.RightDoubleClick, target.MouseAction);
            Assert.Equal(KeyModifiers.Alt | KeyModifiers.Control, target.Modifiers);
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

            Assert.True(target.Matches(null, mouseAction, KeyModifiers.None));
            Assert.False(target.Matches(null, otherMouseAction, KeyModifiers.None));
            Assert.False(target.Matches(null, MouseAction.None, KeyModifiers.None));
        }

        [Fact]
        public void DoesNotMatchNoneMouseActionTest()
        {
            var target = new MouseGesture();

            Assert.False(target.Matches(null, MouseAction.None, KeyModifiers.None));
            Assert.False(target.Matches(null, MouseAction.LeftClick, KeyModifiers.None));
        }

        [Theory]
        [InlineData(KeyModifiers.None)]
        [InlineData(KeyModifiers.Alt)]
        [InlineData(KeyModifiers.Control)]
        [InlineData(KeyModifiers.Shift)]
        [InlineData(KeyModifiers.Alt | KeyModifiers.Control)]
        [InlineData(KeyModifiers.Alt | KeyModifiers.Shift)]
        [InlineData(KeyModifiers.Control | KeyModifiers.Shift)]
        [InlineData(KeyModifiers.Alt | KeyModifiers.Control | KeyModifiers.Shift)]
        public void MatchesModifierKeysTest(KeyModifiers modifiers)
        {
            const KeyModifiers all = (KeyModifiers.Alt | KeyModifiers.Control | KeyModifiers.Shift);

            var target = new MouseGesture(MouseAction.LeftClick, modifiers);
            Assert.True(target.Matches(null, MouseAction.LeftClick, modifiers));
            if (modifiers != KeyModifiers.None)
                Assert.False(target.Matches(null, MouseAction.LeftClick, KeyModifiers.None));
            if (modifiers != all)
                Assert.False(target.Matches(null, MouseAction.LeftClick, modifiers | all));
            modifiers ^= all;
            Assert.False(target.Matches(null, MouseAction.LeftClick, modifiers));
        }
    }
}
