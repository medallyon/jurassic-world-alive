using System;
using System.Collections.Generic;
using System.Linq;

namespace Jurassic_World_Alive
{
    class Menu
    {
        // This is used mainly to allow the Player to read the screen and continue by pressing Enter
        public static void Continue()
        {
            Console.Write("\nPress [ RETURN ] to continue...");
            Console.ReadLine();
        }

        // Clears only the latest line of the Console
        // https://stackoverflow.com/a/8946847/4672263
        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        // This function simply collects the Player's String Input, up to a maximum number of characters
        // Default `maxCharacters` value is virtual `Infinity`
        public static string CollectAnswer(string prompt, float maxCharacters = 1 / 0f)
        {
            Console.Write($"{prompt}\n\n > ");
            string Answer = String.Empty;

            while (Answer.Length == 0 || Answer.Length > maxCharacters)
            {
                ClearCurrentConsoleLine();
                Console.Write(" > ");
                
                ConsoleKeyInfo currentKey = Console.ReadKey();
                while (currentKey.Key != ConsoleKey.Enter)
                {
                    if (!(Char.IsLetterOrDigit(currentKey.KeyChar) || currentKey.Key == ConsoleKey.Spacebar || currentKey.Key == ConsoleKey.Backspace))
                    {
                        ClearCurrentConsoleLine();
                        Console.Write($" > {Answer}");

                        currentKey = Console.ReadKey();
                        continue;
                    }

                    if (Char.IsLetterOrDigit(currentKey.KeyChar) || currentKey.Key == ConsoleKey.Spacebar)
                        Answer += currentKey.KeyChar;

                    if (Answer.Length > maxCharacters || (currentKey.Key == ConsoleKey.Backspace && Answer.Length > 0))
                    {
                        Answer = Answer.Remove(Answer.Length - 1);
                        
                        ClearCurrentConsoleLine();
                        Console.Write($" > {Answer}");
                    }

                    if (currentKey.Key == ConsoleKey.Backspace)
                    {
                        ClearCurrentConsoleLine();
                        Console.Write($" > {Answer}");
                    }

                    currentKey = Console.ReadKey();
                }
            }

            Console.WriteLine();
            return Answer;
        }

        // This function collects the Player's choice for a selection of options
        public static int CollectChoice(string prompt, string[] options)
        {
            Console.Write(prompt + "\n\n");

            for (int i = 0; i < options.Length; i++)
            {
                if (i == 0) Console.Write(" > ");
                else Console.Write("   ");

                Console.Write($"{i + 1}. {options[i]}\n");
            }

            Console.CursorVisible = false;
            int SelectedChoice = 0;
            Console.Write($"\nCurrently Selected: {options[SelectedChoice]}");
            int[] lastStringPos = new int[] { Console.CursorLeft, Console.CursorTop };

            ConsoleKey currentInput = Console.ReadKey().Key;
            int currentOptionCursorPos = Console.CursorTop - 1 - options.Length;

            while (currentInput != ConsoleKey.Enter)
            {
                // Update options ('>' pointer) to reflect a change in selection
                if (currentInput == ConsoleKey.UpArrow)
                {
                    SelectedChoice--;

                    if (SelectedChoice >= 0)
                    {
                        Console.SetCursorPosition(0, --currentOptionCursorPos);
                        Console.Write(" > ");
                        Console.SetCursorPosition(0, Console.CursorTop + 1);
                        Console.Write("   ");
                    }

                    // Also allow for rotating selection
                    else
                    {
                        SelectedChoice = options.Length - 1;

                        Console.SetCursorPosition(0, currentOptionCursorPos);
                        Console.Write("   ");

                        currentOptionCursorPos += SelectedChoice;
                        Console.SetCursorPosition(0, currentOptionCursorPos);
                        Console.Write(" > ");
                    }
                }

                else if (currentInput == ConsoleKey.DownArrow)
                {
                    SelectedChoice++;

                    if (SelectedChoice <= options.Length - 1)
                    {
                        Console.SetCursorPosition(0, ++currentOptionCursorPos);
                        Console.Write(" > ");
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        Console.Write("   ");
                    }

                    // Also allow for rotating selection
                    else
                    {
                        SelectedChoice = 0;

                        Console.SetCursorPosition(0, currentOptionCursorPos);
                        Console.Write("   ");

                        currentOptionCursorPos -= options.Length - 1;
                        Console.SetCursorPosition(0, currentOptionCursorPos);
                        Console.Write(" > ");
                    }
                }

                // Reset cursor to last line
                Console.SetCursorPosition(lastStringPos[0], lastStringPos[1]);

                // Update the final line to reflect current choice
                ClearCurrentConsoleLine();
                Console.Write($"Currently Selected: {options[SelectedChoice]}");

                currentInput = Console.ReadKey().Key;
            }

            ClearCurrentConsoleLine();
            Console.CursorTop--;
            Console.CursorVisible = true;
            return SelectedChoice;
        }

        public static Dictionary<string, string[]> CreateTableElements(string[] columns, string[][] rows)
        {
            Dictionary<string, string[]> table = new Dictionary<string, string[]>();
            for (int i = 0; i < columns.Length; i++)
            {
                string[] items = new string[rows[i].Length];
                for (int j = 0; j < items.Length; j++)
                    items[j] = rows[i][j];

                table.Add(columns[i], items);
            }

            return table;
        }

        public static string GenerateTable(string[] columns, string[][] rows, int padding = 20, int spaceBefore = 0)
        {
            Dictionary<string, string[]> table = CreateTableElements(columns, rows);
            string output = new String(' ', spaceBefore) + String.Join("", table.Select((kv) => kv.Key.PadRight(padding)));
            
            for (int i = 0; i < rows.Length; i++)
            {
                output += "\n" + new String(' ', spaceBefore);
                for (int j = 0; j < table.Count; j++)
                    output += rows[i][j].PadRight(padding);
            }

            return output;
        }
    }
}
