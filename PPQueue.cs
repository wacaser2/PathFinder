using System.Collections.Generic;
namespace PathFinder {
	public class PPQueue<T> : Queue<T>, IPushPop<T> {
		public bool hasNext => Count > 0;
		public bool IsReadOnly => false;

		public void Add(T item) => Enqueue(item);
		public T Pop() => Dequeue();
		public bool Remove(T item) => false;
	}
}
