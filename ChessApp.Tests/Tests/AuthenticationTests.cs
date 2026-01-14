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

        /// <summary>
        /// incializa o caminho do ficheiro de teste e garante que qualquer arquivo existente seja removido antes dos testes.
        /// </summary>
        public AuthenticationTests()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            _testSavePath = Path.Combine(baseDir, "Saves", "users.json");

            if (File.Exists(_testSavePath))
            {
                File.Delete(_testSavePath);
            }
        }
        /// <summary>
        /// Teste para registar um novo utilizador e verificar se os dados são salvos corretamente
        /// </summary>
        [Fact] // fact é um atributo do xunit que indica que este método é um teste
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

        /// <summary>
        /// Teste para efetuar login com credenciais corretas
        /// </summary>
        [Fact]
        public void Login_Should_Fail_With_Wrong_Password()
        {
            var service = new UserService();
            service.Register("HackerAlvo", "senha_secreta");

            var result = service.Login("HackerAlvo", "senha_errada");

            Assert.Null(result);
        }

        /// <summary>
        /// Teste para efetuar login com um utilizador que não existe
        /// </summary>
        [Fact]
        public void Login_Should_Fail_If_User_Does_Not_Exist()
        {
            var service = new UserService();

            var result = service.Login("Fantasma", "123");

            Assert.Null(result);
        }

        /// <summary>
        /// Teste para prevenir o registo de utilizadores duplicados
        /// </summary>
        [Fact]
        public void Register_Should_Prevent_Duplicate_Users()
        {
            var service = new UserService();
            service.Register("Duplicado", "123");

            bool result = service.Register("Duplicado", "456");

            Assert.False(result);
        }

        /// <summary>
        /// Teste para garantir que os dados persistem entre diferentes instâncias do serviço
        /// </summary>
        [Fact]
        public void Data_Should_Persist_Between_Service_Instances()
        {
            var service1 = new UserService();
            service1.Register("Persistente", "abc");

            var service2 = new UserService();
            var user = service2.Login("Persistente", "abc");

            Assert.NotNull(user);
        }

        /// <summary>
        /// Limpa o ficheiro de teste apos a execução dos testes
        /// </summary>
        public void Dispose()
        {
            if (File.Exists(_testSavePath))
            {
                File.Delete(_testSavePath);
            }
        }
    }
}