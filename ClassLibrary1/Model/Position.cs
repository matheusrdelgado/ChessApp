using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp.Model.Model
{
    public class Position
    {
        #region Atributes
        public int Row {  get; set; }
        public int Column { get; set; }
        #endregion

        #region Constructor
        public Position(int row, int column)
        {
            Row = row;
            Column = column;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Método para verficar se a posição existe no tabuleiro
        /// </summary>
        /// <returns>Retorna verdadeiro se a posição é válida, falsa se o contrário</returns>
        public bool IsValid()
        {
            if (Row >= 0 && Row < 8 && Column >= 0 && Column < 8)
                return true;
            return false;
        }

        /// <summary>
        /// Método para comparar posições
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Retorna verdadeiro se forem iguais</returns>
        public override bool Equals(object? obj)
        {
            if(obj == null) return false;
            if (obj is Position)
            { //verifica se e uma posição
                Position other = (Position)obj; // e faz a conversao para uma variavel para poder comparar a row e column
                if (this.Row == other.Row && this.Column == other.Column) return true;
                return false;
            } return false;


        }

        /// <summary>
        /// método para gerar um hashcode único para cada posição e facilitar a localização
        /// </summary>
        /// <returns>Retorna o valor hash</returns>
        public override int GetHashCode()
        {
            int Hash = Row * 8 + Column; //gera um numero 0-63 para cada posicao
            return Hash;
        }

    }
}
