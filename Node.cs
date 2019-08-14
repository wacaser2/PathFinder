using System;
using System.Collections.Generic;
using General;

namespace PathFinder {
	public class Node<T> : IIndexed where T : IIndexed {
		#region Data
		private T _tile;
		private float _cost;
		private Node<T> _next;
		#endregion

		#region Properties
		public T tile => _tile;
		public int idx => _tile.idx;
		public float cost {
			get => _cost;
			set {
				_cost = value;
				reduce?.Invoke(heapIndex);
			}
		}
		public Node<T> next { get => _next; set => _next = value; }
		#endregion

		#region Heap
		public Action<int> reduce = null;   //on cost change
		public int heapIndex;               //function value
		#endregion

		public Node(T tile, float cost, Node<T> next) {
			_tile = tile;
			_cost = cost;
			_next = next;
		}

		#region Operations
		public Node<T> reverse {
			get {
				Node<T> current = this;
				Node<T> path = new Node<T>(current.tile, .001f, null);
				float prevCost = current.cost;
				while(current.next != null) {
					current = current.next;
					path = new Node<T>(current.tile, prevCost, path);
					prevCost = current.cost;
				}
				return path;
			}
		}
		public Node<T> end {
			get {
				Node<T> curr = this;
				while(curr.next != null)
					curr = curr.next;
				return curr;
			}
		}

		public IEnumerable<Node<T>> path {
			get {
				Node<T> current = this;
				do {
					yield return current;
					current = current.next;
				} while(current != null);
			}
		}
		#endregion
	}
}