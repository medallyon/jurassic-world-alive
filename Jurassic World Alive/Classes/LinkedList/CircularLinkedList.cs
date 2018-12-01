using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

namespace Jurassic_World_Alive
{
    class DinoArrayFromJson : List<Dinosaur>
    {
        public string Species;
        public DinosaurType Type;
        public DinosaurPeriod Period;
    }

    class CircularLinkedList : IEnumerable<Dinosaur>
    {
        private Dinosaur[] Elements { get; set; }

        public int Count
        {
            get
            {
                return this.Elements.Length;
            }
        }

        public IEnumerator<Dinosaur> GetEnumerator()
        {
            foreach (Dinosaur x in this.Elements)
                yield return x;
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public Dinosaur this[int i]
        {
            get
            {
                return this.Elements[i];
            }
            set
            {
                this.Elements[i] = value;
            }
        }

        public CircularLinkedList()
        {
            this.Elements = new Dinosaur[0];
        }

        public CircularLinkedList(params Dinosaur[] elements)
        {
            this.Elements = elements;
        }

        public static CircularLinkedList Restore(string sessionName)
        {
            CircularLinkedList newList = new CircularLinkedList();

            if (!sessionName.ToLower().EndsWith(".json"))
                sessionName = $"{Game.SessionPath}{sessionName}.json";

            DinoArrayFromJson jsonDinos = JsonConvert.DeserializeObject<DinoArrayFromJson>(File.ReadAllText(sessionName));

            for (int i = 0; i < jsonDinos.Count; i++)
                newList.Push(new Dinosaur(newList, jsonDinos[i].Species, jsonDinos[i].Type, jsonDinos[i].Period));

            return newList;
        }

        public static void SaveToFile(CircularLinkedList list, string savePath = null)
        {
            if (savePath == null)
                savePath = $"{Game.SessionPath}{Game.CurrentSession.PlayerName.ToLower()}.json";

            string[] serialisedDinos = new string[list.Count];

            for (int i = 0; i < list.Count; i++)
                serialisedDinos[i] = list[i].Serialise();

            string serialisedString = "[" + String.Join(",", serialisedDinos) + "]";
            File.WriteAllText(savePath, serialisedString);
        }

        private Dinosaur[] CreateNewList(int additions = 1)
        {
            if (additions == 0)
                return this.Elements;

            Dinosaur[] newList = new Dinosaur[Math.Max(0, this.Count + additions)];
            for (int i = 0; i < newList.Length; i++)
            {
                if (i == this.Count)
                    break;

                newList[i] = this[i];
            }

            return newList;
        }

        private Dinosaur[] CreateNewList(params Dinosaur[] items)
        {
            if (items.Length == 0)
                return this.Elements;

            Dinosaur[] newList = new Dinosaur[Math.Max(0, this.Count + items.Length)];
            for (int i = 0; i < newList.Length; i++)
            {
                if (i >= this.Count)
                    newList[i] = items[i - this.Count];
                else
                    newList[i] = this[i];
            }

            return newList;
        }

        public CircularLinkedList Push(params Dinosaur[] items)
        {
            this.Elements = this.CreateNewList(items);

            SaveToFile(this);
            return this;
        }

        public CircularLinkedList Push(CircularLinkedList items)
        {
            this.Elements = this.CreateNewList(items.ToArray());

            SaveToFile(this);
            return this;
        }

        public CircularLinkedList Insert(int index, Dinosaur item)
        {
            this.Elements = this.CreateNewList();
            for (int i = this.Count - 1; i > index; i--)
                this[i] = this[i - 1];

            this[index] = item;

            SaveToFile(this);
            return this;
        }

        public CircularLinkedList Pop(int amount = 1)
        {
            Dinosaur[] newList = this.CreateNewList(amount * -1);

            CircularLinkedList poppedItems = new CircularLinkedList(newList);
            this.Elements = newList;

            SaveToFile(this);
            return poppedItems;
        }

        public Dinosaur RemoveAt(int index)
        {
            if (index < 0 || index > this.Count - 1)
                throw new IndexOutOfRangeException();

            Dinosaur removedDino = this[index];
            for (int i = index; i < this.Count - 1; i++)
                this[i] = this[i + 1];

            this.Elements = this.CreateNewList(-1);

            SaveToFile(this);
            return removedDino;
        }

        public CircularLinkedList RemoveAt(int startIndex, int stopIndex)
        {
            if (startIndex < 0 || startIndex > this.Count - 1 || stopIndex < startIndex + 1 || stopIndex > this.Count - 1)
                throw new IndexOutOfRangeException();

            CircularLinkedList newList = new CircularLinkedList();
            CircularLinkedList removedDinos = new CircularLinkedList();
            for (int i = 0; i < this.Count - 1; i++)
            {
                if (i >= startIndex || i <= stopIndex)
                    removedDinos.Push(this[i]);
                else
                    newList.Push(this[i]);
            }

            this.Elements = newList.Elements;

            SaveToFile(this);
            return removedDinos;
        }

        public void Remove(Dinosaur item)
        {
            bool removed = false;
            for (int i = 0; i < this.Count - 1; i++)
            {
                if (this[i] == item)
                {
                    this.RemoveAt(i);
                    removed = true;
                    break;
                }
            }

            if (!removed)
                throw new Exception("The specified item is not part of this Linked List");
        }

        /*
         * Table should look something like this:
         * Species   | Type     | Period
         * ########  | ######## | #########
         */
        public void Visualise(int SelectedChoice = 0)
        {
            if (this.Count == 0)
            {
                Console.Write("The list is empty!\n");
                return;
            }

            string[] columns = this[0].Columns();
            Console.Write("      " + String.Join("  ", columns.Select(x => x.PadRight(20))) + "\n");

            for (int i = 0; i < this.Count; i++)
            {
                // Display the current selection with a caret
                if (i == SelectedChoice)
                    Console.Write(" > ");
                else
                    Console.Write("   ");

                Console.Write($"{i + 1}. {this[i].ToString()}\n");
            }

            Console.Write("\nTap [ RETURN ] to act on the currently selected Dino\n" +
                "Tap [ INSERT ] to insert a Dino at the currently selected index\n" +
                "Tap [ DELETE ] to delete the currently selected Dino\n" +
                "Tap [ BACKSPACE ] to return to the Menu");

            Console.CursorVisible = false;
            int[] lastStringPos = new int[] { Console.CursorLeft, Console.CursorTop };

            ConsoleKey currentInput = Console.ReadKey().Key;
            int currentOptionCursorPos = Console.CursorTop - 4 - (this.Count - SelectedChoice);

            while (currentInput != ConsoleKey.Backspace)
            {
                // Update options ('>' pointer) to reflect a change in selection
                if (currentInput == ConsoleKey.UpArrow || currentInput == ConsoleKey.W)
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
                        SelectedChoice = this.Count - 1;

                        Console.SetCursorPosition(0, currentOptionCursorPos);
                        Console.Write("   ");

                        currentOptionCursorPos += SelectedChoice;
                        Console.SetCursorPosition(0, currentOptionCursorPos);
                        Console.Write(" > ");
                    }
                }

                else if (currentInput == ConsoleKey.DownArrow || currentInput == ConsoleKey.S)
                {
                    SelectedChoice++;

                    if (SelectedChoice <= this.Count - 1)
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

                        currentOptionCursorPos -= this.Count - 1;
                        Console.SetCursorPosition(0, currentOptionCursorPos);
                        Console.Write(" > ");
                    }
                }

