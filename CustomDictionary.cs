using System;
using System.Collections.Generic;
using Terraria.DataStructures;

namespace DawnOfIndustryCore
{
	public class CustomDictionary<T> : Dictionary<Point16, T>
	{
		public Dictionary<Point16, T> internalDict = new Dictionary<Point16, T>();

		public String Name { get; set; }

		public void Add(Point16 key, T value) => internalDict.Add(key, value);

		public bool ContainsKey(Point16 key) => internalDict.ContainsKey(key);

		public bool ContainsKey(int i, int j) => internalDict.ContainsKey(new Point16(i, j));

		public ICollection<Point16> Keys
		{
			get { return internalDict.Keys; }
		}

		public bool Remove(Point16 key) => internalDict.Remove(key);


		public bool TryGetValue(Point16 key, out T value) => internalDict.TryGetValue(key, out value);

		public ICollection<T> Values
		{
			get { return internalDict.Values; }
		}

		public T this[Point16 key]
		{
			get { return internalDict[key]; }
			set { internalDict[key] = value; }
		}

		public T this[int i, int j]
		{
			get { return internalDict[new Point16(i, j)]; }
			set { internalDict[new Point16(i, j)] = value; }
		}

		public void Add(KeyValuePair<Point16, T> item) => internalDict.Add(item.Key, item.Value);

		public void Clear() => internalDict.Clear();

		public bool Contains(KeyValuePair<Point16, T> item) => internalDict.ContainsKey(item.Key) && internalDict.ContainsValue(item.Value);

		public int Count => internalDict.Count;

		private bool IsReadOnly => false;

		public bool Remove(KeyValuePair<Point16, T> item) => internalDict.Remove(item.Key);

		public IEnumerator<KeyValuePair<Point16, T>> GetEnumerator() => internalDict.GetEnumerator();
	}
}