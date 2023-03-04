using System.Text;
using EA;

namespace BarHelper;

public static class FixedSplitContentFiller
{
    public static FixedSplitContentFiller<TContentLeft, TContentRight> Create<TContentLeft, TContentRight>(string separator, int fixedWidth, int fixedWidthIndex, TContentLeft initialContentLeft, TContentRight initialContentRight)
        where TContentLeft : IContentFiller where TContentRight : IContentFiller
    {
        return new FixedSplitContentFiller<TContentLeft, TContentRight>(separator, fixedWidth, fixedWidthIndex, initialContentLeft, initialContentRight);
    }
}

public struct FixedSplitContentFiller<TContentLeft, TContentRight> : IContentFiller where TContentLeft : IContentFiller where TContentRight : IContentFiller
{
    public readonly string Separator;
    private readonly int _fixedWidth;
    private readonly int _fixedWidthIndex;
    public TContentLeft ContentLeft;
    public TContentRight ContentRight;
    private readonly int _separatorWidth;

    public FixedSplitContentFiller(string separator, int fixedWidth, int fixedWidthIndex, TContentLeft initialContentLeft, TContentRight initialContentRight)
    {
        if (fixedWidth < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fixedWidth));
        }
        if (fixedWidthIndex < 0 || fixedWidthIndex > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(fixedWidthIndex));
        }
        Separator = separator;
        _fixedWidth = fixedWidth;
        _fixedWidthIndex = fixedWidthIndex;
        ContentLeft = initialContentLeft;
        ContentRight = initialContentRight;
        _separatorWidth = EastAsianWidth.GetWidth(Separator);
    }

    public void Fill(StringBuilder stringBuilder, int width)
    {
        if (width < 1)
        {
            return;
        }
        if (width <= _separatorWidth)
        {
            StringFillUtil.PadRemaining(stringBuilder, width);
            return;
        }
        int remainingWidth = width - _separatorWidth;
        if (remainingWidth <= _fixedWidth)
        {
            switch (_fixedWidthIndex)
            {
                case 0:
                    ContentLeft.Fill(stringBuilder, width);
                    break;
                case 1:
                    ContentRight.Fill(stringBuilder, width);
                    break;
                default:
                    StringFillUtil.PadRemaining(stringBuilder, width);
                    break;
            }
            return;
        }
        switch (_fixedWidthIndex)
        {
            case 0:
                ContentLeft.Fill(stringBuilder, _fixedWidth);
                stringBuilder.Append(Separator);
                ContentRight.Fill(stringBuilder, remainingWidth - _fixedWidth);
                break;
            case 1:
                ContentLeft.Fill(stringBuilder, remainingWidth - _fixedWidth);
                stringBuilder.Append(Separator);
                ContentRight.Fill(stringBuilder, _fixedWidth);
                break;
            default:
                StringFillUtil.PadRemaining(stringBuilder, width);
                break;
        }
    }
}
