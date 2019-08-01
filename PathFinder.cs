using System;
using System.Collections.Generic;
using System.Linq;

public class PathFinder {

	#region PFData
	private Game _g;
	private Address _addr;
	#endregion

	#region TraversalData
	private Dictionary<int, Node> _map;
	#endregion

	#region ClassCode

	public PathFinder(Address addr = null, Game g = null) {
		_addr = (addr ?? GameManager.currAddr).copy;
		_g = g ?? GameManager.game;
	}

	#endregion

	#region Subclasses

	public class Node {
		#region Data
		private Tile _tile;
		private float _cost;
		private Node _next;
		#endregion

		#region Properties
		public Tile tile => _tile;
		public int idx => _tile.idx;
		public float cost {
			get => _cost;
			set {
				_cost = value;
				reduce?.Invoke(heapIndex);
			}
		}
		public Node next { get => _next; set => _next = value; }
		#endregion

		#region Heap
		public Action<int> reduce = null;   //on cost change
		public int heapIndex;               //function value
		#endregion

		public Node(Tile tile, float cost, Node next) {
			_tile = tile;
			_cost = cost;
			_next = next;
		}

		#region Operations
		public Node reverse {
			get {
				Node current = this;
				Node path = new Node(current.tile, .001f, null);
				float prevCost = current.cost;
				while(current.next != null) {
					current = current.next;
					path = new Node(current.tile, prevCost, path);
					prevCost = current.cost;
				}
				return path;
			}
		}
		public Node end {
			get {
				Node curr = this;
				while(curr.next != null)
					curr = curr.next;
				return curr;
			}
		}

		public IEnumerable<Node> path {
			get {
				Node current = this;
				do {
					yield return current;
					current = current.next;
				} while(current != null);
			}
		}

		#endregion
	}

	#endregion

	#region Methods

	public Node GetPath(int tile, PassFlags pass) => SelectPath(pass).FirstOrDefault(p => p.idx == tile)?.reverse;

	public IEnumerable<Node> SelectPath(PassFlags pass) =>
		SelectDijkstra(tile => (pass & tile.pass) != PassFlags.None, (curr, next) => _g.FindMoveCost(_addr.Tile(curr.idx), next.idx));

	public IEnumerable<Node> SelectCityDistance(int idx) => SelectDijkstra(tile => true, (curr, next) => (next.objIndex == idx ? 0 : 1));

	public IEnumerable<Node> SelectFlatDistance() => SelectBreadthFirst(tile => true, (tile, next) => 1);

	#endregion

	#region Traversals

	private IEnumerable<Node> Traverse(IPushPop<Node> queue, Func<Tile, bool> checkAdd, Func<Tile, Tile, float> findCost) {
		Tile[] tiles = _g.GetTiles(_addr);
		if(tiles == null)
			yield break;

		_map = new Dictionary<int, Node>();

		Node first = new Node(tiles[_addr.t], 0, null);
		queue.Add(first);
		_map.Add(_addr.t, first);

		Node current;
		do {
			current = queue.Pop();
			yield return current;

			foreach(Tile n in _g.GetNeighbors(_addr.Tile(current.idx)).Select(s => tiles[s])) {
				if(checkAdd(n)) {
					float step = current.cost + findCost(current.tile, n);
					if(!_map.ContainsKey(n.idx)) {
						Node next = new Node(n, step, current);
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

	private IEnumerable<Node> SelectDijkstra(Func<Tile, bool> checkAdd, Func<Tile, Tile, float> findCost) =>
		Traverse(
			new Heap<Node>((s, t) => s.cost < t.cost, null, (s, t) => s.reduce = t, (s, t) => s.heapIndex = t),
			checkAdd,
			findCost
			);

	private IEnumerable<Node> SelectBreadthFirst(Func<Tile, bool> checkAdd, Func<Tile, Tile, float> findCost) =>
		Traverse(
			new PPQueue<Node>(),
			checkAdd,
			findCost
			);

	private IEnumerable<Node> SelectDepthFirst(Func<Tile, bool> checkAdd, Func<Tile, Tile, float> findCost) =>
		Traverse(
			new PPStack<Node>(),
			checkAdd,
			findCost
			);

	#endregion

	#region StaticMethods

	public static IEnumerable<Tile> GetWithin(Address addr, int dist) => new PathFinder(addr)
		.SelectFlatDistance()
		.TakeWhile(n => n.cost <= dist)
		.Select(n => n.tile);


	#endregion
}