/*
 * MIT License
 *
 * Copyright (c) 2018 Tilman Wirawat Raendchen
 * University of the West of Scotland
 * Software Development for Games
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

// We name our namespace `Jurassic_World_Alive`
// This namespace will hold all of our classes, which will be available through the syntax "Jurassic_World_Alive.<ClassName>"
namespace Jurassic_World_Alive
{
    // The `Game` class - this will control the overall game loop, such as instantiating new Linked Lists and Dinosaurs, as well as calling the respective menu methods
    class Game
    {
        // This variable tells the `Main` function if the user wants to restart the game or straight up quit
        public bool Restart = false;

        // Here are some static variables that hold some vital information about the current player's session
        public static Game CurrentSession;
        public static readonly string SessionPath = "./sessions/";
        // This `Getter` returns a new <List> of all the names of all player sessions ever created
        public static List<string> SessionNames
        {
            get
            {
                if (!Directory.Exists(SessionPath))
                    Directory.CreateDirectory(SessionPath);
                
                return new List<string>(Directory.GetFiles(SessionPath));
            }
        }
        public static OpenFileDialog FileBrowser = new OpenFileDialog() { Filter = "JSON files|*.json" };

        // This instance of my own implementation of the <CircularLinkedList> is responsible for holding all Dinosaurs
        public CircularLinkedList Dinosaurs { get; set; }
        public string PlayerName { get; set; }

        /*
         * The MAIN method
         * This is where the program starts. It decides if the user is just starting off or restarting from a previous game session.
         * For some reason the `STAThread` attribute needs to be set on the Main method for the File Browser to function
         */
        [STAThread()]
        static void Main(string[] args)
        {
            if (args.Length == 0)
                args = new string[] { null };

            Game instance = new Game(args[0]);

            if (instance.Restart)
                Main(new string[] { instance.PlayerName });
        }

        // The GAME constructor
        // This is where everything starts once a <Game> has been instantiated
        public Game(string playerName = null)
        {
            // Set the static `CurrentSession` variable to `this` so that we can reference this anywhere throughout any class
            // This is important when attempting to Load and Save the Dinosaurs to file
            CurrentSession = this;

            // The player may already have a name, if they choose the restart from a previous Game Session
            if (playerName == null)
                this.PlayerName = Menu.CollectAnswer($"Welcome to Jurassic World Alive! What's your name?");
            else
                this.PlayerName = playerName;

            // Create a new instance of my custom <CircularLinkedList> with no arguments
            this.Dinosaurs = new CircularLinkedList();

            // Check if the player has previously played and saved their progress
            if (SessionNames.Contains($"{Game.SessionPath}{this.PlayerName.ToLower()}.json"))
            {
                int loadSession = Menu.CollectChoice("\nIt seems like you've played before! Would you like to load your most recent play session?", new string[] { "Yes", "No (Resets your previous progress)" });

                // If so, call the static function `Restore` from <CircularLinkedList>, which will restore a previous session into a new <CircularLinkedList> that contains all previous Dinosaurs created by this player
                if (loadSession == 0)
                    this.Dinosaurs = CircularLinkedList.Restore(this.PlayerName.ToLower());
                else
                    this.Dinosaurs = new CircularLinkedList();
            }

            this.ShowMenu();
        }

        private void ShowMenu()
        {
            Console.Clear();
            int playerChoice = Menu.CollectChoice($"{this.PlayerName}, " +
                $"What would you like to do?", new string[] { "Visualise the current list of dinosaurs", "Create a new Dinosaur", "Save Current Dinosaurs to file", "Load Dinosaurs from a file", "Quit" });
            
            Console.Clear();
            // Use of abstraction: Extracting only the most important functions and disregarding low-level details
            // And decomposition: Splitting the problem into smaller, more managable chunks
            if (playerChoice == 0)
                this.VisualiseListWithControls();
            else if (playerChoice == 1)
                this.CreateNewDinoDialog();
            else if (playerChoice == 2)
                this.SaveDinosToFile();
            else if (playerChoice == 3)
                this.LoadDinosFromFile();
            else if (playerChoice == 4)
                this.Quit();

            // If the `Restart` variable was set to true by the `Quit` method, return instead of recursively calling `ShowMenu`
            if (this.Restart)
                return;

            // This method simply prompts the user to press the Enter key in order to continue
            // I found myself re-using the same code over and over and decided to make a static function out of it
            Menu.Continue();
            this.ShowMenu();
        }

        // This visualises the current Dinosaurs stored in `this.Dinosaurs` in an inter-active manner
        // See `<CircularLinkedList>.Visualise` for more information on how I implemented this
        private void VisualiseListWithControls()
        {
            this.Dinosaurs.Visualise();
        }

        // This method prompts the user to create a new Dinosaur and stores it in `this.Dinosaur` using the list's `Push` function
        private void CreateNewDinoDialog()
        {
            Dinosaur dino = Dinosaur.CreateNewDialog(this.Dinosaurs);
            this.Dinosaurs.Push(dino);

            Console.Write($"\nGreat, your new {dino.Species} Dinosaur was added to the Linked List.");
        }

        /*
         * There is no function for either the "Delete Dino" or the "Change Dino Details" functionality.
         * This is because I have implemented this functionality into the `<CircularLinkedList>.Visualise` method, which provides an inter-active interface that allows for inserting, editing, visualising, and deleting of Dinosaurs.
         */

        // This method prompts the player to save their Dinosaurs to a file they choose
        // This does not save the player's Dinosaurs into the persistent session directory, but it allows for the player to save their progress into an external file, which can later be imported from the same or any different session
        private void SaveDinosToFile()
        {
            Console.Write("The File Browser has been opened. Choose a file or press CANCEL to return to the menu.");
            DialogResult result = FileBrowser.ShowDialog();

            Console.Clear();
            if (result == DialogResult.OK)
                CircularLinkedList.SaveToFile(this.Dinosaurs, FileBrowser.FileName);
            else if (result == DialogResult.Cancel)
            {
                Console.WriteLine("The File Browser has been cancelled. Your current dinos will remain.");
                return;
            }

            Console.WriteLine("Your dinosaurs were successfully saved to the chosen file. Congratulations!");
        }

        // This method complements the previous `SaveDinosToFile` method, where it reads a file which the player selects through the file browser and loads the Dinosaurs contained within the file into the current session
        private void LoadDinosFromFile()
        {
            Console.Write("The File Browser has been opened. Choose a file or press CANCEL to return to the menu.");
            DialogResult result = FileBrowser.ShowDialog();

            Console.Clear();
            if (result == DialogResult.OK)
                this.Dinosaurs = CircularLinkedList.Restore(FileBrowser.FileName);
            else if (result == DialogResult.Cancel)
            {
                Console.WriteLine("The File Browser has been cancelled. Your current dinos will remain.");
                return;
            }

            Console.WriteLine($"You have successfully loaded {this.Dinosaurs.Count} Dinosaurs into your program. Congratulations!!");
        }
        
        // I believe this method is quite self-explanatory just from the method name. It simply prompts the player if they want to exit the program or restart the game
        private void Quit()
        {
            int quit = Menu.CollectChoice("Do you intend to Quit or Restart?", new string[] { "Quit", "Restart" });

            if (quit == 0)
            {
                Console.Write($"\nAlright, {this.PlayerName}. I will save your progress and the next time you come back, you can load your data and keep playing where you left off!\n\n");
                CircularLinkedList.SaveToFile(this.Dinosaurs);

                // Initiate the timer for exiting the process
                int exitCount = 8;
                System.Timers.Timer exitTimer = new System.Timers.Timer()
                {
                    Interval = 1000,
                    Enabled = true
                };

                // The following code includes a lambda expression, (used multiple times throughout):
                // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/lambda-expressions
                exitTimer.Elapsed += new ElapsedEventHandler((object source, ElapsedEventArgs e) =>
                {
                    // This function will be executed every time the Timer lapses (1 second)
                    exitCount--;
                    Console.Write($"\x000D[{exitCount}] Press [ RETURN ] to exit.");
                    if (exitCount == 0 || Console.ReadKey().Key == ConsoleKey.Enter)
                        Environment.Exit(0);
                });

                // Keep the process alive
                while (true)
                    Thread.Sleep(6000);
            }

            else
                this.Restart = true;
        }
    }
}
