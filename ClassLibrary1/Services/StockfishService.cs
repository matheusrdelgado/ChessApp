using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp.Model.Services
{
    public class StockfishService
    {
        private Process stockfishProcess;

        public StockfishService(string pathToCheckfishExe)
        {
            stockfishProcess = new Process();
            stockfishProcess.StartInfo.FileName = pathToCheckfishExe;
            stockfishProcess.StartInfo.UseShellExecute = false;
            stockfishProcess.StartInfo.RedirectStandardInput = true;
            stockfishProcess.StartInfo.RedirectStandardOutput = true;
            stockfishProcess.StartInfo.CreateNoWindow = true;
            stockfishProcess.Start();

            //inicia o motor do stockfish
            stockfishProcess.StandardInput.WriteLine("uci");

            public async Task<string> GetBestMoveAsync(string fen)
            {
            await stockfishProcess.StandardInput.WriteLineAsync($"position fen {fen}"); //envia a posicao atual
            await stockfishProcess.StandardInput.WriteLineAsync("go movetime 500"); //manda a engine pensar 500ms (depois definir aqui a dificuldade do bot)

            string line;
            while ((line = await stockfishProcess.StandardOutput.ReadLineAsync()) != null) //le a resopsta ate encontrar o bestmove
            {
                if (line.StartsWith("bestmove"))
                {
                    string[] parts = line.Split(' ');
                    return parts[1];

                }
            }
            return null;
            }
        public void Close() //fecha o processo do stockfish
        {
            if (!stockfishProcess.HasExited)
            {
                stockfishProcess.StandardInput.WriteLine("quit");
                stockfishProcess.Close();
            }
        }
    }
}
