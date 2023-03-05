using System.Text;

namespace BarHelper;

public class EllipsisSuffixContentFiller : IContentFiller
{
    public static EllipsisSuffixContentFiller Create(string message, int initialI)
    {
        return new EllipsisSuffixContentFiller(message, initialI);
    }

    public readonly string Message;
    private readonly string[] _messages;
    private int _i;

    public EllipsisSuffixContentFiller(string message, int initialI)
    {
        Message = message;
        _messages = new[] { $"{message}.", $"{message}..", $"{message}..." };
        _i = initialI;
    }

    public void Fill(StringBuilder stringBuilder, int width)
    {
        StringFillUtil.FillLeft(_messages[_i], stringBuilder, width);
        _i = (_i + 1) % 3;
    }
}
