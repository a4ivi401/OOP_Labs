namespace Lab02.Core.Tree;

public class BinaryTreeNode<T> where T : class
{
    public T Value { get; set; }
    public BinaryTreeNode<T>? Left { get; set; }
    public BinaryTreeNode<T>? Right { get; set; }

    public BinaryTreeNode(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Value = value;
    }
}
