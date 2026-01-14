using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp.Model.Enums
{
    /// <summary>
    /// Tipos de Moviementos especiais
    /// </summary>
    public enum MovementType
    {
        Normal,
        EnPassant,
        Castling,
        Promotion
    }
}
