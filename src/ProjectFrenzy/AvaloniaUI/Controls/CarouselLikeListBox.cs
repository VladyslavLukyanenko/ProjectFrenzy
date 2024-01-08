using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace ProjectFrenzy.AvaloniaUI.Controls
{
    public class CarouselLikeListBox
        : ListBox
    {
        static CarouselLikeListBox()
        {
            ItemsPanelProperty.OverrideDefaultValue<CarouselLikeListBox>(new FuncTemplate<IPanel>(() => new VirtualizingStackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal,
                Spacing = 40
            }));
        }
    }
}
