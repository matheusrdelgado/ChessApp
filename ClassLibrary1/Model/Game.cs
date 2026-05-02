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
        #region Properties
        public Board Board { get; private set; }
        public List<Piece> WhiteCapturedPieces { get; private set; } = new List<Piece>();
        public List<Piece> BlackCapturedPieces { get; private set; } = new List<Piece>();
        public List<Movement> MoveHistory { get; private set; } = new List<Movement>();
        public Color CurrentTurn { get; private set; } = Color.White;
        public GameState State { get; private set; } = GameState.Playing;
        #endregion

        #region Constructor
        public Game()
        {
            Board = new Board();
            Board.InitializeBoard();
        }
        #endregion
        #region Methods

        /// <summary>
        /// Faz um movimento de uma peça do tabuleiro de xadrez.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void MakeMove(Position from, Position to)
        {
            if (State != GameState.Playing)
                throw new InvalidOperationException("Game is not in playing state.");

            Piece piece = Board.GetPiece(from);

            if (piece == null)
                throw new InvalidOperationException("No piece at source position.");

            if (piece.Color != CurrentTurn)
                throw new InvalidOperationException("Not this piece's turn.");

            Board.EnPassantTarget = GetEnPassantTarget();


            if (!piece.CanMoveTo(to, Board)) //logica para o movimento normal
                throw new InvalidOperationException("Invalid move.");

            if (piece is King king && Math.Abs(to.Column - from.Column) == 2)//logica para o castling
            {
                if (to.Column > from.Column)
                {
                    if (IsCheck(CurrentTurn))
                        throw new InvalidOperationException("Cannot castle while in check.");

                    Position middle = new Position(from.Row, (from.Column + to.Column) / 2);
                    if (IsSquareAttacked(middle, CurrentTurn == Color.White ? Color.Black : Color.White)) //verifica se a casa entre o rei e a torre esta sendo atacada
                        throw new InvalidOperationException("Cannot castle through check.");

                    if (IsSquareAttacked(to, CurrentTurn == Color.White ? Color.Black : Color.White)) //verifica o outro lado
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

            if (piece is Pawn && from.Column != to.Column && Board.GetPiece(to) == null) //logica para o en passant
            {
                Movement lastMove = MoveHistory.LastOrDefault(); //pega o ultimo movimento
                if (lastMove != null && //verifica se o ultimo movimento foi um peao que andou duas casas
                    lastMove.PieceMoved is Pawn &&
                    Math.Abs(lastMove.From.Row - lastMove.To.Row) == 2 &&
                    lastMove.To.Column == to.Column &&
                    lastMove.To.Row == from.Row)
                {
                    Position enemyPawnPos = lastMove.To; //posicao do peao inimigo que sera capturado
                    Piece capturedPawn = Board.RemovePiece(enemyPawnPos); //remove o peao inimigo do tabuleiro

                    Board.MovePiece(from, to); //move o peao para a posicao de destino

                    if (IsCheck(CurrentTurn)) //verifica se o movimento nao deixa o rei em cheque
                    {
                        Board.MovePiece(to, from);
                        Board.PlacePiece(capturedPawn, enemyPawnPos);
                        throw new InvalidOperationException("Move results in self-check.");
                    }

                    if (CurrentTurn == Color.White) //adiciona o peao capturado na lista de pecas capturadas
                        WhiteCapturedPieces.Add(capturedPawn);
                    else 
                        BlackCapturedPieces.Add(capturedPawn);

                    MoveHistory.Add(new Movement(piece, from, to, capturedPawn, MovementType.EnPassant, to.ToChessNotation())); //adiciona o movimento ao historico
                    SwitchTurn();
                    return;
                }
            }
            bool wasMoved = piece.HasMoved; //armazena o estado anterior de HasMoved
            Piece capturedPiece = Board.MovePiece(from, to); //realiza o movimento e captura a peca se houver

            if (IsCheck(CurrentTurn)) //verifica se o movimento nao deixa o rei em cheque
            {
                // Reverte o Movimento
                Board.MovePiece(to, from);
                piece.HasMoved = wasMoved;
                if (capturedPiece != null) //se houve captura, restaura a peca capturada
                {
                    Board.PlacePiece(capturedPiece, to);
                }
                throw new InvalidOperationException("Move results in self-check.");
            }


            if (capturedPiece != null) //adiciona a peca capturada na lista de pecas capturadas
            {
                if (CurrentTurn == Color.White)
                    BlackCapturedPieces.Add(capturedPiece);
                else
                    WhiteCapturedPieces.Add(capturedPiece);
            }

            MovementType type = MovementType.Normal; //tipo de movimento padrao
            PieceType? promotion = null;


            if (piece is Pawn pawn && pawn.CanPromote()) //fazer depois a opcao de escolher a peca!!!
            {
                type = MovementType.Promotion;
                promotion = PieceType.Queen;

                Piece queen = new Queen(to, piece.Color); //cria a nova peca (rainha)
                Board.RemovePiece(to);
                Board.PlacePiece(queen, to);
                piece = queen;
            }

            Movement move = new Movement(piece, from, to, capturedPiece, type, to.ToChessNotation());
            move.PromotionPiece = promotion;
            MoveHistory.Add(move);

            SwitchTurn();
        }

        /// <summary>
        /// Retorna a representação FEN atual do tabuleiro.
        /// </summary>
        /// <returns></returns>
        public string GetCurrentFen()
        {
            Position epTarget = GetEnPassantTarget(); //pega a posicao alvo do en passant, se houver, eptarget é null se nao houver

            return Board.GetFen(CurrentTurn, epTarget); //gera a string FEN com a posicao alvo do en passant
        }

        /// <summary>
        /// Verifica se o rei da cor especificada está em cheque.
        /// </summary>
        /// <param name="kingColor"></param>
        /// <returns></returns>
        public bool IsCheck(Color kingColor)
        {
            Position kingPos = Board.GetKingPosition(kingColor);
            Color opponentColor = kingColor == Color.White ? Color.Black : Color.White;
            return IsSquareAttacked(kingPos, opponentColor);
        }

        /// <summary>
        /// Verifica se uma determinada posição está sendo atacada por peças de uma cor específica.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="attackerColor"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Alterna o turno entre as cores branca e preta.
        /// </summary>
        private void SwitchTurn()
        {
            CurrentTurn = CurrentTurn == Color.White ? Color.Black : Color.White;
            CheckForGameOver();
        }

        /// <summary>
        /// Verifica se o jogo terminou em xeque-mate ou empate.
        /// </summary>
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

        /// <summary>
        /// Verifica se a cor especificada tem movimentos legais disponíveis.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private bool HasLegalMoves(Color color)
        {
            List<Piece> pieces = Board.GetAllPiecesOfColor(color);
            foreach (Piece piece in pieces)
            {
                List<Position> candidates = piece.GetValidMoves(Board); //obtem todos os movimentos validos da peca
                foreach (Position to in candidates) //para cada movimento valido é testado se o rei fica em cheque apos o movimento
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

        /// <summary>
        /// Obtém a posição alvo do en passant com base no último movimento.
        /// </summary>
        /// <returns></returns>
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
        #endregion

    }
}




