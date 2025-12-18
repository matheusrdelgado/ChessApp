using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ChessApp.Model.Services;
using ChessApp.Model.Model;

namespace ChessApp.WPF.Views
{
    public partial class LoginWindow : Window
    {
        private readonly UserService _userService;

        public User LoggedUser { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
            _userService = new UserService();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUser.Text) || string.IsNullOrWhiteSpace(txtPass.Password))
            {
                MessageBox.Show("Missing username or password.");
                return;
            }

            var user = _userService.Login(txtUser.Text, txtPass.Password);

            if (user != null)
            {
                LoggedUser = user;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Incorrect username or password.");
            }
        }

        private void BtnOpenRegister_Click(object sender, RoutedEventArgs e)
        {
            var registerWin = new RegisterWindow();
            registerWin.ShowDialog();
        }
    }
}
