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
            Assert.Equal(KeyModifiers.None, target.Modifiers);
            Assert.Empty(target.DisplayString);

            target = new KeyGesture(Keys.B, KeyModifiers.Shift);
            Assert.Equal(Keys.B, target.Key);
            Assert.Equal(KeyModifiers.Shift, target.Modifiers);
            Assert.Empty(target.DisplayString);

            target = new KeyGesture(Keys.C, KeyModifiers.Alt | KeyModifiers.Control, "Test");
            Assert.Equal(Keys.C, target.Key);
            Assert.Equal(KeyModifiers.Alt | KeyModifiers.Control, target.Modifiers);
            Assert.Equal("Test", target.DisplayString);

            Assert.Throws<ArgumentNullException>("displayString", () => new KeyGesture(Keys.D, KeyModifiers.None, null));
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
                Assert.False(target.Matches(null, new KeyEventArgs((key | (Keys)KeyModifiers.Shift))));
            }
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

            var key = Keys.A;
            var target = new KeyGesture(key, modifiers);
            Assert.True(target.Matches(null, new KeyEventArgs(key | (Keys)modifiers)));
            if (modifiers != KeyModifiers.None)
                Assert.False(target.Matches(null, new KeyEventArgs(key)));
            if (modifiers != all)
                Assert.False(target.Matches(null, new KeyEventArgs(key | (Keys)all)));
            modifiers ^= all;
            Assert.False(target.Matches(null, new KeyEventArgs(key | (Keys)modifiers)));
        }
    }
}
