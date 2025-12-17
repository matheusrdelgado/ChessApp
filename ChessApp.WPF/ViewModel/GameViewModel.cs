using ChessApp.Model.Enums;
using ChessApp.Model.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel; //para ObservableCollection que avisa o WPF se adicionar ou remover quadrados
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChessApp.WPF.ViewModel
{
    public class GameViewModel : BaseViewModel
    {
        public Game Game { get; private set; }
        public ObservableCollection<SquareViewModel> BoardSquares { get; set; }
        private SquareViewModel _selectedSquare;

        public GameViewModel()
        {
            Game = new Game();

            BoardSquares = new ObservableCollection<SquareViewModel>();

            InitializeBoardVisuals();

            RefreshBoard();
        }

        private void InitializeBoardVisuals()
        {
            for(int row = 0; row < 8; row++)
            {
                for(int col = 0; col < 8; col++)
                {
                    var square = new SquareViewModel(new Position(row, col));

                    square.ClickCommand = new RelayCommand(param => OnSquareClicked(square));

                    BoardSquares.Add(square);
                }
            }
        }

        private void OnSquareClicked(SquareViewModel clickedSquare)
        {
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

                    if (Game.State == GameState.Checkmate)
                    {
                        MessageBox.Show($"Check-Mate! {Game.CurrentTurn} lost.", "Game Over");
                    }
                    else if (Game.State == GameState.Stalemate)
                    {
                        MessageBox.Show("Stalemate!", "Game Over");
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
    }
}
