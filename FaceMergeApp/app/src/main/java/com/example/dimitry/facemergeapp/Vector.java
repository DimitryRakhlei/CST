package com.example.dimitry.facemergeapp;

import android.graphics.Point;

import java.lang.ref.Reference;

/**
 * Created by dimitry on 11/01/2016.
 */
public class Vector {
    class Line {
        private Point startPos = null;
        private Point endPos = null;
        private Point normal = null;

        public Point getNormal() {
            return normal;
        }

        public void setNormal(Point normal) {
            this.normal = normal;
        }

        public Point getStartPos() {
            return startPos;
        }

        public void setStartPos(Point startPos) {
            this.startPos = startPos;
        }

        public Point getEndPos() {
            return endPos;
        }

        public void setEndPos(Point endPos) {
            this.endPos = endPos;
        }
    }

    private Line line = null;
    private Point vectDirect = null;

    public Vector (Point spos, Point epos) {
        line = new Line();

        line.setStartPos(spos);
        line.setEndPos(epos);

        vectDirect = new Point((epos.x - spos.x), (epos.y - spos.y));
        calculateNormal();
    }

    public Vector() {
        line = new Line();
    }

    public Vector(Vector v ) {
        line = v.line;
        vectDirect = new Point((getEndPos().x - getStartPos().x), (getEndPos().y - getStartPos().y));
        calculateNormal();
    }


    public void setStartPos(Point p) {
        line.setStartPos(p);
        vectDirect = new Point((getEndPos().x - getStartPos().x), (getEndPos().y - getStartPos().y));
        calculateNormal();
    }

    public void setEndPos(Point p) {
        line.setEndPos(p);
        vectDirect = new Point((getEndPos().x - getStartPos().x), (getEndPos().y - getStartPos().y));
        calculateNormal();
    }

    public Point getEndPos(){
        return line.getEndPos();
    }
    public Point getStartPos() {
        return line.getStartPos();
    }

    public Point[] getLine() {
        Point[] p = new Point[2];
        p[0] = line.getStartPos();
        p[1] = line.getEndPos();
        return p;
    }

    public Point getNormal() {
        return line.getNormal();
    }

    public double getLength() {
        return Math.sqrt(
                Math.abs((line.endPos.x - line.startPos.x))
                + Math.abs((line.endPos.y - line.startPos.y))
        );
    }

    public double project( Point p1, Point p2 ,Double len ) {
        double proj = ((double)(p1.x * p2.x) + (double)(p1.y * p2.y)) / len;
        return proj;
    }

    public Double calcWeight(int p, double a, int b, double dist) {
        Double len = getLength();
        Double ret = Math.pow((1) / (0.01 + dist), 2);
        return ret;
    }

    public boolean isSet() {
        return !(line == null);
    }
    /**
     *
     */
    public Point getVectDirect() {
        return vectDirect;
    }
    private void calculateNormal() {
        line.setNormal(new Point(-(vectDirect.y), (vectDirect.x)));
    }
}
