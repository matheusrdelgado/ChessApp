using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessApp.Model.Enums;

namespace ChessApp.Model.Model
{
    public class Board
    {
        public Piece[,] Squares { get; set; }

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
            for(int row = 0; row < 8; row++)
            {
                for(int col = 0; col < 8; col++)
                {
                    Piece piece= Squares[row, col];
                    if(piece != null && piece.Color == color && piece.PieceType == PieceType.King)
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
                PlacePiece(new Pawn(Color.Black, 1, col));
            }
            PlacePiece(new Rook(Color.Black, 0, 0));
            PlacePiece(new Rook(Color.Black, 0, 7));
            PlacePiece(new Knight(Color.Black, 0, 1));
            PlacePiece(new Knight(Color.Black, 0, 6));
            PlacePiece(new Bishop(Color.Black, 0, 2));
            PlacePiece(new Bishop(Color.Black, 0, 5));
            PlacePiece(new King(Color.Black, 0, 4));
            PlacePiece(new Queen(Color.Black, 0, 3));

            for (int col = 0; col < 8; col++)
            {
                PlacePiece(new Pawn(Color.White, 6, col));
            }
            PlacePiece(new Rook(Color.White, 7, 0));
            PlacePiece(new Rook(Color.White, 7, 7));
            PlacePiece(new Knight(Color.White, 7, 1));
            PlacePiece(new Knight(Color.White, 7, 6));
            PlacePiece(new Bishop(Color.White, 7, 2));
            PlacePiece(new Bishop(Color.White, 7, 5));
            PlacePiece(new Queen(Color.White, 7, 3));
            PlacePiece(new King(Color.White, 7, 4));
        }

        #endregion
    }
}
