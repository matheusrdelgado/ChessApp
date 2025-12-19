using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessApp.Model.Model;

namespace ChessApp.Model.Interfaces
{
    public interface IGameFileService
    {
        void SaveGame(Game game, string fileName);
        Game LoadGame(string fileName);
    }
}
