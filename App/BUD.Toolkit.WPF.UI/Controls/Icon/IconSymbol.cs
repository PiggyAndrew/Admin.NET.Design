using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace DSD.Toolkit.WPF.UI;

public class IconSymbol : Control
{
    static IconSymbol()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(IconSymbol), new FrameworkPropertyMetadata(typeof(IconSymbol)));
        IconTypeProperty = DependencyProperty.Register(nameof(IconType), typeof(IconType), typeof(IconSymbol), new PropertyMetadata(IconType.None, OnIconTypeChanged));
    }

    public static readonly DependencyProperty IconTypeProperty;

    public IconType IconType
    {
        get => (IconType)GetValue(IconTypeProperty);
        set => SetValue(IconTypeProperty, value);
    }

    private static void OnIconTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is IconSymbol icon && e.NewValue is IconType iconType)
        {
            var field = typeof(IconType).GetField(iconType.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            icon.Icon = attribute?.Description ?? string.Empty;
        }
    }

    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(string), typeof(IconSymbol), new PropertyMetadata(string.Empty));

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        private set => SetValue(IconProperty, value);
    }
}
