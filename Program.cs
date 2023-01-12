using System;
using System.Linq;

namespace ConsoleApp3
{
    class Program
    {
        static void Main()
        {
            //задаем функцию в виде мест, где стоят 1
            int[] func = { 1, 2, 3 };
            string[] basis = { "&&", "-" };

            //табл. ист.
            var truthTable = CreateTruthTable(func);

            //если базис И, НЕ, ИЛИ - то выводим любую из НФ
            var perfectNormalForm = "";
            if (basis.Contains("&&") && basis.Contains("||") && basis.Contains("-"))
            {
                perfectNormalForm = CreatePNF(truthTable, "&&");
                Console.WriteLine(perfectNormalForm);
            }
            //если базис И, НЕ - надо превратить ИЛИ в И и НЕ по формуле НЕ (НЕ х1 И НЕ х2)
            else if (basis.Contains("&&") && basis.Contains("-"))
            {
                perfectNormalForm = CreatePNF(truthTable, "&&");
                perfectNormalForm = ReplaceFunc(perfectNormalForm, "||");
                Console.WriteLine(perfectNormalForm);
            }
            //если базис ИЛИ, НЕ - надо превратить И в ИЛИ и НЕ по формуле НЕ (НЕ х1 ИЛИ НЕ х2)
            else if (basis.Contains("||") && basis.Contains("-"))
            {
                perfectNormalForm = CreatePNF(truthTable, "||");
                perfectNormalForm = ReplaceFunc(perfectNormalForm, "&&");
                Console.WriteLine(perfectNormalForm);
            }
        }

        /// <summary>
        /// Возвращает табл. ист. для функции func, перечисляющий места единиц в строках табл. 
        /// </summary>
        public static int [,] CreateTruthTable(int [] func)
        {
            //считаем степень двойки покрывающую эти места (для табл. истинности)
            int degree = 0;
            while (Math.Pow(2, degree) <= func[^1])
                degree++;

            //табл. ист.
            int[,] truthTable = new int[(int)Math.Pow(2, degree), degree + 1];
            //сначала в нее запишем все коды Грея
            for (int i = 0; i < Math.Pow(2, degree); i++)
            {
                for (int k = 0; k < degree; k++)
                {
                    string result = Convert.ToString(i, 2); // конвертация
                    var formatted = result.PadLeft(degree, '0'); //добавление 0 в начало
                    var array = formatted.Select(x => int.Parse(x.ToString())).ToArray();//распарсиваем двоичное число на массив цифр
                    truthTable[i, k] = array[k];
                }
            }

            //ставим 1 в табл. ист. на места функции равные 1
            foreach (var k in func)
            {
                truthTable[k, degree] = 1;
            }

            return truthTable;
        }
        /// <summary>
        /// Возвращает совершенную дизъюнктивную нормальную форму или 
        /// соверешенную конъюнктивную нормальную форму в зависимости от 2 аргумента
        /// <param name="truthTable"> таблица истинности функции </param>
        /// <param name="str"> может быть "&&" или "||" 
        /// - флаг конъюнктивная или дизъюнктивная форма</param>
        /// </summary>
        public static string CreatePNF(int[,] truthTable, string str)
        {
            int flag = 0;
            if (str == "&&") { flag = 1; }
            var perfectNormalForm = "";
            for (int i = 0; i < truthTable.GetLength(0); i++)
            {
                for (int k = 0; k < truthTable.GetLength(1) - 1; k++)
                {
                    int lastColumnNum = truthTable.GetLength(1) - 1;
                    if (truthTable[i, lastColumnNum] == flag)
                    {
                        if (truthTable[i, k] == flag)
                            perfectNormalForm += "x" + k.ToString();
                        else
                            perfectNormalForm += "-x" + k.ToString();
                        if (k != lastColumnNum - 1)
                            perfectNormalForm += " && ";
                        if ((k == lastColumnNum - 1))
                            perfectNormalForm += " || ";
                    }

                }
            }
            perfectNormalForm = perfectNormalForm.Remove(perfectNormalForm.Length - 4);
            return perfectNormalForm;
        }
        /// <summary>
        /// 1. Заменить ИЛИ через И и НЕ 
        /// 2. Заменить И через ИЛИ и НЕ
        /// В зависимости от флага
        /// </summary>
        /// <param name="perfectNormalForm"> сднф или скнф в завистимости от флага</param>
        /// <param name="flag"> заменяемый логический оператор </param>
        /// <returns></returns>
        public static string ReplaceFunc(string perfectNormalForm, string flag)
        {
            //разделить нормальную форму на куски склеенные И или ИЛИ
            var clauses = perfectNormalForm.Split(" " + flag + " ");
            var contrFlag = "&&";
            if (flag == "&&") contrFlag = "||";
            string formattedFunc;

            //если базис И, НЕ - надо превратить ИЛИ в И и НЕ по формуле НЕ (НЕ х1 И НЕ х2)
            //если базис ИЛИ, НЕ - надо превратить И в ИЛИ и НЕ по формуле НЕ (НЕ х1 ИЛИ НЕ х2)
            for (int i = 0; i < clauses.Length - 1; i++)
            {
                formattedFunc = "- ( - ( " + clauses[i] + " ) " + contrFlag + " - ( " + clauses[i + 1] + " ) )";
                clauses[i + 1] = formattedFunc;

            }
            return clauses[clauses.Length - 1];
        }
    }
}
