using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestBack
{
    class History<ELT_T>
    {
        private class Node
        {
            public Node(UInt64 key, ELT_T element, Node left, Node right)
            {
                Key = key;
                Element = element;
                Left = left;
                Right = right;
            }
            public UInt64 Key {get; private set;}
            public ELT_T Element {get; private set;}
            public Node Left {get; set; }
            public Node Right {get; set; }
        }

        public History()
        {
            CurrentCounter = 0;
            Mislinked = new Stack<Node>();
            LastNode = null;
            Root = null;
        }

        public void Clear()
        {
            CurrentCounter = 0;
            LastNode = null;
            Root = null;
            Mislinked.Clear();
        }

        public void Insert(UInt64 key, ELT_T element)
        {
            if((CurrentCounter & 1) != 0)
            {
                LastNode.Right = new Node(key, element, null, null);
                for(UInt64 tmp = CurrentCounter; (tmp & 2) != 0 ; tmp>>=1, Mislinked.Pop() );
            }
            else
            {
                if (LastNode != null)
                {
                    Node lastMislinkedNode = Mislinked.Last();
                    lastMislinkedNode.Right = LastNode = new Node(key, element, lastMislinkedNode.Right, null);
                    Mislinked.Push(LastNode);
                }
                else
                {
                    LastNode = Root = new Node(key, element, null, null);
                    Mislinked.Push( LastNode );
                }
            }
            CurrentCounter++;
        }

        public bool UpperBound(UInt64 key, ref ELT_T element)
        {
            Node current = Root, upperBound = null;
            while( current != null )
            {
                if (current.Key < key)
                    current = current.Right;
                else
                {
                    upperBound = current;
                    current = current.Left;
                }
            }
            if(upperBound != null)
                element = upperBound.Element;
            return upperBound != null;
        }

        public bool LeastBound(UInt64 key, ref ELT_T element)
        {
            Node current = Root, leastBound = null;
            while( current != null )
            {
                if( current.Key > key)
                    current = current.Right;
                else
                {
                    leastBound = current;
                    current = current.Left;
                }
            }
            if(leastBound != null)
                element = leastBound.Element;
            return leastBound != null;
        }

        private Stack<Node> Mislinked;
        private Node Root;
        private Node LastNode;
        private UInt64 CurrentCounter;
    }
}