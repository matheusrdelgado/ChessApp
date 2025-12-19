using Xunit;
using ChessApp.Model.Services;
using ChessApp.Model.Model;
using System.IO;
using System;

namespace ChessApp.Tests.Tests
{
    public class AuthenticationTests : IDisposable
    {
        private readonly string _testSavePath;

        public AuthenticationTests()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            _testSavePath = Path.Combine(baseDir, "Saves", "users.json");

            if (File.Exists(_testSavePath))
            {
                File.Delete(_testSavePath);
            }
        }

        [Fact]
        public void Register_Should_Create_New_User_And_Save_To_File()
        {
            var service = new UserService();
            string username = "TestPlayer";
            string password = "123";

            service.Register(username, password);

            var loggedUser = service.Login(username, password);
            Assert.NotNull(loggedUser);
            Assert.Equal(username, loggedUser.Username);

            Assert.True(File.Exists(_testSavePath));
        }

        [Fact]
        public void Login_Should_Fail_With_Wrong_Password()
        {
            var service = new UserService();
            service.Register("HackerAlvo", "senha_secreta");

            var result = service.Login("HackerAlvo", "senha_errada");

            Assert.Null(result);
        }

        [Fact]
        public void Login_Should_Fail_If_User_Does_Not_Exist()
        {
            var service = new UserService();

            var result = service.Login("Fantasma", "123");

            Assert.Null(result);
        }

        [Fact]
        public void Register_Should_Prevent_Duplicate_Users()
        {
            var service = new UserService();
            service.Register("Duplicado", "123");

            bool result = service.Register("Duplicado", "456");

            Assert.False(result);
        }

        [Fact]
        public void Data_Should_Persist_Between_Service_Instances()
        {
            var service1 = new UserService();
            service1.Register("Persistente", "abc");

            var service2 = new UserService();
            var user = service2.Login("Persistente", "abc");

            Assert.NotNull(user);
        }

        public void Dispose()
        {
            if (File.Exists(_testSavePath))
            {
                File.Delete(_testSavePath);
            }
        }
    }
}