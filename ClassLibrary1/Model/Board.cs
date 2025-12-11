using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessApp.Model.Enums;
using System.Text;

namespace ChessApp.Model.Model
{
    public class Board
    {
        private Piece[,] Squares { get; set; }

        #region Constructor
        public Board() //construtor nao recebe parametros para iniciar o array dentro do construtor
        {
            Squares = new Piece[8, 8];
        }
        #endregion

        #region Methods
        /// <summary>
        /// Recebe uma posição e se for válida retorna a peça nessa posição
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Piece GetPiece(Position pos)
        {
            if (pos.IsValid())
            {
                return Squares[pos.Row, pos.Column];
            }
            else
            {
                throw new ArgumentException("Invalid position");
            }
        }
        /// <summary>
        /// O método coloca uma peça numa posição específica do tabuleiro se a posição for válida e não estiver ocupada
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="pos"></param>
        /// <exception cref="ArgumentException"></exception>
        public void PlacePiece(Piece piece, Position pos)
        {
            if (pos.IsValid())
            {
                if (Squares[pos.Row, pos.Column] == null)
                {
                    Squares[pos.Row, pos.Column] = piece;
                    piece.CurrentPosition = pos;
                }
                else
                {
                    throw new ArgumentException("Position occupied");
                }
            }
            else
            {
                throw new ArgumentException("Non existent position");
            }
        }

        /// <summary>
        /// Método para dar a posicao da peça para o outro PlacePiece
        /// </summary>
        /// <param name="piece"></param>
        public void PlacePiece(Piece piece)
        {
            PlacePiece(piece, piece.CurrentPosition);
        }

        /// <summary>
        /// Remove uma peça da posição recebida por parâmetro
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>retorna a peça removida para possibilitar "undo" de movimentos</returns>
        /// <exception cref="ArgumentException"></exception>
        public Piece RemovePiece(Position pos)
        {
            if (pos.IsValid())
            {
                if (Squares[pos.Row, pos.Column] != null)
                {
                    Piece tmp = Squares[pos.Row, pos.Column];
                    Squares[pos.Row, pos.Column] = null;
                    tmp.CurrentPosition = null;
                    return tmp;
                }
                return null;

            }
            else
            {
                throw new ArgumentException("Invalid Position");
            }



        }

        /// <summary>
        /// Move uma peça de uma posição para outra, recebendo ambas as posições como parâmetro
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>Retorna a peça capturada, se houver</returns>
        /// <exception cref="ArgumentException"></exception>
        public Piece MovePiece(Position from, Position to)
        {
            if (!from.IsValid())
            {
                throw new ArgumentException("Invalid 'from' position");
            }
            if (!to.IsValid())
            {
                throw new ArgumentException("Invalid 'to' position");
            }

            Piece pieceToMove = GetPiece(from);
            if (pieceToMove == null)
            {
                throw new ArgumentException("No piece found at the source position");
            }
            Piece captured = RemovePiece(to);
            RemovePiece(from);
            PlacePiece(pieceToMove, to);
            pieceToMove.HasMoved = true;
            return captured;

        }

