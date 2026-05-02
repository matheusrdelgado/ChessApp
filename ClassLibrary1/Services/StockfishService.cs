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
        /// <summary>
        /// Processo do motor Stockfish
        /// </summary>
        private Process stockfishProcess;

        /// <summary>
        /// Inicializa o serviço Stockfish com o caminho para o executável do motor
        /// </summary>
        /// <param name="pathToCheckfishExe"></param>
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
        }

        /// <summary>
        /// obtem o melhor movimento para a posicao FEN fornecida
        /// </summary>
        /// <param name="fen"></param>
        /// <returns></returns>
        public async Task<string> GetBestMoveAsync(string fen, int skillLevel,  int moveTimeMs)
        {
            await stockfishProcess.StandardInput.WriteLineAsync($"setoption name Skill Level value {skillLevel}");
            await stockfishProcess.StandardInput.WriteLineAsync($"position fen {fen}"); //envia a posicao atual
            await stockfishProcess.StandardInput.WriteLineAsync($"go movetime {moveTimeMs}"); //movetime um dos parametros de dificuldade da engine

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
        /// <summary>
        /// Fecha o processo do Stockfish
        /// </summary>
        public void Close()
        {
            if (!stockfishProcess.HasExited)
            {
                stockfishProcess.StandardInput.WriteLine("quit");
                stockfishProcess.Close();
            }
        }
    }
}
