using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ChessApp.Model.Model;
using System.Text.Json;
using ChessApp.Model.Interfaces;

namespace ChessApp.Model.Services
{
    public class GameFileService : IGameFileService
    {
        private readonly string GameDirectory;

        /// <summary>
        /// inicializa o servico de ficheiros de jogo
        /// </summary>
        public GameFileService()
        {
            GameDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves");
            Directory.CreateDirectory(GameDirectory);
        }

        /// <summary>
        /// Guarda o estado do jogo num ficheiro JSON
        /// </summary>
        /// <param name="game"></param>
        /// <param name="fileName"></param>
        public void SaveGame(Game game, string fileName)
        {
            if(!fileName.EndsWith(".json"))
                fileName += ".json";

            string filePath = Path.Combine(GameDirectory, fileName);

            var options = new JsonSerializerOptions { WriteIndented = true };

            string jsonString = JsonSerializer.Serialize(game.MoveHistory, options);
            File.WriteAllText(filePath, jsonString);
        }

        /// <summary>
        /// Carrega o estado do jogo a partir de um ficheiro JSON
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public Game LoadGame(string fileName)
        {
            if (!fileName.EndsWith(".json"))
                fileName += ".json";

            string filePath = Path.Combine(GameDirectory, fileName);
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found");

            string jsonString = File.ReadAllText(filePath);
            var movements = JsonSerializer.Deserialize<List<Movement>>(jsonString);
            Game game = new Game();
            foreach (var move in movements)
            {
                game.Board.MovePiece(move.From, move.To);
                game.MoveHistory.Add(move);
            }

            return game;
        }
    }
}
