using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        #endregion
    }
}
