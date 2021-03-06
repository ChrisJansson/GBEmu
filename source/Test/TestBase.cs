﻿using Core;
using Ploeh.AutoFixture;

namespace Test
{
    public class TestBase
    {
        protected Cpu Sut;
        protected Fixture Fixture;
        protected Core.MMU FakeMmu;

        public TestBase()
        {
            Fixture = new Fixture();
            FakeMmu = new Core.MMU(new ReadWriteMBC(new byte[32.KB()]));
            Sut = new Cpu(FakeMmu);
        }
    }
}