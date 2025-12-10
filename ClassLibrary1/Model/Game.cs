using ChessApp.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 

namespace ChessApp.Model.Model
{
    public class Game
    {
        public Board Board { get; private set; }
        public List<Piece> WhiteCapturedPieces { get; private set; } = new List<Piece>();
        public List<Piece> BlackCapturedPieces { get; private set; } = new List<Piece>();
        public List<Movement> MoveHistory { get; private set; } = new List<Movement>();
        public Color CurrentTurn { get; private set; } = Color.White;
        public GameState State { get; private set; } = GameState.Playing;

        public Game()
        {
            Board = new Board();
        }

        public void MakeMove(Position from, Position to)
        {
            if (State != GameState.Playing)
                throw new InvalidOperationException("Game is not in playing state.");

            Piece piece = Board.GetPiece(from);

            if (piece == null)
                throw new InvalidOperationException("No piece at source position.");

            if (piece.Color != CurrentTurn)
                throw new InvalidOperationException("Not this piece's turn.");

            if (!piece.CanMoveTo(to, Board))
                throw new InvalidOperationException("Invalid move.");

            Piece capturedPiece = Board.MovePiece(from, to);

            if (capturedPiece != null)
            {
                if (CurrentTurn == Color.White)
                    BlackCapturedPieces.Add(capturedPiece);
                else
                    WhiteCapturedPieces.Add(capturedPiece);
            }

            MovementType type = MovementType.Normal;
            PieceType? promotion = null;


            if (piece is Pawn pawn && pawn.CanPromote()) //fazer depois a opcao de escolher a peca
            {
                type = MovementType.Promotion;
                promotion = PieceType.Queen;

                Piece queen = new Queen(to, piece.Color);
                Board.RemovePiece(to);
                Board.PlacePiece(queen, to);
                piece = queen;
            }

            Movement move = new Movement(piece, from, to, capturedPiece, type, to.ToChessNotation());
            move.PromotionPiece = promotion;
            MoveHistory.Add(move);

            SwitchTurn();
        }

        private void SwitchTurn()
        {
            CurrentTurn = CurrentTurn == Color.White ? Color.Black : Color.White;
        }

    }
}




