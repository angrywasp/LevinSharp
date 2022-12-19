using System.Collections.Generic;
using System.Diagnostics;

namespace LevinSharp
{
    public class Section
    {
        private Dictionary<string, object> entries = new Dictionary<string, object>();

        public Dictionary<string, object> Entries => entries;

        public void Add(string key, object value)
        {
            if (entries.ContainsKey(key))
                Debugger.Break();
                
            entries.Add(key, value);
        }
    }
}