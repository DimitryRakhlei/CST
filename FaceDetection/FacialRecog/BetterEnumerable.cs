using System.Collections.Generic;

namespace FacialRecog {
    public static class BetterEnumerable {
        public static IEnumerable<int> SteppedRange(int fromInclusive, int toExclusive, int step) {
            for (int i = fromInclusive; i < toExclusive; i += step) {
                yield return i;
            }
        }
    }
}