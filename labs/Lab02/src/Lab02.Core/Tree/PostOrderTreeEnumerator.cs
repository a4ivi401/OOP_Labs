using System.Collections;

namespace Lab02.Core.Tree;

public class PostOrderTreeEnumerator<T> : IEnumerator<T> where T : class
{
    private readonly BinaryTreeNode<T>? _root;
    private T[] _elements;
    private int _currentIndex;

    public PostOrderTreeEnumerator(BinaryTreeNode<T>? root)
    {
        _root = root;
        _elements = BuildPostOrderList(root);
        _currentIndex = -1;
    }

    public T Current
    {
        get
        {
            if (_currentIndex < 0 || _currentIndex >= _elements.Length)
            {
                throw new InvalidOperationException("Ітератор знаходиться поза межами колекції.");
            }

            return _elements[_currentIndex];
        }
    }

    object? IEnumerator.Current => Current;

    public bool MoveNext()
    {
        if (_currentIndex + 1 < _elements.Length)
        {
            _currentIndex++;
            return true;
        }

        return false;
    }

    public void Reset()
    {
        _elements = BuildPostOrderList(_root);
        _currentIndex = -1;
    }

    public void Dispose()
    {
    }

    private static T[] BuildPostOrderList(BinaryTreeNode<T>? root)
    {
        var list = new List<T>();
        TraversePostOrder(root, list);
        return list.ToArray();
    }

    private static void TraversePostOrder(BinaryTreeNode<T>? node, List<T> result)
    {
        if (node is null)
        {
            return;
        }

        TraversePostOrder(node.Left, result);
        TraversePostOrder(node.Right, result);
        result.Add(node.Value);
    }
}
