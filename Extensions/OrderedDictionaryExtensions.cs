using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEngine.Extensions {
    public static class OrderedDictionaryExtensions {
        public static int IndexOf(this OrderedDictionary dict, object key) {
            int index = 0;
            foreach (var item in dict.Keys)
            {
                if (item == key)
                    return index;
                index++;
            }
            return -1;
        }
    }
}
