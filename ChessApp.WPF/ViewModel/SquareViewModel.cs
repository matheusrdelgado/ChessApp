using ChessApp.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using ChessApp.Model.Enums;
using Color = ChessApp.Model.Enums.Color;
using System.Windows.Input;

namespace ChessApp.WPF.ViewModel
{
    public class SquareViewModel : BaseViewModel
    {
        public Position Position { get; private set; }

        private Brush _backGroundColor;
        public Brush BackgroundColor 
        {
            get { return _backGroundColor; }
            set
            {
                _backGroundColor = value;
                OnPropertyChanged();
            }
        }
        public ICommand ClickCommand { get; set; }

        public SquareViewModel(Position position)
        {
            Position = position;
            ResetColor();
        }

        private string _pieceImage;
        public string PieceImage
        {
            get { return _pieceImage; }
            set
            {
                _pieceImage = value;
                OnPropertyChanged(); //avisa a tela para atualizar
            }
        }

        public void UpdatePiece(Piece piece)
        {
            if (piece == null)
            {
                PieceImage = null;
                return;
            }
            if(piece.Color == Color.White)
            {
                switch (piece.PieceType)
                {
                    case PieceType.Pawn:
                        PieceImage = "/Assets/wP.png";
                        break;
                    case PieceType.Rook:
                        PieceImage = "/Assets/wR.png";
                        break;
                    case PieceType.Knight:
                        PieceImage = "/Assets/wN.png";
                        break;
                    case PieceType.Bishop:
                        PieceImage = "/Assets/wB.png";
                        break;
                    case PieceType.Queen:
                        PieceImage = "/Assets/wQ.png";
                        break;
                    case PieceType.King:
                        PieceImage = "/Assets/wK.png";
                        break;
                }
            }
            else if(piece.Color == Color.Black)
            {
                switch (piece.PieceType)
                {
                    case PieceType.Pawn:
                        PieceImage = "/Assets/bP.png";
                        break;
                    case PieceType.Rook:
                        PieceImage = "/Assets/bR.png";
                        break;
                    case PieceType.Knight:
                        PieceImage = "/Assets/bN.png";
                        break;
                    case PieceType.Bishop:
                        PieceImage = "/Assets/bB.png";
                        break;
                    case PieceType.Queen:
                        PieceImage = "/Assets/bQ.png";
                        break;
                    case PieceType.King:
                        PieceImage = "/Assets/bK.png";
                        break;
                }
            }
        }
        public void Highlight()
        {
            BackgroundColor = Brushes.Green;
        }

        public void ResetColor()
        {
            var converter = new BrushConverter();
            int soma = Position.Row + Position.Column;

            if (soma % 2 == 0)
                BackgroundColor = (Brush)converter.ConvertFromString("#dae4ee");
            else
                BackgroundColor = (Brush)converter.ConvertFromString("#8ca2ad");
        }
    }
}
