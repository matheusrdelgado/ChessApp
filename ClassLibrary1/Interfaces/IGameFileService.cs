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
        /// <summary>
        /// metodo para guardar o estado atual do jogo num ficheiro
        /// </summary>
        /// <param name="game"></param>
        /// <param name="fileName"></param>
        void SaveGame(Game game, string fileName);
        /// <summary>
        /// Metodo para carregar um jogo a partir de um ficheiro
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Game LoadGame(string fileName);
    }
}
