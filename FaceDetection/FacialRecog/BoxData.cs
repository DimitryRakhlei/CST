namespace FacialRecog {
    public class BoxData {
        public BoxData() {
        }

        public BoxData(BoxData d) {
            Highest = d.Highest;
            LeftMost = d.LeftMost;
            Rightmost = d.Rightmost;
            Lowest = d.Lowest;
        }

        //public Point TopLeft { get; set; }
        public int Highest { get; set; } = 256;
        public int LeftMost { get; set; } = 256;
        public int Rightmost { get; set; } = 256;
        public int Lowest { get; set; } = 256;
    }
}