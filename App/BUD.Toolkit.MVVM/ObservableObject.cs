using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Toolkit.MVVM;

public class ObservableObject : INotifyPropertyChanging, INotifyPropertyChanged
{
    public event PropertyChangingEventHandler? PropertyChanging;
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanging(string propertyName)
    {
        PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Set property 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    /// <param name="propertyName"></param>
    protected void SetProperty<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
        {
            return;
        }

        OnPropertyChanging(propertyName);

        oldValue = newValue;

        OnPropertyChanged(propertyName);
    }
}
