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

        /// <summary>
        /// Construtor do RelayCommand
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Evento que notifica quando a capacidade de execução do comando muda, ou seja, quando CanExecute deve ser reavaliado.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value;}
        }

        /// <summary>
        /// Verifica se o comando pode ser executado
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter); //verifica se o comando pode ser executado

        /// <summary>
        /// Executa o comando
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter) => _execute(parameter);
    }
}