                else if (currentInput == ConsoleKey.Enter || currentInput == ConsoleKey.Spacebar)
                {
                    Console.Write("\n\n");
                    Console.CursorVisible = true;

                    int dinoChoice = Menu.CollectChoice($"Now, what would you like to do with {{ {this[SelectedChoice].Rows()[0]} }}?", new string[]
                    {
                        "List Details",
                        "Update Species",
                        "Update Type",
                        "Update Period"
                    });

                    if (dinoChoice == 0)
                    {
                        Console.WriteLine($"\nHere are the selected Dinosaur's details:\n\nSpecies: {this[SelectedChoice].Species}\nType: {this[SelectedChoice].Type}\nPeriod: {this[SelectedChoice].Period}");
                        Menu.Continue();
                    }
                    else if (dinoChoice == 1)
                        this[SelectedChoice].UpdateSpeciesDialog();
                    else if (dinoChoice == 2)
                        this[SelectedChoice].UpdateTypeDialog();
                    else if (dinoChoice == 3)
                        this[SelectedChoice].UpdatePeriodDialog();
                    SaveToFile(this);

                    Console.Clear();
                    this.Visualise();

                    return;
                }

                else if (currentInput == ConsoleKey.Insert)
                {
                    Console.Write("\n\n");
                    Console.CursorVisible = true;

                    Dinosaur newDino = Dinosaur.CreateNewDialog(this);
                    this.Insert(SelectedChoice, newDino);

                    Console.Clear();
                    this.Visualise();

                    return;
                }

                else if (currentInput == ConsoleKey.Delete)
                {
                    this.RemoveAt(SelectedChoice);

                    Console.Clear();
                    this.Visualise();

                    return;
                }

                // Reset cursor to last line
                Console.SetCursorPosition(lastStringPos[0], lastStringPos[1]);
                currentInput = Console.ReadKey().Key;
            }

            Console.CursorVisible = true;
            Console.WriteLine();
        }
    }
}
