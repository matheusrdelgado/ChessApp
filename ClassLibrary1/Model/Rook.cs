using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using ChessApp.Model.Enums;

namespace ChessApp.Model.Model
{
    public class Rook : Piece
    {
        public Rook(Position currentPosition, Color color)
           : base(PieceType.Rook, currentPosition, color)
        {
        }
        public override bool CanMoveTo(Position to, Board board)
        {
            if (!to.IsValid())
            {
                throw new ArgumentException("Invalid Position");
            }

            int rowDiff = Math.Abs(to.Row - CurrentPosition.Row);
            int columnDiff = Math.Abs(to.Column - CurrentPosition.Column);

            if (rowDiff == 0 && columnDiff == 0)
                return false;
            if(rowDiff > 0 && columnDiff > 0)
                return false;

            if (columnDiff == 0 && rowDiff > 0)
            {
                if (to.Row > CurrentPosition.Row) {
                    for (int row = CurrentPosition.Row + 1; row < to.Row; row++)
                    {
                        Piece piece = board.GetPiece(new Position(row, CurrentPosition.Column));
                        if (piece != null)
                            return false;

                    }
                } 
                if (to.Row < CurrentPosition.Row)
                {
                    for (int row = CurrentPosition.Row - 1; row > to.Row; row--)
                    {
                        Piece piece = board.GetPiece(new Position(row, CurrentPosition.Column));
                        if (piece != null)
                            return false;

                    }
                }
            }

            if (columnDiff > 0 && rowDiff == 0)
            {
                if (to.Column > CurrentPosition.Column)
                {
                    for (int col = CurrentPosition.Column + 1; col < to.Column; col++)
                    {
                        Piece piece = board.GetPiece(new Position(CurrentPosition.Row, col));
                        if (piece != null)
                            return false;

                    }
                }
                if (to.Column < CurrentPosition.Column)
                {
                    for (int col = CurrentPosition.Column - 1; col > to.Column; col--)
                    {
                        Piece piece = board.GetPiece(new Position(CurrentPosition.Row, col));
                        if (piece != null)
                            return false;

                    }
                }
            }
            Piece destination = board.GetPiece(to);

            if (destination == null)
                return true;

            if (destination.Color == Color)
                return false;
            return true;
        }

        public override Piece Clone()
        {
            return new Rook(CurrentPosition, Color);
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
