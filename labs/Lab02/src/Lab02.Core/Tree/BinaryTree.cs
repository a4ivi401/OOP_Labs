using System.Collections;

namespace Lab02.Core.Tree;

public class BinaryTree<T> : IEnumerable<T> where T : class
{
    private readonly IComparer<T> _comparer;

    public BinaryTreeNode<T>? Root { get; private set; }
    public int Count { get; private set; }
    public IComparer<T> Comparer => _comparer;

    public BinaryTree(IComparer<T>? comparer = null)
    {
        _comparer = comparer ?? Comparer<T>.Default;
    }

    public BinaryTree(IEnumerable<T> items, IComparer<T>? comparer = null) : this(comparer)
    {
        ArgumentNullException.ThrowIfNull(items);
        AddRange(items);
    }

    public void Add(T item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (Root is null)
        {
            Root = new BinaryTreeNode<T>(item);
            Count = 1;
            return;
        }

        BinaryTreeNode<T> current = Root;
        while (true)
        {
            int comparison = _comparer.Compare(item, current.Value);
            if (comparison < 0)
            {
                if (current.Left is null)
                {
                    current.Left = new BinaryTreeNode<T>(item);
                    Count++;
                    return;
                }

                current = current.Left;
            }
            else
            {
                if (current.Right is null)
                {
                    current.Right = new BinaryTreeNode<T>(item);
                    Count++;
                    return;
                }

                current = current.Right;
            }
        }
    }

    public void AddRange(IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        foreach (T item in items)
        {
            Add(item);
        }
    }

    public bool Contains(T item)
    {
        ArgumentNullException.ThrowIfNull(item);

        BinaryTreeNode<T>? current = Root;
        while (current is not null)
        {
            int comparison = _comparer.Compare(item, current.Value);
            if (comparison == 0)
            {
                return true;
            }

            current = comparison < 0 ? current.Left : current.Right;
        }

        return false;
    }

    public void Clear()
    {
        Root = null;
        Count = 0;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new PostOrderTreeEnumerator<T>(Root);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public T[] ToPostOrderArray()
    {
        var list = new List<T>(Count);
        TraversePostOrder(Root, list);
        return list.ToArray();
    }

    public T[] ToInOrderArray()
    {
        var list = new List<T>(Count);
        TraverseInOrder(Root, list);
        return list.ToArray();
    }

    public T[] ToPreOrderArray()
    {
        var list = new List<T>(Count);
        TraversePreOrder(Root, list);
        return list.ToArray();
    }

    private static void TraversePostOrder(BinaryTreeNode<T>? node, List<T> result)
    {
        if (node is null) return;
        TraversePostOrder(node.Left, result);
        TraversePostOrder(node.Right, result);
        result.Add(node.Value);
    }

    private static void TraverseInOrder(BinaryTreeNode<T>? node, List<T> result)
    {
        if (node is null) return;
        TraverseInOrder(node.Left, result);
        result.Add(node.Value);
        TraverseInOrder(node.Right, result);
    }

    private static void TraversePreOrder(BinaryTreeNode<T>? node, List<T> result)
    {
        if (node is null) return;
        result.Add(node.Value);
        TraversePreOrder(node.Left, result);
        TraversePreOrder(node.Right, result);
    }
}
