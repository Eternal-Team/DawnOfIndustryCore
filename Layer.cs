using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace DawnOfIndustryCore
{
	public class Layer<G>
	{
		public CustomDictionary<G> elements = new CustomDictionary<G>();

		public TagCompound Save() => new TagCompound
		{
			["Keys"] = elements.Keys.ToList(),
			["Values"] = elements.Values.ToList()
		};

		public void Load(TagCompound tag)
		{
			elements.internalDict = tag.GetList<Point16>("Keys")
				.Zip(tag.GetList<G>("Values"), (x, y) => new KeyValuePair<Point16, G>(x, y))
				.ToDictionary(x => x.Key, x => x.Value);
		}
	}

	public class CustomDictionary<T> : Dictionary<Point16, T>
	{
		public Dictionary<Point16, T> internalDict = new Dictionary<Point16, T>();

		public new void Add(Point16 key, T value) => internalDict.Add(key, value);

		public new bool ContainsKey(Point16 key) => internalDict.ContainsKey(key);

		public bool ContainsKey(int i, int j) => internalDict.ContainsKey(new Point16(i, j));

		public new ICollection<Point16> Keys
		{
			get { return internalDict.Keys; }
		}

		public new bool Remove(Point16 key) => internalDict.Remove(key);

		public new bool TryGetValue(Point16 key, out T value) => internalDict.TryGetValue(key, out value);

		public new ICollection<T> Values
		{
			get { return internalDict.Values; }
		}

		public new T this[Point16 key]
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

		public new void Clear() => internalDict.Clear();

		public bool Contains(KeyValuePair<Point16, T> item) => internalDict.ContainsKey(item.Key) && internalDict.ContainsValue(item.Value);

		public new int Count => internalDict.Count;

		public bool Remove(KeyValuePair<Point16, T> item) => internalDict.Remove(item.Key);

		public new IEnumerator<KeyValuePair<Point16, T>> GetEnumerator() => internalDict.GetEnumerator();
	}
}