using System.Text;

namespace BarHelper;

public class EllipsisSuffixContentFiller : IContentFiller
{
    public static EllipsisSuffixContentFiller Create(string message, int initialI)
    {
        return new EllipsisSuffixContentFiller(message, initialI);
    }

    public readonly string Message;
    private readonly StringBuilder _content;
    private int _i;

    public EllipsisSuffixContentFiller(string message, int initialI)
    {
        Message = message;
        _content = new StringBuilder(Message);
        _i = initialI;
    }

    public void Fill(StringBuilder stringBuilder, int width)
    {
        _content.Length = Message.Length;
        _content.Append('.', _i + 1);
        _i = (_i + 1) % 3;
        StringFillUtil.FillLeft(_content.ToString(), stringBuilder, width);
    }
}
