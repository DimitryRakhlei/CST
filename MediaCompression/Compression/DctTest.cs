using System.Windows.Forms;

namespace Compression {
    public class DctTest {
        public static void TestDct(PictureBox first, PictureBox second) {
            /*for (int oi = 0; oi < manager.LumList.Count; oi += 16) {
                for (int oj = 0; oj < manager.LumList.Count; oj += 16) {
                    List<List<float>> C = manager.LumList.GetSquare(oi, oj, 16);
                    List<List<float>> R = manager2.LumList.GetSquare(oi, oj, 16);
                    List<List<float>> checkBlock = C.GetSquare(4, 4, 8);
                    float mad = ExaustiveSearch.MeanDifference(checkBlock, R, 8, 4, 4);
                    Point sp = new Point(oi + 4, oj + 4);
                    Point ep = new Point(oi + 4, oj + 4);

                    for (int i = 0; i < oi + 8; i++) {
                        for (int j = 0; j < oj + 8; j++) {
                            float nmad = ExaustiveSearch.MeanDifference(checkBlock, R, 8, i, j);
                            if (nmad < mad) {
                                mad = nmad;
                                ep = new Point(i + oi, j + oj);
                            }
                        }
                    }
                    mvs.Add(new Tuple<Point, Point>(sp, ep));
                }
            }

            Bitmap map1 = new Bitmap(manager.LumList.Count, manager.LumList.Count);
            Bitmap map2 = new Bitmap(manager2.LumList.Count, manager2.LumList.Count);

            for (int i = 0; i < map1.Height; i++) {
                for (int j = 0; j < map1.Width; j++) {
                    map1.SetPixel(i, j, Color.FromArgb((int)manager.LumList[i][j], 0,0,0));
                    map2.SetPixel(i, j, Color.FromArgb((int)manager2.LumList[i][j], 0, 0, 0));
                }
            }
            using (Graphics g = Graphics.FromImage(map1)) {
                Pen p = Pens.AliceBlue;
                foreach (Tuple<Point, Point> mv in mvs) {
                    g.DrawLine(p, mv.Item1, mv.Item2);
                }
            }

            first.Image = map1;
            second.Image = map2; */
        }
    }
}