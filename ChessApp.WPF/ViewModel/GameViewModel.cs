using ChessApp.Model.Enums;
using ChessApp.Model.Model;
using ChessApp.Model.Services;
using ChessApp.WPF.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel; //para ObservableCollection que avisa o WPF se adicionar ou remover quadrados
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace ChessApp.WPF.ViewModel
{
    public class GameViewModel : BaseViewModel
    {
        //services
        private readonly UserService _userService;
        private readonly GameFileService _gameFileService;
        
        //game proprierties
        public Game Game { get; private set; }
        public ObservableCollection<SquareViewModel> BoardSquares { get; set; }

        private SquareViewModel _selectedSquare;

        //Game State
        private bool _isGameRunning;
        public bool IsGameRunning
        {
            get { return _isGameRunning; }
            set 
            { 
                _isGameRunning = value;
                OnPropertyChanged();
            }
        }
        //Username properties
        private string _inputUsername;
        public string InputUsername
        {
            get { return _inputUsername; }
            set { _inputUsername = value; OnPropertyChanged(); }
        }
        private User _currentUser;
        public User CurrentUser
        {
            get { return _currentUser; }
            private set
            {
                _currentUser = value;
                OnPropertyChanged();
                UpdateVisibilities(); 
            }
        }
        //Menu or game window
        private Visibility _menuVisibility = Visibility.Visible;
        public Visibility MenuVisibility { get { return _menuVisibility; } set { _menuVisibility = value; OnPropertyChanged(); } }

        private Visibility _gameVisibility = Visibility.Collapsed;
        public Visibility GameVisibility { get { return _gameVisibility; } set { _gameVisibility = value; OnPropertyChanged(); } }

        //login / logout button
        private Visibility _loginButtonVisibility = Visibility.Visible;
        public Visibility LoginButtonVisibility { get { return _loginButtonVisibility; } set { _loginButtonVisibility = value; OnPropertyChanged(); } }

        private Visibility _userAreaVisibility = Visibility.Collapsed;
        public Visibility UserAreaVisibility { get { return _userAreaVisibility; } set { _userAreaVisibility = value; OnPropertyChanged(); } }

        public ICommand NewGameCommand { get; set; }
        public ICommand LoginCommand { get; set; }
        public ICommand RegisterCommand { get; set; }
        public ICommand SaveGameCommand { get; set; }
        public ICommand OpenLoginCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        public ICommand OpenHistoryCommand { get; set; }
        public ICommand GiveUpCommand { get; set; }
        public ICommand ResignCommand { get; set; }

        public GameViewModel()
        {
            //Initialize Services
            _userService = new UserService();
            _gameFileService = new GameFileService();

            //  Initialize Board
            Game = new Game();
            BoardSquares = new ObservableCollection<SquareViewModel>();
            InitializeBoardVisuals();
            RefreshBoard();
            IsGameRunning = false;

            NewGameCommand = new RelayCommand(param => StartNewGame()); // => lambda function
            SaveGameCommand = new RelayCommand(param => SaveCurrentGame(), param => IsGameRunning);
            LoginCommand = new RelayCommand(p => PerformLogin(p));
            RegisterCommand = new RelayCommand(p => PerformRegister(p));


            OpenLoginCommand = new RelayCommand(p =>
            {
                var loginWin = new LoginWindow();
                if (loginWin.ShowDialog() == true)
                {
                    CurrentUser = loginWin.LoggedUser; // Gets logged user
                }
            });
            LogoutCommand = new RelayCommand(p => CurrentUser = null);

            OpenHistoryCommand = new RelayCommand(p =>
            {
                if (CurrentUser != null) new HistoryWindow(CurrentUser.Username).ShowDialog();
            });

            ResignCommand = new RelayCommand(p => Resign());
        }

        private void PerformLogin(object parameter)
        {
            //gets parameter so password box is able to read it
            var passwordBox = parameter as PasswordBox;
            string password = passwordBox?.Password;

            if (string.IsNullOrWhiteSpace(InputUsername) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Missing username or password.");
                return;
            }

            var user = _userService.Login(InputUsername, password);
            if (user != null)
            {
                CurrentUser = user;
                MessageBox.Show($"Welcome back {user.Username}!");
                // clean passwordbox
                passwordBox.Password = "";
            }
            else
            {
                MessageBox.Show("Incorrect user or password.");
            }
        }

        private void PerformRegister(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            string password = passwordBox?.Password;

            if (string.IsNullOrWhiteSpace(InputUsername) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Missing username or password.");
                return;
            }

            // try to register
            bool success = _userService.Register(InputUsername, password);

            if (success)
            {
                MessageBox.Show("Succesfuly created account, now log in.");
                _userService.SaveUsers(); // save user
            }
            else
            {
                MessageBox.Show("User already exists.");
            }
        }

        private void StartNewGame()
        {
            Game = new Game();
            ResetAllSquares();
            RefreshBoard();
            IsGameRunning = true;

            MenuVisibility = Visibility.Collapsed;
            GameVisibility = Visibility.Visible;
        }

        private void PerformLogin(string user, string pass)
        {
            var loggedUser = _userService.Login(user, pass);
            if(loggedUser != null)
            {
                CurrentUser = loggedUser;
                MessageBox.Show($"Welcome, {CurrentUser.Username}!");
            }
            else
            {
                _userService.Register(user, pass);
                CurrentUser = _userService.Login(user, pass);
                MessageBox.Show($"Account created {user}!");
            }
        }

        private void SaveCurrentGame()
        {
            if (Game.MoveHistory.Count > 0)
            {
                string filename = $"Game_{DateTime.Now:ddMMyyyy_HHmmss}";
                _gameFileService.SaveGame(Game, filename);
                MessageBox.Show("Game saved!");
            }
        }

        private void InitializeBoardVisuals()
        {
            var lightColor = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFrom("#dae9f4");
            var darkColor = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFrom("#94b2c6");

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    var square = new SquareViewModel(new Position(row, col));

                    bool isLight = (row + col) % 2 == 0;
                    square.BackgroundColor = isLight ? lightColor : darkColor;

                    square.ClickCommand = new RelayCommand(param => OnSquareClicked(square));
                    BoardSquares.Add(square);
                }
            }
        }

        private void OnSquareClicked(SquareViewModel clickedSquare)
        {
            if (!IsGameRunning)
            {
                MessageBox.Show("Select New Game to play!");
                return;
            }
            var piece = Game.Board.GetPiece(clickedSquare.Position);
            if (_selectedSquare == null)
            {
                ResetAllSquares();
                if (piece != null && piece.Color == Game.CurrentTurn)
                {
                    _selectedSquare = clickedSquare;
                    _selectedSquare.Highlight();

                    foreach (var pos in piece.GetValidMoves(Game.Board))
                    {
                        var square = BoardSquares.FirstOrDefault(s => s.Position.Row == pos.Row && s.Position.Column == pos.Column);
                        square?.HighlightPossibleMove();
                    }
                    return;
                }
            }
            else
            {

                if (_selectedSquare == clickedSquare)
                {
                    ResetAllSquares();
                    _selectedSquare = null;
                    return;
                }

                try
                {
                    Game.MakeMove(_selectedSquare.Position, clickedSquare.Position);

                    ResetAllSquares();
                    RefreshBoard();

                    _selectedSquare = null;

                    CheckGameOver();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Invalid Move");
                }
                finally
                {
                    _selectedSquare = null;
                }
            }
            
        }

        private void CheckGameOver()
        {
            if (Game.State == GameState.Checkmate)
            {
                IsGameRunning = false;
                MessageBox.Show($"Checkmate! {Game.CurrentTurn} lost.");

                if (CurrentUser != null) //players statistcs
                {
                    CurrentUser.AddWin();
                    _userService.SaveUsers();
                }
                AutoSaveGame();
                ReturnToMenu();
            }
        }
        private void ResetAllSquares()
        {
            foreach (var square in BoardSquares)
            {
                square.ResetColor();
            }
        }

        public void RefreshBoard()
        {
            foreach(var square in BoardSquares)
            {
                Piece piece = Game.Board.GetPiece(square.Position);

                square.UpdatePiece(piece);
            }
        }

        private void UpdateVisibilities()
        {
            if (CurrentUser != null)
            {
                LoginButtonVisibility = Visibility.Collapsed;
                UserAreaVisibility = Visibility.Visible;
            }
            else
            {
                LoginButtonVisibility = Visibility.Visible;
                UserAreaVisibility = Visibility.Collapsed;
            }
        }

        private void Resign()
        {
            MessageBox.Show($"Game over. {(Game.CurrentTurn == Color.White ? "Black" : "White")} Won!");
            AutoSaveGame();
            ReturnToMenu();
        }

        private void ReturnToMenu()
        {
            MenuVisibility = Visibility.Visible;
            GameVisibility = Visibility.Collapsed;
            BoardSquares.Clear();
        }

        private void AutoSaveGame()
        {
            if (CurrentUser != null && Game.MoveHistory.Any())
            {
                try
                {
                    string filename = $"{CurrentUser.Username}_{DateTime.Now:yyyyMMdd_HHmmss}";

                    _gameFileService.SaveGame(Game, filename);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao guardar partida: " + ex.Message);
                }
            }
        }
    }
}
