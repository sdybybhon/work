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
        {
            Console.WriteLine("Введите математическое выражение:");
            string expression = Console.ReadLine(); 

            List<object> tokens = GetTokens(expression); 
            List<object> rpnTokens = ConvertToRPN(tokens);  
            double result = EvaluateRPN(rpnTokens); 

            Console.WriteLine("Результат: " + result); 
        }

        public static List<object> GetTokens(string expression)
        //Пробегаюсь по всему выражению и каждый его символ закидывает(число или оператор) закидывает в список токенов.
        //Если символ это цифра, то он попадает в токены. 
        //Если символ это оператор, то текущий токен добавляет в список токенов, а после оператор добавляется в список токенов.
        //Если символ является `(` или `)`, он добавляется в список токенов. 
        {
            List<object> tokens = new List<object>(); 
            string currentToken = "";

            foreach (char c in expression) 
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
                    if (!String.IsNullOrEmpty(currentToken)) 
                    {
                        tokens.Add(double.Parse(currentToken));
                        currentToken = "";
                    }
                    tokens.Add(c);
                }
            }

            if (!String.IsNullOrEmpty(currentToken))
            {
                tokens.Add(double.Parse(currentToken));
            }

            return tokens;
        }

        public static List<object> ConvertToRPN(List<object> tokens)
        //Принимает список токенов и работаем с ОПЗ.                       
        //Юзаем стек, чтобы раскидать всё по приоритетам
        {
            List<object> output = new List<object>(); 
            Stack<object> operatorStack = new Stack<object>(); 

            foreach (object token in tokens) 
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

            while (operatorStack.Count > 0) 
            {
                output.Add(operatorStack.Pop());
            }

            return output; 
        }
        public static double EvaluateRPN(List<object> rpnTokens)
        //Здесь мы уже получили список токенов в ОПЗ.                       
        //Здесь юзаю стек операндов для хранения чисел. 
        //Если токен в моём списке это число то оно добавляется в стек операндов.
        //Если токен в моём списке это оператор,то  метод извлекает два операнда из стека.
        //Затем выполяет математическую операцию и добавляет результат обратно в стек операндов.
        {
            Stack<double> operandStack = new Stack<double>(); 

            foreach (object token in rpnTokens) 
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
        //Для ConvertToRPN
        //Здесь принимаем оператор и возвращаем его приоритет.
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
