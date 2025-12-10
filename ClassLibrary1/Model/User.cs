using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChessApp.Model.Model
{
    public class User
    {
        [JsonInclude] //para avisar o serializador que pode incluir esse atributo
        public string Username { get; private set; }
        [JsonInclude]
        public string Password { get; private set; }
        [JsonInclude]
        public int Wins { get; private set; }
        [JsonInclude]
        public int Losses { get; private set; }
        [JsonInclude]
        public int Draws { get; private set; }

        public User(string username, string password)
        {
            Username = username;
            Password = password;
            Wins = 0;
            Losses = 0;
            Draws = 0;
        }
        [JsonConstructor]
        private User() { } //construtor vazio para o serializador
        public void AddWin()
        {
            Wins++;
        }

        public void AddLoss()
        {
            Losses++;
        }

        public void AddDraw()
        {
            Draws++;
        }
    }
}
