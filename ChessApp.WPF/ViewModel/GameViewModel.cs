using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel; //para ObservableCollection que avisa o WPF se adicionar ou remover quadrados
using ChessApp.Model.Model;
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
            if(_selectedSquare == null)
            {
                var piece = Game.Board.GetPiece(clickedSquare.Position);
                if(piece != null)
                {
                    if(piece.Color == Game.CurrentTurn)
                    {
                        _selectedSquare = clickedSquare;
                        _selectedSquare.Highlight();
                    }
                }
            }
            else
            {
                if(_selectedSquare == clickedSquare)
                {
                    _selectedSquare.ResetColor();
                    _selectedSquare = null;
                    return;
                }

                try
                {
                    Game.MakeMove(_selectedSquare.Position, clickedSquare.Position);
                    RefreshBoard();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Jogada Inválida");
                }
                finally
                {
                    _selectedSquare.ResetColor();
                    _selectedSquare = null;
                }
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
