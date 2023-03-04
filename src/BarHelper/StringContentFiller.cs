using System.Text;

namespace BarHelper;

public readonly struct StringContentFiller : IContentFiller
{
    public static StringContentFiller Create(string content, ContentAlignment alignment)
    {
        return new StringContentFiller(content, alignment);
    }

    public readonly string Content;
    public readonly ContentAlignment Alignment;

    public StringContentFiller(string content, ContentAlignment alignment)
    {
        Content = content;
        Alignment = alignment;
    }

    public void Fill(StringBuilder stringBuilder, int width)
    {
        switch (Alignment)
        {
            case ContentAlignment.Left:
                StringFillUtil.FillLeft(Content, stringBuilder, width);
                break;
            case ContentAlignment.Right:
                StringFillUtil.FillRight(Content, stringBuilder, width);
                break;
            default:
                StringFillUtil.PadRemaining(stringBuilder, width);
                break;
        }
    }
}
