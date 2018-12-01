using System;

namespace Jurassic_World_Alive
{
    // These `enum`s declare the different Types and Periods, as outlined in the specification brief
    enum DinosaurType { Carnivorous, Herbivorous }
    enum DinosaurPeriod { Jurassic, Triassic, Cretaceous }

    /*
     * This class acts as the "Node" for <CircularLinkedList>, for which I originally created a <CircularLinkedListNode>,
     * which was then inherited into the <Dinosaur> class, and even featured some use of polymorphism.
     * 
     * The reason I decided not to inherit <Dinosaur> from <CircularLinkedListNode> is because this project is very specific
     * to the Dinosaur class, so it is a lot easier to implement this class into <CircularLinkedListNode>.
     * I believe there are some early commits that show the attempted implementation of this system.
     */
    class Dinosaur
    {
        // Create variables that identify this Dinosaur
        public string Species { get; set; }
        public DinosaurType Type { get; set; }
        public DinosaurPeriod Period { get; set; }
        // Create a reference to the <CircularLinkedList> that will be holding this Dinosaur
        public CircularLinkedList ParentList { get; set; }

        // Create a public Getter that reflects this instance's ID based on its position in the `ParentList`
        public int ID
        {
            get
            {
                for (int i = 0; i < this.ParentList.Count; i++)
                {
                    if (this == this.ParentList[i])
                        return i;
                }

                return -1;
            }
        }

        private int nextIndex
        {
            get
            {
                if (this.ID == this.ParentList.Count - 1)
                    return 0;
                return this.ID + 1;
            }
        }
        // This returns the next Dinosaur in the referenced <CircularLinkedList>, which I unfortunately never made use of
        public Dinosaur Next
        {
            get
            {
                return this.ParentList[this.nextIndex];
            }
        }

        private int previousIndex
        {
            get
            {
                if (this.ID == 0)
                    return this.ParentList.Count - 1;
                return this.ID - 1;
            }
        }
        // This returns the previous Dinosaur in the referenced <CircularLinkedList>, which I unfortunately never made use of
        public Dinosaur Previous
        {
            get
            {
                return this.ParentList[this.previousIndex];
            }
        }
        
        // This static function creates a new dialog in the console, guiding the player to create a new Dinosaur object
        public static Dinosaur CreateNewDialog(CircularLinkedList parentList)
        {
            string species = Menu.CollectAnswer("Let's make a new Dinosaur! What is the dinosaur's Species Name going to be?", 19);
            DinosaurType type = (DinosaurType)Menu.CollectChoice("\nGreat! What about the dinosaur's Type?", new string[] { DinosaurType.Carnivorous.ToString(), DinosaurType.Herbivorous.ToString() });
            DinosaurPeriod period = (DinosaurPeriod)Menu.CollectChoice("\nLastly, which period did the dinosaur live in?", new string[] { DinosaurPeriod.Jurassic.ToString(), DinosaurPeriod.Triassic.ToString(), DinosaurPeriod.Cretaceous.ToString() });

            return new Dinosaur(parentList, species, type, period);
        }

        // The default constructor for <Dinosaur>
        public Dinosaur(CircularLinkedList parentList, string speciesName, DinosaurType type, DinosaurPeriod period)
        {
            this.ParentList = parentList;
            this.Species = speciesName;
            this.Type = type;
            this.Period = period;
        }

        // This simply calls the referenced <CircularLinkedList>'s `Remove` function with the `this` argument
        public Dinosaur Remove()
        {
            this.ParentList.Remove(this);
            return this;
        }

        // Similar to the static `CreateNewDialog` method, this method guides the player through updating this Dinosaur's Species
        public void UpdateSpeciesDialog()
        {
            this.Species = Menu.CollectAnswer($"\nAlright, what is {{ {this.Species} }}'s new Species going to be?");
            Console.Write($"\nThe dinosaur's Species was updated to {{ {this.Species} }}.");

            // Persistence
            CircularLinkedList.SaveToFile(this.ParentList);
        }

        // Similar to the static `CreateNewDialog` method, this method guides the player through updating this Dinosaur's Type
        public void UpdateTypeDialog()
        {
            this.Type = (DinosaurType)Menu.CollectChoice($"\nAlright, what is {{ {this.Species} }}'s new Type going to be?", new string[] { DinosaurType.Carnivorous.ToString(), DinosaurType.Herbivorous.ToString() });
            Console.Write($"\nThe dinosaur's Type was updated to {{ {this.Type} }}.");

            // Persistence
            CircularLinkedList.SaveToFile(this.ParentList);
        }

        // Similar to the static `CreateNewDialog` method, this method guides the player through updating this Dinosaur's Period
        public void UpdatePeriodDialog()
        {
            this.Period = (DinosaurPeriod)Menu.CollectChoice($"\nAlright, what is {{ {this.Species} }}'s new Period going to be?", new string[] { DinosaurPeriod.Jurassic.ToString(), DinosaurPeriod.Triassic.ToString(), DinosaurPeriod.Cretaceous.ToString() });
            Console.Write($"\nThe dinosaur's Period was updated to {{ {this.Period} }}.");

            // Persistence
            CircularLinkedList.SaveToFile(this.ParentList);
        }

        // A special function used in `<CircularLinkedList>.Visualise`
        public string[] Columns()
        {
            return new string[] { "Species", "Type", "Period" };
        }

        // A special function used in `<CircularLinkedList>.Visualise`
        public string[] Rows()
        {
            return new string[] { this.Species, this.Type.ToString(), this.Period.ToString() };
        }

        // A custom `ToString` function
        public override string ToString()
        {
            return $"{this.Species.PadRight(20)}| {this.Type.ToString().PadRight(20)}| {this.Period.ToString().PadRight(20)}";
        }

        // This special function serialises the current object into a JSON string, enabling it to be
        // saved easily to any full or part of a JSON file
        public string Serialise()
        {
            return $@"{{""Species"": ""{this.Species}"", ""Type"": ""{this.Type}"", ""Period"": ""{this.Period}""}}";
        }
        // Add an Americanised alias
        public string Serialize()
        {
            return this.Serialise();
        }
    }
}
