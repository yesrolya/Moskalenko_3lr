﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moskalenko_3lr
{
    class Node
    {
        string leftSide;
        string rightSide;
        List<Node> nodes;
        bool solved = false;
        bool noSolution = false;
        
        public Node (string left, string right) {
            
            leftSide = DeleteImplication(left);
            leftSide = DeleteDoubleNot(leftSide);
            rightSide = DeleteImplication(right);
            rightSide = DeleteDoubleNot(rightSide);
            this.SplitSides(false);
            this.SplitSides(true);
            SortSides();
            CompareSides();
            nodes = new List<Node>();
            if (!solved)
                NextStep();
        }

        string DeleteImplication(string s)
        {
            string side = s;
            //импликация задается знаком -
            while (side.IndexOf('-') != -1)
            {
                var pos = side.IndexOf('-');
                var posNot = pos - 2;
                var len = 0;
                if (side[pos - 1] == ')')
                {
                    for (int i = pos - 1; i >= 0; i--)
                    {
                        len++;
                        if (side[i] == '(')
                        {
                            posNot = i;
                            break;
                        }
                    }
                    side = side.Substring(0, posNot) + '!'
                        + side.Substring(posNot, len) + '+' + side.Substring(pos + 1, side.Length - posNot - len - 1);
                }
                else
                {
                    side = side.Substring(0, pos - 1) + '!'
                        + side.Substring(pos - 1, 1) + '+' + side.Substring(pos + 1, side.Length - pos - 1);
                }
            }
            return side;
        }

        string DeleteDoubleNot (string s)
        {
            string side = s;
            //двойное отрицание задается !!
            while (side.IndexOf("!!") != -1) {
                var pos = side.IndexOf("!!");
                side = side.Substring(0, pos) + side.Substring(pos + 2, side.Length - pos - 2);
            }
            return side;
        }

        void SplitSides (bool left)
        {
            //3) редактирование
            char splitter = (left ? '*' : '+');
            for (int count = 0; count < 5; count++)
            {
                string temp = "";
                //сначала делим строку по знаку
                string[] side = new string [(left? 
                    leftSide.Split(splitter, ',').Length: 
                    rightSide.Split(splitter, ',').Length)];
                side = (left ?
                    leftSide.Split(splitter, ',') :
                    rightSide.Split(splitter, ','));

                //заменяем знаки запятыми
                for (int i = 0; i < side.Length; i++)
                {
                    int balance = 0;
                    if (side[i].IndexOf('(') == -1)
                    {
                        //соединяем строки без скобок
                        temp += side[i] + ',';
                    }
                    else
                    {
                        //для строк со скобками объединяем до достижения баланса
                        string t = side[i];
                        for (int j = 0; j < side[i].Length; j++)
                        {
                            if (side[i][j] == '(') balance += 1;
                            if (side[i][j] == ')') balance += -1;
                        }
                        while (balance != 0)
                        {
                            i++;
                            t += splitter + side[i];
                            for (int j = 0; j < side[i].Length; j++)
                            {
                                if (side[i][j] == '(') balance += 1;
                                if (side[i][j] == ')') balance += -1;
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
            foreach (var s in sr)
            {
                temp += s + ',';
            }
            rightSide = temp.Substring(0, temp.Length - 1);
        }

        void CompareSides()
        {
            if (rightSide == leftSide)
            {
                solved = true;
                Console.WriteLine("YAHOO");
            }
                Console.WriteLine(leftSide + '=' + rightSide);

        }

        void NextStep ()
        {
            if (this.IsNot())
            {
                var temp = DeleteNot();
                var t = temp.Split('=');
                //Console.WriteLine(t[0] + '=' + t[1]);
                nodes.Add(new Node(t[0], t[1]));
            }

            List<string> reduct = new List<string>();
            reduct = Reduction();
            if (reduct.Count() != 0)
                foreach (var r in reduct)
                {
                    var t = r.Split('=');
                    if (t[0] != leftSide || t[1] != rightSide)
                        //Console.WriteLine(t[0] + '=' + t[1]);
                        nodes.Add(new Node(t[0], t[1]));
                }
            if (nodes == null)
                noSolution = true;
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

        List<string> Reduction()
        {
            string initial = leftSide + '=' + rightSide;

            List<string> solution = new List<string>();
            int pos1 = 0, pos2;
            int currentPos = leftSide.IndexOf('+');
            if (currentPos == -1) currentPos = rightSide.IndexOf('*') + leftSide.Length + 1;
            //ЯРЧАЙШИЙ ПРИМЕР ГОВНОКОДА 
            //СМОТРЕТЬ НИЖЕ
            while (currentPos != -1)
            {
                char splitter = (currentPos <= leftSide.Length ? '+' : '*');
                //находим положение запятых вокруг первого '+'
                for (int k = currentPos; k > 0; k--)
                {
                    if (initial[k] == ',')
                    {
                        pos1 = k;
                        break;
                    }
                }
                pos2 = initial.IndexOf(',', currentPos);
                if (pos2 == -1) pos2 = initial.Length - 1;
                int balance = 0;
                string temp = "";

                foreach (string str in initial.Substring(pos1 + (pos1 == 0? 0: 1), pos2 - pos1 - (pos1 == 0 ? 0 : 1)).Split(splitter))
                {
                    if (str.IndexOf('(') != -1 || str.IndexOf(')') != -1 || balance != 0)
                    {
                        for (int y = 0; y < str.Length; y++)
                        {
                            if (str[y] == '(') balance += 1;
                            if (str[y] == ')') balance -= 1;
                        }
                        temp += str + splitter;
                    }
                    else
                    {
                        temp = str + ' ';
                    }
                    if (balance == 0)
                    {
                        solution.Add(
                        (pos1 != 0? initial.Substring(0, pos1 + 1): "") +
                        temp.Substring(0, temp.Length-1) +
                        initial.Substring(pos2, initial.Length - pos2)
                        );
                        temp = "";
                        balance = 0;
                    }
                }
                currentPos = initial.IndexOf('+', pos2);
                if (currentPos == -1 || currentPos > leftSide.Length) currentPos = initial.IndexOf('*', pos2);
            }

            
            return solution;
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
            var side = "f*(q)*(p*((o*o)-!q))*pp*(x+((p+o)-s)+i)";
            string leftSide = "q*(p+!q)*(!p+s)";
            string rightSide = "s";

            Node n = new Node(leftSide, rightSide);


            Console.WriteLine(side);
            Console.ReadKey();
        }
    }
}
