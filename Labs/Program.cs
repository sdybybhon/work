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


namespace Lab5
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите выражение:");
            string expression = Console.ReadLine(); //Считывая выражение

            List<Token> tokens = Tokenize(expression); //Работаю с методами 
            Queue<Token> rpn = ConvertToRPN(tokens); //Работаю с методами 

            double result = EvaluateRPN(rpn);
            Console.WriteLine("Результат: " + result); //Выводим результат)
        }

        // Метод для разбиения выражения на токены
        static List<Token> Tokenize(string expression)
        //Этим методом разбиваю выражение на токены и возвращаю список токенов.
        //Прохожу по каждому символу в выражении и определяет, является ли символ числом, операцией или скобкой.
        //Если число, то он добавляет его в список токенов типа Number. 
        //Если это операция или скобка, то он добавляет их в список токенов типа Operation или Parenthesis.
        {
            List<Token> tokens = new List<Token>(); //создаю пустой список токенов.
            string currentNumber = "";

            foreach (char c in expression) //пробегаюсь по каждому символу.
            {
                if (char.IsDigit(c) || c == '.') //проверяю, является ли символ цифрой или точкой.
                {
                    currentNumber += c;
                }
                else
                {
                    if (!string.IsNullOrEmpty(currentNumber)) //проверяю, является ли текущее число пустым или же относится к null.
                    {
                        double number = double.Parse(currentNumber);
                        tokens.Add(new Number(number));
                        currentNumber = "";
                    }

                    if (c == '(' || c == ')')
                    {
                        tokens.Add(new Parenthesis(c.ToString()));
                    }
                    else if (c == '+' || c == '-' || c == '*' || c == '/')
                    {
                        tokens.Add(new Operation(c.ToString(), GetPriority(c.ToString())));
                    }
                }
            }

            if (!string.IsNullOrEmpty(currentNumber))
            {
                double number = double.Parse(currentNumber);
                tokens.Add(new Number(number));
            }

            return tokens; // возвращаю список токенов.
        }

        // Метод для преобразования выражения в обратную польскую запись (ОПЗ)
        static Queue<Token> ConvertToRPN(List<Token> tokens)
        //Здесь просто преобразую список токенов в ОПЗ и возвращает очередь токенов.
        //это себе: ОБЪЯСНЕНИЕ ОПЗ: http://decoding.dax.ru/practic/polishrecord/polishrecord.html
        //Если токен является числом, то он просто добавляется в очередь. 
        //Если токен является операцией, то метод проверяет приоритет операции и сравнивает его с операциями, уже находящимися в стеке.
        //Если токен является скобкой, то метод проверяет, является ли она открывающей или закрывающей. 
        //И т.п, по итогу получим очередь ОПЗ
        {
            Queue<Token> rpn = new Queue<Token>(); //Создаю пустую очередь токенов для хранения ОПЗ.
            Stack<Token> stack = new Stack<Token>(); //Создаю пустой стек токенов для выполнения преобразований.

            foreach (Token token in tokens) //Пробегаюсь по всем токенам
            {
                if (token is Number)
                {
                    rpn.Enqueue(token);
                }
                else if (token is Operation)
                {
                    while (stack.Count > 0 && stack.Peek() is Operation && ((Operation)stack.Peek()).Priority >= ((Operation)token).Priority)
                    {
                        rpn.Enqueue(stack.Pop());
                    }

                    stack.Push(token);
                }
                else if (token is Parenthesis)
                {
                    if (token.Value == "(")
                    {
                        stack.Push(token);
                    }
                    else if (token.Value == ")")
                    {
                        while (stack.Count > 0 && !(stack.Peek() is Parenthesis) && ((stack.Peek() as Parenthesis)?.Value != "("))
                        {
                            rpn.Enqueue(stack.Pop());
                        }

                        if (stack.Count == 0 || !(stack.Peek() is Parenthesis) || ((stack.Peek() as Parenthesis)?.Value != "("))
                        {
                            throw new InvalidOperationException("Несогласованные скобки");
                        }

                        stack.Pop();
                    }
                }
            }

            while (stack.Count > 0)
            {
                if (stack.Peek() is Parenthesis)
                {
                    throw new InvalidOperationException("Несогласованные скобки");
                }

                rpn.Enqueue(stack.Pop());
            }

            return rpn;
        }

        static double EvaluateRPN(Queue<Token> rpn)
        //Здесь вычисляем то, что получили в ОПЗ. 
        {
            Stack<double> stack = new Stack<double>();

            while (rpn.Count > 0)
            {
                Token token = rpn.Dequeue();

                if (token is Number)
                {
                    stack.Push(((Number)token).Value);
                }
                else if (token is Operation)
                {
                    if (stack.Count < 2)
                    {
                        throw new InvalidOperationException("Неправильное количество операндов");
                    }

                    double operand2 = stack.Pop();
                    double operand1 = stack.Pop();
                    double result = PerformOperation(operand1, operand2, ((Operation)token).Value);
                    stack.Push(result);
                }
            }

            return stack.Pop();
        }

        static double PerformOperation(double operand1, double operand2, string operation)
        // Метод для выполнения маттмаческих операции
        {
            switch (operation)
            {
                case "+":
                    return operand1 + operand2;
                case "-":
                    return operand1 - operand2;
                case "*":
                    return operand1 * operand2;
                case "/":
                    return operand1 / operand2;
                default:
                    throw new InvalidOperationException("Недопустимая операция");
            }
        }

        static int GetPriority(string operation)
        // Метод для определения приоритета операции
        {
            switch (operation)
            {
                case "+":
                case "-":
                    return 1;
                case "*":
                case "/":
                    return 2;
                default:
                    return 0;
            }
        }
    }

    abstract class Token
    // Базовый класс для токенов
    {
        public string Value { get; protected set; }
    }

    class Number : Token
    // Класс для числовых токенов
    {
        public double Value { get; private set; }

        public Number(double value)
        {
            Value = value;
        }
    }

    class Operation : Token
    // Класс для токенов операций
    {
        public int Priority { get; private set; }

        public Operation(string value, int priority)
        {
            Value = value;
            Priority = priority;
        }
    }

    class Parenthesis : Token
    // Класс для токенов скобок
    {
        public Parenthesis(string value)
        {
            Value = value;
        }
    }
}