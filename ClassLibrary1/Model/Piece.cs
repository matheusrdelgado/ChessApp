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
        /// <summary>
        /// Método para obter os movimentos válidos
        /// </summary>
        /// <param name="board"></param>
        /// <returns>Lista de posições válidas</returns>
        public abstract List<Position> GetValidMoves(Board board); //virtual pq as subclasses podem substituir

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
    }
}
