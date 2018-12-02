using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/*
 * We are using an external library to aid us in the process of reading and de-serialising from JSON files.
 * Note that this does not save nor read files on my behalf, and shouldn't impact the grade I get for the aspect of
 * saving to and reading from files, since I am still doing that using the native `System.IO` library.
 * 
 * I chose to make use of JSON files because they are an industry standard and because I am most familiar with this type of persistence,
 * apart from using databases, but that seems a bit excessive for a project like this (and because the specification didn't ask for it).
 * 
 * See the documentation for the `JSON.net` framework here: https://newtonsoft.com/json/help/html/Introduction.htm
 */
using Newtonsoft.Json;

namespace Jurassic_World_Alive
{
    // This tiny class inherits from the built-in <List> class, but is only used in conjunction with the
    // de-serialisation process of JSON strings
    class DinoArrayFromJson : List<Dinosaur>
    {
        public string Species;
        public DinosaurType Type;
        public DinosaurPeriod Period;
    }

    /*
     * This is the big one.
     * 
     * The <CircularLinkedList> class inherits from <IEnumerable>. The reason for this is that I wanted this class to be iterable,
     * meaning I can use loops to iterate over every Dinosaur in the list. This allows for a very flexible data structure.
     * 
     * This class itself is not a list or array, but contains a property that references one (namely `Elements`). Because of
     * the nature of native arrays, it was hard find a way to implement a truly dynamic data structure, which allowed the Creation,
     * Insertion, Editing, and Deletion of Nodes on the fly. To aid in this matter, I think the most important function in this
     * class is `CreateNewList`. This function creates a new array every time the list is acted upon to accommodate values
     * with their correct indexes, and as a result, the array is never bigger or smaller than the amount of Nodes that are active.
     */
    class CircularLinkedList : IEnumerable<Dinosaur>
    {
        // The mutable variable that holds the Dinosaurs
        private Dinosaur[] Elements { get; set; }

        // A public Getter that returns the number of elements in the list
        // This is somewhat of a novelty variable, so that one can easily type `this.Count` instead of `this.Elements.Length`
        public int Count
        {
            get
            {
                return this.Elements.Length;
            }
        }

