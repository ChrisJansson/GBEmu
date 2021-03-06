﻿using System;
using Core;
using Ploeh.AutoFixture;
using Xunit;

namespace Test
{
    public class CpuTestBase
    {
        public readonly FakeMmu FakeMmu;
        public readonly Cpu Cpu;
        private readonly ushort _initialProgramCounter;
        private readonly long _initialCycles;
        public Fixture Fixture;
        private readonly ushort _initialStackPointer;

        public CpuTestBase()
        {
            Fixture = new Fixture();
            FakeMmu = new FakeMmu();
            Fixture.Inject<IMmu>(FakeMmu);

            Cpu = Fixture.Build<Cpu>()
                .Without(x => x.IE)
                .Without(x => x.IF)
                .Without(x => x.IME)
                .Without(x => x.Halted)
                .Create();

            _initialProgramCounter = Cpu.ProgramCounter;
            _initialCycles = Cpu.Cycles;
            _initialStackPointer = Cpu.SP;
        }

        protected void AssertFlags(Action<FlagAssertion> assertion)
        {
            var flagAssertion = new FlagAssertion(Cpu);
            assertion(flagAssertion);
        }

        protected void Flags(Action<FlagSetter> setter)
        {
            var flagSetter = new FlagSetter(Cpu);
            setter(flagSetter);
        }

        protected void AdvancedClock(int expectedIncrement)
        {
            Assert.Equal(_initialCycles + expectedIncrement, Cpu.Cycles);
        }

        protected void AdvancedProgramCounter(int expectedIncrement)
        {
            Assert.Equal(_initialProgramCounter + expectedIncrement, Cpu.ProgramCounter);
        }

        protected void DecrementedStackPointer(int expectedIncrement)
        {
            Assert.Equal(_initialStackPointer - expectedIncrement, Cpu.SP);
        }

        protected void ExecutingCB(byte opCode)
        {
            FakeMmu.Memory[Cpu.ProgramCounter + 1] = opCode;
            Cpu.Execute(0xCB);
        }

        protected void Execute(byte opCode, params byte[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                FakeMmu.Memory[Cpu.ProgramCounter + 1 + i] = args[i];
            }
            Cpu.Execute(opCode);
        }

        public void Set(RegisterMapping register, byte value)
        {
            register.Set(Cpu, value);
        }

        public void Set(RegisterPair register, ushort value)
        {
            register.Set(Cpu, (byte) (value >> 8), (byte) value);
        }

        protected class FlagSetter
        {
            private readonly Cpu _cpu;

            public FlagSetter(Cpu cpu)
            {
                _cpu = cpu;
            }

            public FlagSetter Subtract()
            {
                _cpu.N = 1;
                return this;
            }

            public FlagSetter HalfCarry()
            {
                _cpu.HC = 1;
                return this;
            }

            public FlagSetter Zero()
            {
                _cpu.Z = 1;
                return this;
            }

            public FlagSetter ResetCarry()
            {
                _cpu.Carry = 0;
                return this;
            }

            public FlagSetter Carry()
            {
                _cpu.Carry = 1;
                return this;
            }

            public FlagSetter ResetZero()
            {
                _cpu.Z = 0;
                return this;
            }

            public FlagSetter ResetHalfCarry()
            {
                _cpu.HC = 0;
                return this;
            }

            public FlagSetter ResetSubtract()
            {
                _cpu.N = 0;
                return this;
            }
        }

        protected class FlagAssertion
        {
            private readonly Cpu _cpu;

            public FlagAssertion(Cpu cpu)
            {
                _cpu = cpu;
            }

            public FlagAssertion ResetSubtract()
            {
                Assert.Equal(0, _cpu.N);
                return this;
            }

            public FlagAssertion ResetHalfCarry()
            {
                Assert.Equal(0, _cpu.HC);
                return this;
            }

            public FlagAssertion SetCarry()
            {
                Assert.Equal(1, _cpu.Carry);
                return this;
            }

            public FlagAssertion ResetCarry()
            {
                Assert.Equal(0, _cpu.Carry);
                return this;
            }

            public FlagAssertion ResetZero()
            {
                Assert.Equal(0, _cpu.Z);
                return this;
            }

            public FlagAssertion SetZero()
            {
                Assert.Equal(1, _cpu.Z);
                return this;
            }

            public FlagAssertion HalfCarry()
            {
                Assert.Equal(1, _cpu.HC);
                return this;
            }

            public FlagAssertion SetSubtract()
            {
                Assert.Equal(1,_cpu.N);
                return this;
            }

            public FlagAssertion SetHalfCarry()
            {
                Assert.Equal(1, _cpu.HC);
                return this;
            }
        }
    }
}