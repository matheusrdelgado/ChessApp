using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChessApp.WPF
{
    /// <summary>
    /// logica de interacao para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window //partial class porque o XAML gera outra parte da classe
    {
        /// <summary>
        /// Inicializa uma nova instancia da classe <see cref="MainWindow"/>.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Manipulador de evento para o botao de sair.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Manipulador de evento para o movimento do mouse sobre uma casa do tabuleiro.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Square_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is FrameworkElement element)
            {
                var square = element.Tag as ViewModel.SquareViewModel;
                if (square != null && square.PieceImage != null)
                {
                    if (square.ClickCommand != null && square.ClickCommand.CanExecute(null))
                    {
                        square.ClickCommand.Execute(null);
                    }

                    DragDrop.DoDragDrop(element, square, DragDropEffects.Move);
                }
            }
        }

        /// <summary>
        /// Manipulador de evento para o movimento do mouse sobre uma casa do tabuleiro.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Square_Drop(object sender, DragEventArgs e)
        {
            if (sender is FrameworkElement targetElement && e.Data.GetDataPresent(typeof(ViewModel.SquareViewModel)))
            {
                var sourceSquare = e.Data.GetData(typeof(ViewModel.SquareViewModel)) as ViewModel.SquareViewModel;
                var targetSquare = targetElement.Tag as ViewModel.SquareViewModel;

                var vm = DataContext as ViewModel.GameViewModel;
                if (vm != null && sourceSquare != null && targetSquare != null)
                {
                    vm.ProcessDragDrop(sourceSquare, targetSquare);
                }
            }
        }
    }
}