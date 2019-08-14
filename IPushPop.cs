using System.Collections.Generic;
namespace PathFinder {
	public interface IPushPop<T> : ICollection<T> {
		T Pop();
		T Peek();
		bool hasNext { get; }
	}
}
