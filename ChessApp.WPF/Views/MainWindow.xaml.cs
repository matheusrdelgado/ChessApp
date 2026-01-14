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
    }
}