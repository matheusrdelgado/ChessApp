using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using ChessApp.Model.Enums;

namespace ChessApp.Model.Model
{
    public class Bishop : Piece
    {
        #region Atributes
        public Bishop(Position currentPosition, Color color)
           : base(PieceType.Bishop, currentPosition, color)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Determina se uma peça pode se mover para a posição especificada no tabuleiro.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        public override bool CanMoveTo(Position to, Board board)
        {
            if (board == null) return false;
            if (!to.IsValid()) return false;

            int rowDiff = Math.Abs(to.Row - CurrentPosition.Row);
            int columnDiff = Math.Abs(to.Column - CurrentPosition.Column);

            if (to.Equals(CurrentPosition))
                return false;
            if (rowDiff != columnDiff)
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
        /// Cria uma cópia da peça Bispo atual.
        /// </summary>
        /// <returns></returns>
        public override Piece Clone()
        {
            return new Bishop(CurrentPosition, Color);
        }

        /// <summary>
        /// Obtém todos os movimentos válidos para o bispo no tabuleiro especificado.
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
