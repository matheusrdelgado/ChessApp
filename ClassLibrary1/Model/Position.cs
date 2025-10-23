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
        public bool IsValid()
        {
            if (Row >= 0 && Row < 8 && Column >= 0 && Column < 8)
                return true;
            return false;
        }

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

        public override int GetHashCode()//usado para localizar rapidamente objetos iguais e nenhuma posicao diferente pode gerar o mesmo numero
        {
            int Hash = Row * 8 + Column; //gera um numero 0-63 para cada posicao
            return Hash;
        }

    }
}
