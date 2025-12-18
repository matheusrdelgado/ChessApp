using ChessApp.Model.Services;
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

namespace ChessApp.WPF.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly UserService _userService;

        public RegisterWindow()
        {
            InitializeComponent();
            _userService = new UserService();
        }

        private void BtnConfirmRegister_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRegUser.Text) || string.IsNullOrWhiteSpace(txtRegPass.Password))
            {
                MessageBox.Show("Missing username or password.");
                return;
            }

            if (txtRegPass.Password != txtRegConfirm.Password)
            {
                MessageBox.Show("Passwords must match!");
                return;
            }

            if (_userService.Register(txtRegUser.Text, txtRegPass.Password))
            {
                MessageBox.Show("Successfully register! Log in.");
                _userService.SaveUsers();
                Close();
            }
            else
            {
                MessageBox.Show("This user name already exists.");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
