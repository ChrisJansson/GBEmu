﻿using System;

namespace Core
{
    public class MBC1 : IMBC
    {
        private readonly CartridgeHeader.RamSizeEnum _ramSize;
        private readonly byte[] _rom;
        private readonly byte[] _ram;
        private byte _lowRomSelect;
        private byte _highRomSelect;
        private byte _ramEnable;
        private byte _ramSelect;
        private byte _romRamModeSelect;

        public MBC1(byte[] rom, CartridgeHeader.RomSizeEnum romSize, CartridgeHeader.RamSizeEnum ramSize)
        {
            _ramSize = ramSize;
            _ram = new byte[ramSize.ToKB()];
            AssertRomSize(rom, romSize);

            _rom = rom;
        }

        public byte GetByte(ushort address)
        {
            if (address < 0x4000)
            {
                return _rom[address];
            }

            const int romBankSize = 0x4000;
            if (address >= 0x4000 && address < 0x8000)
            {
                var lowerSelect = (_lowRomSelect & 0x1F);
                var upperSelect = _romRamModeSelect == 0 ? (_highRomSelect & 0x03) : 0;
                var bank = (upperSelect << 5) | lowerSelect;
                if (bank == 0 || bank == 0x20 || bank == 0x40 || bank == 0x60)
                    bank++;
                var offset = romBankSize * bank;
                var baseAddress = address - 0x4000;
                return _rom[baseAddress + offset];
            }

            if (address >= 0xA000 && address < 0xC000)
            {
                if (((_ramEnable) != 0x0A) || _ramSize == CartridgeHeader.RamSizeEnum.None)
                    return 0;

                var ramBank = _romRamModeSelect == 0 ? 0 : _ramSelect;
                var ramAddress = (address - 0xA000) + ramBank * 0x2000;
                return _ram[ramAddress];
            }

            return _rom[address];
        }

        public void SetByte(ushort address, byte value)
        {
            if (address < 0x2000)
            {
                _ramEnable = (byte)(value & 0x0A);
            }

            if (address >= 0x2000 && address < 0x4000)
                _lowRomSelect = value;
            if (address >= 0x4000 && address < 0x6000)
            {
                _highRomSelect = value;
                _ramSelect = value;
            }
            if (address >= 0x6000 && address < 0x8000)
                _romRamModeSelect = (byte)(value & 0x01);

            if (address >= 0xA000 && address < 0xC000)
            {
                if (_ramEnable != 0x0A || _ramSize == CartridgeHeader.RamSizeEnum.None)
                {
                    return;
                }

                var ramBank = _romRamModeSelect == 0 ? 0 : _ramSelect;
                var ramAddress = (address - 0xA000) + 0x2000 * ramBank;
                _ram[ramAddress] = value;
            }
        }

        private static void AssertRomSize(byte[] rom, CartridgeHeader.RomSizeEnum romSize)
        {
            if (romSize == CartridgeHeader.RomSizeEnum._32KB && rom.Length != 32.KB())
                throw new InvalidOperationException();
            if (romSize == CartridgeHeader.RomSizeEnum._64KB && rom.Length != 64.KB())
                throw new InvalidOperationException();
            if (romSize == CartridgeHeader.RomSizeEnum._128KB && rom.Length != 128.KB())
                throw new InvalidOperationException();
            if (romSize == CartridgeHeader.RomSizeEnum._256KB && rom.Length != 256.KB())
                throw new InvalidOperationException();
            if (romSize == CartridgeHeader.RomSizeEnum._512KB && rom.Length != 512.KB())
                throw new InvalidOperationException();
            if (romSize == CartridgeHeader.RomSizeEnum._1MB && rom.Length != 1024.KB())
                throw new InvalidOperationException();
            if (romSize == CartridgeHeader.RomSizeEnum._2MB && rom.Length != 2048.KB())
                throw new InvalidOperationException();
            if (romSize == CartridgeHeader.RomSizeEnum._1_1MB || romSize == CartridgeHeader.RomSizeEnum._1_2MB || romSize == CartridgeHeader.RomSizeEnum._1_5MB || romSize == CartridgeHeader.RomSizeEnum._4MB)
                throw new InvalidOperationException();
        }
    }
}