using ChessApp.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp.Model.Model
{
    public class Knight : Piece
    {
        public Knight(Position currentPosition, Color color) 
            : base(PieceType.Knight, currentPosition, color)
        {
        }

        public override bool CanMoveTo(Position to, Board board)
        {
            if (board == null) return false;
            if (!to.IsValid()) return false;

            int rowDiff = Math.Abs(to.Row - CurrentPosition.Row);
            int columnDiff = Math.Abs(to.Column - CurrentPosition.Column);  

            if (to.Equals(CurrentPosition)) return false;
            if(!((rowDiff == 2 && columnDiff == 1) || (rowDiff == 1 && columnDiff == 2))) return false;

            Piece destination = board.GetPiece(to);

            if (destination == null)
                return true;

            if (destination.Color == Color)
                return false;
            return true;
        }

        public override Piece Clone()
        {
            return new Knight(CurrentPosition, Color);
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
