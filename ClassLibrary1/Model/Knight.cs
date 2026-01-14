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
        #region Constructors
        public Knight(Position currentPosition, Color color) 
            : base(PieceType.Knight, currentPosition, color)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Determina se o cavalo pode se mover para a posicao especificada no tabuleiro
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

            if (to.Equals(CurrentPosition)) return false; // Movimento para a mesma posicao nao e permitido
            if (!((rowDiff == 2 && columnDiff == 1) || (rowDiff == 1 && columnDiff == 2))) return false;

            Piece destination = board.GetPiece(to);

            if (destination == null)
                return true;

            if (destination.Color == Color)
                return false;
            return true;
        }

        /// <summary>
        /// Clona o cavalo
        /// </summary>
        /// <returns></returns>
        public override Piece Clone()
        {
            return new Knight(CurrentPosition, Color);
        }

        /// <summary>
        /// Retorna uma lista de movimentos validos para o cavalo no tabuleiro especificado
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
