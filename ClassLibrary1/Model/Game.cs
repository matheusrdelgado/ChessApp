using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp.Model.Model
{
    public class Game
    {
        public Board Board { get; set; }
        public List<Piece> WhiteCapturedPieces { get; private set; } = new List<Piece>();
        public List<Piece> BlackCapturedPieces { get; private set; } = new List<Piece>();


    }




