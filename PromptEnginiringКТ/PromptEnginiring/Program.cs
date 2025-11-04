using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;

class Program
{
    static void Main(string[] args)
    {
        CheckTriggerWords();
        return;
        CategoriseWords();
        CompareTexts();
        Console.WriteLine();
        Console.WriteLine("==========================");
        Console.WriteLine();
        CompareWords();
        CheckTriggerWords();
    }

    private static HashSet<string> LoadWordsFromFile(string filePath)
    {
        string text = File.ReadAllText(filePath);
        var words = ExtractWords(text);
        return words;

    }

    static Regex Email = new Regex(@".+@(mail\.ru|gmail\.com|spb\.ithub\.ru|yandex\.ru)");

    static Regex MathFunction = new Regex(@"[\+=\-\*\/]{1}");

    static Regex Words = new Regex(@"^[a-zA-Zа-яА-Я]+$");

    static HashSet<string> ExtractWords(string text)
    {
        return text.Split(
            [' ', '\n', '\r'],
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(i=>CleanWord(i))
            .ToHashSet();
    }

    private const string CleanGroup = @"\.\(,:!";
    static Regex Cleaner = new Regex($"^[{CleanGroup}]*(?<word>[^{CleanGroup}]+)[{CleanGroup}]*$");
    
    private static String CleanWord(string s)
    {
        var m = Cleaner.Match(s).Groups["word"];
        if(m.Success) return m.Value;

        return s;
    }

    static void CategoriseWords()
    {
        string filePath = "file3.txt";
        var words = LoadWordsFromFile(filePath);
        foreach (var word in words)
        {
            Console.Write($"Слово '{word}' - ");
            if (Email.IsMatch(word))
            {
                Console.WriteLine($"емейл");
            }
            else if (MathFunction.IsMatch(word))
            {
                Console.WriteLine($"математическая функция");
            }
            else if (Words.IsMatch(word))
            {
                Console.WriteLine($"просто слово");
            }
            else
            {
                Console.WriteLine($"прочие");
            }
        }

    }

    static void CompareTexts()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(
            "ЗАДАНИЕ: 1. Входящие два текстовых файла, в которых какое-то кол-во слов, есть ли какое-то слово из второго файла в первом, реализация любая от true or false до кол-ва слов найденных в обоих файлах. КОД в CSharp");
        Console.ResetColor();

        string file1Path = "file1.txt";
        string file2Path = "file2.txt";

        try
        {
            // Читаем содержимое файлов
            // Извлекаем слова, приводим к нижнему регистру
            var words1 = LoadWordsFromFile(file1Path);
            var words2 = LoadWordsFromFile(file2Path);

            // Находим пересечение слов
            var commonWords = words1.Intersect(words2);

            if (commonWords.Any())
            {
                Console.WriteLine("Слова из первого файла, которые присутствуют во втором:");
                foreach (var word in commonWords)
                {
                    Console.WriteLine(word);
                }
            }
            else
            {
                Console.WriteLine("Совпадающих слов нет.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при чтении файлов или обработке данных:");
            Console.WriteLine(ex.Message);
        }

    }

    static void CompareWords()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(
            "ЗАДАНИЕ: 2. Входящие два файла, каждый с одним словом, проверить на сколько процентов они совпадают. КОД в CSharp");
        Console.ResetColor();

        string file1Path = "word1.txt";
        string file2Path = "word2.txt";

        try
        {
            // Читаем содержимое файлов
            // Извлекаем слова, приводим к нижнему регистру
            var words1 = LoadWordsFromFile(file1Path);
            var words2 = LoadWordsFromFile(file2Path);

            if (words1.Count != words2.Count && words1.Count != 1)
            {
                throw new InvalidDataException("Проверьте число слов в файлах!");
            }

            var w1 = words1.Single();
            var w2 = words2.Single();

            // Находим пересечение слов
            var commonChars = w1.Intersect(w2).ToList();

            if (commonChars.Any())
            {
                Console.WriteLine($"Символы из первого слова '{w1}', которые присутствуют во втором слове '{w2}':");
                foreach (var c in commonChars)
                {
                    Console.WriteLine(c);
                }
            }
            else
            {
                Console.WriteLine("Совпадающих слов нет.");
            }

            double x1 = (1.0 * commonChars.Count) / w1.Count();
            double x2 = (1.0 * commonChars.Count) / w2.Count();
            Console.WriteLine($"Первое слово совпадает со вторым на {x1:P}!!");
            Console.WriteLine($"Второе слово совпадает с первым на {x2:P}!!");

        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при чтении файлов или обработке данных:");
            Console.WriteLine(ex.Message);
        }
    }

    static void CheckTriggerWords()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(
            "ЗАДАНИЕ: 3. На вход подаются TriggerWords.txt и Text.txt, если есть хотя-бы 85 процентов слов, совпадающих с Text.txt. И определить % соотношение типов слов. Есть слово, математическая функция, емейл и прочие.");
        Console.ResetColor();
        string file4Path = "TriggerWords.txt";
        string file5Path = "text.txt";

        var triggerWords = LoadWordsFromFile(file4Path);
        var words = LoadWordsFromFile(file5Path);
        var commonWords = words.Intersect(triggerWords).ToList();

        double x1 = commonWords.Count * 100.0 / words.Count;
        if (x1 < 85)
        {
            Console.WriteLine($"Процент совпавших слов меньше 85:{x1}% - выходим");
            return;
        }

        int emailsCount = 0;
        int mathFunctionsCount = 0;
        int wordsCount = 0;
        int otherWordsCount = 0;

        foreach (var word in words)
        {
            if (Email.IsMatch(word))
            {
                emailsCount++;
            }
            else if (MathFunction.IsMatch(word))
            {
                mathFunctionsCount++;
            }
            else if (Words.IsMatch(word))
            {
                wordsCount++;
            }
            else
            {
                otherWordsCount++;
            }
        }
        
        double emailsProcents = emailsCount * 1.0 / words.Count;
        double  mathFunctionsProcents = mathFunctionsCount * 1.0 / words.Count;
        double wordsProcents = wordsCount * 1.0 / words.Count;
        double otherWordsProcents = otherWordsCount * 1.0 / words.Count;
        
        Console.WriteLine($"{"emailsProcents",-25} -> {emailsProcents,10:P}");
        Console.WriteLine($"{"mathFunctionsProcents",-25} -> {mathFunctionsProcents,10:P}");
        Console.WriteLine($"{"wordsProcents",-25} -> {wordsProcents,10:P}");
        Console.WriteLine($"{"otherWordsProcents",-25} -> {otherWordsProcents,10:P}");
        
        
    }
}