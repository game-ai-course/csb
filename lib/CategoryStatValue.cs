using System;
using System.Collections.Generic;
using System.Linq;

namespace CG
{
    public class CategoryStatValue
    {
        public long Count { get; private set; }

        private readonly Dictionary<string, int> freq = new Dictionary<string, int>();

        public void Add(string value, int count = 1)
        {
            if (!freq.TryGetValue(value, out int f))
                f = 0;
            freq[value] = f + count;
            Count += count;
        }

        public void AddAll(CategoryStatValue value)
        {
            Count += value.Count;
            foreach (var kv in value.freq)
                Add(kv.Key, kv.Value);
        }

        public override string ToString()
        {
            var rating = freq.OrderByDescending(f => f.Value).Select(kv => $"{kv.Key} ({kv.Value})");
            return string.Join("; ", rating.Take(3));
        }

        public string ToDetailedString(int topCount = 10)
        {
            var rating = freq.OrderByDescending(f => f.Value).Select(kv => FormattableString.Invariant($"{kv.Value*100.0/Count,5:0.0}\t{kv.Value,5}\t{kv.Key}"));
            return string.Join("\n", rating.Take(topCount)) + "\nTotal count: " + Count ;
        }
    }
}
