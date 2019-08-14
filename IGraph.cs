using System.Collections.Generic;
using General;

namespace PathFinder {
	public interface IGraph<T> where T : IIndexed {
		T[] tiles { get; }
		IEnumerable<T> GetNeighbors(int idx);
	}
}
