using ChessApp.Model.Enums;
using ChessApp.Model.Model;
using System;

namespace ChessApp.Model.Factories
{
    public static class PieceFactory
    {
        /// <summary>
        /// Metodo para criar as peças do jogo
        /// </summary>
        /// <param name="type"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Piece CreatePiece(PieceType type, Position position, Color color)
        {
            switch (type)
            {
                case PieceType.Pawn:
                    return new Pawn(position, color);
                case PieceType.Rook:
                    return new Rook(position, color);
                case PieceType.Knight:
                    return new Knight(position, color);
                case PieceType.Bishop:
                    return new Bishop(position, color);
                case PieceType.Queen:
                    return new Queen(position, color);
                case PieceType.King:
                    return new King(position, color);
                default:
                    throw new ArgumentException("Invalid piece type");
            }
        }
    }
}