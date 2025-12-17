using ChessApp.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

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
            Board.InitializeBoard();
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


            if (!piece.CanMoveTo(to, Board)) //logica para o movimento normal
                throw new InvalidOperationException("Invalid move.");

            if (piece is King king && Math.Abs(to.Column - from.Column) == 2)//logica para o castling
            {
                if (to.Column > from.Column)
                {
                    if (IsCheck(CurrentTurn))
                        throw new InvalidOperationException("Cannot castle while in check.");

                    Position middle = new Position(from.Row, (from.Column + to.Column) / 2);
                    if (IsSquareAttacked(middle, CurrentTurn == Color.White ? Color.Black : Color.White))
                        throw new InvalidOperationException("Cannot castle through check.");

                    if (IsSquareAttacked(to, CurrentTurn == Color.White ? Color.Black : Color.White))
                        throw new InvalidOperationException("Cannot castle into check.");

                    if (!king.CanCastleShort(Board))
                        throw new InvalidOperationException("Cannot castle short.");

                    Piece rook = Board.GetPiece(new Position(from.Row, 7));
                    Board.MovePiece(new Position(from.Row, 7), new Position(from.Row, 5));
                }
                else
                {
                    if (!king.CanCastleLong(Board))
                        throw new InvalidOperationException("Cannot castle long.");

                    Piece rook = Board.GetPiece(new Position(from.Row, 0));
                    Board.MovePiece(new Position(from.Row, 0), new Position(from.Row, 3));
                }

                Board.MovePiece(from, to);
                MoveHistory.Add(new Movement(piece, from, to, null, MovementType.Castling, to.ToChessNotation()));
                SwitchTurn();
                return;
            }

            if (piece is Pawn && from.Column != to.Column && Board.GetPiece(to) == null)
            {
                Movement lastMove = MoveHistory.LastOrDefault();
                if (lastMove != null &&
                    lastMove.PieceMoved is Pawn &&
                    Math.Abs(lastMove.From.Row - lastMove.To.Row) == 2 &&
                    lastMove.To.Column == to.Column &&
                    lastMove.To.Row == from.Row)
                {
                    Position enemyPawnPos = lastMove.To;
                    Piece capturedPawn = Board.RemovePiece(enemyPawnPos);

                    Board.MovePiece(from, to);

                    if (IsCheck(CurrentTurn))
                    {
                        Board.MovePiece(to, from);
                        Board.PlacePiece(capturedPawn, enemyPawnPos);
                        throw new InvalidOperationException("Move results in self-check.");
                    }

                    if (CurrentTurn == Color.White)
                        WhiteCapturedPieces.Add(capturedPawn);
                    else
                        BlackCapturedPieces.Add(capturedPawn);

                    MoveHistory.Add(new Movement(piece, from, to, capturedPawn, MovementType.EnPassant, to.ToChessNotation()));
                    SwitchTurn();
                    return;
                }
            }
            bool wasMoved = piece.HasMoved;
            Piece capturedPiece = Board.MovePiece(from, to);

            if (IsCheck(CurrentTurn))
            {
                // Reverte o Movimento
                Board.MovePiece(to, from);
                piece.HasMoved = wasMoved;
                if (capturedPiece != null)
                {
                    Board.PlacePiece(capturedPiece, to);
                }
                throw new InvalidOperationException("Move results in self-check.");
            }


            if (capturedPiece != null)
            {
                if (CurrentTurn == Color.White)
                    BlackCapturedPieces.Add(capturedPiece);
                else
                    WhiteCapturedPieces.Add(capturedPiece);
            }

            MovementType type = MovementType.Normal;
            PieceType? promotion = null;


            if (piece is Pawn pawn && pawn.CanPromote()) //fazer depois a opcao de escolher a peca!!!
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

        public bool IsCheck(Color kingColor)
        {
            Position kingPos = Board.GetKingPosition(kingColor);
            Color opponentColor = kingColor == Color.White ? Color.Black : Color.White;
            return IsSquareAttacked(kingPos, opponentColor);
        }

        private bool IsSquareAttacked(Position pos, Color attackerColor)
        {
            List<Piece> attackers = Board.GetAllPiecesOfColor(attackerColor);
            foreach (var attacker in attackers)
            {
                if (attacker.CanMoveTo(pos, Board))
                    return true;
            }
            return false;
        }

        private void SwitchTurn()
        {
            CurrentTurn = CurrentTurn == Color.White ? Color.Black : Color.White;
            CheckForGameOver();
        }

        private void CheckForGameOver()
        {
            if (!HasLegalMoves(CurrentTurn))
            {
                if (IsCheck(CurrentTurn))
                {
                    State = GameState.Checkmate;
                }
                else
                {
                    State = GameState.Stalemate;
                }
            }
        }

        private bool HasLegalMoves(Color color)
        {
            List<Piece> pieces = Board.GetAllPiecesOfColor(color);
            foreach (Piece piece in pieces)
            {
                List<Position> candidates = piece.GetValidMoves(Board);
                foreach (Position to in candidates)
                {
                    Position from = piece.CurrentPosition;
                    bool wasMoved = piece.HasMoved;
                    Piece captured = Board.MovePiece(from, to);

                    bool kingSafe = !IsCheck(color);

                    Board.MovePiece(to, from);
                    piece.HasMoved = wasMoved;
                    if (captured != null)
                        Board.PlacePiece(captured, to);

                    if (kingSafe)
                        return true;
                }
            }
            return false;
        }
        private Position GetEnPassantTarget()
        {
            Movement lastMove = MoveHistory.LastOrDefault();

            if (lastMove == null) return null;

            if (lastMove.PieceMoved.PieceType == PieceType.Pawn)
            {
                
                int rowDiff = lastMove.From.Row - lastMove.To.Row; // verifica se andou duas casas, ou seja, diferenca de 2 linhas

                if (Math.Abs(rowDiff) == 2) //se andou duas casas 
                {
                    int middleRow = (lastMove.From.Row + lastMove.To.Row) / 2;
                    return new Position(middleRow, lastMove.From.Column);
                }
            }

            return null;
        }

    }
}




