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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.IO;

using Newtonsoft.Json;

namespace Jurassic_World_Alive
{
    class Game
    {
        public bool Restart = false;

        public static Game CurrentSession;
        public static readonly string SessionPath = "./sessions/";
        public static List<string> SessionNames
        {
            get
            {
                if (!Directory.Exists(SessionPath))
                    Directory.CreateDirectory(SessionPath);
                
                return new List<string>(Directory.GetFiles(SessionPath));
            }
        }

        public CircularLinkedList Dinosaurs { get; set; }
        public string PlayerName { get; set; }

        public Game(string playerName = null)
        {
            CurrentSession = this;

            if (playerName == null)
                this.PlayerName = Menu.CollectAnswer($"Welcome to Jurassic World Alive! What's your name?");
            else
                this.PlayerName = playerName;

            if (SessionNames.Contains($"{this.PlayerName.ToLower()}.json"))
            {
                int loadSession = Menu.CollectChoice("\nIt seems like you've played before! Would you like to load your most recent play session?", new string[] { "Yes", "No" });

                if (loadSession == 0)
                    this.Dinosaurs = CircularLinkedList.Restore(this.PlayerName);
                else
                    this.Dinosaurs = new CircularLinkedList();
            }

            else
                this.Dinosaurs = new CircularLinkedList();

            this.ShowMenu();
        }

        private void ShowMenu()
        {
            Console.Clear();
            int playerChoice = Menu.CollectChoice($"{this.PlayerName}, " +
                $"What would you like to do?", new string[] { "Visualise the current list of dinosaurs", "Create a new Dinosaur", "Remove a Dinosaur", "Display a Dinosaur's information", "Load Dinosaurs from a file", "Save Dinosaurs to file", "Quit" });
            
            Console.Clear();
            // Use of abstraction: Extracting only the most important functions and disregarding low-level details
            // And decomposition: Splitting the problem into smaller, more managable chunks
            if (playerChoice == 0)
                this.VisualiseListWithControls();
            else if (playerChoice == 1)
                this.CreateNewDinoDialog();
            else if (playerChoice == 2)
                this.RemoveDinoDialog();
            else if (playerChoice == 3)
                this.DisplayDinoInfo();
            else if (playerChoice == 4)
                this.LoadDinosFromFile();
            else if (playerChoice == 5)
                this.SaveDinosToFile();
            else if (playerChoice == 6)
                this.Quit();

            Menu.Continue();
            this.ShowMenu();
        }

        private void VisualiseListWithControls()
        {
            throw new NotImplementedException();
        }

        private void CreateNewDinoDialog()
        {
            string species = Menu.CollectAnswer("Let's make a new Dinosaur! What is the dinosaur's Species Name going to be?");
            DinosaurType type = (DinosaurType)Menu.CollectChoice("\nGreat! What about the dinosaur's Type?", new string[] { DinosaurType.Carnivorous.ToString(), DinosaurType.Herbivorous.ToString() });
            DinosaurPeriod period = (DinosaurPeriod)Menu.CollectChoice("\nLastly, in which period did the dinosaur live?", new string[] { DinosaurPeriod.Jurassic.ToString(), DinosaurPeriod.Triassic.ToString(), DinosaurPeriod.Cretaceous.ToString() });

            Dinosaur dino = new Dinosaur(this.Dinosaurs, species, type, period);
            this.Dinosaurs.Push(dino);

            Console.Write($"\nGreat, your new {dino.Species} Dinosaur was added to the Linked List.");
        }

        private void RemoveDinoDialog()
        {
            throw new NotImplementedException();
        }

        private void DisplayDinoInfo()
        {
            throw new NotImplementedException();
        }

        private void LoadDinosFromFile()
        {
            throw new NotImplementedException();
        }

        private void SaveDinosToFile()
        {
            throw new NotImplementedException();
        }

        private void Quit()
        {
            throw new NotImplementedException();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                args = new string[] { null };

            Game instance = new Game(args[0]);

            if (instance.Restart)
                Main(new string[] { instance.PlayerName });
        }
    }
}
