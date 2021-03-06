﻿using Core;
using Xunit;

namespace Test.Display
{
    public class LYCompare : CpuTestBase
    {
        private readonly Core.Display _sut;
        private readonly FakeMmu _fakeMmu;

        public LYCompare()
        {
            _fakeMmu = new FakeMmu();
            _sut = new Core.Display(_fakeMmu, new FakeDisplayRenderer());
        }

        [Fact]
        public void Coincidence_flag_is_zero_when_LYC_and_LY_are_different()
        {
            _sut.LYC = 100;

            _sut.AdvanceToScanLine(99);

            Assert.False(CoincidenceFlag);
        }

        [Fact]
        public void Has_not_raised_LYC_interrupt_before_LY_and_LYC_are_same()
        {
            _sut.STAT |= 0x40;
            _fakeMmu.Memory[RegisterAddresses.IF] = 0xFD;
            _sut.LYC = 102;

            _sut.AdvanceToScanLine(100);

            Assert.Equal(0xFD, _fakeMmu.Memory[RegisterAddresses.IF]);
        }

        [Fact]
        public void Coincidence_flag_is_one_when_LYC_and_LY_are_same()
        {
            _sut.LYC = 123;

            _sut.AdvanceToScanLine(123);

            Assert.True(CoincidenceFlag);
        }

        [Fact]
        public void Raises_coincidence_interrupt_when_LYC_and_LY_are_same()
        {
            _sut.STAT |= 0x40;
            _fakeMmu.Memory[RegisterAddresses.IF] = 0x18;
            _sut.LYC = 123;

            _sut.AdvanceToScanLine(123);

            Assert.Equal(0x1A, _fakeMmu.Memory[RegisterAddresses.IF]);
        }

        [Fact]
        public void Raises_coincidence_interrupt_for_vertical_blanking_lines()
        {
            _sut.STAT |= 0x40;
            _fakeMmu.Memory[RegisterAddresses.IF] = 0x18;
            _sut.LYC = 150;

            _sut.AdvanceToScanLine(150);

            Assert.Equal(0x1B, _fakeMmu.Memory[RegisterAddresses.IF]);
        }

        [Fact]
        public void Does_not_raise_interrupt_when_coincidence_interrupt_is_disabled()
        {
            _sut.STAT = 0;
            _fakeMmu.Memory[RegisterAddresses.IF] = 0x18;
            _sut.LYC = 150;

            _sut.AdvanceToScanLine(150);

            Assert.Equal(0x19, _fakeMmu.Memory[RegisterAddresses.IF]);
        }

        private bool CoincidenceFlag
        {
            get { return ((_sut.STAT >> 2) & 0x1) == 0x1; }
        }
    }
}