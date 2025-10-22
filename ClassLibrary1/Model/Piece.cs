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
        //método para obter os movimentos válidos
        public List<Position> GetValidMoves(Board board) //virtual pq as subclasses podem substituir
        {
            
        }

        public abstract bool CanMoveTo(Position pos, Board board); //abstrato pq todas as subclasses sao obrigadas a implementar


        public abstract Piece Clone();
    }
}
