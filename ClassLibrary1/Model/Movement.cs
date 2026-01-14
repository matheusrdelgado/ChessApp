using ChessApp.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp.Model.Model
{
    public class Movement
    {
        #region Properties
        public Piece PieceMoved { get; set; }
        public Position From { get; set; }
        public Position To { get; set; }
        public Piece CapturedPiece { get; set; }
        public MovementType MovementType { get; set; }
        public string Notation { get; set; } = string.Empty;
        public DateTime DateTime { get; set; } = DateTime.Now;
        public PieceType? PromotionPiece { get; set; }
        #endregion

        #region Constructors
        public Movement(Piece pieceMoved, Position from, Position to, Piece capturedPiece, MovementType movementType, string notation)
        {
            PieceMoved = pieceMoved;
            From = from;
            To = to;
            CapturedPiece = capturedPiece;
            MovementType = movementType;
            Notation = notation;
        }
        #endregion

        /// <summary>
        /// Para devolver a notação do movimento
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Notation;
        }





    }
}
