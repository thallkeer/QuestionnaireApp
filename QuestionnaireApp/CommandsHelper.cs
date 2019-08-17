using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;

namespace QuestionnaireApp
{
    public static class CommandsHelper
    {
        public const string HELP = "-help";
        public const string NEW_PROFILE = "-new_profile";
        public const string STATISTICS = "-statistics";
        public const string SAVE = "-save";
        public const string GOTO_QUESTION = "-goto_question";
        public const string GOTO_PREV_QUESTION = "-goto_prev_question";
        public const string RESTART_PROFILE = "-restart_profile";
        public const string FIND = "-find";
        public const string DELETE = "-delete";
        public const string LIST = "-list";
        public const string LIST_TODAY = "-list_today";
        public const string ZIP = "-zip";
        public const string EXIT = "-exit";

        public readonly static Dictionary<string, (string method, string commandDescription)> CommandDescriptions;

        static CommandsHelper()
        {
            CommandDescriptions = new Dictionary<string, (string method, string commandDescription)>
        {
            {HELP, (string.Empty,@"Show a list of available commands with a description") },
            {NEW_PROFILE, (string.Empty,@"Fill out a new questionnaire") },
            {STATISTICS, (nameof(IOCommands.GetStatistics),@"Show statistics for all completed questionnaires") },
            {SAVE, (nameof(IOCommands.Save),@"Save the completed questionnaire") },
            {GOTO_QUESTION, (string.Empty,@"<Question number> - Return to the specified question
(The command is available only when filling out the questionnaire, it is entered instead of answer to
any question)") },
            {GOTO_PREV_QUESTION, (string.Empty,@"Return to the previous question (The command is
available only when filling out the questionnaire, it is entered instead of the answer to any
question)") },
            {RESTART_PROFILE, (string.Empty,@"Fill out the questionnaire again (The command is
available only when filling out the questionnaire, it is entered instead of the answer to any
question)") },
            {FIND, (nameof(IOCommands.FindQuestionary),@"""<Filename>"" - Find questionnaire and show it's data in
console") },
            {DELETE, (nameof(IOCommands.DeleteQuestionary),@"""<Filename>"" - Delete the specified questionnaire") },
            {LIST, (nameof(IOCommands.ListQuestionaries),@"Show a list of file names of all saved questionnaires") },
            {LIST_TODAY, (nameof(IOCommands.ListTodayQuestionaries),@"Show a list of file names of all saved questionnaires, created today") },
            {ZIP, (nameof(IOCommands.Zip),@"""<Filename>"" <Path to save> - To pack the specified questionnaire in the archive and save the archive at the specified path") },
            {EXIT, (nameof(string.Empty),"Exit application") }
        };
        }        

        public static void PrintAvailableCommands()
        {
            foreach (KeyValuePair<string, (string method,string commandDescription)> kvp in CommandDescriptions)
            {
                Console.WriteLine($"cmd: {kvp.Key} - {kvp.Value.commandDescription}");
            }
        }

        public static string GetMethodByCommand(string commandName)
        {
            return CommandDescriptions[commandName].method;
        }

        public static bool IsCommand(string input)
        {
            return CommandDescriptions.ContainsKey(input.ExtractCommand());
        }

        public static void ExecuteCommand(string input, Questionary questionary)
        {
            Type t = typeof(IOCommands);
            MethodInfo mi = t.GetMethod(GetMethodByCommand(input.ExtractCommand()));
            if (mi == null)
                return;
            //TODO: Fix problem with arguments 
            ParameterInfo[] parameters = mi.GetParameters();
            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(Questionary))
                mi.Invoke(null, new object[] { questionary });
            else
                mi.Invoke(null, input.ExtractArguments());
        }
    }

    public static class StringExtensions
    {
        public static string ExtractCommand(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            return str.Split(' ')[0];
        }

        public static string[] ExtractArguments(this string str)
        {
            int quoteIndex = str.IndexOf('"');
            if (quoteIndex != -1)
            {

            }

            string[] res = str.Split(' ').Skip(1).ToArray();
            return res.Length == 0 ? null : res;
        }
    }
}
