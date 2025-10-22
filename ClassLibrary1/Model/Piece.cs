using ChessApp.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp.Model.Model
{
    public class Piece
    {
        #region Attributes
        public PieceType pieceType { get; set; }
        public Position currentPosition { get; set; }
        public Color color { get; set; }
        public bool hasMoved { get; set; }
        #endregion

        #region Constructor
        public Piece(PieceType type, Position currentPosition, Color color)
        {
            pieceType = type;
            currentPosition = currentPosition;
            color = color;
            hasMoved = false;
        }
        #endregion
        //método para obter os movimentos válidos
        public List<Position> GetValidMoves(Board board)
        {

        }
        
        public bool CanMoveTo(Position pos, Board board)
        {
           
        }

        public Piece Clone()
        {
            return new Piece(this.pieceType, this.currentPosition, this.color)
            {
                hasMoved = this.hasMoved
            };
        }
    }
}
