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
        #region Atributes
        public Rook(Position currentPosition, Color color)
           : base(PieceType.Rook, currentPosition, color)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Determina se a torre pode se mover para a posicao do tabuleiro
        /// </summary>
        /// <param name="to"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        public override bool CanMoveTo(Position to, Board board)
        {
            if (!to.IsValid()) return false;
            int rowDiff = Math.Abs(to.Row - CurrentPosition.Row);
            int columnDiff = Math.Abs(to.Column - CurrentPosition.Column);

            if (to.Equals(CurrentPosition))
                return false;
            if (rowDiff > 0 && columnDiff > 0)
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

        /// <summary>
        /// Clona a torre
        /// </summary>
        /// <returns></returns>
        public override Piece Clone()
        {
            return new Rook(CurrentPosition, Color);
        }


        /// <summary>
        /// Retorna uma lista de movimentos validos para a torre
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
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
