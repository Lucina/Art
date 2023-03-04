using System.Text;
using EA;

namespace BarHelper;

public static class SplitContentFiller
{
    public static SplitContentFiller<TContentLeft, TContentRight> Create<TContentLeft, TContentRight>(string separator, float weightLeft, float weightRight, TContentLeft initialContentLeft, TContentRight initialContentRight)
        where TContentLeft : IContentFiller where TContentRight : IContentFiller
    {
        return new SplitContentFiller<TContentLeft, TContentRight>(separator, weightLeft, weightRight, initialContentLeft, initialContentRight);
    }
}

public struct SplitContentFiller<TContentLeft, TContentRight> : IContentFiller where TContentLeft : IContentFiller where TContentRight : IContentFiller
{
    public readonly string Separator;
    private readonly float _weightLeft;
    private readonly float _weightRight;
    public TContentLeft ContentLeft;
    public TContentRight ContentRight;
    private readonly int _separatorWidth;

    public SplitContentFiller(string separator, float weightLeft, float weightRight, TContentLeft initialContentLeft, TContentRight initialContentRight)
    {
        if (weightLeft < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(weightLeft));
        }
        if (weightRight < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(weightRight));
        }
        Separator = separator;
        _weightLeft = weightLeft;
        _weightRight = weightRight;
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
        float totalWeight = _weightLeft + _weightRight;
        int widthLeft;
        int widthRight;
        if (totalWeight <= float.Epsilon)
        {
            widthLeft = remainingWidth >> 1;
            widthRight = remainingWidth - widthLeft;
        }
        else
        {
            if (_weightLeft <= float.Epsilon)
            {
                widthLeft = 0;
                widthRight = remainingWidth;
            }
            else
            {
                if (_weightRight <= float.Epsilon)
                {
                    widthRight = 0;
                }
                else
                {
                    widthRight = Math.Clamp((int)Math.Round((double)_weightRight * remainingWidth / totalWeight), 0, remainingWidth);
                }
                widthLeft = remainingWidth - widthRight;
            }
        }
        ContentLeft.Fill(stringBuilder, widthLeft);
        stringBuilder.Append(Separator);
        ContentRight.Fill(stringBuilder, widthRight);
    }
}
