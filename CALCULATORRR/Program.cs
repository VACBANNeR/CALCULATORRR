using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using calculator;
using Newtonsoft.Json;

namespace calculator
{
    internal class Program
    {
        private static double GetNumberFromUser(string message)
        {
            double number;
            while (true)
            {
                Console.WriteLine(message);
                if (double.TryParse(Console.ReadLine(), out number))
                {
                    return number;
                }
                else
                {
                    Console.WriteLine("Ошибка!!! Введите корректное число.");
                }
            }
        }

        private static double PerformOperation(double a, double b, char oper)
        {
            double result = 0;

            switch (oper)
            {
                case '+':
                    result = a + b;
                    Console.WriteLine("" + a + "+" + b + " = " + result + ".");
                    break;
                case '-':
                    result = a - b;
                    Console.WriteLine("" + a + " - " + b + " = " + result + ".");
                    break;
                case '*':
                    result = a * b;
                    Console.WriteLine("" + a + " * " + b + " = " + result + ".");
                    break;
                case '/':
                    if (b == 0)
                    {
                        Console.WriteLine("Деление на 0 невозможно!/Division by 0 is impossible!");
                    }
                    else
                    {
                        result = a / b;
                        Console.WriteLine("" + a + " : " + b + " = " + result + ".");
                    }
                    break;
                case '^':
                    result = Math.Pow(a, b);
                    Console.WriteLine(a + " ^ " + b + " = " + result);
                    break;
                case 's':
                    result = Math.Sin(a);
                    Console.WriteLine("Sin  " + a + " = " + result + ".");
                    break;
                case 'c':
                    result = Math.Cos(a);
                    Console.WriteLine("Cos  " + a + " = " + result + ".");
                    break;
                case 't':
                    result = Math.Tan(a);
                    Console.WriteLine("Tan  " + a + " = " + result + ".");
                    break;
                case 'k':
                    result = 1 / Math.Tan(a);
                    Console.WriteLine("Ctg  " + a + " = " + result + ".");
                    break;
                default:
                    Console.WriteLine("Неизвестный оператор/Unknown operator.");
                    break;
            }

            return result;
        }

        public static void Main(string[] args)
        {
            // Читает файл с конфигурацией
            string json = File.ReadAllText("config.json");
            Config config = JsonConvert.DeserializeObject<Config>(json);

            // Выбор языка меню
            Console.WriteLine("Выберите язык / Select language:");
            Console.WriteLine("1. Русский");
            Console.WriteLine("2. English");
            int languageChoice = GetLanguageChoice();

            Menu selectedMenu = languageChoice == 1 ? config.MenuRU : config.MenuEN;

            // Меню выбора калькулятора
            Console.WriteLine(selectedMenu.SelectCalculator);
            Console.WriteLine("1. " + selectedMenu.Basic);
            Console.WriteLine("2. " + selectedMenu.Trigonometric);

            // Выбор пользователя
            int calculatorChoice = GetCalculatorChoice(selectedMenu);

            // Запуск калькулятора который выбрал пользователь
            if (calculatorChoice == 1)
            {
                RunCalculator(config.BasicOperations, selectedMenu);
            }
            else if (calculatorChoice == 2)
            {
                RunCalculator(config.TrigonometricOperations, selectedMenu);
            }
        }

