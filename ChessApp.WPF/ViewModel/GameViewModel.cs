using ChessApp.Model.Enums;
using ChessApp.Model.Interfaces;
using ChessApp.Model.Model;
using ChessApp.Model.Services;
using ChessApp.WPF.ApiClient;
using ChessApp.WPF.Contracts;
using ChessApp.WPF.Views;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IGameApiService _gameApiService;
        private readonly IAuthApiService _authApiService;
        private readonly IGameSyncService _gameSyncService;
        private StockfishService _stockfishService;
        private string? _currentGameId;

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

        private string _apiStatusText = "● Modo Local";
        public string ApiStatusText
        {
            get => _apiStatusText;
            set { _apiStatusText = value; OnPropertyChanged(); }
        }

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
        public ICommand ShowPvEPanelCommand { get; set; }
        public ICommand SetColorCommand { get; set; }
        public int MoveTimeMs { get; set; }
        public int SkillLevel { get; set; }
        public ICommand SetDifficultyCommand { get; set; }
        public ICommand ConfirmPvECommand { get; set; }
        private Visibility _difficultyPanelVisibility = Visibility.Collapsed;
        public Visibility DifficultyPanelVisibility
        {
            get { return _difficultyPanelVisibility; }
            set { _difficultyPanelVisibility = value; OnPropertyChanged(); }
        }
        private string _selectedColorText = "";
        public string SelectedColorText
        {
            get { return _selectedColorText; }
            set { _selectedColorText = value; OnPropertyChanged(); }
        }
        private string _selectedDifficultyText = "";
        public string SelectedDifficultyText
        {
            get { return _selectedDifficultyText; }
            set { _selectedDifficultyText = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Construtor padrão do GameViewModel com resolução via contentor de dependências.
        /// </summary>
        public GameViewModel() : this(
            App.Services.GetRequiredService<IGameApiService>(),
            App.Services.GetRequiredService<IAuthApiService>(),
            App.Services.GetRequiredService<IGameSyncService>(),
            new UserService(),
            new GameFileService())
        {
        }

        /// <summary>
        /// Construtor parametrizado para permitir testes unitários e substituição de serviços.
        /// </summary>
        public GameViewModel(
            IGameApiService gameApiService,
            IAuthApiService authApiService,
            IGameSyncService gameSyncService,
            IUserService userService,
            IGameFileService gameFileService)
        {
            _gameApiService = gameApiService ?? throw new ArgumentNullException(nameof(gameApiService));
            _authApiService = authApiService ?? throw new ArgumentNullException(nameof(authApiService));
            _gameSyncService = gameSyncService ?? throw new ArgumentNullException(nameof(gameSyncService));
            _userService = userService;
            _gameFileService = gameFileService;

            // Subscreve a jogadas recebidas em tempo real (ponto de extensão SignalR)
            _gameSyncService.MoveReceived += OnRemoteMoveReceived;

            // Subscreve a alterações no estado de autenticação
            _authApiService.AuthenticationStateChanged += isAuthenticated =>
            {
                if (!isAuthenticated)
                {
                    CurrentUser = null;
                }
                else if (!string.IsNullOrEmpty(_authApiService.CurrentUsername))
                {
                    CurrentUser = new User(_authApiService.CurrentUsername, string.Empty);
                }
            };

            // Restaura automaticamente a sessão se já houver credenciais guardadas no vault DPAPI
            if (_authApiService.IsAuthenticated && !string.IsNullOrEmpty(_authApiService.CurrentUsername))
            {
                CurrentUser = new User(_authApiService.CurrentUsername, string.Empty);
            }

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

            ShowPvEPanelCommand = new RelayCommand(param =>
            {
                IsPvE = true;
                DifficultyPanelVisibility = Visibility.Visible;
            });

            SetColorCommand = new RelayCommand(param =>
            {
                if (param as string == "White")
                {
                    PlayerColor = Color.White;
                    SelectedColorText = "⬜ White selected";
                }
                else
                {
                    PlayerColor = Color.Black;
                    SelectedColorText = "⬛ Black selected";
                }
            });

            SetDifficultyCommand = new RelayCommand(param => SetDifficultyParams(param as string));

            ConfirmPvECommand = new RelayCommand(param =>
            {
                DifficultyPanelVisibility = Visibility.Collapsed;
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
            LogoutCommand = new RelayCommand(async p =>
            {
                CurrentUser = null;
                await _authApiService.LogoutAsync();
            });

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

            // Regista a nova partida na API em segundo plano sem bloquear a UI local
            string gameMode = IsPvE ? "PvE-Stockfish" : "Local";
            string? difficulty = IsPvE ? _selectedDifficultyText : null;
            _ = RegisterGameOnApiAsync(gameMode, PlayerColor.ToString(), difficulty);

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

                    NotifyRemoteMove();
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
                string winnerResult = Game.CurrentTurn == Color.White ? "BlackWin" : "WhiteWin";
                MessageBox.Show($"Checkmate! {Game.CurrentTurn} lost.");

                if (CurrentUser != null) //players statistcs
                {
                    CurrentUser.AddWin();
                    _userService.SaveUsers();
                }
                AutoSaveGame();
                _ = ReportFinishGameToApiAsync(winnerResult, "Checkmate");
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
            foreach (var square in BoardSquares)
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
            string winnerResult = Game.CurrentTurn == Color.White ? "BlackWin" : "WhiteWin";
            MessageBox.Show($"Game over. {(Game.CurrentTurn == Color.White ? "Black" : "White")} Won!");
            AutoSaveGame();
            _ = ReportFinishGameToApiAsync(winnerResult, "Resignation");
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
            DifficultyPanelVisibility = Visibility.Collapsed;
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

            await Task.Delay(1000); //1000ms de delay para humanizar

            try //pega o melhor movimento do stockfish e faz o movimento
            {
                string fen = Game.GetCurrentFen();
                string bestMoveString = await _stockfishService.GetBestMoveAsync(fen, SkillLevel, MoveTimeMs);    

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

        /// <summary>
        /// Processa o arrastar e soltar de uma casa para outra no tabuleiro.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void ProcessDragDrop(SquareViewModel source, SquareViewModel target)
        {
            if (!IsGameRunning) return;
            if (source == target) return;

            var piece = Game.Board.GetPiece(source.Position);
            if (piece == null || piece.Color != Game.CurrentTurn) return;

            try
            {
                Game.MakeMove(source.Position, target.Position);

                _selectedSquare = null;
                ResetAllSquares();
                RefreshBoard();
                NotifyRemoteMove();
                CheckGameOver();

                if (IsGameRunning && IsPvE && Game.CurrentTurn != PlayerColor)
                {
                    PlayBotTurn();
                }
            }
            catch (Exception)
            {
                _selectedSquare = null;
                ResetAllSquares();
            }
        }
        private void SetDifficultyParams(string difficulty)
        {
            switch (difficulty)
            {
                case "Beginner":
                    SkillLevel = 3;
                    MoveTimeMs = 100;
                    SelectedDifficultyText = "✔ Beginner";
                    break;
                case "Intermediate":
                    SkillLevel = 8;
                    MoveTimeMs = 500;
                    SelectedDifficultyText = "✔ Intermediate";
                    break;
                case "Advanced":
                    SkillLevel = 14;
                    MoveTimeMs = 1000;
                    SelectedDifficultyText = "✔ Advanced";
                    break;
                case "GrandMaster":
                    SkillLevel = 18;
                    MoveTimeMs = 2000;
                    SelectedDifficultyText = "✔ GrandMaster";
                    break;
                case "Impossible":
                    SkillLevel = 20;
                    MoveTimeMs = 5000;
                    SelectedDifficultyText = "✔ Impossible";
                    break;
            }
        }

        #region Métodos de Integração com a ChessApi

        /// <summary>
        /// Regista o início de uma nova partida na API externa e obtém o GameId único.
        /// Caso a API esteja inacessível, não bloqueia o jogo local.
        /// </summary>
        private async Task RegisterGameOnApiAsync(string gameMode, string playerColor, string? difficulty)
        {
            try
            {
                var dto = new CreateGameDto(gameMode, playerColor, difficulty);
                var response = await _gameApiService.CreateGameAsync(dto);
                if (response.Success && response.Data != null)
                {
                    _currentGameId = response.Data.GameId;
                    ApiStatusText = "● Conectado à API";
                }
                else
                {
                    ApiStatusText = "● Modo Offline";
                }
            }
            catch
            {
                // Falha de ligação tratada sem travar o jogo local
                ApiStatusText = "● Modo Offline";
            }
        }

        /// <summary>
        /// Reporta o resultado final e o histórico de lances da partida à API externa.
        /// </summary>
        private async Task ReportFinishGameToApiAsync(string result, string reason)
        {
            if (string.IsNullOrEmpty(_currentGameId)) return;

            try
            {
                string gameMode = IsPvE ? "PvE-Stockfish" : "Local";
                var moves = BuildMoveDtos();
                var finishDto = new FinishGameDto(_currentGameId, gameMode, result, reason, moves, Game.GetCurrentFen());
                await _gameApiService.FinishGameAsync(finishDto);
            }
            catch
            {
                // Falha ao reportar tratada de forma silenciosa para garantir a fluidez do jogo local
            }
            finally
            {
                _currentGameId = null;
            }
        }

        /// <summary>
        /// Converte o histórico de jogadas local para o formato de DTOs esperado pela API.
        /// </summary>
        private List<MoveDto> BuildMoveDtos()
        {
            var list = new List<MoveDto>();
            int num = 1;
            foreach (var m in Game.MoveHistory)
            {
                char fromCol = (char)('a' + m.From.Column);
                int fromRank = 8 - m.From.Row;
                char toCol = (char)('a' + m.To.Column);
                int toRank = 8 - m.To.Row;

                list.Add(new MoveDto(
                    num++,
                    $"{fromCol}{fromRank}",
                    $"{toCol}{toRank}",
                    m.PieceMoved?.PieceType.ToString() ?? "Unknown",
                    m.Notation,
                    m.PromotionPiece?.ToString())
                {
                    Timestamp = m.DateTime
                });
            }
            return list;
        }

        /// <summary>
        /// Ponto de extensão para notificar o adversário remoto sobre um movimento local via SignalR.
        /// </summary>
        private void NotifyRemoteMove()
        {
            if (_gameSyncService.IsConnected && Game.MoveHistory.Any())
            {
                var last = Game.MoveHistory.Last();
                char fromCol = (char)('a' + last.From.Column);
                int fromRank = 8 - last.From.Row;
                char toCol = (char)('a' + last.To.Column);
                int toRank = 8 - last.To.Row;

                var moveDto = new MoveDto(
                    Game.MoveHistory.Count,
                    $"{fromCol}{fromRank}",
                    $"{toCol}{toRank}",
                    last.PieceMoved?.PieceType.ToString() ?? "Unknown",
                    last.Notation,
                    last.PromotionPiece?.ToString())
                {
                    Timestamp = last.DateTime
                };

                _ = _gameSyncService.SendMoveAsync(moveDto);
            }
        }

        /// <summary>
        /// Manipulador invocado quando uma jogada remota é recebida via SignalR (PvP online).
        /// Aplica o movimento no tabuleiro local sem alterar a lógica de regras.
        /// </summary>
        private void OnRemoteMoveReceived(MoveDto move)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (!IsGameRunning) return;
                try
                {
                    var (from, to) = ParseStockfishMove(move.From + move.To);
                    Game.MakeMove(from, to);
                    ResetAllSquares();
                    RefreshBoard();
                    CheckGameOver();
                }
                catch
                {
                    // Ignora movimentos remotos inválidos
                }
            });
        }

        #endregion
    }
}
