using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace DSD.Toolkit.MVVM.DialogServices;

public class DialogService : IDialogService, IDialogRegistry
{
    private readonly Dictionary<string, ViewDefinition> _definitions = new Dictionary<string, ViewDefinition>();

    public void Register<T, TViewModel>(string title) where T : Window
    {
        if (!_definitions.ContainsKey(title))
        {
            _definitions.Add(title, new ViewDefinition()
            {
                View = typeof(T),
                ViewModel = typeof(TViewModel),
            });
        }
    }

    private Window GetViewInstance(string title)
    {
        if (!_definitions.TryGetValue(title, out ViewDefinition viewModel))
        {
            viewModel.ViewInstance = Activator.CreateInstance(viewModel.View) as Window;
        }

        Activator.CreateInstance(viewModel.ViewModel);



        return viewModel.ViewInstance;
    }

    public void Show(string title)
    {

    }

    public DialogResult ShowDialog(string title)
    {
        Window window = GetViewInstance(title);
        DialogStatus dialogStatus = DialogStatus.OK;
        if (window.ShowDialog() == false)
        {
            dialogStatus = DialogStatus.CANCEL;
        }

        return new DialogResult()
        {
            DialogStatus = dialogStatus,
        };
    }

    private struct ViewDefinition
    {
        public Type View { get; set; }

        public Type ViewModel { get; set; }

        public Window ViewInstance { get; set; }
    }
}