        private static int GetLanguageChoice()
        {
            int languageChoice;
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out languageChoice) && (languageChoice == 1 || languageChoice == 2))
                {
                    return languageChoice;
                }
                else
                {
                    Console.WriteLine("Введите корректный номер языка/Enter the correct language number.");
                }
            }
        }

        private static int GetCalculatorChoice(Menu menu)
        {
            int calculatorChoice;
            while (true)
            {
                Console.WriteLine(menu.EnterCalculatorNumber);
                if (int.TryParse(Console.ReadLine(), out calculatorChoice) && (calculatorChoice == 1 || calculatorChoice == 2))
                {
                    return calculatorChoice;
                }
                else
                {
                    Console.WriteLine(menu.IncorrectCalculatorNumber);
                }
            }
        }

        private static void RunCalculator(List<Operation> operations, Menu menu)
        {
            while (true)
            {
                // Выводит меню с операциями для выбора
                StringBuilder operationMenu = new StringBuilder();
                operationMenu.AppendLine(menu.SelectOperation);
                int operationIndex = 1;
                foreach (var operation in operations)
                {
                    operationMenu.AppendLine($"{operationIndex++}. {operation.Description}");
                }
                Console.WriteLine(operationMenu);

                // Получить выбор операции пользователя
                int operationChoice = GetOperationChoice(operations.Count, menu);

                // Ввод чисел пользователя
                double a, b;
                if (operations[operationChoice - 1].Symbol != 's' &&
                    operations[operationChoice - 1].Symbol != 'c' &&
                    operations[operationChoice - 1].Symbol != 't' &&
                    operations[operationChoice - 1].Symbol != 'k')
                {
                    a = GetNumberFromUser(menu.EnterNumberA);
                    b = GetNumberFromUser(menu.EnterNumberB);
                }
                else
                {
                    a = GetNumberFromUser(menu.EnterNumberA);
                    b = 0;
                }

                // Выполнение операции
                double result = PerformOperation(a, b, operations[operationChoice - 1].Symbol);

                // Вывод результатов
                Console.WriteLine($"{menu.Result}: {result}");

                // Спросить пользователя, хочет ли он продолжить
                Console.WriteLine(menu.ContinuePrompt);
                if (Console.ReadLine() != menu.ContinueYes)
                    break;
            }
        }

        private static int GetOperationChoice(int operationsCount, Menu menu)
        {
            int operationChoice;
            while (true)
            {
                Console.WriteLine(menu.EnterOperationNumber);
                if (int.TryParse(Console.ReadLine(), out operationChoice) && (operationChoice >= 1 && operationChoice <= operationsCount))
                {
                    return operationChoice;
                }
                else
                {
                    Console.WriteLine(menu.IncorrectOperationNumber);
                }
            }
        }
    }

    public class Operation
    {
        public char Symbol { get; set; }
        public string Description { get; set; }

        public Operation(char symbol, string description)
        {
            Symbol = symbol;
            Description = description;
        }
    }

    public class Config
    {
        public List<Operation> BasicOperations { get; set; }
        public List<Operation> TrigonometricOperations { get; set; }
        public Menu MenuRU { get; set; }
        public Menu MenuEN { get; set; }
    }

    public class Menu
    {
        public string SelectCalculator { get; set; }
        public string Basic { get; set; }
        public string Trigonometric { get; set; }
        public string EnterCalculatorNumber { get; set; }
        public string IncorrectCalculatorNumber { get; set; }
        public string SelectOperation { get; set; }
        public string EnterOperationNumber { get; set; }
        public string IncorrectOperationNumber { get; set; }
        public string EnterNumberA { get; set; }
        public string EnterNumberB { get; set; }
        public string Result { get; set; }
        public string ContinuePrompt { get; set; }
        public string ContinueYes { get; set; }
        public string ContinueNo { get; set; }
    }
}

//CONFIG

    {
  "BasicOperations": [
    {
      "Symbol": "+",
      "Description": "Сложение/Addition"
    },
    {
    "Symbol": "-",
      "Description": "Вычитание/Subtraction"
    },
    {
    "Symbol": "*",
      "Description": "Умножение/Multiplication"
    },
    {
    "Symbol": "/",
      "Description": "Деление/Division"
    },
    {
    "Symbol": "^",
      "Description": "Возведение в степень/Exponentiation"
    }
  ],
  "TrigonometricOperations": [
    {
      "Symbol": "s",
      "Description": "Синус/Sine"
    },
    {
    "Symbol": "c",
      "Description": "Косинус/Сosine"
    },
    {
    "Symbol": "t",
      "Description": "Тангенс/Tangent"
    },
    {
    "Symbol": "k",
      "Description": "Котангенс/Cotangence"
    }
  ],
  "MenuRU": {
    "SelectCalculator": "Выберите тип калькулятора",
    "Basic": "Базовый",
    "Trigonometric": "Тригонометрический",
    "EnterCalculatorNumber": "Введите номер калькулятора",
    "IncorrectCalculatorNumber": "Неверный номер калькулятора",
    "SelectOperation": "Выберите операцию",
    "EnterOperationNumber": "Введите номер операции",
    "IncorrectOperationNumber": "Неверный номер операции",
    "EnterNumberA": "Введите число A",
    "EnterNumberB": "Введите число B",
    "Result": "Результат",
    "ContinuePrompt": "Хотите продолжить? (да/нет)",
    "ContinueYes": "да",
    "ContinueNo": "нет"

  },
  "MenuEN": {
    "SelectCalculator": "Select type of calculator",
    "Basic": "Basic",
    "Trigonometric": "Trigonometric",
    "EnterCalculatorNumber": "Enter calculator number",
    "IncorrectCalculatorNumber": "Incorrect calculator number",
    "SelectOperation": "Select an operation",
    "EnterOperationNumber": "Enter operation number",
    "IncorrectOperationNumber": "Incorrect operation number",
    "EnterNumberA": "Enter number A",
    "EnterNumberB": "Enter number B",
    "Result": "Result",
    "ContinuePrompt": "Do you want to continue? (yes/no)",
    "ContinueYes": "yes",
    "ContinueNo": "no"
  }
}