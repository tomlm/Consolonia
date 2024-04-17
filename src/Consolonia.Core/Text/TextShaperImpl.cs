using System;
using Avalonia.Media.TextFormatting;
using Avalonia.Platform;

namespace Consolonia.Core.Text
{
    internal class TextShaperImpl : ITextShaperImpl
    {
        public ShapedBuffer ShapeText(ReadOnlyMemory<char> text, TextShaperOptions options)
        {
            var glyphInfos = new ShapedBuffer(text, text.Length,
                new GlyphTypefaceImpl(), 1, 0 /*todo: must be 1 for right to left?*/);
            for (int i = 0; i < glyphInfos.Length; i++)
            {
                glyphInfos[i] = new GlyphInfo(text.Span[i], 0, 1);
            }
            return glyphInfos;
        }
    }
}