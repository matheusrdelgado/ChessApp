using ChessApp.Model.Model;
using ChessApp.Model.Enums;
using System.Windows.Media; // Necessário para Brushes e ColorConverter
using System.Windows.Input;
using System.Windows.Controls;
// Alias para não confundir com System.Windows.Media.Color no resto do código
using Color = ChessApp.Model.Enums.Color;
using PieceType = ChessApp.Model.Enums.PieceType; // Facilitar o switch

namespace ChessApp.WPF.ViewModel
{
    public class SquareViewModel : BaseViewModel
    {
        public Position Position { get; private set; }

        private static readonly SolidColorBrush LightColor = new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString("#EBF3F5"));
        private static readonly SolidColorBrush DarkColor = new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString("#8CA2AD"));
        private static readonly SolidColorBrush HighlightColor = new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString("#F6F696"));
        private static readonly SolidColorBrush PossibleMoveColor = new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString("#A9D08E"));

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
            BackgroundColor = HighlightColor;
        }

        public void ResetColor()
        {
            if ((Position.Row + Position.Column) % 2 == 0)
            {
                BackgroundColor = LightColor;
            }
            else
            {
                BackgroundColor = DarkColor;
            }
        }

        public void HighlightPossibleMove()
        {
            BackgroundColor = PossibleMoveColor;
        }
    }
}
