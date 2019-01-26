using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moskalenko_3lr
{
    class Tree
    {

    }

    class Node
    {
        string leftSide;
        string rightSide;
        List<Node> nodes;
        Node rootNode;
        bool solved = false;
        bool noSolution = false;
        
        public Node (string left, string right) {
            //в конструкторе надо сразу решения возможные находить
            //чтобы дальше они рекурсивно создавали следующие ветки
            //также прописать, что не нужно создавать листья при решенном или отсутствии решения
        }

        void DeleteImplication (string side)
        {
            //импликация задается знаком -
            while (side.IndexOf('-') != -1)
            {
                var pos = side.IndexOf('-');
                side = side.Substring(0, pos - 1) + '!'
                    + side.Substring(pos - 1, 1) + '+' + side.Substring(pos + 1, side.Length - pos - 1);
            }
        }

        void DeleteDoubleNot (string side)
        {
            //двойное отрицание задается !!
            while (side.IndexOf("!!") != -1) {
                var pos = side.IndexOf("!!");
                side = side.Substring(0, pos) + side.Substring(pos + 2, side.Length - pos - 2);
            }
        }

        void SplitSide (bool left)
        {
            //3) редактирование
            var splitter = '+';
            if (left)
                splitter = '*';
            for (int count = 0; count < 5; count++)
            {
                string temp = "";
                //сначала делим строку по знаку
                string[] ls;
                if (left)
                    ls = leftSide.Split(splitter, ',');
                else
                    ls = rightSide.Split(splitter, ',');

                //заменяем знаки запятыми
                for (int i = 0; i < ls.Length; i++)
                {
                    int balance = 0;
                    if (ls[i].IndexOf('(') == -1)
                    {
                        //соединяем строки без скобок
                        temp += ls[i] + ',';
                    }
                    else
                    {
                        //для строк со скобками объединяем до достижения баланса
                        string t = ls[i];
                        for (int j = 0; j < ls[i].Length; j++)
                        {
                            if (ls[i][j] == '(') balance += 1;
                            if (ls[i][j] == ')') balance += -1;
                        }
                        while (balance != 0)
                        {
                            i++;
                            t += splitter + ls[i];
                            for (int j = 0; j < ls[i].Length; j++)
                            {
                                if (ls[i][j] == '(') balance += 1;
                                if (ls[i][j] == ')') balance += -1;
                            }
                        }

                        if (t[0] == '(' && t[t.Length - 1] == ')')
                            t = t.Substring(1, t.Length - 2);

                        temp += t + ',';
                    }
                }
                if (left)
                    leftSide = temp.Substring(0, temp.Length - 1);
                else
                    rightSide = temp.Substring(0, temp.Length - 1);
            }

            SortSides();
        }
        
        void SortSides ()
        {
            var sl = leftSide.Split(',');
            Array.Sort(sl);
            string temp = "";
            foreach (var s in sl)
            {
                temp += s + ',';
            }
            leftSide = temp.Substring(0, temp.Length - 1);

            var sr = rightSide.Split(',');
            Array.Sort(sr);
            temp = "";
            foreach (var s in sl)
            {
                temp += s + ',';
            }
            rightSide = temp.Substring(0, temp.Length - 1);
            CompareSides();
        }

        void CompareSides()
        {
            if (rightSide == leftSide) solved = true;
        }

        void NextStep ()
        {
            if (this.IsNot())
            {
                var temp = DeleteNot();
                var t =  temp.Split('=');

                nodes.Add(new Node(t[0], t[1]));
            }
            //КАРОЧ ТОП СТРАТА
            //СНАЧАЛА ПРОВЕРЯЕМ НАШ ЭКЗЕМПЛЯР НА ВОЗМОЖНЫЕ ДЕЙСТВИЯ
            //ПОТОМ СОЗДАЕМ ЭКЗЕМПЛЯР ОТ ЛЧ И ПЧ, ПОЛУЧЕННЫХ ИЗ ФУНКЦИЙ ЭТИХ ДЕЙСТВИЙ
            //КОГДА ОН БУДЕТ СОЗДАН РЕКУРСИВНО ЗАПУСКАЮТСЯ ТЕ ЖЕ ФУНКЦИИ НО ДЛЯ ИЗМЕНЕННОГО
            //ПРИ СОЗДАНИИ, В КОНСТРКТОРЕ, НАДО ЭТИ ФУНКЦИИ ЗАПУСКАТЬ
            //ПОТОМ И ССЫЛОЧКИ МОЖНО ДОБАВИТЬ В СПИСОК ПОТОМКОВ

            //2) диаметральная инверсия
            //по первому знаку 
            //удаление первого знака
            //удаление следующей за ! скобки и последней
            //перенос в противоположную сторону
            //сортировка

            //4) дихотомическая редукция


            //5) остановка
            //если ни одно нельзя решить
        }

        bool Stop ()
        {
            //ничего с этим больше не сделать?
            SortSides();
            if (solved == true)
                return true;
            else if (leftSide.IndexOf('+') == -1 && leftSide.IndexOf('*') == -1 && leftSide.IndexOf('!') == -1
                && leftSide.IndexOf('-') == -1)
                return true;
            else
                return false;
        }

        //поиск отрицаний
        bool IsNot()
        {
            foreach (var s in leftSide.Split(','))
            {
                if (s[0] == '!' && ((s[1] == '(' && s[s.Length - 1] == ')') || (s.Length == 2)))
                    return true;
            }
            foreach (var s in rightSide.Split(','))
            {
                if (s[0] == '!' && ((s[1] == '(' && s[s.Length - 1] == ')') || (s.Length == 2)))
                    return true;
            }
            
            return false;
        }

        string DeleteNot ()
        {
            //2) диаметральная инверсия
            var sl = leftSide.Split(',');
            var sr = rightSide.Split(',');
            List<string> left = new List<string>();
            List<string> right = new List<string>();
            foreach (var s in sl)
            {
                if (s[0] == '!')
                {
                    string temp = s;
                    if (s[1] == '(' && s[s.Length - 1] == ')')
                    {
                        temp = s.Substring(2, s.Length - 3);
                    }
//КОСТЫЛЬ, МАТЬ
                    else if (s.Length == 2)
                    {
                        temp = s.Substring(1,s.Length - 1);
                    }
                    right.Add(temp);
                }
                else
                {
                    left.Add(s);
                }
            }

            foreach (var s in sr)
            {
                if (s[0] == '!')
                {
                    string temp = s;
                    if (s[1] == '(' && s[s.Length - 1] == ')')
                    {
                        temp = s.Substring(2, s.Length - 3);
                    }
                    else if (s.IndexOf('+') == -1 && s.IndexOf('-') == -1 && s.IndexOf('*') == -1)
                    {
                        temp = s.Substring(1, s.Length - 1);
                    }
                    left.Add(temp);
                }
                else
                {
                    right.Add(s);
                }
            }
            string tLeft = left[0];
            string tRight = right[0];

            for (int i = 1; i < left.Count(); i++)
                tLeft += ',' + left[i];

            for (int i = 1; i < right.Count(); i++)
                tRight += ',' + right[i];

            return tLeft + "=" + tRight;
        }

    }
    class Program
    {
        static void Main(string[] args)
        {
            var leftSide = "f*(q)*(p*(o+!q))*pp*(p-s)";
            var rightSide = "q+((r))";
            bool left = true;
            var splitter = '+';
            if (left)
                splitter = '*';
            for (int count = 0; count < 3; count++)
            {
                string temp = "";
                //сначала делим строку по знаку
                string[] ls;
                if (left)
                    ls = leftSide.Split(splitter, ',');
                else
                    ls = rightSide.Split(splitter, ',');
                for (int i = 0; i < ls.Length; i++)
                {
                    int balance = 0;
                    if (ls[i].IndexOf('(') == -1)
                    {
                        //соединяем строки без скобок
                        temp += ls[i] + ',';
                    }
                    else
                    {
                        //для строк со скобками объединяем до достижения баланса
                        string t = ls[i];
                        for (int j = 0; j < ls[i].Length; j++)
                        {
                            if (ls[i][j] == '(') balance += 1;
                            if (ls[i][j] == ')') balance += -1;
                        }
                        while (balance != 0)
                        {
                            i++;
                            t += splitter + ls[i];
                            for (int j = 0; j < ls[i].Length; j++)
                            {
                                if (ls[i][j] == '(') balance += 1;
                                if (ls[i][j] == ')') balance += -1;
                            }
                        }

                        if (t[0] == '(' && t[t.Length - 1] == ')')
                            t = t.Substring(1, t.Length - 2);

                        temp += t + ',';
                    }
                }
                if (left)
                    leftSide = temp.Substring(0, temp.Length - 1);
                else
                    rightSide = temp.Substring(0, temp.Length - 1);
            }
            var sl = leftSide.Split(',');
            Array.Sort(sl);
            string tem = "";
            foreach (var s in sl)
            {
                tem += s + ',';
            }
            leftSide = tem.Substring(0, tem.Length - 1);
            Console.WriteLine(leftSide);
            Console.ReadKey();
        }
    }
}
