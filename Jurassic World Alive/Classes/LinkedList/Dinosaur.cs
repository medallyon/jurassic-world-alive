using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jurassic_World_Alive
{
    enum DinosaurType { Carnivorous, Herbivorous }
    enum DinosaurPeriod { Jurassic, Triassic, Cretaceous }

    class Dinosaur
    {
        public string Species { get; set; }
        public DinosaurType Type { get; set; }
        public DinosaurPeriod Period { get; set; }
        public CircularLinkedList ParentList { get; set; }

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
        public Dinosaur Previous
        {
            get
            {
                return this.ParentList[this.previousIndex];
            }
        }

        public static Dinosaur CreateNewDialog(CircularLinkedList parentList)
        {
            string species = Menu.CollectAnswer("Let's make a new Dinosaur! What is the dinosaur's Species Name going to be?", 19);
            DinosaurType type = (DinosaurType)Menu.CollectChoice("\nGreat! What about the dinosaur's Type?", new string[] { DinosaurType.Carnivorous.ToString(), DinosaurType.Herbivorous.ToString() });
            DinosaurPeriod period = (DinosaurPeriod)Menu.CollectChoice("\nLastly, which period did the dinosaur live in?", new string[] { DinosaurPeriod.Jurassic.ToString(), DinosaurPeriod.Triassic.ToString(), DinosaurPeriod.Cretaceous.ToString() });

            return new Dinosaur(parentList, species, type, period);
        }

        public Dinosaur(CircularLinkedList parentList, string speciesName, DinosaurType type, DinosaurPeriod period)
        {
            this.ParentList = parentList;
            this.Species = speciesName;
            this.Type = type;
            this.Period = period;
        }

        public Dinosaur Remove()
        {
            this.ParentList.Remove(this);
            return this;
        }

        public string[] Columns()
        {
            return new string[] { "Species", "Type", "Period" };
        }

        public string[] Rows()
        {
            return new string[] { this.Species, this.Type.ToString(), this.Period.ToString() };
        }

        public override string ToString()
        {
            return $"{this.Species.PadRight(20)}| {this.Type.ToString().PadRight(20)}| {this.Period.ToString().PadRight(20)}";
        }

        public void UpdateSpeciesDialog()
        {
            this.Species = Menu.CollectAnswer($"\nAlright, what is {{ {this.Species} }}'s new Species going to be?");
            Console.Write($"\nThe dinosaur's Species was updated to {{ {this.Species} }}.");
        }

        public void UpdateTypeDialog()
        {
            this.Type = (DinosaurType)Menu.CollectChoice($"\nAlright, what is {{ {this.Species} }}'s new Type going to be?", new string[] { DinosaurType.Carnivorous.ToString(), DinosaurType.Herbivorous.ToString() });
            Console.Write($"\nThe dinosaur's Type was updated to {{ {this.Type} }}.");
        }

        public void UpdatePeriodDialog()
        {
            this.Period = (DinosaurPeriod)Menu.CollectChoice($"\nAlright, what is {{ {this.Species} }}'s new Period going to be?", new string[] { DinosaurPeriod.Jurassic.ToString(), DinosaurPeriod.Triassic.ToString(), DinosaurPeriod.Cretaceous.ToString() });
            Console.Write($"\nThe dinosaur's Period was updated to {{ {this.Period} }}.");
        }
    }
}
