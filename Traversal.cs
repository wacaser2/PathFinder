using System;
using System.Collections.Generic;

using General;

namespace PathFinder {
	public class Traversal<T> where T : IIndexed {
		#region Data
		private IGraph<T> _graph;
		private IEnumerable<int> _startTiles;
		#endregion

		#region Constructors
		public Traversal(IGraph<T> graph, int start) {
			_graph = graph;
			_startTiles = new[] { start };
		}

		public Traversal(IGraph<T> graph, IEnumerable<int> start) {
			_graph = graph;
			_startTiles = start;
		}
		#endregion

		#region PrivateMethods
		private IEnumerable<Node<T>> Traverse(IPushPop<Node<T>> queue, Func<T, bool> checkAdd, Func<T, T, float> findCost) {
			T[] tiles = _graph.tiles;
			if(tiles == null)
				yield break;

			Dictionary<int, Node<T>> _map = new Dictionary<int, Node<T>>();

			foreach(int start in _startTiles) {
				Node<T> first = new Node<T>(tiles[start], 0, null);
				queue.Add(first);
				_map.Add(start, first);
			}

			Node<T> current;
			do {
				current = queue.Pop();
				yield return current;

				foreach(T n in _graph.GetNeighbors(current.idx)) {
					if(checkAdd(n)) {
						float step = current.cost + findCost(current.tile, n);
						if(!_map.ContainsKey(n.idx)) {
							Node<T> next = new Node<T>(n, step, current);
							_map.Add(n.idx, next);
							queue.Add(next);
						} else if(step < _map[n.idx].cost) {
							_map[n.idx].cost = step;
							_map[n.idx].next = current;
						}

					}
				}
			} while(queue.hasNext);
		}
		#endregion

		#region Traversals
		public IEnumerable<Node<T>> SelectDijkstra(Func<T, bool> checkAdd, Func<T, T, float> findCost) =>
			Traverse(new Heap<Node<T>>((s, t) => s.cost < t.cost, null, (s, t) => s.reduce = t, (s, t) => s.heapIndex = t), checkAdd, findCost);
		public IEnumerable<Node<T>> SelectBreadthFirst(Func<T, bool> checkAdd, Func<T, T, float> findCost) =>
			Traverse(new PPQueue<Node<T>>(), checkAdd, findCost);
		public IEnumerable<Node<T>> SelectDepthFirst(Func<T, bool> checkAdd, Func<T, T, float> findCost) =>
			Traverse(new PPStack<Node<T>>(), checkAdd, findCost);
		#endregion
	}
}