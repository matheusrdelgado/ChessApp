using ChessApp.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp.Model.Model
{
    public abstract class Piece
    {
        #region Attributes
        public PieceType PieceType { get; set; }
        public Position CurrentPosition { get; set; }
        public Color Color { get; set; }
        public bool HasMoved { get; set; }
        #endregion

        #region Constructor
        protected Piece(PieceType type, Position currentPosition, Color color)
        {
            PieceType = type;
            CurrentPosition = currentPosition;
            Color = color;
            HasMoved = false;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Método para obter os movimentos válidos
        /// </summary>
        /// <param name="board"></param>
        /// <returns>Lista de posições válidas</returns>
        public abstract List<Position> GetValidMoves(Board board);

        /// <summary>
        /// Método para verificar as posições possíveis de movimento
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="board"></param>
        /// <returns>Se o movimento para a posição é válido</returns>
        public abstract bool CanMoveTo(Position pos, Board board); //abstrato pq todas as subclasses sao obrigadas a implementar

        /// <summary>
        /// Método para clonar a peça
        /// </summary>
        /// <returns>Retorna a cópia da peça</returns>
        public abstract Piece Clone(); //testar movimentos hipoteticos

        /// <summary>
        /// Método para verificar se o caminho entre duas posições está livre
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        protected bool FreePath(Position from, Position to, Board board)
        {
            if (!from.IsValid()) return false;
            if (!to.IsValid()) return false;
            if (board == null) return false;

            int rowDirection = to.Row - from.Row;
            int colDirection = to.Column - from.Column;
            
            if(rowDirection > 0) rowDirection = 1;
            else if(rowDirection < 0) rowDirection = -1;

            if(colDirection > 0) colDirection = 1;
            else if(colDirection < 0) colDirection = -1;

            int currentRow = from.Row + rowDirection;
            int currentCol = from.Column + colDirection;

            while(currentRow != to.Row || currentCol != to.Column)
            {
                Piece piece = board.GetPiece(new Position(currentRow, currentCol));
                if (piece != null)
                    return false;
                currentRow += rowDirection;
                currentCol += colDirection;
            } return true;
        }

        #endregion
    }
}
