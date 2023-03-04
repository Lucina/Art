using System.Runtime.InteropServices;
using System.Text;
using EA;

namespace BarHelper;

internal static class StringFillUtil
{
    public static void FillLeft(string content, StringBuilder stringBuilder, int width)
    {
        if (width < 1)
        {
            return;
        }
        using var enumerator = content.EnumerateRunes();
        if (!enumerator.MoveNext())
        {
            return;
        }
        var prevRune = enumerator.Current;
        Span<char> buf = stackalloc char[2];
        while (enumerator.MoveNext())
        {
            int prevRuneWidth = EastAsianWidth.GetWidth(prevRune.Value);
            if (prevRuneWidth + 1 > width)
            {
                stringBuilder.Append('…');
                PadRemaining(stringBuilder, width - 1);
                return;
            }
            prevRune.EncodeToUtf16(buf);
            stringBuilder.Append(buf);
            buf.Clear();
            width -= prevRuneWidth;
            prevRune = enumerator.Current;
        }
        int lastRuneWidth = EastAsianWidth.GetWidth(prevRune.Value);
        if (lastRuneWidth > width)
        {
            stringBuilder.Append('…');
            PadRemaining(stringBuilder, width - 1);
            return;
        }
        prevRune.EncodeToUtf16(buf);
        stringBuilder.Append(buf);
        PadRemaining(stringBuilder, width - lastRuneWidth);
    }

    public static void FillRight(string content, StringBuilder stringBuilder, int width)
    {
        if (width < 1)
        {
            return;
        }
        const int spanCount = 256;
        if (content.Length <= spanCount)
        {
            // it is impossible to drive without a license!
            // runes each take at least one utf16 code unit, so using length as max theoretical works
            Span<RuneInfo> runes = stackalloc RuneInfo[spanCount];
            int i = 0;
            foreach (var rune in content.EnumerateRunes())
            {
                runes[i++] = new RuneInfo(rune);
            }
            FillRight(runes[..i], stringBuilder, width);
        }
        else
        {
            List<RuneInfo> list = new(spanCount * 2);
            foreach (var rune in content.EnumerateRunes())
            {
                list.Add(new RuneInfo(rune));
            }
            FillRight(CollectionsMarshal.AsSpan(list), stringBuilder, width);
        }
    }

    private static void FillRight(Span<RuneInfo> span, StringBuilder stringBuilder, int width)
    {
        if (width < 1)
        {
            return;
        }
        if (span.Length == 0)
        {
            PadRemaining(stringBuilder, width);
            return;
        }
        int availableWidth = width;
        int startPos;
        for (startPos = span.Length - 1; startPos >= 1; startPos--)
        {
            ref var info = ref span[startPos];
            int runeWidth = EastAsianWidth.GetWidth(info.Rune.Value);
            info.Width = runeWidth;
            if (availableWidth < runeWidth + 1)
            {
                break;
            }
            availableWidth -= runeWidth;
        }
        if (startPos == 0)
        {
            ref var firstInfo = ref span[0];
            int firstRuneWidth = EastAsianWidth.GetWidth(firstInfo.Rune.Value);
            firstInfo.Width = firstRuneWidth;
            if (firstRuneWidth > availableWidth)
            {
                startPos++;
                stringBuilder.Append('…');
                PadRemaining(stringBuilder, availableWidth - 1);
            }
            else
            {
                availableWidth -= firstRuneWidth;
                PadRemaining(stringBuilder, availableWidth);
            }
        }
        else
        {
            startPos++;
            if (availableWidth > 0)
            {
                stringBuilder.Append('…');
                PadRemaining(stringBuilder, availableWidth - 1);
            }
        }
        Span<char> buf = stackalloc char[2];
        for (int i = startPos; i < span.Length; i++)
        {
            span[i].Rune.EncodeToUtf16(buf);
            stringBuilder.Append(buf);
            buf.Clear();
        }
    }

    private struct RuneInfo
    {
        public Rune Rune;
        public int Width;

        public RuneInfo(Rune rune)
        {
            Rune = rune;
            Width = 0;
        }
    }

    internal static void PadRemaining(StringBuilder stringBuilder, int remaining)
    {
        stringBuilder.Insert(stringBuilder.Length, " ", remaining);
    }
}
