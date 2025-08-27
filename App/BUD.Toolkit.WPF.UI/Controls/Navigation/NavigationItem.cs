using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DSD.Toolkit.WPF.UI;

public class NavigationItem
{
    public NavigationItem(string title, IconType iconType, string path)
    {
        Title = title;
        Icon = iconType;
        Path = path;
    }

    public string Title { get; set; } = default!;

    public IconType Icon { get; set; }

    public string Path { get; set; } = default!;
}
