using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp.Model.Enums
{
    /// <summary>
    /// Estados possiveis do jogo
    /// </summary>
    public enum GameState
    {
        Playing,
        Stalemate,
        Draw,
        Resigned,
        NotStarted,
        Checkmate
    }
}
