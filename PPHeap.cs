using System;
using System.Collections.Generic;
using General;

namespace PathFinder {
	public class PPHeap<T> : Heap<T>, IPushPop<T> {
		public PPHeap(Func<T, T, bool> comparer, IEnumerable<T> initial = null, Action<T, Action<int>> prep = null, Action<T, int> setIndex = null) : base(comparer, initial, prep, setIndex) { }
	}
}