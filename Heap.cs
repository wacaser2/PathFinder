using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heap<T> : ICollection<T>, IPushPop<T> {

	#region Data

	private List<T> _arr;

	#endregion

	#region Lambdas

	private Func<int, int, bool> _compare;
	private Action<int> _set = (s) => { };
	private Action<T> _prep = (s) => { };

	#endregion

	#region ClassCode

	public Heap(Func<T, T, bool> comparer, IEnumerable<T> initial = null, Action<T, Action<int>> prep = null, Action<T, int> setIndex = null) {
		_compare = (s, t) => comparer(_arr[s], _arr[t]);
		if(prep != null)
			_prep = (s) => prep(s, Reduce);
		if(setIndex != null)
			_set = (s) => { setIndex(_arr[s], s); };
		_arr = new List<T> { default };
		if(initial != null) {
			var it = initial.GetEnumerator();
			while(it.MoveNext()) {
				_arr.Add(it.Current);
				_prep(it.Current);
			}
			Heapify();
		}
	}

	#region Subclasses

	public class HeapEnumerator : IEnumerator, IEnumerator<T> {
		Heap<T> l;

		public HeapEnumerator(Heap<T> h) {
			l = new Heap<T>(null);
			l._compare = h._compare;
			l._arr = new List<T>(h._arr);
		}

		public object Current => l.Peek();

		T IEnumerator<T>.Current => l.Peek();

		public void Dispose() {
		}

		public bool MoveNext() {
			l.Pop();
			return l.hasNext;
		}

		public void Reset() {
		}
	}

	#endregion

	#endregion

	#region HeapProperties

	public bool hasNext => _arr.Count > 1;

	#endregion

	#region PrivateMethods

	private void Heapify() {
		for(int i = (_arr.Count >> 1); i > 0; i--) {
			SiftDown(i);
		}
	}

	private void SiftUp(int i) {
		if(i <= 1)
			return;
		int p = i >> 1;
		if(_compare(p, i))
			return;
		Swap(p, i);
		SiftUp(p);
	}

	private void SiftDown(int i) {
		if(i < 1 || i >= _arr.Count)
			return;
		int l = (i << 1), r = (i << 1) + 1;
		if(_arr.Count < r)
			return;
		if(_arr.Count == r || _compare(l, r))
			r = l;
		if(_compare(i, r))
			return;
		Swap(i, r);
		SiftDown(r);
	}

	private void Swap(int p, int i) {
		T tmp = _arr[p];
		_arr[p] = _arr[i];
		_arr[i] = tmp;
		_set(p);
		_set(i);
	}

	private void Reduce(int i) {
		if(i < _arr.Count)
			SiftUp(i);
	}

	#endregion

	#region PublicMethods

	public void Push(T t) {
		_arr.Add(t);
		_prep(t);
		_set(_arr.Count - 1);
		SiftUp(_arr.Count - 1);
	}

	public T Pop() {
		if(_arr.Count == 1)
			return default;
		_arr[0] = _arr[1];
		_set(0);
		_arr[1] = _arr[_arr.Count - 1];
		_arr.RemoveAt(_arr.Count - 1);
		SiftDown(1);
		return _arr[0];
	}

	public T Peek() {
		if(_arr.Count == 1)
			return default;
		return _arr[1];
	}

	public T Replace(T t) {
		_arr[0] = t;
		Swap(0, 1);
		SiftDown(1);
		return _arr[0];
	}

	#endregion

	#region ICollectionProperties

	public int Count => _arr.Count - 1;

	public bool IsReadOnly => false;

	#endregion

	#region ICollectionMethods

	public void Add(T item) => Push(item);

	public void Clear() {
		_arr.Clear();
		_arr.Add(default);
	}

	public bool Contains(T item) => _arr.FindIndex(1, (s) => s.Equals(item)) != -1;

	public void CopyTo(T[] array, int arrayIndex) {
		for(int i = 0; i < Count; i++) {
			array[arrayIndex + i] = _arr[i + 1];
		}
	}

	public bool Remove(T item) {
		int idx = _arr.FindIndex(1, (s) => s.Equals(item));
		if(idx == -1)
			return false;
		Swap(idx, _arr.Count - 1);
		_arr.RemoveAt(_arr.Count - 1);
		SiftUp(idx);
		SiftDown(idx);
		return true;
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return new HeapEnumerator(this);
	}

	public IEnumerator<T> GetEnumerator() {
		return new HeapEnumerator(this);
	}

	#endregion
}