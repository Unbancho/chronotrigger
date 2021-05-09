using System;
using System.Collections;
using System.Collections.Generic;

namespace ChronoTrigger.Extensions
{
    internal enum NodeColor : byte
    {
        Black,
        Red
    }

    public sealed class SortedSetF<T> : ICollection<T> where T: unmanaged, IComparable<T>
    {
        private Node _root;
        private readonly IComparer<T> _comparer;

        public SortedSetF()
        {
            _comparer = Comparer<T>.Default;
        }

        public void Add(T item)
        {
            if (_root == null)
            {
                _root = new (item, NodeColor.Black);
                Count = 1;
                return;
            }


            var current = _root;
            Node parent = null;
            Node grandParent = null;
            Node greatGrandParent = null;

            var order = 0;
            while (current != null)
            {
                order = _comparer.Compare(item, current.Item);
                if (order == 0)
                {
                    _root!.ColorBlack();
                    return;
                }

                if (current.Is4Node)
                {
                    current.Split4Node();
                    if (Node.IsNonNullRed(parent))
                    {
                        InsertionBalance(current, ref parent!, grandParent!, greatGrandParent!);
                    }
                }

                greatGrandParent = grandParent;
                grandParent = parent;
                parent = current;
                current = (order < 0) ? current.Left : current.Right;
            }
            
            var node = new Node(item, NodeColor.Red);
            if (order > 0)
            {
                parent!.Right = node;
            }
            else
            {
                parent!.Left = node;
            }

            if (parent.IsRed)
            {
                InsertionBalance(node, ref parent!, grandParent!, greatGrandParent!);
            }

            _root!.ColorBlack();
            ++Count;
        }
        
        private void InsertionBalance(Node current, ref Node parent, Node grandParent, Node greatGrandParent)
        {
            var parentIsOnRight = grandParent.Right == parent;
            var currentIsOnRight = parent.Right == current;

            Node newChildOfGreatGrandParent;
            if (parentIsOnRight == currentIsOnRight)
            {
                newChildOfGreatGrandParent = currentIsOnRight ? grandParent.RotateLeft() : grandParent.RotateRight();
            }
            else
            {
                newChildOfGreatGrandParent = currentIsOnRight ? grandParent.RotateLeftRight() : grandParent.RotateRightLeft();
                parent = greatGrandParent;
            }

            grandParent.ColorRed();
            newChildOfGreatGrandParent.ColorBlack();

            ReplaceChildOrRoot(greatGrandParent, grandParent, newChildOfGreatGrandParent);
        }
        
        private void ReplaceChildOrRoot(Node parent, Node child, Node newChild)
        {
            if (parent != null)
            {
                parent.ReplaceChild(child, newChild);
            }
            else
            {
                _root = newChild;
            }
        }

        void ICollection<T>.Add(T item) => Add(item);

        public void Clear()
        {
            _root = null;
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array) => CopyTo(array, 0, Count);

        public void CopyTo(T[] array, int index) => CopyTo(array, index, Count);

        public void CopyTo(T[] array, int index, int count)
        {
            var i = index;
            foreach (var c in this)
            {
                array[i++] = c;
                if(count == i) break;
            }
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public int Count { get; private set; } = 0;
        public bool IsReadOnly => false;

        internal sealed class Node
        {
            public Node(T item, NodeColor color)
            {
                Item = item;
                Color = color;
            }

            public static bool IsNonNullRed(Node node) => node != null && node.IsRed;

            public T Item { get; }

            public Node Left { get; set; }

            public Node Right { get; set; }

            private NodeColor Color { get; set; }

            public bool IsRed => Color == NodeColor.Red;

            public bool Is4Node => IsNonNullRed(Left) && IsNonNullRed(Right);

            public void ColorBlack() => Color = NodeColor.Black;

            public void ColorRed() => Color = NodeColor.Red;

            public void Split4Node()
            {
                ColorRed();
                Left!.ColorBlack();
                Right!.ColorBlack();
            }

            public Node RotateLeft()
            {
                var child = Right!;
                Right = child!.Left;
                child.Left = this;
                return child;
            }
            
            public Node RotateLeftRight()
            {
                var child = Left!;
                var grandChild = child!.Right!;

                Left = grandChild!.Right;
                grandChild.Right = this;
                child.Right = grandChild.Left;
                grandChild.Left = child;
                return grandChild;
            }
            
            public Node RotateRight()
            {
                var child = Left!;
                Left = child!.Right;
                child.Right = this;
                return child;
            }
            
            public Node RotateRightLeft()
            {
                var child = Right!;
                var grandChild = child!.Left!;

                Right = grandChild!.Left;
                grandChild.Left = this;
                child.Left = grandChild.Right;
                grandChild.Right = child;
                return grandChild;
            }

            public void ReplaceChild(Node child, Node newChild)
            {
                if (Left == child)
                {
                    Left = newChild;
                }
                else
                {
                    Right = newChild;
                }
            }
        }

        private static int Log2(int value)
        {
            var result = 0;
            while (value > 0)
            {
                result++;
                value >>= 1;
            }
            return result;
        }

        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private int TotalCount() { return Count; }

        private static bool IsWithinRange() => true;

        private struct Enumerator : IEnumerator<T>
        {
            private readonly SortedSetF<T> _tree;

            private readonly Stack<Node> _stack;
            private Node _current;

            private readonly bool _reverse;

            internal Enumerator(SortedSetF<T> set)
                : this(set, false)
            {
            }

            private Enumerator(SortedSetF<T> set, bool reverse)
            {
                _tree = set;

                _stack = new (2 * Log2(set.TotalCount() + 1));
                _current = null;
                _reverse = reverse;

                Initialize();
            }

            private void Initialize()
            {
                _current = null;
                var node = _tree._root;
                while (node != null)
                {
                    var next = (_reverse ? node.Right : node.Left);
                    var other = (_reverse ? node.Left : node.Right);
                    if (IsWithinRange())
                    {
                        _stack.Push(node);
                        node = next;
                    }
                    else if (next == null || !IsWithinRange())
                    {
                        node = other;
                    }
                    else
                    {
                        node = next;
                    }
                }
            }

            public bool MoveNext()
            {
                if (_stack.Count == 0)
                {
                    _current = null;
                    return false;
                }

                _current = _stack.Pop();
                var node = (_reverse ? _current!.Left : _current!.Right);
                while (node != null)
                {
                    var next = (_reverse ? node.Right : node.Left);
                    var other = (_reverse ? node.Left : node.Right);
                    if (IsWithinRange())
                    {
                        _stack.Push(node);
                        node = next;
                    }
                    else if (other == null || !IsWithinRange())
                    {
                        node = next;
                    }
                    else
                    {
                        node = other;
                    }
                }
                return true;
            }

            public void Dispose() { }

            public T Current => _current?.Item ?? default;

            object IEnumerator.Current => _current!.Item;

            private void Reset()
            {
                _stack.Clear();
                Initialize();
            }

            void IEnumerator.Reset() => Reset();
        }

    }
}