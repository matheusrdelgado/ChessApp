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
        #region Atributes
        public King(Position currentPosition, Color color)
            : base(PieceType.King, currentPosition, color)
        {
        }
        #endregion

        #region Methods
        public override Piece Clone()
        {
            return new King(CurrentPosition, Color);
        }
        public override bool CanMoveTo(Position to, Board board)
        {
            if (!to.IsValid()) return false;

            int rowDiff = Math.Abs(to.Row - CurrentPosition.Row);
            int columnDiff = Math.Abs(to.Column - CurrentPosition.Column);

            if (rowDiff == 0 && columnDiff == 0 || rowDiff > 1 || columnDiff > 1)
            {
                return false;
            }

            Piece piece = board.GetPiece(to);

            if (piece == null)
                return true;

            if (piece.Color == Color)
                return false;
            return true;

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

        /// <summary>
        /// Método para verificar se é possível realizar o roque pequeno
        /// </summary>
        /// <param name="board"></param>
        /// <param name="kingSide"></param>
        /// <returns>um booleano</returns>
        public bool CanCastleShort(Board board)
        {
            if (board == null) return false;

            if (HasMoved) return false;

            int row = CurrentPosition.Row;
            if (Color == Color.Black)
                if(row != 0)
                    return false;
            if (Color == Color.White)
                if (row != 7)
                    return false;

            int column = CurrentPosition.Column;
            if(column != 4)
                return false;


            Piece rook = board.GetPiece(new Position(CurrentPosition.Row, 7));
            if (rook != null && rook.Color == Color && rook.PieceType == PieceType.Rook && !rook.HasMoved)
            {
                for (int col = CurrentPosition.Column + 1; col < 7; col++)
                {
                    Piece piece = board.GetPiece(new Position(CurrentPosition.Row, col));
                    if (piece != null)
                        return false;
                }
                return true;
            } return false;
           
        }

        /// <summary>
        /// Método para verificar se é possivel realizar o roque longo
        /// </summary>
        /// <param name="board"></param>
        /// <returns>um booleano</returns>
        public bool CanCastleLong(Board board)
        {
            if (board == null) return false;

            if (HasMoved) return false;

            int row = CurrentPosition.Row;
            if (Color == Color.Black)
                if (row != 0)
                    return false;
            if (Color == Color.White)
                if (row != 7)
                    return false;

            int column = CurrentPosition.Column;
            if (column != 4)
                return false;


            Piece rook = board.GetPiece(new Position(CurrentPosition.Row, 0));
            if (rook != null && rook.Color == Color && rook.PieceType == PieceType.Rook && !rook.HasMoved)
            {
                for (int col = CurrentPosition.Column - 1; col > 0; col--)
                {
                    Piece piece = board.GetPiece(new Position(CurrentPosition.Row, col));
                    if (piece != null)
                        return false;
                }
                return true;
            }
            return false;

        }
        #endregion
    }
}