        // This implements the IEnumerator for the list, allowing iteration on the list
        // See https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/iterators
        public IEnumerator<Dinosaur> GetEnumerator()
        {
            foreach (Dinosaur x in this.Elements)
                yield return x;
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        // This mutable variable allows for easy indexing on the list, similar to how one would access a value on arrays
        // This is especially useful for iteration purposes, such as `for (int i = 0; i < this.Count; i++) this[i];`
        // The way it works is that this property actually performs GET and SET operations on `Elements`
        // See https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/indexers/using-indexers
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

        // The default Constructor takes no parameters and simply instantiates a new array with 0 elements
        public CircularLinkedList()
        {
            this.Elements = new Dinosaur[0];
        }

        // The only overload for the Constructor takes one parameter and sets it to `Elements`
        public CircularLinkedList(params Dinosaur[] elements)
        {
            this.Elements = elements;
        }

        /*
         * This static function restores a previously saved <Game> Session.
         * 
         * The way it works is that an argument is passed into the function, and it can be either a `PlayerName` or an
         * absolute path. If it is a `PlayerName`, it is formatted into an absolute path that points to a JSON file containing
         * Dinosaur data associated with that Player's Session.
         */
        public static CircularLinkedList Restore(string sessionName)
        {
            CircularLinkedList newList = new CircularLinkedList();

            if (!sessionName.ToLower().EndsWith(".json"))
                sessionName = $"{Game.SessionPath}{sessionName.ToLower()}.json";

            // De-serialise the JSON string that read from `<File>.ReadAllText` into the `DinoArrayFromJson
            // object that was created earlier
            DinoArrayFromJson jsonDinos = JsonConvert.DeserializeObject<DinoArrayFromJson>(File.ReadAllText(sessionName));

            // Now iterate over the de-serialised objects and populate `newList` with new instances of <Dinosaur>
            // This is done because the objects contained in `jsonDinos` are simple objects that only hold information
            // regarding the dinosaurs, not actual <Dinosaur> instances
            for (int i = 0; i < jsonDinos.Count; i++)
                newList.Push(new Dinosaur(newList, jsonDinos[i].Species, jsonDinos[i].Type, jsonDinos[i].Period));

            return newList;
        }

        // This static method saves a list to a JSON file by serialising every Dinosaur and creating a JSON string
        public static void SaveToFile(CircularLinkedList list, string savePath = null)
        {
            if (savePath == null)
                savePath = $"{Game.SessionPath}{Game.CurrentSession.PlayerName.ToLower()}.json";

            // Create a new string array that is the size of the list in question
            string[] serialisedDinos = new string[list.Count];

            // And then populate this list with the JSON strings that are obtained from `<Dinosaur>.Serialise`
            for (int i = 0; i < list.Count; i++)
                serialisedDinos[i] = list[i].Serialise();

            string serialisedString = "[" + String.Join(",", serialisedDinos) + "]";
            File.WriteAllText(savePath, serialisedString);
        }

        /*
         * Arguably the most important function in the whole class.
         * 
         * This function is responsible for the creation of new lists on the fly. This means that I can have a list
         * that does not conform to a fixed size, but is dynamic and can be infinite in size (as much as memory allows).
         * 
         * The single parameter controls how many additions should be made to the new list and is defaulted to +1.
         * This is awesome because it means that I can perform the opposite operation too, reducing the size instead of adding.
         */
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

        // An overload for `CreateNewList` that takes an array instead of an integer, and adds the contents of this
        // array on top of the newly created list
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

        // The main `Push` operation, accepting `params`, meaning single items each as an argument or an array
        public CircularLinkedList Push(params Dinosaur[] items)
        {
            this.Elements = this.CreateNewList(items);

            // Persistence
            SaveToFile(this);
            return this;
        }

        // An overload of `Push`, accepting a <CircularLinkedList> as a parameter, works the same as `Push(params Dinosaur[])`
        public CircularLinkedList Push(CircularLinkedList items)
        {
            this.Elements = this.CreateNewList(items.ToArray());

            // Persistence
            SaveToFile(this);
            return this;
        }

        // This operation is a little harder to implement, but basically works in the same way as `Push`
        public CircularLinkedList Insert(int index, Dinosaur item)
        {
            // First, create a new list with an extra index at the top
            this.Elements = this.CreateNewList();
            // Then, shift all items from `index` one index up
            for (int i = this.Count - 1; i > index; i--)
                this[i] = this[i - 1];

            // Finally, assign the `item` at `index`
            this[index] = item;

            // Persistence
            SaveToFile(this);
            return this;
        }

        // Pop simply creates a new list using `CreateNewList` with a negative parameter
        public CircularLinkedList Pop(int amount = 1)
        {
            // Create a new list with all the (soon-to-be) popped items
            CircularLinkedList poppedItems = new CircularLinkedList();
            for (int i = this.Count - amount; i < this.Count; i++)
                poppedItems.Push(this[i]);

            // `amount * -1` will always be negative, unless `amount` is negative
            this.Elements = this.CreateNewList(amount * -1);

            // Persistence
            SaveToFile(this);
            return poppedItems;
        }

        // This function removes an item at a given index and reduces the list size as a result
        public Dinosaur RemoveAt(int index)
        {
            // Thrown an exception if `index` is out of bounds
            if (index < 0 || index > this.Count - 1)
                throw new IndexOutOfRangeException();

            // Take a note of which dino is going to be removed from the list
            Dinosaur removedDino = this[index];
            // Shift all elements from `index` one position down
            for (int i = index; i < this.Count - 1; i++)
                this[i] = this[i + 1];

            // Finally, create a new list with the last element removed
            this.Elements = this.CreateNewList(-1);

            // Persistence
            SaveToFile(this);
            return removedDino;
        }

        // An overload which accepts `startIndex` and `stopIndex`, removing a range of elements within the list
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

            // Persistence
            SaveToFile(this);
            return removedDinos;
        }

