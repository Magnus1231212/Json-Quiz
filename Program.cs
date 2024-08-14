using System.Text.Json;
using System;
using System.Dynamic;
using Newtonsoft.Json;

namespace Json_Quiz
{
    class ListsContainer
    {
        public List<string> QuestionText { get; set; }
        public List<List<string>> Options { get; set; }
        public List<string> CorrectAnswer { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            bool running = true;
            while (running)
            {
                int counter = 0;
                Console.WriteLine("Welcome to the Quiz App!");
                List<string> quizes = GetQuizes();
                Console.WriteLine("Select the quiz you want to take or enter 0 to exit:");
                Console.WriteLine();
                foreach (string quiz in quizes)
                {
                    counter++;
                    Console.WriteLine($"{counter}. {quiz}");
                }
                Console.WriteLine("0. Exit");
                Console.WriteLine();
                Console.Write("Enter the quiz number: ");
                Int32.TryParse(Console.ReadLine(), out int SearchOption);
                if (SearchOption == 0)
                {
                    running = false;
                }
                else if (SearchOption > 0 && SearchOption <= quizes.Count)
                {
                    RunQuiz(SearchOption);
                }
                else
                {
                    Console.WriteLine("Invalid input. Please try again.");
                }
            }
        }

        public static List<string> GetQuizes()
        {
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "quiz");

            if (Directory.Exists(directoryPath))
            {
                List<string> quizes = new List<string>();
                string[] files = Directory.GetFiles(directoryPath);

                foreach (string file in files)
                {
                    string fContent = File.ReadAllText(file);

                    using (JsonDocument doc = JsonDocument.Parse(fContent))
                    {
                        JsonElement root = doc.RootElement;

                        string title = root.GetProperty("quiz").GetProperty("title").GetString();

                        quizes.Add(title);
                    }
                }
                return quizes;
            }
            else
            {
                Console.WriteLine("Failed to get Quiz Dir!");
                return new List<string>();
            }
        }

        public static ListsContainer GetQuizQuestions(int number)
        {
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "quiz");

            if (Directory.Exists(directoryPath))
            {
                string[] files = Directory.GetFiles(directoryPath);

                string fContent = File.ReadAllText(files[number - 1]);

                return ParseJson(fContent);
            }
            else
            {
                Console.WriteLine("Failed to get Quiz Dir!");
                return null;
            }
        }

        static ListsContainer ParseJson(string json)
        {
            var parsedJson = JsonConvert.DeserializeObject<dynamic>(json);

            var listsContainer = new ListsContainer
            {
                QuestionText = new List<string>(),
                Options = new List<List<string>>(),
                CorrectAnswer = new List<string>()
            };

            foreach (var question in parsedJson.quiz.questions)
            {
                listsContainer.QuestionText.Add((string)question.question_text);

                var options = new List<string>();
                foreach (var option in question.options)
                {
                    options.Add((string)option.option_text);
                }
                listsContainer.Options.Add(options);

                listsContainer.CorrectAnswer.Add((string)question.correct_answer);
            }

            return listsContainer;
        }

        public static void RunQuiz(int number)
        {
            Console.Clear();

            int score = 0;
            var quiz = GetQuizQuestions(number);

            if (quiz == null)
            {
                Console.WriteLine("Quiz not found.");
                return;
            }

            for (int i = 0; i < quiz.QuestionText.Count; i++)
            {
                Console.WriteLine($"Question {i + 1}: {quiz.QuestionText[i]}");

                for (int j = 0; j < quiz.Options[i].Count; j++)
                {
                    Console.WriteLine($"{(char)('a' + j)}. {quiz.Options[i][j]}");
                }

                Console.Write("Your answer: ");
                string userAnswer = Console.ReadLine();

                if (userAnswer.Equals(quiz.CorrectAnswer[i], StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Correct!\n");
                    score++;
                }
                else
                {
                    Console.WriteLine($"Wrong! The correct answer was {quiz.CorrectAnswer[i]}.\n");
                }
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }

            Console.WriteLine($"Quiz finished! Your score: {score}/{quiz.QuestionText.Count}");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}