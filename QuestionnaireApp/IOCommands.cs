using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QuestionnaireApp
{
    public static class IOCommands
    {
        private const string catalogName = "Анкеты";

        private static DirectoryInfo GetOrCreateRootDirectory()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string combinedPath = Path.Combine(currentDirectory, catalogName);

            DirectoryInfo directoryInfo = new DirectoryInfo(combinedPath);
            if (!Directory.Exists(combinedPath))
                directoryInfo.Create();
            return directoryInfo;
        }

        public static void Save(Questionary questionary)
        {
            var directoryInfo = GetOrCreateRootDirectory();

            FileInfo fileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, questionary.FIO + ".txt"));
            if (!fileInfo.Exists)
            {
                try
                {
                    using (var sw = fileInfo.CreateText())                    
                        sw.Write(questionary.ToString());                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public static void FindQuestionary(string path)
        {
            var directoryInfo = GetOrCreateRootDirectory();
            FileInfo fileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, path + ".txt"));
            using (var sr = fileInfo.OpenText())
                Console.WriteLine(Questionary.GetQuestionaryFromStream(sr).ToString());
        }

        public static void DeleteQuestionary(string path)
        {

        }
        
        public static void ListQuestionaries()
        {
            var directoryInfo = GetOrCreateRootDirectory();
            foreach (var file in directoryInfo.EnumerateFiles())
            {                
                Console.WriteLine(file.Name);
            }
        }

        //TODO: переделать на получение даты заполнения из файла
        public static void ListTodayQuestionaries()
        {
            var directoryInfo = GetOrCreateRootDirectory();
            foreach (var file in directoryInfo.EnumerateFiles())
            {
                if (file.LastWriteTime.Date == DateTime.Today.Date)
                    Console.WriteLine(file.Name);
            }
        }

/*
Нужно вывести в консоль следующие данные:
1. Средний возраст всех опрошенных: <Посчитать средний возраст всех тех, кто
заполнял анкеты, целое число> (год, года, лет в зависимости от полученного
числа, т.е если средний возраст получился 22, то вывести 22 года, если 25, то
25 лет итд)
2. Самый популярный язык программирования: <Название языка
программирования, который большинство пользователей указали как
любимый>
3. Самый опытный программист: <ФИО человека, у которого указан самый
большой опыт работы>
*/
        public static void GetStatistics()
        {
            var directoryInfo = GetOrCreateRootDirectory();
            List<Questionary> questionaries = new List<Questionary>();
            foreach (var file in directoryInfo.EnumerateFiles())
            {
                using (var sr = file.OpenText())
                    questionaries.Add(Questionary.GetQuestionaryFromStream(sr));
            }

            if (questionaries.Count != 0)
            {
                int averageAge = (int)questionaries.Select(q => Utils.GetAge(q.DateOfBirth)).Average();

                string ageName = String.Empty;
                switch (averageAge%10)
                {
                    case 1:
                        ageName = "год";
                        break;
                    case 2:
                    case 3:
                    case 4:
                        ageName = "года";
                        break;
                    default:
                        ageName = "лет";
                        break;
                }
                
                string mostPopularLanguage = questionaries.GroupBy(q => q.FavouriteLanguage)
                                                          .OrderByDescending(g => g.Count())
                                                          .First().Key;
                string mostExperienced = questionaries.OrderByDescending(q => q.Experience).First().FIO;

                Console.WriteLine($"Средний возраст опрошенных: {averageAge} {ageName}");
                Console.WriteLine($"Самый популярный язык программирования: {mostPopularLanguage}");
                Console.WriteLine($"Самый опытный программист: {mostExperienced}");
            }
        }

        public static void Zip(string fileName, string path)
        {

        }
    }
}
