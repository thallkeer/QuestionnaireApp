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
        private class AskResult
        {
            public bool ValidAnswer { get; set; }
            public string AnswerOrCommand { get; set; }            

            public AskResult(bool valid, string answerOrCommand)
            {
                ValidAnswer = valid;
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

        public Questionary()
        {
            this.CurrentState = QuestionaryState.AnsweringFio;
            this.completedQuestions = new Dictionary<int, bool>()
            {
                {1,false},
                {2,false},
                {3,false},
                {4,false},
                {5,false},
            };
            this.answerHandlersByState = new Dictionary<QuestionaryState, AnswerHandlersItem>
            {
                { QuestionaryState.AnsweringFio, new AnswerHandlersItem(AskFio, (fio) => HandleFio(fio)) },
                { QuestionaryState.AnsweringBirthDate,new AnswerHandlersItem(AskDateOfBirth, (dob) => HandleBirthDate(dob)) },
                { QuestionaryState.AnsweringFavLanguage,new AnswerHandlersItem(AskFavLanguage, (favLang) => HandleFavouriteLanguage(favLang)) },
                { QuestionaryState.AnsweringExperience,new AnswerHandlersItem(AskExperience, (experience) => HandleExperience(experience) )},
                { QuestionaryState.AnsweringPhone,new AnswerHandlersItem(AskPhoneNumber, (phone) => HandlePhoneNumber(phone)) }
            };
        }

        private static readonly List<string> ProgrammingLanguages = new List<string>
        {
            "PHP", "JavaScript","C","C++","Java","C#","Python", "Ruby"
        };           
        
        private AnswerHandlersItem GetAnswerHandlerForCurrentState()
        {
            return answerHandlersByState[this.CurrentState];
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
                                if (askResult.ValidAnswer)
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
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return questionary;
        }

        private void MarkCompleted(QuestionaryState state)
        {
            this.completedQuestions[(int)state] = true;
        }

        private int GetFirstUnanswered()
        {
            foreach (var questionNumber in this.completedQuestions.Keys)
            {
                if (!this.completedQuestions[questionNumber])
                {
                    return questionNumber;
                }
            }
            return -1;
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

        public override string ToString()
        {
            return
$@"1. ФИО: {this.FIO}
2. Дата рождения: {this.DateOfBirth.ToShortDateString()}
3. Любимый язык программирования: {this.FavouriteLanguage}
4. Опыт программирования на указанном языке: {this.Experience.ToString()}
5. Мобильный телефон: {this.PhoneNumber}

Анкета заполнена: {this.CompletionDate.ToString()}";
        }

        private static AskResult AskPhoneNumber()
        {
            string answer;
            bool answered = AskQuestion($@"Укажите номер вашего мобильного телефона: ",
                checkPhoneNumber, out answer);
            return new AskResult(answered, answer);

            //TODO: поменять на правильную регулярку
            bool checkPhoneNumber(string input)
            {
                //Regex regex = new Regex(@"/^(8|\+7)\d{3}\d{7}$/", RegexOptions.Compiled);
                //return regex.IsMatch(input);
                foreach (char c in input)
                {
                    if (!char.IsDigit(c))
                        return false;
                }
                return true;
            }
        }

        private static AskResult AskExperience()
        {
            string answer;
            bool answered = AskQuestion($@"Укажите ваш опыт программирования на указанном языке (полных лет): ",
                checkExperience, out answer);
            return new AskResult(answered, answer);

            bool checkExperience(string input)
            {
                int res;
                return int.TryParse(input,out res);
            }
        }

        private static AskResult AskFavLanguage()
        {
            string answer;
            bool answered = AskQuestion($@"Выберите свой любимый язык программирования из предложенных: {string.Join(", ", ProgrammingLanguages)}",
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
            bool answered = AskQuestion(@"Введите вашу дату рождения в формате дд.мм.гггг (дата.месяц.год): ", checkDoB, out answer);
            return new AskResult(answered, answer);

            bool checkDoB(string input)
            {
                DateTime date; // date of birth
                return DateTime.TryParseExact(input, "dd.mm.yyyy", null, DateTimeStyles.None, out date);
            }
        }        
               
        private static AskResult AskFio()
        {
            string answer;
            bool answered = AskQuestion("Введите ваши ФИО: ", checkFIO, out answer);
            return new AskResult(answered, answer);

            bool checkFIO(string input)
            {
                return !string.IsNullOrWhiteSpace(input) && !ContainsDigits(input);
            }
        }

        private void HandleFio(string fio)
        {
            this.FIO = fio;
        }
        private void HandleBirthDate(string birthDate)
        {
            this.DateOfBirth = DateTime.ParseExact(birthDate, "dd.mm.yyyy", null, DateTimeStyles.None);
        }
        private void HandleFavouriteLanguage(string favLang)
        {
            this.FavouriteLanguage = favLang;
        }
        private void HandleExperience(string experince)
        {
            this.Experience = Convert.ToInt32(experince);
        }
        private void HandlePhoneNumber(string phoneNumber)
        {
            this.PhoneNumber = phoneNumber;
        }       
        private static bool AskQuestion(string text, Func<string,bool> checkAnswer, out string answer)
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
        static bool ContainsDigits(string input)
        {
            foreach (char c in input)
            {
                if (char.IsDigit(c))
                    return true;
            }
            return false;
        }

        private static string ParseValueFromLine(string line)
        {
            int beginIndex = line.IndexOf(':') + 1;
            return line.Substring(beginIndex).TrimStart();
        }

        public static Questionary GetQuestionaryFromStream(StreamReader sr)
        {
            Questionary questionary = new Questionary();
            questionary.HandleFio(ParseValueFromLine(sr.ReadLine()));
            questionary.HandleBirthDate(ParseValueFromLine(sr.ReadLine()));
            questionary.HandleFavouriteLanguage(ParseValueFromLine(sr.ReadLine()));
            questionary.HandleExperience(ParseValueFromLine(sr.ReadLine()));
            questionary.HandlePhoneNumber(ParseValueFromLine(sr.ReadLine()));
            questionary.CompletionDate = DateTime.Parse(ParseValueFromLine(sr.ReadToEnd()));
            return questionary;
        }
    }    
}
