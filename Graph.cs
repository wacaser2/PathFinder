using System;
using System.Collections.Generic;
using General;

namespace PathFinder {
	public class Graph<T> : IGraph<T> where T : IIndexed {
		private Func<T[]> _tiles;
		private Func<int, IEnumerable<T>> _neighbors;

		public T[] tiles => _tiles();
		public IEnumerable<T> GetNeighbors(int idx) => _neighbors(idx);

		public Graph(Func<T[]> tiles, Func<int, IEnumerable<T>> neighbors) {
			_tiles = tiles;
			_neighbors = neighbors;
		}
	}
}