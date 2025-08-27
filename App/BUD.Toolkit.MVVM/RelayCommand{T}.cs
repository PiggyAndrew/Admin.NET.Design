using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DSD.Toolkit.MVVM;

public class RelayCommand<T> : ICommand
{
    private readonly Action<T?> _execute;

    private readonly Func<T?, bool>? _canExecute;

    public event EventHandler? CanExecuteChanged;

    public RelayCommand(Action<T?> execute)
    {
        if (execute == null)
        {
            throw new ArgumentNullException();
        }

        _execute = execute;
    }

    public RelayCommand(Action<T?> execute, Func<T?, bool> canExecute)
    {
        if (execute == null)
        {
            throw new ArgumentNullException();
        }

        if (canExecute == null)
        {
            throw new ArgumentNullException();
        }

        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        if (parameter is null && default(T) is not null)
        {
            return false;
        }

        if (!CastType(parameter, out T? result))
        {
            return false;
        }

        return _canExecute?.Invoke(result) != false;
    }

    public void RaiseCanExecuteChanged()
    {
        this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Execute(object parameter)
    {
        if (!CastType(parameter, out T? result))
        {
            return;
        }

        if (result == null)
        {
            return;
        }

        _execute.Invoke(result);
    }

    private bool CastType(object? parameter, out T? result)
    {
        if (parameter is null && default(T) is null)
        {
            result = default;
            return true;
        }

        if (parameter is T tValue)
        {
            result = tValue;
            return true;
        }

        result = default;
        return false;
    }
}
