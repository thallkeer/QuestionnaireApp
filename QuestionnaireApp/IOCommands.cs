using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace QuestionnaireApp
{
    public static class IOCommands
    {
        private static readonly string CATALOG_NAME = "Profiles";
        private static DirectoryInfo RootDirectory;

        static IOCommands()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string combinedPath = Path.Combine(currentDirectory, CATALOG_NAME);

            RootDirectory = new DirectoryInfo(combinedPath);
            RootDirectory.Create();
        }

        public static void Save(Questionary questionary)
        {
            if (questionary == null)
                return;

            if (questionary.CompletionDate == null)
            {
                Console.WriteLine("Cannot save questionnaire because it is not completed!");
                return;
            }


            FileInfo fileInfo = new FileInfo(Path.Combine(RootDirectory.FullName, questionary.FIO + ".txt"));
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

        private static FileInfo GetFileInfoByPath(string path)
        {
            if (path == null || path.Length == 0)
            {
                throw new ArgumentNullException("path");
            }
            
            FileInfo fileInfo = new FileInfo(Path.Combine(RootDirectory.FullName, path + ".txt"));

            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("The file was not found.", path);
            }

            return fileInfo;
        }

        public static void FindQuestionary(string path)
        {
            FileInfo fileInfo = GetFileInfoByPath(path);
            using (var sr = fileInfo.OpenText())
                Console.WriteLine(Questionary.GetQuestionaryFromStream(sr).ToString());
        }

        public static void DeleteQuestionary(string path)
        {
            FileInfo fileInfo = GetFileInfoByPath(path);
            fileInfo.Delete();
        }
        
        public static void ListQuestionaries()
        {
            foreach (var file in RootDirectory.EnumerateFiles())
            {                
                Console.WriteLine(file.Name);
            }
        }

        //TODO: переделать на получение даты заполнения из файла
        public static void ListTodayQuestionaries()
        {
            foreach (var file in RootDirectory.EnumerateFiles())
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
            List<Questionary> questionaries = new List<Questionary>();
            foreach (var file in RootDirectory.EnumerateFiles())
            {
                using (var sr = file.OpenText())
                    questionaries.Add(Questionary.GetQuestionaryFromStream(sr));
            }

            Questionary.PrintStatistics(questionaries);
        }

        /// <summary>
        /// Method packs the specified file to the archive and saves it at the specified path
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="path"></param>
        public static void Zip(string fileName, string path)
        {           
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("path");

            FileInfo file = GetFileInfoByPath(fileName.Trim('"'));

            ZipArchive zipArchive;
            if (!File.Exists(path))
                zipArchive = ZipFile.Open(path, ZipArchiveMode.Create);
            else zipArchive = ZipFile.OpenRead(path);

            zipArchive.CreateEntryFromFile(file.DirectoryName, fileName + ".txt");
        }
    }
}
