using ChessApp.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChessApp.Model.Services
{
    public class UserService
    {
        private readonly string UsersFilePath;
        public List<User> Users { get; private set; }
        public UserService()
        {
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves");
            Directory.CreateDirectory(folder);
            UsersFilePath = Path.Combine(folder, "users.json");
            LoadUsers();
        }

        private void LoadUsers()
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

        public void SaveUsers()
        {
            var options = new JsonSerializerOptions //Configurações de serialização
            {
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize(Users, options);
            File.WriteAllText(UsersFilePath, json);
        }

        public bool Register(string username, string password)
        {
            if (Users.Any(u => u.Username == username))
                return false;
            Users.Add(new User(username, password));
            SaveUsers();
            return true;
        }

        public User Login(string username, string password)
        {
            return Users.FirstOrDefault(u => u.Username == username && u.Password == password); 
        }


    }
}
