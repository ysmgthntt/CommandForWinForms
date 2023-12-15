#nullable disable

namespace CommandForWinForms.Tests
{
    public class KeyGestureTest
    {
        [Fact]
        public void ConstructorTest()
        {
            var target = new KeyGesture(Keys.A);
            Assert.Equal(Keys.A, target.Key);
            Assert.Equal(ModifierKeys.None, target.Modifiers);
            Assert.Empty(target.DisplayString);

            target = new KeyGesture(Keys.B, ModifierKeys.Shift);
            Assert.Equal(Keys.B, target.Key);
            Assert.Equal(ModifierKeys.Shift, target.Modifiers);
            Assert.Empty(target.DisplayString);

            target = new KeyGesture(Keys.C, ModifierKeys.Alt | ModifierKeys.Control, "Test");
            Assert.Equal(Keys.C, target.Key);
            Assert.Equal(ModifierKeys.Alt | ModifierKeys.Control, target.Modifiers);
            Assert.Equal("Test", target.DisplayString);

            Assert.Throws<ArgumentNullException>("displayString", () => new KeyGesture(Keys.D, ModifierKeys.None, null));
        }

        [Theory]
        [InlineData(Keys.A, Keys.Z)]
        [InlineData(Keys.F1, Keys.F12)]
        [InlineData(Keys.Escape, Keys.Escape)]
        public void MatchesKeyTest(Keys from, Keys to)
        {
            for (Keys key = from; key <= to; key++)
            {
                var target = new KeyGesture(key);
                Assert.True(target.Matches(null, new KeyEventArgs(key)));
                Assert.False(target.Matches(null, new KeyEventArgs((key + 1))));
                Assert.False(target.Matches(null, new KeyEventArgs((key | (Keys)ModifierKeys.Shift))));
            }
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

            var key = Keys.A;
            var target = new KeyGesture(key, modifiers);
            Assert.True(target.Matches(null, new KeyEventArgs(key | (Keys)modifiers)));
            if (modifiers != ModifierKeys.None)
                Assert.False(target.Matches(null, new KeyEventArgs(key)));
            if (modifiers != all)
                Assert.False(target.Matches(null, new KeyEventArgs(key | (Keys)all)));
            modifiers ^= all;
            Assert.False(target.Matches(null, new KeyEventArgs(key | (Keys)modifiers)));
        }
    }
}
