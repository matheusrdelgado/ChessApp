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
        bool Register(string username, string password);
        User Login(string username, string password);
        void SaveUsers();
        void LoadUsers();
    }
}