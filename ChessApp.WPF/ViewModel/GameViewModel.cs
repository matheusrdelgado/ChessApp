using ChessApp.Model.Enums;
using ChessApp.Model.Interfaces;
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
using System.Windows.Controls;
using System.Windows.Input;

namespace ChessApp.WPF.ViewModel
{
    public class GameViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IGameFileService _gameFileService;
        private StockfishService _stockfishService;

        //game proprierties
        public Game Game { get; private set; }
        public ObservableCollection<SquareViewModel> BoardSquares { get; set; }

        private SquareViewModel _selectedSquare;
        public Color PlayerColor { get; set; } = Color.White;

        public bool IsPvE { get; private set; }

        private bool _isGameRunning;
        //game running
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

        /// <summary>
        /// Comandos para botoes e interacoes do jogo
        /// </summary>
        public ICommand NewGameCommand { get; set; }
        public ICommand LoginCommand { get; set; }
        public ICommand RegisterCommand { get; set; }
        public ICommand SaveGameCommand { get; set; }
        public ICommand OpenLoginCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        public ICommand OpenHistoryCommand { get; set; }
        public ICommand GiveUpCommand { get; set; }
        public ICommand ResignCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand NewGamePvECommand { get; set; }
        public ICommand NewGamePvEBlackCommand { get; set; }

        /// <summary>
        /// Construtor do GameViewModel
        /// </summary>
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

  
            NewGameCommand = new RelayCommand(param => // => funcao lambda para comandos simples, ou seja, sem muitos passos
            {
                IsPvE = false;
                PlayerColor = Color.White;
                StartNewGame();
            });
            NewGamePvECommand = new RelayCommand(param =>
            {
                IsPvE = true;
                PlayerColor = Color.White;
                StartNewGame();
            });
            NewGamePvEBlackCommand = new RelayCommand(param =>
            {
                IsPvE = true;
                PlayerColor = Color.Black;
                StartNewGame();
            });
            SaveGameCommand = new RelayCommand(param => SaveCurrentGame(), param => IsGameRunning);
            LoginCommand = new RelayCommand(p => PerformLogin(p));
            RegisterCommand = new RelayCommand(p => PerformRegister(p));

            //login/logout commands
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
            CloseCommand = new RelayCommand(p => Application.Current.Shutdown());

