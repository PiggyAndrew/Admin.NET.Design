using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DSD.Toolkit.MVVM;

/// <summary>
/// A command implementation that relays its functionality to other objects by invoking delegates.
/// Provides a way to bind commands between the ViewModel and the View in MVVM pattern.
/// </summary>
public class RelayCommand : IRelayCommand
{
    /// <summary>
    /// Event that is fired when the ability to execute the command changes.
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// The action to execute when the command is invoked.
    /// </summary>
    private readonly Action? _execute;

    /// <summary>
    /// The predicate that determines if the command can be executed.
    /// </summary>
    private readonly Func<bool>? _canExecute;

    /// <summary>
    /// Initializes a new instance of RelayCommand that can always execute.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    public RelayCommand(Action execute)
    {
        _execute = execute;
    }

    /// <summary>
    /// Initializes a new instance of RelayCommand with both execute and canExecute delegates.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    public RelayCommand(Action execute, Func<bool> canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    /// <summary>
    /// Raises the CanExecuteChanged event to indicate the command's availability has changed.
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Determines whether the command can be executed in its current state.
    /// </summary>
    /// <param name="parameter">Data used by the command (not used in this implementation).</param>
    /// <returns>True if the command can be executed; otherwise, false.</returns>
    public bool CanExecute(object parameter)
    {
        if (_canExecute == null)
        {
            return true;
        }

        return _canExecute.Invoke() != false;
    }

    /// <summary>
    /// Executes the command's logic.
    /// </summary>
    /// <param name="parameter">Data used by the command (not used in this implementation).</param>
    public void Execute(object parameter)
    {
        _execute?.Invoke();
    }
}
