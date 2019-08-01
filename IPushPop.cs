using System.Collections.Generic;

public interface IPushPop<T> : ICollection<T> {
    T Pop();
    T Peek();
    bool hasNext { get; }
}

public class PPQueue<T> : Queue<T>, IPushPop<T> {
    public bool hasNext => Count > 0;
    public bool IsReadOnly => false;

    public void Add(T item) => Enqueue(item);
    public T Pop() => Dequeue();
    public bool Remove(T item) => false;
}

public class PPStack<T> : Stack<T>, IPushPop<T> {
    public bool hasNext => Count > 0;
    public bool IsReadOnly => false;

    public void Add(T item) => Push(item);
    public bool Remove(T item) => false;
}