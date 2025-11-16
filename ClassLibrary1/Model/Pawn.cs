using ChessApp.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp.Model.Model
{
    public class Pawn : Piece
    {
        public Pawn(Position currentPosition, Color color) 
            : base(PieceType.Pawn, currentPosition, color)
        {
        }

        public override bool CanMoveTo(Position to, Board board)
        {
            if (board == null) return false;
            if (!to.IsValid()) return false;

            if (to.Equals(CurrentPosition)) return false;

            int rowDiff = Math.Abs(to.Row - CurrentPosition.Row);
            int columnDiff = Math.Abs(to.Column - CurrentPosition.Column);

            if (Color == Color.White && to.Row >= CurrentPosition.Row) return false;
            if (Color == Color.Black && to.Row <= CurrentPosition.Row) return false;

            Piece destination = board.GetPiece(to);

            if (columnDiff == 0 && destination != null) return false;
            if(rowDiff > 1 && rowDiff < 3 && HasMoved == true && columnDiff == 0 && !FreePath(CurrentPosition, to, board)) return false;
            if (rowDiff > 1 && rowDiff < 3 && columnDiff == 0 && !FreePath(CurrentPosition, to, board)) return false;
            if(rowDiff > 1 && rowDiff < 3 && Color == Color.White && CurrentPosition.Row != 6 && destination != null) return false;
            if (rowDiff > 1 && rowDiff < 3 && Color == Color.Black && CurrentPosition.Row != 1 && destination != null) return false;
            if (rowDiff > 1 && rowDiff < 3 && HasMoved == true) return false;
            if(columnDiff > 1) return false;
            if(HasMoved == true && rowDiff > 1 ) return false;
            if(rowDiff == 1 && destination != null) return false;


            if (columnDiff == 1 && rowDiff == 1 && destination == null) return false;
            if (columnDiff == 1 && rowDiff == 1 && destination != null && destination.Color == Color) return false;
        }

        public override Piece Clone()
        {
            return new Pawn(CurrentPosition, Color);
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
