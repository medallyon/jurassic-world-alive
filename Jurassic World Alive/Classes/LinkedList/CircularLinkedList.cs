using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Newtonsoft.Json;

namespace Jurassic_World_Alive
{
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
            this.Elements = new CircularLinkedListNode[0];
        }

        public CircularLinkedList(params Dinosaur[] elements)
        {
            this.Elements = elements;
        }

        private CircularLinkedListNode[] CreateNewList(int additions = 1)
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
            return this;
        }

        public CircularLinkedList Push(CircularLinkedList items)
        {
            this.Elements = this.CreateNewList(items.ToArray());
            return this;
        }

        public CircularLinkedList Insert(int index, Dinosaur item)
        {
            this.Elements = this.CreateNewList();
            for (int i = index; i < this.Count - 2; i++)
                this[i + 1] = this[i];

            this[index] = item;
            return this;
        }

        public CircularLinkedList Pop(int amount = 1)
        {
            Dinosaur[] newList = this.CreateNewList(amount * -1);

            CircularLinkedList poppedItems = new CircularLinkedList(newList);
            this.Elements = newList;

            return poppedItems;
        }

        public Dinosaur RemoveAt(int index)
        {
            if (index < 0 || index > this.Count - 1)
                throw new IndexOutOfRangeException();

            Dinosaur removedDino = this[index];
            for (int i = index; i < this.Count - 2; i++)
                this[i] = this[i + 1];

            this.Elements = this.CreateNewList(-1);
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
    }
}
