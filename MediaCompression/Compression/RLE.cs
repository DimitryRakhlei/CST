using System.Collections.Generic;
using System.Linq;

namespace Compression {
    public static class Rle {
        private static List<sbyte> Stringify(IEnumerable<sbyte[]> list) {
            return list.SelectMany(a => a).ToList();
        }

        /// <summary>
        ///     Decodes RLE data
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>actual data</returns>
        public static List<sbyte[]> DecodeRle(sbyte[] data) {
            bool even = true;
            List<sbyte> unrle = new List<sbyte>();
            sbyte val = 0;
            foreach (sbyte dat in data) {
                if (even) {
                    val = dat;
                }
                else {
                    for (int i = 0; i < dat; i++) {
                        unrle.Add(val);
                    }
                }
                even = !even;
            }

            List<sbyte[]> ret = new List<sbyte[]>();
            int index = 0;
            for (int i = 0; i < unrle.Count/64; i++) {
                sbyte[] block = new sbyte[64];
                for (int j = 0; j < 64; j++) {
                    block[j] = unrle[index++];
                }
                ret.Add(block);
            }

            return ret;
        }

        /// <summary>
        ///     Encodes the data into a single byte array
        /// </summary>
        /// <param name="list">data to be encoded</param>
        /// <returns>encoded data</returns>
        public static sbyte[] BetterEncode(List<sbyte[]> list) {
            List<sbyte> ret = new List<sbyte>();
            sbyte count = 1;
            List<sbyte> blist = Stringify(list);
            sbyte curval = blist[0];
            for (int j = 0; j < blist.Count; j++) {
                if (j + 1 >= blist.Count) {
                    ret.Add(curval);
                    ret.Add(count);
                }
                else if (blist[j] == blist[j + 1]) {
                    count++;
                    curval = blist[j + 1];
                }
                else {
                    ret.Add(curval);
                    ret.Add(count);
                    curval = blist[j + 1];
                    count = 1;
                }
            }

            return ret.ToArray();
        }
    }
}