            try //inicializa stockfish
            {
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Engine", "stockfish.exe");

                if (System.IO.File.Exists(path))
                {
                    _stockfishService = new StockfishService(path);
                }
                else
                {
                    MessageBox.Show("Stockfish.exe wasn't found in Engine directory.");
                }
            }
            catch (Exception ex) //trata erro de inicializacao
            {
                MessageBox.Show("Failed to load stockfish: " + ex.Message);
            }
        }

        /// <summary>
        /// metodo de login
        /// </summary>
        /// <param name="parameter"></param>
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
                MessageBox.Show($"Welcome {user.Username}!");
                // clean passwordbox
                passwordBox.Password = "";
            }
            else
            {
                MessageBox.Show("Incorrect user or password.");
            }
        }

        /// <summary>
        /// metodo de registo
        /// </summary>
        /// <param name="parameter"></param>
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

        /// <summary>
        /// Inicia um novo jogo
        /// </summary>
        private void StartNewGame()
        {
            Game = new Game();
            BoardSquares.Clear();
            InitializeBoardVisuals();
            RefreshBoard();
            IsGameRunning = true;

            MenuVisibility = Visibility.Collapsed;
            GameVisibility = Visibility.Visible;

            if (IsPvE && PlayerColor == Color.Black)
            {
                PlayBotTurn();
            }
        }

        /// <summary>
        /// guarda o jogo atual
        /// </summary>
        private void SaveCurrentGame()
        {
            if (Game.MoveHistory.Count > 0)
            {
                string filename = $"Game_{DateTime.Now:ddMMyyyy_HHmmss}";
                _gameFileService.SaveGame(Game, filename);
                MessageBox.Show("Game saved!");
            }
        }

        /// <summary>
        /// Inicializa os visuais do tabuleiro
        /// </summary>
        private void InitializeBoardVisuals()
        {
            if (PlayerColor == Color.White)
            {
                for (int row = 0; row < 8; row++)
                {
                    for (int col = 0; col < 8; col++)
                    {
                        CreateSquare(row, col);
                    }
                }
            }
            else
            {
                for (int row = 7; row >= 0; row--)
                {
                    for (int col = 7; col >= 0; col--)
                    {
                        CreateSquare(row, col);
                    }
                }
            }
        }

        /// <summary>
        /// Cria um quadrado no tabuleiro para a posicao dada
        /// com o objetivo de adicionar comandos de clique
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void CreateSquare(int row, int col)
        {
            var square = new SquareViewModel(new Position(row, col));
            square.ClickCommand = new RelayCommand(param => OnSquareClicked(square));
            BoardSquares.Add(square);
        }

        /// <summary>
        /// Metodo chamado quando um quadrado e clicado
        /// </summary>
        /// <param name="clickedSquare"></param>
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

                    if (IsGameRunning && IsPvE && Game.CurrentTurn != PlayerColor) //stockfish
                    {
                        PlayBotTurn();
                    }
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

        /// <summary>
        /// Verifica se o jogo acabou
        /// </summary>
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

        /// <summary>
        /// Reseta a cor de todos os quadrados do tabuleiro quando um movimento e feito
        /// </summary>
        private void ResetAllSquares()
        {
            foreach (var square in BoardSquares)
            {
                square.ResetColor();
            }
        }
        /// <summary>
        /// Atualiza as pecas no tabuleiro visual
        /// </summary>
        public void RefreshBoard()
        {
            foreach(var square in BoardSquares)
            {
                Piece piece = Game.Board.GetPiece(square.Position);

                square.UpdatePiece(piece);
            }
        }

        /// <summary>
        /// Atualiza as visibilidades dos botoes de login/logout
        /// </summary>
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

        /// <summary>
        /// Metodo para desistir do jogo
        /// </summary>
        private void Resign()
        {
            MessageBox.Show($"Game over. {(Game.CurrentTurn == Color.White ? "Black" : "White")} Won!");
            AutoSaveGame();
            ReturnToMenu();
        }

        /// <summary>
        /// Retorna ao menu principal
        /// </summary>
        private void ReturnToMenu()
        {
            MenuVisibility = Visibility.Visible;
            GameVisibility = Visibility.Collapsed;
            BoardSquares.Clear();
        }

        /// <summary>
        /// guarda automaticamente o jogo quando acaba
        /// </summary>
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
                    MessageBox.Show("Error in saving match: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Parseia a string de movimento do Stockfish para posicoes de origem e destino
        /// ou seja, converte "e2e4" para (6,4) e (4,4)
        /// </summary>
        /// <param name="moveString"></param>
        /// <returns></returns>
        private (Position from, Position to) ParseStockfishMove(string moveString)
        {
            // moveString ex: "e2e4" ou "e7e8q" (promotion)

            var fromCol = moveString[0] - 'a';       // 'e' - 'a' = 4
            var fromRow = 8 - (moveString[1] - '0'); // 8 - 2 = 6 

            var toCol = moveString[2] - 'a';
            var toRow = 8 - (moveString[3] - '0');

            return (new Position(fromRow, fromCol), new Position(toRow, toCol));
        }

        /// <summary>
        /// metodo para o stockfish jogar
        /// </summary>
        private async void PlayBotTurn()
        {
            if (_stockfishService == null)
            {
                MessageBox.Show("Error: Stockfish was not initialized properly");
                return;
            }

            await Task.Delay(500); //500ms de delay para parecer mais humano

            try //pega o melhor movimento do stockfish e faz o movimento
            {
                string fen = Game.GetCurrentFen();
                string bestMoveString = await _stockfishService.GetBestMoveAsync(fen);

                if (!string.IsNullOrEmpty(bestMoveString))
                {
                    var (from, to) = ParseStockfishMove(bestMoveString);

                    Game.MakeMove(from, to);

                    ResetAllSquares();
                    RefreshBoard();
                    CheckGameOver();
                }
            }
            catch (Exception ex) //trata erros do stockfish
            {
                MessageBox.Show($"Engine exploded {ex.Message}");
            }
        }
    }
}