        /// <summary>
        /// método para limpar o tabuleiro
        /// </summary>
        public void ClearBoard()
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Piece piece = Squares[row, col];
                    if (piece != null)
                    {
                        piece.CurrentPosition = null;
                    }
                    Squares[row, col] = null;
                }
            }
        }

        /// <summary>
        /// Retorna todas as peças da cor recebida como parâmetro
        /// </summary>
        /// <param name="color"></param>
        /// <returns>Lista de peças da cor especificada</returns>
        public List<Piece> GetAllPiecesOfColor(Color color)
        {
            List<Piece> pieces = new List<Piece>();
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Piece piece = Squares[row, col];
                    if (piece != null && piece.Color == color)
                    {
                        pieces.Add(piece);
                    }
                }
            }
            return pieces;
        }
        /// <summary>
        /// Método para obter a posição do rei da cor recebida como parâmetro
        /// </summary>
        /// <param name="color"></param>
        /// <returns>A posição do rei</returns>
        /// <exception cref="ArgumentException"></exception>
        public Position GetKingPosition(Color color)
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Piece piece = Squares[row, col];
                    if (piece != null && piece.Color == color && piece.PieceType == PieceType.King)
                    {
                        return piece.CurrentPosition;
                    }

                }
            }
            throw new ArgumentException("King not found");
        }

        /// <summary>
        /// Método para inicializar o tabuleiro e colocar as peças nas posições iniciais
        /// </summary>
        public void InitializeBoard()
        {
            ClearBoard();
            for (int col = 0; col < 8; col++)
            {
                PlacePiece(new Pawn(new Position(1, col), Color.Black)); //a posicao esta escrita duas vezes porque o PlacePiece pede a posicao como segundo parametro
            }
            PlacePiece(new Rook(new Position(0, 0), Color.Black));
            PlacePiece(new Knight(new Position(0, 1), Color.Black));
            PlacePiece(new Bishop(new Position(0, 2), Color.Black));
            PlacePiece(new Queen(new Position(0, 3), Color.Black));
            PlacePiece(new King(new Position(0, 4), Color.Black));
            PlacePiece(new Bishop(new Position(0, 5), Color.Black));
            PlacePiece(new Knight(new Position(0, 6), Color.Black));
            PlacePiece(new Rook(new Position(0, 7), Color.Black));

            for (int col = 0; col < 8; col++)
            {
                PlacePiece(new Pawn(new Position(6, col), Color.White));
            }
            PlacePiece(new Rook(new Position(7, 0), Color.White));
            PlacePiece(new Knight(new Position(7, 1), Color.White));
            PlacePiece(new Bishop(new Position(7, 2), Color.White));
            PlacePiece(new Queen(new Position(7, 3), Color.White));
            PlacePiece(new King(new Position(7, 4), Color.White));
            PlacePiece(new Bishop(new Position(7, 5), Color.White));
            PlacePiece(new Knight(new Position(7, 6), Color.White));
            PlacePiece(new Rook(new Position(7, 7), Color.White));
        }

        public string GetFen(Color CurrentTurn, Position enPassantSquare = null)
        {
            StringBuilder fen = new StringBuilder();

            for (int row = 0; row < 8; row++) //tabuleiro
            {
                int emptyCount = 0;
                for (int col = 0; col < 8; col++)
                {
                    Piece piece = Squares[row, col];
                    if (piece == null)
                    {
                        emptyCount++;
                    }
                    else
                    {
                        if (emptyCount > 0)
                        {
                            fen.Append(emptyCount);
                            emptyCount = 0;
                        }
                        char pieceChar = GetPieceChar(piece);
                        fen.Append(pieceChar);
                    }
                }
                if (emptyCount > 0) fen.Append(emptyCount);
                if (row < 7) fen.Append('/');
            }
            fen.Append(CurrentTurn == Color.White ? " w " : " b "); //quem joga

            string castling = "";

            Piece wKing = GetPiece(new Position(7, 4)); //rei branco
            if (wKing is King && !wKing.HasMoved)
            {
                Piece wRookKing = GetPiece(new Position(7, 7));
                Piece wRookQueen = GetPiece(new Position(7, 0));
                if (wRookKing is Rook && !wRookKing.HasMoved) castling += "K";
                if (wRookQueen is Rook && !wRookQueen.HasMoved) castling += "Q";
            }
            Piece bKing = GetPiece(new Position(0, 4)); //rei preto
            if (bKing is King && !bKing.HasMoved)
            {
                Piece bRookKing = GetPiece(new Position(0, 7));
                Piece bRookQueen = GetPiece(new Position(0, 0));
                if (bRookKing is Rook && !bRookKing.HasMoved) castling += "k";
                if (bRookQueen is Rook && !bRookQueen.HasMoved) castling += "q";
            }
            fen.Append(string.IsNullOrEmpty(castling) ? "-" : castling);
            fen.Append(" ");

            if (enPassantSquare != null)
            {
                fen.Append(enPassantSquare.ToChessNotation().ToLower());
            }
            else
            {
                fen.Append("-");
            }

            fen.Append(" 0 1");
            return fen.ToString();
        }
        private char GetPieceChar(Piece piece)
        {
            char c = ' ';
            switch (piece.PieceType)
            {
                case PieceType.Pawn: c = 'p'; break;
                case PieceType.Rook: c = 'r'; break;
                case PieceType.Knight: c = 'n'; break;
                case PieceType.Bishop: c = 'b'; break;
                case PieceType.Queen: c = 'q'; break;
                case PieceType.King: c = 'k'; break;
            }
            return piece.Color == Color.White ? char.ToUpper(c) : c;

            #endregion
        }
    }
}
