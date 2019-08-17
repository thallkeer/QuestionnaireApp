using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;

namespace QuestionnaireApp
{
    public class Questionary
    {
        private static readonly string dateFormat = "dd.mm.yyyy";
        private class AskResult
        {
            public bool IsValidAnswer { get; set; }
            public string AnswerOrCommand { get; set; }            

            public AskResult(bool valid, string answerOrCommand)
            {
                IsValidAnswer = valid;
                AnswerOrCommand = answerOrCommand;
            }            
        }
        private class AnswerHandlersItem
        {
            public Func<AskResult> AskFunc { get; private set; }
            public Action<string> AnswerHandler { get; private set; }

            public AnswerHandlersItem(Func<AskResult> askFunc, Action<string> answerHandler)
            {
                this.AskFunc = askFunc;
                this.AnswerHandler = answerHandler;
            }
        }
        private enum QuestionaryState
        {
            AnsweringFio = 1,
            AnsweringBirthDate,
            AnsweringFavLanguage,
            AnsweringExperience,
            AnsweringPhone,
            Start,
            Finish
        }

        public string FIO { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FavouriteLanguage { get; set; }
        public int Experience { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CompletionDate { get; set; }
        private QuestionaryState CurrentState { get; set; }               
        private Dictionary<int, bool> completedQuestions;
        private readonly Dictionary<QuestionaryState, AnswerHandlersItem> answerHandlersByState;
        private static readonly List<string> ProgrammingLanguages;

        static Questionary()
        {
            ProgrammingLanguages = new List<string>
            {
                "PHP", "JavaScript","C","C++","Java","C#","Python", "Ruby"
            };
        }
        public Questionary()
        {
            this.CurrentState = QuestionaryState.AnsweringFio;
            this.completedQuestions = new Dictionary<int, bool>
            {
                {1,false},
                {2,false},
                {3,false},
                {4,false},
                {5,false},
            };
            this.answerHandlersByState = new Dictionary<QuestionaryState, AnswerHandlersItem>
            {
                { QuestionaryState.AnsweringFio, new AnswerHandlersItem(AskFio, (fio) => SetFio(fio)) },
                { QuestionaryState.AnsweringBirthDate,new AnswerHandlersItem(AskDateOfBirth, (dob) => SetBirthDate(dob)) },
                { QuestionaryState.AnsweringFavLanguage,new AnswerHandlersItem(AskFavLanguage, (favLang) => SetFavouriteLanguage(favLang)) },
                { QuestionaryState.AnsweringExperience,new AnswerHandlersItem(AskExperience, (experience) => SetExperience(experience) )},
                { QuestionaryState.AnsweringPhone,new AnswerHandlersItem(AskPhoneNumber, (phone) => SetPhoneNumber(phone)) }
            };
        }

        private AnswerHandlersItem GetAnswerHandlerForCurrentState()
        {
            return answerHandlersByState[this.CurrentState];
        }
        private int GetFirstUnanswered()
        {
            foreach (var questionNumber in this.completedQuestions.Keys)
            {
                if (!this.completedQuestions[questionNumber])                
                    return questionNumber;                
            }
            return -1;
        }
        private void MarkCompleted(QuestionaryState state)
        {
            this.completedQuestions[(int)state] = true;
        }
        private void MoveNext()
        {
            int currentStateInt = (int)this.CurrentState;
            if (completedQuestions.ContainsKey(currentStateInt + 1) && !this.completedQuestions[currentStateInt + 1])
                this.CurrentState = (QuestionaryState)currentStateInt + 1;
            else
            {
                int unaswered = GetFirstUnanswered();
                if (unaswered == -1)
                    this.CurrentState = QuestionaryState.Finish;
                else this.CurrentState = (QuestionaryState)unaswered;
            }
        }
        private void ExecuteCommand(string command)
        {
            string[] commandAndArgs = command.Split(' ');
            string commandName = commandAndArgs[0];
            if (!CommandsHelper.IsCommand(commandName))
                return;

            switch (commandName)
            {
                case CommandsHelper.GOTO_PREV_QUESTION:
                    {
                        int curStateInt = (int)this.CurrentState;
                        if (curStateInt != 1)
                            this.CurrentState = (QuestionaryState)curStateInt - 1;
                    }
                    break;
                case CommandsHelper.GOTO_QUESTION:
                    {
                        int questionNumber;
                        if (int.TryParse(commandAndArgs[1], out questionNumber))
                        {
                            if (Enum.IsDefined(typeof(QuestionaryState), questionNumber))
                                this.CurrentState = (QuestionaryState)questionNumber;
                            else
                                Console.WriteLine("Неверный номер вопроса, введите номер от 1 до 5");
                        }
                    }
                    break;
                case CommandsHelper.RESTART_PROFILE:
                    {
                        this.CurrentState = QuestionaryState.Start;
                    }
                    break;
                default:
                    {

                    }
                    break;
            }
        }
        private void SetFio(string fio)
        {
            this.FIO = fio;
        }
        private void SetBirthDate(string birthDate)
        {
            this.DateOfBirth = DateTime.ParseExact(birthDate, dateFormat, null, DateTimeStyles.None);
        }
        private void SetFavouriteLanguage(string favLang)
        {
            this.FavouriteLanguage = favLang;
        }
        private void SetExperience(string experince)
        {
            this.Experience = Convert.ToInt32(experince);
        }
        private void SetPhoneNumber(string phoneNumber)
        {
            this.PhoneNumber = phoneNumber;
        }

        public static Questionary StartQuestioning()
        {
            Questionary questionary = new Questionary();
            try
            {
                AskResult askResult = null;
                bool finished = false;
                while (!finished)
                {
                    switch (questionary.CurrentState)
                    {
                        case QuestionaryState.Start:
                            {
                                questionary = new Questionary();
                                questionary.CurrentState = QuestionaryState.AnsweringFio;
                            }
                            break;
                        case QuestionaryState.AnsweringFio:
                        case QuestionaryState.AnsweringBirthDate:
                        case QuestionaryState.AnsweringFavLanguage:
                        case QuestionaryState.AnsweringExperience:
                        case QuestionaryState.AnsweringPhone:
                            {
                                AnswerHandlersItem answerHandler = questionary.GetAnswerHandlerForCurrentState();
                                askResult = answerHandler.AskFunc();
                                if (askResult.IsValidAnswer)
                                {
                                    answerHandler.AnswerHandler(askResult.AnswerOrCommand);
                                    questionary.MarkCompleted(questionary.CurrentState);
                                    questionary.MoveNext();
                                }
                                else
                                    questionary.ExecuteCommand(askResult.AnswerOrCommand);
                            }
                            break;
                        case QuestionaryState.Finish:
                            {
                                finished = true;
                                questionary.CompletionDate = DateTime.Now;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return questionary;
        }
        public static Questionary GetQuestionaryFromStream(StreamReader sr)
        {
            Questionary questionary = new Questionary();
            questionary.SetFio(ParseValueFromLine(sr.ReadLine()));
            questionary.SetBirthDate(ParseValueFromLine(sr.ReadLine()));
            questionary.SetFavouriteLanguage(ParseValueFromLine(sr.ReadLine()));
            questionary.SetExperience(ParseValueFromLine(sr.ReadLine()));
            questionary.SetPhoneNumber(ParseValueFromLine(sr.ReadLine()));
            questionary.CompletionDate = DateTime.Parse(ParseValueFromLine(sr.ReadToEnd()));
            return questionary;
        }
        public static void PrintStatistics(List<Questionary> questionaries)
        {
            if (questionaries == null || questionaries.Count == 0)
                return;

            int averageAge = (int)questionaries.Select(q => GetAge(q.DateOfBirth)).Average();

            string ageName = String.Empty;
            switch (averageAge % 10)
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
        
        private static AskResult AskPhoneNumber()
        {
            string answer;
            bool answered = AskQuestion($@"Enter your phone number: ",
                checkPhoneNumber, out answer);
            return new AskResult(answered, answer);

            //TODO: поменять на правильную регулярку
            bool checkPhoneNumber(string input)
            {
                Regex regex = new Regex(@"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$", RegexOptions.Compiled);
                return regex.IsMatch(input);
            }
        }
        private static AskResult AskExperience()
        {
            string answer;
            bool answered = AskQuestion($@"Enter your programming experience in the specified language (full years): ",
                checkExperience, out answer);
            return new AskResult(answered, answer);

            bool checkExperience(string input)
            {
                int res;
                return int.TryParse(input, out res);
            }
        }
        private static AskResult AskFavLanguage()
        {
            string answer;
            bool answered = AskQuestion($@"Choose your favorite programming language from the proposed: {string.Join(", ", ProgrammingLanguages)}",
                checkFavLanguage, out answer);
            return new AskResult(answered, answer);

            bool checkFavLanguage(string input)
            {
                return ProgrammingLanguages.Contains(input);
            }
        }
        private static AskResult AskDateOfBirth()
        {
            string answer;
            bool answered = AskQuestion($@"Enter your date of birth in {dateFormat} format: ", checkDoB, out answer);
            return new AskResult(answered, answer);

            bool checkDoB(string input)
            {
                DateTime date; // date of birth
                return DateTime.TryParseExact(input, dateFormat, null, DateTimeStyles.None, out date);
            }
        }
        private static AskResult AskFio()
        {
            string answer;
            bool answered = AskQuestion("Enter your full name: ", checkFIO, out answer);
            return new AskResult(answered, answer);

            bool checkFIO(string input)
            {
                return !string.IsNullOrWhiteSpace(input) && !ContainsDigits(input);
            }
        }
        private static bool AskQuestion(string text, Func<string, bool> checkAnswer, out string answer)
        {
            answer = String.Empty;
            bool isCommand = false;
            do
            {
                Console.WriteLine(text);
                answer = Console.ReadLine();
                if (CommandsHelper.IsCommand(answer.Split(' ')[0]))
                    isCommand = true;
            }
            while (!checkAnswer(answer) && !isCommand);

            return !isCommand;
        }
        private static string ParseValueFromLine(string line)
        {
            int beginIndex = line.IndexOf(':') + 1;
            return line.Substring(beginIndex).TrimStart();
        }
        private static int GetAge(DateTime birthDate)
        {
            int diff = DateTime.Now.Year - birthDate.Year;
            if ((birthDate.Month > DateTime.Now.Month) || (birthDate.Month == DateTime.Now.Month && birthDate.Day > DateTime.Now.Day))
                diff--;
            return diff;
        }
        private static bool ContainsDigits(string input)
        {
            foreach (char c in input)
            {
                if (char.IsDigit(c))
                    return true;
            }
            return false;
        }

        public override string ToString()
        {
            return
$@"1. Full name: {this.FIO}
2. Date of Birth: {this.DateOfBirth.ToShortDateString()}
3. Favourite programming language: {this.FavouriteLanguage}
4. Programming experience on the specified language: {this.Experience.ToString()}
5. Phone number: {this.PhoneNumber}

Profile filled: {this.CompletionDate.ToString()}";
        }
    }    
}
