using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessApp.Model.Enums;

namespace ChessApp.Model.Model
{
    public class Board
    {
        public Piece[,] Squares { get; set; }

        #region Constructor
        public Board() //construtor nao recebe parametros para iniciar o array dentro do construtor
        {
            Squares = new Piece[8, 8];
        }
        #endregion

        #region Methods
        public Piece GetPiece(Position pos)
        {
            if (pos.IsValid())
            {
                return Squares[pos.Row, pos.Column];
            }
            else
            {
                throw new ArgumentException("Invalid position");
            }
        }
        public void PlacePiece(Piece piece, Position pos)
        {
            if (pos.IsValid())
            {
                if (Squares[pos.Row, pos.Column] == null)
                {
                    Squares[pos.Row, pos.Column] = piece;
                    piece.CurrentPosition = pos;
                }
                else
                {
                    throw new ArgumentException("Position occupied");
                }
            }
            else
            {
                throw new ArgumentException("Non existent position");
            }
        }

        public Piece RemovePiece(Position pos)
        {
            if (pos.IsValid())
            {
                if (Squares[pos.Row, pos.Column] != null)
                {
                    Piece tmp = Squares[pos.Row, pos.Column];
                    Squares[pos.Row, pos.Column] = null;
                    tmp.CurrentPosition = null;
                    return tmp;
                }
                return null;

            }
            else
            {
                throw new ArgumentException("Invalid Position");
            }

            
         
        }

        public Piece MovePiece(Position from, Position to)
        {
            if (!from.IsValid())
            {
                throw new ArgumentException("Invalid 'from' position");
            }
            if (!to.IsValid())
            {
                throw new ArgumentException("Invalid 'to' position");
            }
       
            Piece pieceToMove = GetPiece(from);
            if (pieceToMove == null)
            {
                 throw new ArgumentException("No piece found at the source position");
            }
            Piece captured = RemovePiece(to);
            RemovePiece(from);
            PlacePiece(pieceToMove, to);
            pieceToMove.HasMoved = true;
            return captured;

        }

        public void ClearBoard()
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Piece piece = Squares[row, col];
                    if (piece != null)
                    {
                        piece.CurrentPosition = null;
                    }
                    Squares[row, col] = null;
                }
            }
        }

        public List<Piece> GetAllPiecesOfColor(Color color)
        {
            List<Piece> pieces = new List<Piece>();
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Piece piece = Squares[row, col];
                    if (piece != null && piece.Color == color)
                    {
                        pieces.Add(piece);
                    }
                }
            }
            return pieces;
        }

        public Position GetKingPosition(Color color)
        {
            for(int row = 0; row < 8; row++)
            {
                for(int col = 0; col < 8; col++)
                {
                    Piece piece= Squares[row, col];
                    if(piece != null && piece.Color == color && piece.PieceType == PieceType.King)
                    {
                        return piece.CurrentPosition;
                    }
                  
                }
            }
            throw new ArgumentException("King not found");
        }

        public void InitializeBoard()
        {
            ClearBoard();
            for (int col = 0; col < 8; col++)
            {
                PlacePiece(new Pawn(Color.Black, 1, col));
            }
            PlacePiece(new Rook(Color.Black, 0, 0));
            PlacePiece(new Rook(Color.Black, 0, 7));
            PlacePiece(new Knight(Color.Black, 0, 1));
            PlacePiece(new Knight(Color.Black, 0, 6));
            PlacePiece(new Bishop(Color.Black, 0, 2));
            PlacePiece(new Bishop(Color.Black, 0, 5));
            PlacePiece(new King(Color.Black, 0, 4));
            PlacePiece(new Queen(Color.Black, 0, 3));

            for (int col = 0; col < 8; col++)
            {
                PlacePiece(new Pawn(Color.White, 6, col));
            }
            PlacePiece(new Rook(Color.White, 7, 0));
            PlacePiece(new Rook(Color.White, 7, 7));
            PlacePiece(new Knight(Color.White, 7, 1));
            PlacePiece(new Knight(Color.White, 7, 6));
            PlacePiece(new Bishop(Color.White, 7, 2));
            PlacePiece(new Bishop(Color.White, 7, 5));
            PlacePiece(new Queen(Color.White, 7, 3));
            PlacePiece(new King(Color.White, 7, 4));
        }

        #endregion
    }
}
