using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Blob;

namespace EvernestBack
{
    internal class History
    {
        private long _elementCounter;
        private Node _lastNode;
        private Node _root;
        private readonly Stack<Node> _mislinked;

        public History()
        {
            _elementCounter = 0;
            _mislinked = new Stack<Node>();
            _lastNode = null;
            _root = null;
        }

        public void Clear()
        {
            _elementCounter = 0;
            _lastNode = null;
            _root = null;
            _mislinked.Clear();
        }

        public void Insert(long key, ulong element)
        {
            if ((_elementCounter & 1) != 0)
            {
                _lastNode.Right = new Node(key, element, null, null);
                for (var tmp = _elementCounter; (tmp & 2) != 0; tmp >>= 1, _mislinked.Pop())
                {
                }
            }
            else
            {
                if (_lastNode != null)
                {
                    var lastMislinkedNode = _mislinked.Last();
                    lastMislinkedNode.Right = _lastNode = new Node(key, element, lastMislinkedNode.Right, null);
                    _mislinked.Push(_lastNode);
                }
                else
                {
                    _lastNode = _root = new Node(key, element, null, null);
                    _mislinked.Push(_lastNode);
                }
            }
            _elementCounter++;
        }

        public bool UpperBound(long key, ref ulong element)
        {
            Node current = _root, upperBound = null;
            while (current != null)
            {
                if (current.Key < key)
                    current = current.Right;
                else
                {
                    upperBound = current;
                    current = current.Left;
                }
            }
            if (upperBound != null)
                element = upperBound.Element;
            return upperBound != null;
        }

        public bool LowerBound(long key, ref ulong element)
        {
            Node current = _root, leastBound = null;
            while (current != null)
            {
                if (current.Key > key)
                    current = current.Right;
                else
                {
                    leastBound = current;
                    current = current.Left;
                }
            }
            if (leastBound != null)
                element = leastBound.Element;
            return leastBound != null;
        }

        public bool GreaterElement(ref ulong element)
        {
            if (_lastNode != null)
                element = _lastNode.Element;
            return _lastNode != null;
        }

        public Byte[] Serialize() //missing endianness check/byte reordering
        {
            var byteCount = sizeof (long) + _elementCounter*(sizeof (long) + sizeof (ulong));
            var serializedHistory = new Byte[byteCount + sizeof (long)];
            var sizeBytes = BitConverter.GetBytes(byteCount);
            var treeCountBytes = BitConverter.GetBytes(_elementCounter);
            Buffer.BlockCopy(sizeBytes, 0, serializedHistory, 0, sizeof (long));
            Buffer.BlockCopy(treeCountBytes, 0, serializedHistory, sizeof (long), sizeof (long));
            var offset = sizeof (long) + sizeof (long);
            //infix traversal (writes nodes in their key's order)
            var currentlyVisitedNodes = new Stack<Node>();
            Node currentNode;
            currentlyVisitedNodes.Push(_root);
            while ((currentNode = currentlyVisitedNodes.Last().Left) != null)
                currentlyVisitedNodes.Push(currentNode);
            while (currentlyVisitedNodes.Count > 0)
            {
                currentNode = currentlyVisitedNodes.First();
                var keyBytes = BitConverter.GetBytes(currentNode.Key);
                var elementBytes = BitConverter.GetBytes(currentNode.Element);
                Buffer.BlockCopy(keyBytes, 0, serializedHistory, offset, sizeof (long));
                offset += sizeof (long);
                Buffer.BlockCopy(elementBytes, 0, serializedHistory, offset, sizeof (ulong));
                offset += sizeof (ulong);
                currentNode = currentNode.Right;
                currentlyVisitedNodes.Pop();
                if (currentNode != null)
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
            var elementCount = BitConverter.ToInt64(src, offset);
            offset += sizeof (long);
            for (long i = 0; i < elementCount; i++)
            {
                var key = BitConverter.ToInt64(src, offset);
                offset += sizeof (long);
                var element = BitConverter.ToUInt64(src, offset);
                offset += sizeof (ulong);
                Insert(key, element);
            }
        }

        public void ReadFromBlob(CloudBlockBlob blob)
        {
            var sizeBytes = new Byte[sizeof (long)];
            blob.DownloadRangeToByteArray(sizeBytes, 0, 0, sizeof (long));
            var byteCount = BitConverter.ToInt64(sizeBytes, 0);
            var serializedHistory = new Byte[byteCount];
            blob.DownloadRangeToByteArray(serializedHistory, 0, sizeof (long), byteCount);
            Deserialize(serializedHistory, 0);
        }

        private class Node
        {
            public Node(long key, ulong element, Node left, Node right)
            {
                Key = key;
                Element = element;
                Left = left;
                Right = right;
            }

            public long Key { get; private set; }
            public ulong Element { get; private set; }
            public Node Left { get; set; }
            public Node Right { get; set; }
        }
    }
}