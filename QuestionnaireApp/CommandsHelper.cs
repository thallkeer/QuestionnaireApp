using System;
using System.Collections.Generic;
using System.Text;

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

        public static void PrintAvailableCommands()
        {
            foreach (KeyValuePair<string, string> kvp in CommandDescriptions)
            {
                Console.WriteLine($"cmd: {kvp.Key} - {kvp.Value}");
            }
        }

        public static bool IsCommand(string input)
        {
            return input.StartsWith('-') && CommandsHelper.CommandDescriptions.ContainsKey(input);
        }

        public readonly static Dictionary<string, string> CommandDescriptions = new Dictionary<string, string>
        {
            {HELP, @"Показать список доступных команд с описанием" },
            {NEW_PROFILE, @"Заполнить новую анкету" },
            {STATISTICS, @"Показать статистику всех заполненных анкет" },
            {SAVE, @"Сохранить заполненную анкету" },
            {GOTO_QUESTION, @"n <Номер вопроса> - Вернуться к указанному вопросу
(Команда доступна только при заполнении анкеты, вводится вместо ответа на
любой вопрос)" },
            {GOTO_PREV_QUESTION, @"Вернуться к предыдущему вопросу (Команда
доступна только при заполнении анкеты, вводится вместо ответа на любой
вопрос)" },
            {RESTART_PROFILE, @"Заполнить анкету заново (Команда доступна только при
заполнении анкеты, вводится вместо ответа на любой вопрос)
" },
            {FIND, @"<Имя файла анкеты> - Найти анкету и показать данные анкеты в
консоль
" },
            {DELETE, @"<Имя файла анкеты> - Удалить указанную анкету" },
            {LIST, @"Показать список названий файлов всех сохранённых анкет" },
            {LIST_TODAY, @"Показать список названий файлов всех сохранённых анкет,
созданных сегодня" },
            {ZIP, @"<Имя файла анкеты> <Путь для сохранения архива> - Запаковать
указанную анкету в архив и сохранить архив по указанному пути" },
            {EXIT, "Выйти из приложения" }
        };
    }
}
