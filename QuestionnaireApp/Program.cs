using System;
using System.Collections.Generic;

namespace QuestionnaireApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Questionnaire";
            Console.WriteLine("Select an action. Type -help to see all available commands.");

            Questionary questionary=null;

            while (true)
            {                
                string command = Console.ReadLine();
                switch (command)
                {
                    case CommandsHelper.HELP:
                        CommandsHelper.PrintAvailableCommands();
                        break;
                    case CommandsHelper.NEW_PROFILE:
                        questionary = Questionary.StartQuestioning();
                        Console.WriteLine("Select an action. Type -help to see all available commands.");
                        break;
                    case CommandsHelper.SAVE:
                        if (questionary != null)
                            IOCommands.Save(questionary);
                        break;
                    case CommandsHelper.LIST:
                        IOCommands.ListQuestionaries();
                        break;                    
                    case CommandsHelper.LIST_TODAY:
                        IOCommands.ListTodayQuestionaries();
                        break;
                    case CommandsHelper.STATISTICS:
                        IOCommands.GetStatistics();
                        break;
                    case CommandsHelper.EXIT:
                        Environment.Exit(0);
                        break;
                }
            }
        }
    }
}
