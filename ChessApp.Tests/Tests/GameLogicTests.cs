using Xunit;
using ChessApp.Model.Model;
using ChessApp.Model.Enums;
using System;
using System.Linq;

namespace ChessApp.Tests.Tests
{
    public class GameLogicTests
    {
        [Fact]
        public void NewGame_Should_InitializeBoardCorrectly() //basic game initiantion
        {
            var game = new Game();

            var whitePawns = game.Board.GetAllPiecesOfColor(Color.White).Count(p => p.PieceType == PieceType.Pawn);
            var blackKing = game.Board.GetKingPosition(Color.Black);

            Assert.Equal(8, whitePawns); 
            Assert.Equal(new Position(0, 4), blackKing); 
            Assert.Equal(Color.White, game.CurrentTurn); 
        }

        [Fact]
        public void Pawn_Should_Move_Forward_Two_Squares_On_First_Move()
        {
            var game = new Game();
            var from = new Position(6, 4); 
            var to = new Position(4, 4); 

            game.MakeMove(from, to);

            var pieceAtDestination = game.Board.GetPiece(to);
            Assert.NotNull(pieceAtDestination);
            Assert.Equal(PieceType.Pawn, pieceAtDestination.PieceType);
            Assert.Equal(Color.Black, game.CurrentTurn);
        }

        [Fact]
        public void Knight_Should_Jump_Over_Pieces()
        {
            var game = new Game();
            var from = new Position(7, 1); 
            var to = new Position(5, 2);   

            game.MakeMove(from, to);

            var knight = game.Board.GetPiece(to);
            Assert.NotNull(knight);
            Assert.Equal(PieceType.Knight, knight.PieceType);
        }

        [Fact]
        public void Player_Should_Not_Move_Opponent_Pieces()
        {
            var game = new Game(); 
            var blackPawnPos = new Position(1, 0);

            Assert.Throws<InvalidOperationException>(() =>
            {
                game.MakeMove(blackPawnPos, new Position(2, 0));
            });
        }

        [Fact]
        public void Castling_Should_Be_Allowed_Short_Castle()
        {
            var game = new Game();

            game.MakeMove(new Position(6, 4), new Position(4, 4));
            game.MakeMove(new Position(1, 0), new Position(2, 0));
            game.MakeMove(new Position(7, 5), new Position(4, 2));
            game.MakeMove(new Position(1, 1), new Position(2, 1));
            game.MakeMove(new Position(7, 6), new Position(5, 5));
            game.MakeMove(new Position(1, 2), new Position(2, 2));

            var kingStart = new Position(7, 4); 
            var kingDest = new Position(7, 6); 

            game.MakeMove(kingStart, kingDest);

            var king = game.Board.GetPiece(kingDest);
            var rook = game.Board.GetPiece(new Position(7, 5)); 

            Assert.Equal(PieceType.King, king.PieceType);
            Assert.Equal(PieceType.Rook, rook.PieceType);
        }

        [Fact]
        public void ScholarsMate_Should_Result_In_Checkmate()
        {
            var game = new Game();

            game.MakeMove(new Position(6, 4), new Position(4, 4));
            game.MakeMove(new Position(1, 4), new Position(3, 4));

            game.MakeMove(new Position(7, 3), new Position(3, 7));
            game.MakeMove(new Position(0, 1), new Position(2, 2));

            game.MakeMove(new Position(7, 5), new Position(4, 2));
            game.MakeMove(new Position(0, 6), new Position(2, 5));

            game.MakeMove(new Position(3, 7), new Position(1, 5));

            Assert.Equal(GameState.Checkmate, game.State);
            Assert.Equal(Color.Black, game.CurrentTurn);
        }
    }
}