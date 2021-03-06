﻿using System;
using System.Collections.Generic;
using System.Linq;
using Ploeh.AutoFixture;
using Xunit;
using Xunit.Extensions;

namespace Test.CpuTests
{
    public class SET_x_HLm : CpuTestBase
    {
        private readonly ushort _hl;

        public SET_x_HLm()
        {
            _hl = Fixture.Create<ushort>();
        }

        [Theory, PropertyData("Bits")]
        public void Advances_counters(int bit)
        {
            ExecutingCB(CreateOpCode(bit));

            AdvancedProgramCounter(2);
            AdvancedClock(4);
        }

        [Theory, PropertyData("Bits")]
        public void Resets_bit_at_hl_location(int bit)
        {
            RegisterPair.HL.Set(Cpu, (byte)(_hl >> 8), (byte)_hl);
            FakeMmu.SetByte(_hl, 0x00);

            ExecutingCB(CreateOpCode(bit));

            var expected = (1 << bit);
            Assert.Equal(expected, FakeMmu.Memory[_hl]);
        }

        private static byte CreateOpCode(int bit)
        {
            return (byte)(0xC6 | (bit << 3));
        }

        public static IEnumerable<object[]> Bits
        {
            get
            {
                return Enumerable.Range(0, 8)
                    .Select(x => new Object[] { x })
                    .ToList();
            }
        }
    }
}