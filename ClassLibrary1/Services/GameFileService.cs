using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ChessApp.Model.Model;
using System.Text.Json;

namespace ChessApp.Model.Services
{
    public class GameFileService
    {
        private readonly string GameDirectory;

        public GameFileService()
        {
            GameDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves");
            Directory.CreateDirectory(GameDirectory);
        }

        public void SaveGame(Game game, string fileName)
        {
            if(!fileName.EndsWith(".json"))
                fileName += ".json";

            string filePath = Path.Combine(GameDirectory, fileName);

            var options = new JsonSerializerOptions { WriteIndented = true };

            string jsonString = JsonSerializer.Serialize(game.MoveHistory, options);
            File.WriteAllText(filePath, jsonString);
        }

        public Game LoadGame(string fileName, Movement? movement)
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
                game.Board.MovePiece(move.From, move.To);
            return game;
        }
    }
}