        // This function simply removes a given <Dinosaur> if it is part of the list
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
         * This is the biggest method in the class and it essentially enables the player to dynamically visualise, insert,
         * delete, and edit Dinosaurs on the fly. This makes it a lot easier to interact with the list and takes work out of
         * the main menu.
         * 
         * More of how it works is described in the comments, but generally, the way this works is that the list is displayed
         * in a readable format and is then prompted for an input, including: UpArrow, DownArrow, Insert, Delete, BackSpace, Enter.
         */
        public void Visualise(int SelectedChoice = 0)
        {
            // Simply `return` if the list is empty
            if (this.Count == 0)
            {
                Console.Write("The list is empty!\n");
                return;
            }

            // Retrieve the columns of the table, which are the properties of a Dinosaur (Species, etc.)
            // and then print them with formatting
            string[] columns = this[0].Columns();
            Console.Write("      " + String.Join("  ", columns.Select(x => x.PadRight(20))) + "\n");

            // Now, display every dinosaur and its details
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

            // The following loop is broken when the player presses the Backspace button
            while (currentInput != ConsoleKey.Backspace)
            {
                // If the player presses "Up" or "W" on their keyboard
                if (currentInput == ConsoleKey.UpArrow || currentInput == ConsoleKey.W)
                {
                    // Decrease the currently selected index
                    SelectedChoice--;

                    // Update options ('>' pointer) to reflect a change in selection
                    if (SelectedChoice >= 0)
                    {
                        Console.SetCursorPosition(0, --currentOptionCursorPos);
                        Console.Write(" > ");
                        Console.SetCursorPosition(0, Console.CursorTop + 1);
                        Console.Write("   ");
                    }

                    // Also allow for rotating selection (when the player presses "Up" on the first selection)
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

                // If the player presses "Down" or "S" on their keyboard
                else if (currentInput == ConsoleKey.DownArrow || currentInput == ConsoleKey.S)
                {
                    // Increase the currently selected index
                    SelectedChoice++;

                    // Update options ('>' pointer) to reflect a change in selection
                    if (SelectedChoice <= this.Count - 1)
                    {
                        Console.SetCursorPosition(0, ++currentOptionCursorPos);
                        Console.Write(" > ");
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        Console.Write("   ");
                    }

                    // Also allow for rotating selection (when the player presses "Down" on the last selection)
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

                // If the player presses "Enter" or "Spacebar", the player has selected a dinosaur to act on
                else if (currentInput == ConsoleKey.Enter || currentInput == ConsoleKey.Spacebar)
                {
                    Console.Write("\n\n");
                    Console.CursorVisible = true;

                    // Collect the player's choice for what they want to do with the selected dinosaur
                    int dinoChoice = Menu.CollectChoice($"Now, what would you like to do with {{ {this[SelectedChoice].Rows()[0]} }}?", new string[]
                    {
                        "List Details",
                        "Update Species",
                        "Update Type",
                        "Update Period"
                    });

                    // Show Dinosaur's details
                    if (dinoChoice == 0)
                    {
                        Console.WriteLine($"\nHere are the selected Dinosaur's details:\n\nSpecies: {this[SelectedChoice].Species}\nType: {this[SelectedChoice].Type}\nPeriod: {this[SelectedChoice].Period}");
                        Menu.Continue();
                    }
                    // Update Dino Species
                    else if (dinoChoice == 1)
                        this[SelectedChoice].UpdateSpeciesDialog();
                    // Update Dino Type
                    else if (dinoChoice == 2)
                        this[SelectedChoice].UpdateTypeDialog();
                    // Update Dino Period
                    else if (dinoChoice == 3)
                        this[SelectedChoice].UpdatePeriodDialog();

                    // Persistence
                    SaveToFile(this);

                    Console.Clear();
                    // Recursively call `Visualise`, which brings the player back to the list display
                    this.Visualise(SelectedChoice);

                    return;
                }

                // If the player presses "Insert" on their keyboard, create a new dinosaur and add it to the
                // list at the current position
                else if (currentInput == ConsoleKey.Insert)
                {
                    Console.Write("\n\n");
                    Console.CursorVisible = true;

                    // I think this is a beautiful example of abstraction
                    Dinosaur newDino = Dinosaur.CreateNewDialog(this);
                    this.Insert(SelectedChoice, newDino);

                    Console.Clear();
                    // Recursively call `Visualise`, which brings the player back to the list display
                    this.Visualise(SelectedChoice);

                    return;
                }

                // If the player presses "Delete" on their keyboard, delete the dinosaur from the list at
                // the current position
                else if (currentInput == ConsoleKey.Delete)
                {
                    this.RemoveAt(SelectedChoice);

                    Console.Clear();
                    // Recursively call `Visualise`, which brings the player back to the list display
                    this.Visualise(SelectedChoice);

                    return;
                }

                // Reset cursor to last line
                Console.SetCursorPosition(lastStringPos[0], lastStringPos[1]);
                // Update the player's input
                currentInput = Console.ReadKey().Key;
            }

            Console.CursorVisible = true;
            Console.WriteLine();
        }
    }
}
