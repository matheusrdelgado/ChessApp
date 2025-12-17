using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChessApp.WPF.ViewModel
{
    public class RelayCommand : ICommand //ligar os botoes do wpf ao viewmodel
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value;}
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter); //verifica se o comando pode ser executado

        public void Execute(object parameter) => _execute(parameter);
    }
}
