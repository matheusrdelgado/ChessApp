using ChessApp.Model.Interfaces;
using ChessApp.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChessApp.Model.Interfaces;

namespace ChessApp.Model.Services
{
    public class UserService : IUserService
    {
        /// <summary>
        /// caminho do ficheiro onde os utilizadores são guardados
        /// </summary>
        private readonly string UsersFilePath;
        /// <summary>
        /// Lista de utilizadores registados
        /// </summary>
        public List<User> Users { get; private set; }
        /// <summary>
        /// Construtor da classe UserService
        /// </summary>
        public UserService()
        {
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves");
            Directory.CreateDirectory(folder);
            UsersFilePath = Path.Combine(folder, "users.json");
            LoadUsers();
        }

        /// <summary>
        /// Carrega os utilizadores do ficheiro JSON
        /// </summary>
        public void LoadUsers()
        {
            Users = new List<User>();

            if (File.Exists(UsersFilePath))
            {
                string json = File.ReadAllText(UsersFilePath);
                var loadedUsers = JsonSerializer.Deserialize<List<User>>(json);

                if (loadedUsers != null)
                {
                    Users = loadedUsers;
                }
            }
        }

        /// <summary>
        /// Guarda os utilizadores no ficheiro JSON
        /// </summary>
        public void SaveUsers()
        {
            var options = new JsonSerializerOptions //Configurações de serialização
            {
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize(Users, options);
            File.WriteAllText(UsersFilePath, json);
        }

        /// <summary>
        /// Regista um novo utilizador
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Register(string username, string password)
        {
            if (Users.Any(u => u.Username == username)) // Verifica se o utilizador ja existe
                return false;
            Users.Add(new User(username, password)); // Adiciona o novo utilizador a lista
            SaveUsers();
            return true;
        }

        /// <summary>
        /// Faz login
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public User Login(string username, string password)
        {
            return Users.FirstOrDefault(u => u.Username == username && u.Password == password); 
        }


    }
}
