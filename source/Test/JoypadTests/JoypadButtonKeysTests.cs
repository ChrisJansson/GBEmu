﻿using Core;
using Xunit;
using Xunit.Extensions;

namespace Test.JoypadTests
{
    public class JoypadButtonKeysTests
    {
        private Joypad _sut;

        public JoypadButtonKeysTests()
        {
            _sut = new Joypad(new FakeMmu());
            _sut.P1 = 0x10; //Select button keys
        }

        [Theory]
        [InlineData(false, (byte)0x1F)]
        [InlineData(true, (byte)0x1E)]
        public void Bit_0_is_button_A(bool isPressed, byte expectedP1)
        {
            _sut.A = isPressed;

            Assert.Equal(expectedP1, _sut.P1);
        }

        [Theory]
        [InlineData(false, (byte)0x1F)]
        [InlineData(true, (byte)0x1D)]
        public void Bit_1_is_button_B(bool isPressed, byte expectedP1)
        {
            _sut.B = isPressed;

            Assert.Equal(expectedP1, _sut.P1);
        }

        [Theory]
        [InlineData(false, (byte)0x1F)]
        [InlineData(true, (byte)0x1B)]
        public void Bit_2_is_select_button(bool isPressed, byte expectedP1)
        {
            _sut.Select = isPressed;

            Assert.Equal(expectedP1, _sut.P1);
        }

        [Theory]
        [InlineData(false, (byte)0x1F)]
        [InlineData(true, (byte)0x17)]
        public void Bit_3_is_start_button(bool isPressed, byte expectedP1)
        {
            _sut.Start = isPressed;

            Assert.Equal(expectedP1, _sut.P1);
        }

        [Fact]
        public void Combines_buttons_into_P1()
        {
            _sut.Start = true;
            _sut.B = true;

            Assert.Equal(0x15, _sut.P1);
        }
    }
}