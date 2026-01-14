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
        #region Attributes
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
        #endregion
        #region Constructors
        public User(string username, string password)
        {
            Username = username;
            Password = password;
            Wins = 0;
            Losses = 0;
            Draws = 0;
        }
        [JsonConstructor] //indica que esse construtor deve ser usado pelo serializador
        #endregion
        #region Methods
        /// <summary>
        /// Construtor vazio para o serializador.
        /// </summary>
        private User() { }
        /// <summary>
        /// Acrescenta uma vitória ao utilizador.
        /// </summary>
        public void AddWin()
        {
            Wins++;
        }
        /// <summary>
        /// Acrescenta uma derrota ao utilizador.
        /// </summary>
        public void AddLoss()
        {
            Losses++;
        }
        /// <summary>
        /// Acrescenta um empate ao utilizador.
        /// </summary>
        public void AddDraw()
        {
            Draws++;
        }
        #endregion
    }
}
