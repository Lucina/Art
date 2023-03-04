using System.Text;

namespace BarHelper;

public struct ProgressContentFiller : IContentFiller
{
    public static ProgressContentFiller Create(float initialProgress = 0)
    {
        return new ProgressContentFiller(initialProgress);
    }

    public float Progress;

    public ProgressContentFiller(float initialProgress = 0)
    {
        Progress = initialProgress;
    }

    private static ReadOnlySpan<char> Bits => new[] { ' ', '·', '+', '#', '█' };

    public void Fill(StringBuilder stringBuilder, int width)
    {
        if (width < 1)
        {
            return;
        }
        float progressValue = width * Math.Clamp(Progress, 0.0f, 1.0f);
        int item = Math.Clamp((int)progressValue, 0, width);
        for (int i = 0; i < item; i++)
        {
            stringBuilder.Append(Bits[4]);
        }
        if (item == width)
        {
            return;
        }
        int itemCompletion = Math.Clamp((int)Math.Round(4.0f * (progressValue - item)), 0, 4);
        stringBuilder.Append(Bits[itemCompletion]);
        for (int i = item + 1; i < width; i++)
        {
            stringBuilder.Append(' ');
        }
    }
}
