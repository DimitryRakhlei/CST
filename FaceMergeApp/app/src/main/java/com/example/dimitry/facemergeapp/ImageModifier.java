package com.example.dimitry.facemergeapp;

import android.graphics.Bitmap;
import android.graphics.Color;
import android.graphics.Point;
import android.graphics.PointF;
import android.media.Image;
import android.widget.ImageView;

import java.util.ArrayList;


public class ImageModifier {
    private Bitmap map;
    private ImageView view;
    private Bitmap newMap;
    ArrayList<Vector> list1;
    ArrayList<Vector> list2;
    ImageView ivMerge;

    public ImageModifier(Bitmap b, ImageView v, ImageView ivmerge) {
        map = b;
        view = v;
        ivMerge = ivmerge;
    }
    public Bitmap calcMap( ArrayList<ArrayList<Vector>> a , DataCarrier courier, int which) {
        Bitmap temp;
        temp = map.copy(map.getConfig(), true);
        newMap = map.copy(map.getConfig(), true);
        Bitmap source = courier.getMap2().copy(courier.getMap2().getConfig(), true);
        list1 = new ArrayList<Vector>();
        list2 = new ArrayList<Vector>();
        for ( int i = 0; i < (a.size()/2); i++) {
            list1.add(a.get(i).get(i));
            list2.add(a.get(i + (a.size()/2)).get(i));
        }
        //list1 = a;
        //list2 = courier.getList2();

        for (int x = 0; x < temp.getWidth(); x++) {
            for (int y = 0; y < temp.getHeight(); y++) {
                ArrayList<Double> pointWeights = new ArrayList<Double>();
                ArrayList<PointF> points = new ArrayList<PointF>();
                for (Vector v : list1) {
                    points.add(process(x, y, v, pointWeights));
                }

                double sum = 0;
                for(Double d : pointWeights)
                    sum += d;
                PointF t;
                Double cw;
                for (int i=0; i< points.size(); i++) {
                    t = points.get(i);
                    cw = pointWeights.get(i);
                    t.x = (float)((t.x - x) * cw);
                    t.y = (float)((t.y - y ) * cw);
                    points.set(i, t);
                }
                PointF pointSum = new PointF(0, 0);
                for (PointF p : points) {
                    pointSum.x +=  p.x;
                    pointSum.y += p.y;
                }
                pointSum.x /= sum;
                pointSum.y /= sum;

                pointSum.x += x;
                pointSum.y += y;



                if (pointSum.x >= temp.getWidth() ){
                    pointSum.x = temp.getWidth() - 1;
                }
                if (pointSum.y >= temp.getHeight()) {
                    pointSum.y = temp.getHeight() - 1;
                }
                if (pointSum.x < 0) {
                    pointSum.x = 0;
                }
                if (pointSum.y < 0) {
                    pointSum.y = 0;
                }


                if (pointSum.x >= temp.getWidth() ){
                    pointSum.x = temp.getWidth() - 1;
                }
                if (pointSum.y >= temp.getHeight()) {
                    pointSum.y = temp.getHeight() - 1;
                }
                if (pointSum.x < 0) {
                    pointSum.x = 0;
                }
                if (pointSum.y < 0) {
                    pointSum.y = 0;
                }
                newMap.setPixel(x,y, source.getPixel( (int)pointSum.x, (int)pointSum.y));

            }
        }

        //map = temp;
        System.out.println("done");
        return newMap;
//        view.setImageBitmap(map);
//        ivMerge.setImageBitmap(map);
    }

    private PointF process(int x, int y, Vector v, ArrayList<Double> weights) {
        //get the line with the same index from list 2
        Vector other = list2.get( list1.indexOf(v));

        //obtain PQ vector
        PointF PQ = new PointF(v.getEndPos().x - v.getStartPos().x, v.getEndPos().y - v.getStartPos().y);

        //obtain N vector
        PointF N = new PointF(-PQ.y, PQ.x);

        //obtain XP vector
        PointF XP = new PointF(v.getStartPos().x - x, v.getStartPos().y - y);

        //obtain PX for good measure
        PointF PX = new PointF(x - v.getStartPos().x, y - v.getStartPos().y);

        //obtain d
        double d = project(N, XP);

        //obtain f
        double f = project(PQ, PX);

        //make f fractional
        f /= Math.sqrt(Math.pow(PQ.x, 2) + Math.pow(PQ.y, 2));


        /*
         For the second part now
          */


        //double x-value for X zero prime
        double xzp = other.getStartPos().x;

        //double x-value for Y zero prime
        double yzp = other.getStartPos().y;

        //Point X zero prime
        PointF XZP;

        //PQ prime
        PointF PQp = new PointF(other.getEndPos().x - other.getStartPos().x, other.getEndPos().y - other.getStartPos().y);

        //N prime
        PointF Np = new PointF(-PQp.y, PQp.x);

        //calculate xzp X-zero-prime
        xzp = xzp + (PQp.x  * f) - (d * Np.x / Math.sqrt(Math.pow(Np.x, 2) + Math.pow(Np.y, 2)));

        //calculate yzp y-zero-prime
        yzp = yzp + (PQp.y  * f) - (d * Np.y / Math.sqrt(Math.pow(Np.x, 2) + Math.pow(Np.y, 2)));

        //the point PZP
        PointF PZP = new PointF((float)xzp, (float)yzp);

        //get weight
         double weight = v.calcWeight(0, 0.01, 2, d);

        weights.add(weight);
        return PZP;
    }


    private double project(Point p1, Point p2 ) {
        return (p1.x * p2.x + p1.y * p2.y) / (Math.sqrt(Math.pow(p1.x, 2) + Math.pow(p1.y, 2)));
    }

    private double project(PointF p1, PointF p2 ) {
        return (p1.x * p2.x + p1.y * p2.y) / (Math.sqrt(Math.pow(p1.x, 2) + Math.pow(p1.y, 2)));
    }
}

