﻿namespace Test
{
    public class CP_n : CPTestBase
    {
        protected override void ExecuteCompareATo(byte value)
        {
            Execute(CreateOpCode(), value);
        }

        protected override int Length
        {
            get { return 2; }
        }

        private byte CreateOpCode()
        {
            return 0xFE;
        }
    }
}