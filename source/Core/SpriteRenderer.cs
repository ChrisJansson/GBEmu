﻿using System.Diagnostics;

namespace Core
{
    public interface ISpriteRenderer
    {
        void Render(int line, DisplayRenderer.Tile[] tiles, Pixel[] frameBuffer);
    }

    public class SpriteRenderer : ISpriteRenderer
    {
        private readonly IMmu _mmu;

        public SpriteRenderer(IMmu mmu)
        {
            _mmu = mmu;
        }

        public void Render(int line, DisplayRenderer.Tile[] tiles, Pixel[] frameBuffer)
        {
            var lcdc = _mmu.GetByte(RegisterAddresses.LCDC);
            var spriteEnable = (lcdc & 0x02) == 0x02;
            if (!spriteEnable)
                return;

            var obp0 = ExtractShades(RegisterAddresses.OBP0);
            var obp1 = ExtractShades(RegisterAddresses.OBP1);

            for (var sprite = 0; sprite < 40; sprite++)
            {
                var spriteAddress = (ushort)(0xFE00 + sprite * 4);
                var yPos = _mmu.GetByte(spriteAddress);
                var displayY = yPos - 16;
                var spriteY = line - displayY;
                var largeSprites = (lcdc & 0x04) == 0x04;
                var spriteSize = largeSprites ? 16 : 8;
                if (spriteY >= 0 && spriteY < spriteSize)
                {
                    var xPos = _mmu.GetByte((ushort)(spriteAddress + 1));
                    var tileNumber = _mmu.GetByte((ushort)(spriteAddress + 2));
                    var attributes = _mmu.GetByte((ushort)(spriteAddress + 3));

                    //Debug.Assert((attributes & 0x80) != 0x80, "OBJ-to-BG priority is not implemented");

                    var flipY = (attributes & 0x40) == 0x40;
                    var sourceY = flipY ? (spriteSize - spriteY - 1) : spriteY;
                    var shades = (attributes & 0x10) == 0x10 ? obp1 : obp0;

                    var tile = GetTile(largeSprites, tileNumber, sourceY, tiles);
                    DrawSprite(xPos, sourceY % 8, attributes, tile, line * DisplayRenderer.WindowWidth, frameBuffer, shades);
                }
            }
        }

        private DisplayShades[] ExtractShades(ushort address)
        {
            var value = _mmu.GetByte(address);
            var shades = new[]
            {
                (DisplayShades) (value & 0x3),
                (DisplayShades) ((value >> 2) & 0x3),
                (DisplayShades) ((value >> 4) & 0x3),
                (DisplayShades) ((value >> 6) & 0x3)
            };
            return shades;
        }

        private DisplayRenderer.Tile GetTile(bool useLargeSprites, byte tileNumber, int spriteYCoord, DisplayRenderer.Tile[] tiles)
        {
            if (useLargeSprites)
            {
                var firstTile = spriteYCoord <= 7;
                if (firstTile)
                {
                    return tiles[tileNumber & 0xFE];
                }

                return tiles[tileNumber | 0x01];
            }

            return tiles[tileNumber];
        }

        private static void DrawSprite(byte xPos, int sourceY, byte attributes, DisplayRenderer.Tile tile, int framebufferOffset, Pixel[] frameBuffer, DisplayShades[] shades)
        {
            var displayXstart = xPos - 8;
            var flipX = (attributes & 0x20) == 0x20;

            for (var spriteX = 0; spriteX < 8; spriteX++)
            {
                var displayX = displayXstart + spriteX;
                if (displayX >= 0 && displayX < DisplayRenderer.WindowWidth)
                {
                    var sourceX = flipX ? (7 - spriteX) : spriteX;
                    var color = tile.Pixels[sourceX + sourceY * 8];
                    if (color > 0)
                        frameBuffer[framebufferOffset + displayX] = new Pixel(color, shades[color]);
                }
            }
        }
    }
}