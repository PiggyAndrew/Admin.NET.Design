using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DSD.Toolkit.MVVM;

public interface IRelayCommand : ICommand
{
    void RaiseCanExecuteChanged();
}
