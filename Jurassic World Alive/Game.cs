using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Newtonsoft.Json;

namespace Jurassic_World_Alive
{
    class Game
    {
        public bool Restart = false;

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
            if (playerName == null)
            {
                this.PlayerName = Menu.CollectAnswer($"Welcome to Jurassic World Alive! What's your name?");

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
            }

            else
            {
                this.PlayerName = playerName;
                this.Dinosaurs = CircularLinkedList.Restore(this.PlayerName);
            }

            this.ShowMenu();
        }

        private void ShowMenu()
        {
            Console.Clear();
            int playerChoice = Menu.CollectChoice("What would you like to do?", new string[] { "Visualise the current list of dinosaurs", "Create a new Dinosaur", "Remove a Dinosaur", "Display a Dinosaur's information", "Load Dinosaurs from a file", "Save Dinosaurs to file", "Quit" });

            Console.Clear();
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

            Console.Write("\nTo proceed, press [ RETURN ]...");
            Console.ReadLine();
            this.ShowMenu();
        }

        private void VisualiseListWithControls()
        {
            throw new NotImplementedException();
        }

        private void CreateNewDinoDialog()
        {
            throw new NotImplementedException();
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
            Game instance = new Game();

            if (instance.Restart)
                Main(args);
        }
    }
}
