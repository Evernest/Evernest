using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace EvernestBack
{
    class History
    {
        private class Node
        {
            public Node(long key, ulong element, Node left, Node right)
            {
                Key = key;
                Element = element;
                Left = left;
                Right = right;
            }
            public long Key {get; private set;}
            public ulong Element {get; private set;}
            public Node Left {get; set; }
            public Node Right {get; set; }
        }

        public History()
        {
            ElementCounter = 0;
            Mislinked = new Stack<Node>();
            LastNode = null;
            Root = null;
        }

        public void Clear()
        {
            ElementCounter = 0;
            LastNode = null;
            Root = null;
            Mislinked.Clear();
        }

        public void Insert(long key, ulong element)
        {
            if((ElementCounter & 1) != 0)
            {
                LastNode.Right = new Node(key, element, null, null);
                for(long tmp = ElementCounter; (tmp & 2) != 0 ; tmp>>=1, Mislinked.Pop() );
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
            ElementCounter++;
        }

        public bool UpperBound(long key, ref ulong element)
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

        public bool LowerBound(long key, ref ulong element)
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

        public bool GreaterElement(ref ulong element)
        {
            if(LastNode != null)
                element = LastNode.Element;
            return LastNode != null;
        }

        public Byte[] Serialize() //missing endianness check/byte reordering
        {
            long byteCount = sizeof(long) + ElementCounter * (sizeof(long) + sizeof(ulong));
            Byte[] serializedHistory = new Byte[byteCount+sizeof(long)];
            Byte[] sizeBytes = BitConverter.GetBytes(byteCount);
            Byte[] treeCountBytes = BitConverter.GetBytes(ElementCounter);
            Buffer.BlockCopy(sizeBytes, 0, serializedHistory, 0, sizeof(long));
            Buffer.BlockCopy(treeCountBytes, 0, serializedHistory, sizeof(long), sizeof(long));
            int offset = sizeof(long)+sizeof(long);
            //infix traversal (writes nodes in their key's order)
            Stack<Node> currentlyVisitedNodes = new Stack<Node>();
            Node currentNode;
            currentlyVisitedNodes.Push(Root);
            while ((currentNode = currentlyVisitedNodes.Last().Left) != null)
                currentlyVisitedNodes.Push(currentNode);
            Byte[] keyBytes;
            Byte[] elementBytes;
            while (currentlyVisitedNodes.Count > 0)
            {
                currentNode = currentlyVisitedNodes.First();
                keyBytes = BitConverter.GetBytes(currentNode.Key);
                elementBytes = BitConverter.GetBytes(currentNode.Element);
                Buffer.BlockCopy(keyBytes, 0, serializedHistory, offset, sizeof(long));
                offset += sizeof(long);
                Buffer.BlockCopy(elementBytes, 0, serializedHistory, offset, sizeof(ulong));
                offset += sizeof(ulong);
                currentNode = currentNode.Right;
                currentlyVisitedNodes.Pop();
                if(currentNode != null)
                {
                    currentlyVisitedNodes.Push(currentNode);
                    while ((currentNode = currentlyVisitedNodes.First().Left) != null)
                        currentlyVisitedNodes.Push(currentNode);
                }
            }
            return serializedHistory;
        }

        public void Deserialize(Byte[] src, int offset)
        {
            Clear();
            long elementCount = BitConverter.ToInt64(src, offset);
            long key;
            ulong element;
            offset += sizeof(long);
            for( long i = 0 ; i < elementCount ; i++ )
            {
                key = BitConverter.ToInt64(src, offset);
                offset += sizeof(long);
                element = BitConverter.ToUInt64(src, offset);
                offset += sizeof(ulong);
                Insert(key, element);
            }
        }

        public void ReadFromBlob(CloudBlockBlob blob)
        {
            Byte[] sizeBytes = new Byte[sizeof(long)];
            blob.DownloadRangeToByteArray(sizeBytes, 0, 0, sizeof(long));
            long byteCount = BitConverter.ToInt64(sizeBytes, 0);
            Byte[] serializedHistory = new Byte[byteCount];
            blob.DownloadRangeToByteArray(serializedHistory, 0, sizeof(long), byteCount);
            Deserialize(serializedHistory, 0);
        }

        private Stack<Node> Mislinked;
        private Node Root;
        private Node LastNode;
        private long ElementCounter;
    }
}