﻿using System;
using Core;
using Xunit;

namespace Test.MMU
{
    public class MMUTests
    {
        //General Memory Map
        //  8000 - 9FFF   8KB Video RAM(VRAM)(switchable bank 0 - 1 in CGB Mode)
        //  FE00 - FE9F   Sprite Attribute Table(OAM)
        //  FF00 - FF7F   I / O Ports

        [Fact]
        public void Reads_0000_to_7FFF_from_MBC() //ROM
        {
            AssertReadsRangeFromMBC(startAddress: 0, endAddress: 0x7FFF);
        }

        [Fact]
        public void Writes_0000_to_7FFF_to_MBC() //ROM
        {
            AssertWritesRangeToMBC(startAddress: 0, endAddress: 0x7FFF);
        }

        [Fact]
        public void Reads_A000_to_BFFF_from_MBC() //External RAM
        {
            AssertReadsRangeFromMBC(startAddress: 0xA000, endAddress: 0xBFFF);
        }

        [Fact]
        public void Writes_A000_to_BFFF_to_MBC() //External ram
        {
            AssertWritesRangeToMBC(startAddress: 0xA000, endAddress: 0xBFFF);
        }

        [Fact]
        public void Read_Writes_8000_to_9FFF() //Video ram
        {
            var mmu = new Core.MMU(CreateFakeMBC());

            var random = new Random();
            for (int i = 0x8000; i < 0x9FFF + 1; i++)
            {
                var expected = (byte)random.Next(0, 255);
                mmu.SetByte((ushort)i, expected);
                var actual = mmu.GetByte((ushort)i);
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void Read_writes_C000_to_FDFF()
        {
            var mmu = new Core.MMU(CreateFakeMBC());

            var random = new Random();
            for (int i = 0xC000; i < 0xFDFF + 1; i++)
            {
                var expected = (byte)random.Next(0, 255);
                mmu.SetByte((ushort)i, expected);
                var actual = mmu.GetByte((ushort)i);
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void Cannot_write_to_FEA0_to_FEFF()
        {
            var mmu = new Core.MMU(CreateFakeMBC());

            var random = new Random();
            for (int i = 0xFEA0; i < 0xFEFF + 1; i++)
            {
                mmu.SetByte((ushort)i, (byte)random.Next(0, 255));
                var actual = mmu.GetByte((ushort)i);
                Assert.Equal(0, actual);
            }
        }

        [Fact]
        public void Read_writes_FF80_to_FFFE() //HRAM
        {
            var mmu = new Core.MMU(CreateFakeMBC());

            var random = new Random();
            for (int i = 0xFF80; i < 0xFFFE + 1; i++)
            {
                var expected = (byte)random.Next(0, 255);
                mmu.SetByte((ushort)i, expected);
                var actual = mmu.GetByte((ushort)i);
                Assert.Equal(expected, actual);
            }
        }

        private void AssertReadsRangeFromMBC(ushort startAddress, ushort endAddress)
        {
            var mbc = CreateFakeMBC();
            var mmu = new Core.MMU(mbc);

            for (int i = startAddress; i < endAddress + 1; i++)
            {
                var expected = mbc.Memory[i];
                var actual = mmu.GetByte((ushort)i);
                Assert.Equal(expected, actual);
            }
        }

        private static void AssertWritesRangeToMBC(int startAddress, int endAddress)
        {
            var mbc = CreateFakeMBC();
            var mmu = new Core.MMU(mbc);
            var random = new Random();
            for (var i = startAddress; i < endAddress + 1; i++)
            {
                var expected = (byte)random.Next(0, 255);
                mmu.SetByte((ushort)i, expected);
                var actual = mbc.Memory[i];
                Assert.Equal(expected, actual);
            }
        }

        private static FakeMBC CreateFakeMBC()
        {
            var mbc = new FakeMBC();
            var random = new Random();
            random.NextBytes(mbc.Memory);
            return mbc;
        }
    }

    public class FakeMBC : IMBC
    {
        public byte[] Memory = new byte[ushort.MaxValue + 1];

        public byte GetByte(ushort address)
        {
            return Memory[address];
        }

        public void SetByte(ushort address, byte value)
        {
            Memory[address] = value;
        }
    }
}
