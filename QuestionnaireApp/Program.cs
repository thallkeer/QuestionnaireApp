using System;
using System.Collections.Generic;
using System.Reflection;

namespace QuestionnaireApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Questionnaire";
            Console.WriteLine("Select an action. Type -help to see all available commands.");

            Questionary questionary = null;

            while (true)
            {
                string input = Console.ReadLine();
                if (!CommandsHelper.IsCommand(input))
                    continue;

                switch (input)
                {
                    case CommandsHelper.HELP:
                        CommandsHelper.PrintAvailableCommands();
                        break;
                    case CommandsHelper.NEW_PROFILE:
                        questionary = Questionary.StartQuestioning();
                        Console.WriteLine("Select an action. Type -help to see all available commands.");
                        break;                    
                    case CommandsHelper.EXIT:
                        Environment.Exit(0);
                        break;
                    default:
                        CommandsHelper.ExecuteCommand(input, questionary);
                        break;
                }
            }
        }
    }
}
