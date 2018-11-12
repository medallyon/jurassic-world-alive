using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jurassic_World_Alive
{
    enum DinosaurType { Carnivorous, Herbivorous }
    enum DinosaurPeriod { Jurassic, Triassic, Cretaceous }

    class Dinosaur : CircularLinkedListNode
    {
        public string Species { get; set; }
        public DinosaurType Type { get; set; }
        public DinosaurPeriod Period { get; set; }

        public Dinosaur(CircularLinkedList parentList, string speciesName, DinosaurType type, DinosaurPeriod period) : base(parentList)
        {
            this.Species = speciesName;
            this.Type = type;
            this.Period = period;
        }
    }
}
