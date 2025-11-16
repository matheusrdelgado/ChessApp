using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessApp.Model.Enums;

namespace ChessApp.Model.Model
{
    public class Queen : Piece
    {
        #region Atributes
        public Queen(Position currentPosition, Color color)
           : base(PieceType.Queen, currentPosition, color)
        {
        }
        #endregion

        #region Methods
        public override bool CanMoveTo(Position to, Board board)
        {
            if (board == null) return false;
            if (!to.IsValid()) return false;

            int rowDiff = Math.Abs(to.Row - CurrentPosition.Row);
            int columnDiff = Math.Abs(to.Column - CurrentPosition.Column);

            if (to.Equals(CurrentPosition))
                return false;
            if (rowDiff != 0 && columnDiff != 0 && rowDiff != columnDiff)
                return false;

            if (!FreePath(CurrentPosition, to, board))
                return false;

            Piece destination = board.GetPiece(to);

            if (destination == null)
                return true;

            if (destination.Color == Color)
                return false;
            return true;
        }

        public override Piece Clone()
        {
            return new Queen(CurrentPosition, Color);
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
        #endregion
    }
}
