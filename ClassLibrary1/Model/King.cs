using ChessApp.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp.Model.Model
{
    public class King : Piece
    {
        public King(Position currentPosition, Color color)
            : base(PieceType.King, currentPosition, color)
        {
        }
        public override Piece Clone()
        {
            return new King(CurrentPosition, Color);
        }
        public override bool CanMoveTo(Position to, Board board)
        {
            if (!to.IsValid())
            {
                throw new ArgumentException("Invalid Position");
            }

            int rowDiff = Math.Abs(to.Row - CurrentPosition.Row);
            int columnDiff = Math.Abs(to.Column - CurrentPosition.Column);

            if (rowDiff == 0 && columnDiff == 0 || rowDiff > 1 || columnDiff > 1)
            {
                return false;
            }

            Piece piece = board.GetPiece(to);

            if (piece == null)
            {
                return true;
            }   
            if (piece.Color == Color)
            {
                return false;
            } return true;
            
        }

        public override List<Position> GetValidMoves(Board board)
        {
            List<Position> valid = new List<Position>();
            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 8; row++)
                {
                    Position pos = new(row, col);

                    if (CanMoveTo(pos, board))
                    {
                        valid.Add(pos);
                    }
                }
            }
            return valid;
        }
    }   
}
