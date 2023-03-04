using System.Text;

namespace BarHelper;

public interface IContentFiller
{
    void Fill(StringBuilder stringBuilder, int width);
}
