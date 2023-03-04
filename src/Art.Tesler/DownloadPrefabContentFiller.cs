using System.Text;
using BarHelper;

namespace Art.Tesler;

public struct DownloadPrefabContentFiller : IContentFiller
{
    public static DownloadPrefabContentFiller Create(string initialName)
    {
        return new DownloadPrefabContentFiller(initialName);
    }

    public BorderContentFiller<SplitContentFiller<StringContentFiller, FixedSplitContentFiller<ColorContentFiller<ProgressContentFiller>, StringContentFiller>>> Content;

    public DownloadPrefabContentFiller(string initialName)
    {
        Content = BorderContentFiller.Create("[", "]",
            SplitContentFiller.Create("|", 0.25f, 0.75f,
                StringContentFiller.Create(initialName, ContentAlignment.Left),
                FixedSplitContentFiller.Create("|", 6, 1,
                    ColorContentFiller.Create(ConsoleColor.Green, ProgressContentFiller.Create()),
                    StringContentFiller.Create("0.0%", ContentAlignment.Right))));
    }

    public void SetName(string name)
    {
        Content.Content.ContentLeft = new StringContentFiller(name, ContentAlignment.Left);
    }

    public void SetProgress(float progress)
    {
        progress = Math.Clamp(progress, 0.0f, 1.0f);
        Content.Content.ContentRight.ContentLeft.Content.Progress = progress;
        Content.Content.ContentRight.ContentRight = new StringContentFiller($"{100.0f * progress:F1}%", ContentAlignment.Right);
    }

    public void Fill(StringBuilder stringBuilder, int width)
    {
        Content.Fill(stringBuilder, width);
    }
}
