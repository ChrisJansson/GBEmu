namespace Core
{
    public class MMU : IMmu
    {
        private readonly IMBC _mbc;

        private readonly byte[] _memory;

        public MMU(IMBC mbc)
        {
            _mbc = mbc;
            _memory = new byte[ushort.MaxValue + 1];
        }

        public Timer Timer;
        public IDisplay Display;
        public Cpu Cpu;
        public IJoypad Joypad;

        public byte GetByte(ushort address)
        {
            if (address < 0x8000)
            {
                return _mbc.GetByte(address);
            }

            if (address >= 0xA000 && address <= 0xBFFF)
            {
                return _mbc.GetByte(address);
            }

            switch (address)
            {
                case RegisterAddresses.P1:
                    return Joypad.P1;
                case RegisterAddresses.LCDC:
                    return Display.LCDC;
                case RegisterAddresses.LCDSTAT:
                    return Display.STAT;
                case RegisterAddresses.BGP:
                    return Display.BackgroundPaletteData;
                case RegisterAddresses.LY:
                    return Display.Line;
                case RegisterAddresses.LYC:
                    return Display.LYC;
                case RegisterAddresses.IE:
                    return Cpu.IE;
                case RegisterAddresses.IF:
                    return Cpu.IF;
                case RegisterAddresses.TIMA:
                    return Timer.TIMA;
                case RegisterAddresses.TMA:
                    return Timer.TMA;
                case RegisterAddresses.TAC:
                    return Timer.TAC;
                case RegisterAddresses.DIV:
                    return Timer.DIV;
                case RegisterAddresses.DMA:
                    return Display.DMA;
                default:
                    return _memory[address];
            }
        }

        public void SetByte(ushort address, byte value)
        {
            if (address < 0x8000)
            {
                _mbc.SetByte(address, value);
                return;
            }

            if (address >= 0xA000 && address < 0xC000)
            {
                _mbc.SetByte(address, value);
                return;
            }

            if (address >= 0xFEA0 && address < 0xFF00)
            {
                return;
            }

            switch (address)
            {
                case RegisterAddresses.P1:
                    Joypad.P1 = value;
                    break;
                case RegisterAddresses.LCDC:
                    Display.LCDC = value;
                    break;
                case RegisterAddresses.LCDSTAT:
                    Display.STAT = value;
                    break;
                case RegisterAddresses.BGP:
                    Display.BackgroundPaletteData = value;
                    break;
                case RegisterAddresses.LY:
                    break;
                case RegisterAddresses.LYC:
                    Display.LYC = value;
                    break;
                case RegisterAddresses.IE:
                    Cpu.IE = value;
                    break;
                case RegisterAddresses.IF:
                    Cpu.IF = value;
                    break;
                case RegisterAddresses.TIMA:
                    Timer.TIMA = value;
                    break;
                case RegisterAddresses.TMA:
                    Timer.TMA = value;
                    break;
                case RegisterAddresses.TAC:
                    Timer.TAC = value;
                    break;
                case RegisterAddresses.DIV:
                    Timer.DIV = value;
                    break;
                case RegisterAddresses.DMA:
                    Display.DMA = value;
                    break;
                default:
                    _memory[address] = value;
                    break;
            }
        }
    }
}