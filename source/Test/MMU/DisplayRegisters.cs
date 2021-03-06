﻿using Core;
using Xunit;

namespace Test.MMU
{
    public class DisplayRegisters
    {
        private readonly Core.MMU _sut;
        private readonly FakeDisplay _display;

        public DisplayRegisters()
        {
            _sut = new Core.MMU(null);

            _display = new FakeDisplay();
            _sut.Display = _display;
        }

        [Fact]
        public void FF47_sets_display_palette_data()
        {
            _sut.SetByte(0xFF47, 0xFA);

            Assert.Equal(0xFA, _display.BackgroundPaletteData);
        }

        [Fact]
        public void FF47_gets_display_palette_data()
        {
            _display.BackgroundPaletteData = 0xAB;

            var result = _sut.GetByte(0xFF47);

            Assert.Equal(0xAB, result);
        }

        [Fact]
        public void FF42_stores_scroll_y()
        {
            _sut.SetByte(0xFF42, 0xCE);

            var result = _sut.GetByte(RegisterAddresses.ScrollY);

            Assert.Equal(0xCE, result);
        }

        [Fact]
        public void FF43_stores_scroll_x()
        {
            _sut.SetByte(0xFF43, 0x49);

            var result = _sut.GetByte(RegisterAddresses.ScrollX);

            Assert.Equal(0x49, result);
        }

        [Fact]
        public void FF44_indicates_current_display_line()
        {
            _display.Line = 137;

            var result = _sut.GetByte(RegisterAddresses.LY);

            Assert.Equal(137, result);
        }

        [Fact]
        public void FF44_is_read_only()
        {
            _display.Line = 123;

            _sut.SetByte(RegisterAddresses.LY, 0);

            Assert.Equal(123, _display.Line);
        }

        [Fact]
        public void FF40_is_LCDC_on_display()
        {
            _sut.SetByte(RegisterAddresses.LCDC, 0xFC);

            Assert.Equal(0xFC, _display.LCDC);
        }

        [Fact]
        public void FF40_depends_on_display_LCDC()
        {
            _display.LCDC = 0xAB;

            var actual = _sut.GetByte(RegisterAddresses.LCDC);

            Assert.Equal(0xAB, actual);
        }

        [Fact]
        public void FF41_is_LCDStat_on_display()
        {
            _sut.SetByte(RegisterAddresses.LCDSTAT, 0xF8);

            Assert.Equal(0xF8, _display.STAT);
        }

        [Fact]
        public void FF41_depdends_on_display_STAT()
        {
            _display.STAT = 0xAB;

            var actual = _sut.GetByte(RegisterAddresses.LCDSTAT);

            Assert.Equal(0xAB, actual);
        }

        [Fact]
        public void FF46_writes_to_DMA()
        {
            _sut.SetByte(0xFF46, 0xF2);

            Assert.Equal(0xF2, _display.DMA);
        }

        [Fact]
        public void FF46_reads_from_DMA()
        {
            _display.DMA = 0xF5;

            var actual = _sut.GetByte(0xFF46);

            Assert.Equal(0xF5, actual);
        }

        [Fact]
        public void FF45_reads_from_LYC()
        {
            _display.LYC = 0xAB;

            var actual = _sut.GetByte(0xFF45);

            Assert.Equal(0xAB, actual);
        }

        [Fact]
        public void FF45_writes_to_LYC()
        {
            _sut.SetByte(0xFF45, 0xAC);

            Assert.Equal(0xAC, _display.LYC);
        }
    }

    public class FakeDisplay : IDisplay
    {
        public byte LYC { get; set; }
        public byte STAT { get; set; }
        public byte DMA { get; set; }
        public byte BackgroundPaletteData { get; set; }
        public byte Line { get; set; }
        public byte LCDC { get; set; }
    }
}