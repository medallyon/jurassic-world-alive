using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jurassic_World_Alive
{
    class CircularLinkedListNode
    {
        protected CircularLinkedList ParentList { get; set; }
        
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

        public int NextIndex
        {
            get
            {
                if (this.ID == this.ParentList.Count - 1)
                    return 0;
                return this.ID + 1;
            }
        }
        public CircularLinkedListNode Next
        {
            get
            {
                return this.ParentList[this.NextIndex];
            }
        }

        public int PreviousIndex
        {
            get
            {
                if (this.ID == 0)
                    return this.ParentList.Count - 1;
                return this.ID - 1;
            }
        }
        public CircularLinkedListNode Previous
        {
            get
            {
                return this.ParentList[this.PreviousIndex];
            }
        }

        public CircularLinkedListNode(CircularLinkedList parentList)
        {
            this.ParentList = parentList;
        }

        public virtual string[] Columns()
        {
            return new string[] { };
        }

        public virtual string[] Rows()
        {
            return new string[] { };
        }

        public CircularLinkedListNode Remove()
        {
            this.ParentList.Remove(this);
            return this;
        }
    }
}
