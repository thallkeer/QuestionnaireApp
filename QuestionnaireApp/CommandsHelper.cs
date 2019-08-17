using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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
            {HELP, (string.Empty,@"Показать список доступных команд с описанием") },
            {NEW_PROFILE, (string.Empty,@"Заполнить новую анкету") },
            {STATISTICS, (nameof(IOCommands.GetStatistics),@"Показать статистику всех заполненных анкет") },
            {SAVE, (nameof(IOCommands.Save),@"Сохранить заполненную анкету") },
            {GOTO_QUESTION, (string.Empty,@"n <Номер вопроса> - Вернуться к указанному вопросу
(Команда доступна только при заполнении анкеты, вводится вместо ответа на
любой вопрос)") },
            {GOTO_PREV_QUESTION, (string.Empty,@"Вернуться к предыдущему вопросу (Команда
доступна только при заполнении анкеты, вводится вместо ответа на любой
вопрос)") },
            {RESTART_PROFILE, (string.Empty,@"Заполнить анкету заново (Команда доступна только при
заполнении анкеты, вводится вместо ответа на любой вопрос)
") },
            {FIND, (nameof(IOCommands.FindQuestionary),@"<Имя файла анкеты> - Найти анкету и показать данные анкеты в
консоль
") },
            {DELETE, (nameof(IOCommands.DeleteQuestionary),@"<Имя файла анкеты> - Удалить указанную анкету") },
            {LIST, (nameof(IOCommands.ListQuestionaries),@"Показать список названий файлов всех сохранённых анкет") },
            {LIST_TODAY, (nameof(IOCommands.ListTodayQuestionaries),@"Показать список названий файлов всех сохранённых анкет,
созданных сегодня") },
            {ZIP, (nameof(IOCommands.Zip),@"<Имя файла анкеты> <Путь для сохранения архива> - Запаковать
указанную анкету в архив и сохранить архив по указанному пути") },
            {EXIT, (nameof(string.Empty),"Выйти из приложения") }
        };
        }        

        public static void PrintAvailableCommands()
        {
            foreach (KeyValuePair<string, (string method,string description)> kvp in CommandDescriptions)
            {
                Console.WriteLine($"cmd: {kvp.Key} - {kvp.Value.description}");
            }
        }

        public static string GetMethodByCommand(string commandName)
        {
            return CommandDescriptions[commandName].method;
        }

        public static bool IsCommand(string input)
        {
            return CommandDescriptions.ContainsKey(input);
        }

        

       
    }

    public static class StringExtensions
    {
        public static string ExtractCommand(this string str)
        {            
            return str.Split(' ')[0];
        }

        public static string[] ExtractArguments(this string str)
        {
            string[] res = str.Split(' ').Skip(1).ToArray();
            return res.Length == 0 ? null : res;
        }
    }
}
