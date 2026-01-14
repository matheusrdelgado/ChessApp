using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessApp.Model.Model;

namespace ChessApp.Model.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Metodo para registar um novo utilizador
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool Register(string username, string password);
        /// <summary>
        /// Metodo para fazer login de um utilizador
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        User Login(string username, string password);
        /// <summary>
        /// Metodo para guardar os utilizadores num ficheiro
        /// </summary>
        void SaveUsers();
        /// <summary>
        /// Metodo para carregar os utilizadores de um ficheiro
        /// </summary>
        void LoadUsers();
    }
}