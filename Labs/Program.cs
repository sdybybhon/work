using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace lab4
{
    public class Program
    {
        public static void Main()
        //Здесь я запрашиваю у пользователя математическое выражение, после вызываю методы для преобразования выражения и вывожу ответ. 
        {
            Console.WriteLine("Введите математическое выражение:");
            string expression = Console.ReadLine(); //Считывая выражение

            List<object> tokens = GetTokens(expression); //Работаю с методами 
            List<object> rpnTokens = ConvertToRPN(tokens); //Работаю с методами 
            double result = EvaluateRPN(rpnTokens); //Работаю с методами 

            Console.WriteLine("Результат: " + result); //Выводим результат)
        }

        public static List<object> GetTokens(string expression)
        //Этот метод принимает выражение пользователя и преобразует его с список токенов. 
        //Далее метод пробегается по всему выражению и каждый его символ закидывает(число или оператор) закидывает в список токенов.
        //Если символ это цифра, то он попадает в токены. 
        //Если символ это оператор, то текущий токен добавляет в список токенов, а после оператор добавляется в список токенов.
        //По итогу, если ластовый токен является числом, он также добавляется в список токенов.
        //Дальше смотрим метод ConvertToRPN.
        //PS: Если символ является `(` или `)`, он добавляется в список токенов. 
        {
            List<object> tokens = new List<object>(); //создаем список, в который будем добавлять токены
            string currentToken = "";

            foreach (char c in expression) //проходимся по каждому символу в выражении и смотреть на объяснение метода выше
            {
                if (Char.IsDigit(c))
                {
                    currentToken += c;
                }
                else if (c == '+' || c == '-' || c == '*' || c == '/')
                {
                    if (!String.IsNullOrEmpty(currentToken))
                    {
                        tokens.Add(double.Parse(currentToken));
                        currentToken = "";
                    }
                    tokens.Add(c);
                }
                else if (c == '(' || c == ')')
                {
                    if (!String.IsNullOrEmpty(currentToken)) //проверяем, не является ли текущий токен пустым.
                    {
                        tokens.Add(double.Parse(currentToken));
                        currentToken = "";
                    }
                    tokens.Add(c); //  возвращаем список токенов.
                }
            }

            if (!String.IsNullOrEmpty(currentToken))
            {
                tokens.Add(double.Parse(currentToken));
            }

            return tokens;
        }

        public static List<object> ConvertToRPN(List<object> tokens)
        //Принимает список токенов и работаем с ОПЗ.                        Это себе: (ЕСЛИ ЗАБЫЛ, ТО СМОТРЕТЬ http://decoding.dax.ru/practic/polishrecord/polishrecord.html)
        //Юзаем стек, чтобы раскидать всё по приоритетам.                   Смотреть метод GetPrecedence
        //Если попадается оператор, то метод проверяет приоритет оператора. 
        //Если оператор на вершине стека имеет больший или равный приоритет, то закидывает операторы из стека в конечный список, пока это вообще возмножно.
        //Затем текущий оператор добавляется в стек
        //Ну и в конце концов, все оставшиеся операторы из стека добавляются в конечный список.
        {
            List<object> output = new List<object>(); //создаем список, в который будем добавлять токены в ОПЗ
            Stack<object> operatorStack = new Stack<object>(); //наш стек

            foreach (object token in tokens) //пробегаемся по всем токенм
            {
                if (token is double)
                {
                    output.Add(token);
                }
                else if (token is char)
                {
                    char op = (char)token;

                    if (op == '(')
                    {
                        operatorStack.Push(op);
                    }
                    else if (op == ')')
                    {
                        while (operatorStack.Count > 0 && (char)operatorStack.Peek() != '(')
                        {
                            output.Add(operatorStack.Pop());
                        }

                        if (operatorStack.Count > 0 && (char)operatorStack.Peek() == '(')
                        {
                            operatorStack.Pop();
                        }
                    }
                    else
                    {
                        while (operatorStack.Count > 0 && (char)operatorStack.Peek() != '(' && GetPrecedence(op) <= GetPrecedence((char)operatorStack.Peek()))
                        {
                            output.Add(operatorStack.Pop());
                        }

                        operatorStack.Push(op);
                    }
                }
            }

            while (operatorStack.Count > 0) //проверяем, пока в стеке операторов есть элементы
            {
                output.Add(operatorStack.Pop());
            }

            return output; //возвращаем список RPN токенов.
        }
        public static double EvaluateRPN(List<object> rpnTokens)
        //Здесь мы уже получили список токенов в ОПЗ.                        Это себе: (ЕСЛИ ЗАБЫЛ, ТО СМОТРЕТЬ http://decoding.dax.ru/practic/polishrecord/polishrecord.html)
        //Здесь юзаю стек операндов для хранения чисел. 
        //Если токен в моём списке это число то оно добавляется в стек операндов.
        //Если токен в моём списке это оператор,то  метод извлекает два операнда из стека.
        //Затем выполяет математическую операцию и добавляет результат обратно в стек операндов.
        //Ну и в конце у нас результат вычисления выражения. Оно находится на вершине стека операндов и возвращается.
        {
            Stack<double> operandStack = new Stack<double>(); //создаем стек, в который будем добавлять операнды

            foreach (object token in rpnTokens) //проходимся по каждому токену в списке RPN токенов
            {
                if (token is double)
                {
                    operandStack.Push((double)token);
                }
                else if (token is char)
                {
                    char op = (char)token;

                    double operand2 = operandStack.Pop();
                    double operand1 = operandStack.Pop();

                    double result = 0;

                    switch (op)
                    {
                        case '+':
                            result = operand1 + operand2;
                            break;
                        case '-':
                            result = operand1 - operand2;
                            break;
                        case '*':
                            result = operand1 * operand2;
                            break;
                        case '/':
                            result = operand1 / operand2;
                            break;
                    }

                    operandStack.Push(result);
                }
            }

            return operandStack.Pop();
        }

        public static int GetPrecedence(char op)
        //Это используется в методе ConvertToRPN
        //Здесь принимаем оператор и возвращаем его приоритет.
        //У операторов "+" и "-" приоритет 1. 
        //У операторов "*" и "/" приоритет 2. 
        //Если оператор не является ни одним из указанных, возвращается 0.
        {
            if (op == '+' || op == '-')
            {
                return 1;
            }
            else if (op == '*' || op == '/')
            {
                return 2;
            }

            return 0;
        }
    }
}