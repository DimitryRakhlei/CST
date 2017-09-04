using System;
using System.Collections.Generic;
using System.Drawing;

namespace Compression {
    public static class ExaustiveSearch {
        /// <summary>
        ///     Calculates Mean Difference for one NxN block
        /// </summary>
        /// <param name="C">source frame</param>
        /// <param name="R">dest frame</param>
        /// <param name="N">block size</param>
        /// <param name="x">position of search</param>
        /// <param name="y">y pos of search</param>
        /// <returns></returns>
        public static float MeanDifference(List<List<float>> C, List<List<float>> R, int N, int x, int y) {
            float MAD = 0;
            for (int i = 0; i < N; i++) {
                for (int j = 0; j < N; j++) {
                    if (j + y >= N || i + x >= N) continue;
                    MAD += Math.Abs(C[i][j] - R[i + x][j + y]);
                }
            }
            return MAD /= N*N;
        }

        /// <summary>
        ///     Performs the search on two lists using two managers which contain data
        ///     about two images.
        /// </summary>
        /// <param name="manager">data on prev frame</param>
        /// <param name="manager2">data on new frame</param>
        /// <returns></returns>
        public static List<Tuple<Point, Point>> Search(DataManager manager, DataManager manager2) {
            List<Tuple<Point, Point>> mvs = new List<Tuple<Point, Point>>();

            for (int oi = 0; oi < manager.LumList.Count; oi += 16) {
                for (int oj = 0; oj < manager.LumList.Count; oj += 16) {
                    List<List<float>> C = manager.LumList.GetSquare(oi, oj, 16);
                    List<List<float>> R = manager2.LumList.GetSquare(oi, oj, 16);
                    List<List<float>> checkBlock = C.GetSquare(4, 4, 8);
                    float mad = MeanDifference(checkBlock, R, 8, 4, 4);
                    Point sp = new Point(oi + 4, oj + 4);
                    Point ep = new Point(oi + 4, oj + 4);

                    for (int i = 0; i < oi + 8; i++) {
                        for (int j = 0; j < oj + 8; j++) {
                            float nmad = MeanDifference(checkBlock, R, 8, i, j);
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
                    map1.SetPixel(i, j, Color.FromArgb((int) manager.LumList[i][j], 0, 0, 0));
                    map2.SetPixel(i, j, Color.FromArgb((int) manager2.LumList[i][j], 0, 0, 0));
                }
            }
            using (Graphics g = Graphics.FromImage(map1)) {
                Pen p = Pens.AliceBlue;
                foreach (Tuple<Point, Point> mv in mvs) {
                    g.DrawLine(p, mv.Item1, mv.Item2);
                }
            }

            manager.box.Image = map1;
            manager2.rbox.Image = map2;

            return mvs;
        }
    }
}