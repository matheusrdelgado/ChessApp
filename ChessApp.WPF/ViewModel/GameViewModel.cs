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

                    BoardSquares.Add(square);
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
