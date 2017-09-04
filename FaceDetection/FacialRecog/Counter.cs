using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacialRecog {
    class Counter {
        private int Count { get; set; } = 0;
        private int Restart { get; set; } = 9;

        public Counter(int c, int r) {
            Count = c;
            Restart = r;
        }

        public Counter() {
            
        }

        public bool Inc() {
            if (Count != Restart) {
                Count++;
                return false;
            }
            Count = 0;
            return true;
        }
    }
}
