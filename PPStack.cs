using System.Collections.Generic;
namespace PathFinder {
	public class PPStack<T> : Stack<T>, IPushPop<T> {
		public bool hasNext => Count > 0;
		public bool IsReadOnly => false;

		public void Add(T item) => Push(item);
		public bool Remove(T item) => false;
	}
}