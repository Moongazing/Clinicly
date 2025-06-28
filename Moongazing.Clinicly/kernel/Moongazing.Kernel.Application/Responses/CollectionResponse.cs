namespace Moongazing.Kernel.Application.Responses;

public class CollectionResponse<T> : ICollection<T>
{
    private readonly ICollection<T> Items = new List<T>();

    public int Count => Items.Count;
    public bool IsReadOnly => Items.IsReadOnly;

    public void Add(T item) => Items.Add(item);
    public void Clear() => Items.Clear();
    public bool Contains(T item) => Items.Contains(item);
    public void CopyTo(T[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    public bool Remove(T item) => Items.Remove(item);
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => Items.GetEnumerator();
}
