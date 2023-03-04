using System.Text;

namespace BarHelper;

public static class ColorContentFiller
{
    internal const string Reset = "\u001b[0m";

    internal static readonly Dictionary<ConsoleColor, string> s_sequences = new()
    {
        /*{Color.Black, "\u001b[30;1m"},*/
        { ConsoleColor.Red, "\u001b[31;1m" },
        { ConsoleColor.Green, "\u001b[32;1m" },
        { ConsoleColor.Yellow, "\u001b[33;1m" },
        { ConsoleColor.Blue, "\u001b[34;1m" },
        { ConsoleColor.Magenta, "\u001b[35;1m" },
        { ConsoleColor.Cyan, "\u001b[36;1m" },
        { ConsoleColor.White, "\u001b[37;1m" },
    };

    public static ColorContentFiller<TContent> Create<TContent>(ConsoleColor initialColor, TContent initialContent)
        where TContent : IContentFiller
    {
        return new ColorContentFiller<TContent>(initialColor, initialContent);
    }
}

public struct ColorContentFiller<TContent> : IContentFiller where TContent : IContentFiller
{
    public ConsoleColor Color;
    public TContent Content;

    public ColorContentFiller(ConsoleColor initialColor, TContent initialContent)
    {
        Color = initialColor;
        Content = initialContent;
    }


    public void Fill(StringBuilder stringBuilder, int width)
    {
        if (width < 1)
        {
            return;
        }
        if (ColorContentFiller.s_sequences.TryGetValue(Color, out string? sequence))
        {
            stringBuilder.Append(sequence);
        }
        Content.Fill(stringBuilder, width);
        stringBuilder.Append(ColorContentFiller.Reset);
    }
}
