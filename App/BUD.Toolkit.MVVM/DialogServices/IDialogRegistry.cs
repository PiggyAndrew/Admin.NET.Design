using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DSD.Toolkit.MVVM.DialogServices;

public interface IDialogRegistry
{
    public void Register<T, TViewModel>(string title) where T : Window;
}
