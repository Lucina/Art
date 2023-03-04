using System.Text;
using EA;

namespace BarHelper;

public static class BorderContentFiller
{
    public static BorderContentFiller<TContent> Create<TContent>(string left, string right, TContent initialContent)
        where TContent : IContentFiller
    {
        return new BorderContentFiller<TContent>(left, right, initialContent);
    }
}

public struct BorderContentFiller<TContent> : IContentFiller where TContent : IContentFiller
{
    public readonly string Left;
    public readonly string Right;
    private readonly int _borderWidth;
    public TContent Content;

    public BorderContentFiller(string left, string right, TContent initialContent)
    {
        Left = left;
        Right = right;
        _borderWidth = EastAsianWidth.GetWidth(Left) + EastAsianWidth.GetWidth(Right);
        Content = initialContent;
    }


    public void Fill(StringBuilder stringBuilder, int width)
    {
        if (width < 1)
        {
            return;
        }
        if (_borderWidth > width)
        {
            StringFillUtil.PadRemaining(stringBuilder, width);
            return;
        }
        stringBuilder.Append(Left);
        Content.Fill(stringBuilder, width - _borderWidth);
        stringBuilder.Append(Right);
    }
}
