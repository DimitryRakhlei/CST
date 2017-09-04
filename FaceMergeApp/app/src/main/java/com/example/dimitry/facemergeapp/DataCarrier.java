package com.example.dimitry.facemergeapp;

import android.app.Activity;
import android.graphics.Bitmap;
import android.graphics.Point;
import android.graphics.PointF;
import android.media.Image;
import android.view.View;
import android.widget.ImageView;
import android.widget.RelativeLayout;

import java.util.ArrayList;

/**
 * Created by Dimitry on 29/01/2016.
 */
public class DataCarrier {
    private int dialogValue = 5;
    private ImageModifier mod;
    private ArrayList<Vector> list1;
    private ArrayList<Vector> list2;

    public RelativeLayout getPlayView() {
        return playView;
    }

    public void setPlayView(RelativeLayout playView) {
        this.playView = playView;
    }

    private RelativeLayout playView;

    public RelativeLayout getProgress() {
        return progress;
    }

    public void setProgress(RelativeLayout progress) {
        this.progress = progress;
    }

    RelativeLayout progress;

    public Activity getMainprog() {
        return mainprog;
    }

    public void setMainprog(Activity mainprog) {
        this.mainprog = mainprog;
    }

    private Activity mainprog;


    public DataCarrier() {
        forward = new ArrayList<ArrayList<Vector>>();
        backward = new ArrayList<ArrayList<Vector>>();
    }

    public ArrayList<ArrayList<Vector>> getForward() {
        return forward;
    }

    public void setForward(ArrayList<ArrayList<Vector>> forward) {
        this.forward = forward;
    }

    private ArrayList<ArrayList<Vector>> forward;

    public ArrayList<ArrayList<Vector>> getBackward() {
        return backward;
    }

    public void setBackward(ArrayList<ArrayList<Vector>> backward) {
        this.backward = backward;
    }

    private ArrayList<ArrayList<Vector>> backward;

    public ArrayList<Bitmap> getDatamaps() {
        return datamaps;
    }

    public void setDatamaps(ArrayList<Bitmap> datamaps) {
        this.datamaps = datamaps;
    }

    private ArrayList<Bitmap> datamaps;

    public ImageView getMergeView() {
        return mergeView;
    }

    public void setMergeView(ImageView mergeView) {
        this.mergeView = mergeView;
    }

    ImageView mergeView;

    public Bitmap getMap1() {
        return map1;
    }

    public void setMap1(Bitmap map1) {
        this.map1 = map1;
    }

    Bitmap map1;

    public Bitmap getMap2() {
        return map2;
    }

    public void setMap2(Bitmap map2) {
        this.map2 = map2;
    }

    Bitmap map2;
    void setMod (ImageModifier m) {
        mod = m;
    }

    public void setList1(ArrayList<Vector> v) {
        list1 = v;
    }

    public ArrayList<Vector> getList1() {
        return list1;
    }

    public void setList2(ArrayList<Vector> v) {
        list2 = v;
    }

    public ArrayList<Vector> getList2() {
        return list2;
    }

    ImageModifier getMod() {
        return mod;
    }

    void setDialogValue(int i ) {
        dialogValue = i;
    }

    int getDialogValue() {
        return dialogValue;
    }

    public void fillForward() {
        for (int i = 0; i < list1.size(); i++) {
            forward.add(createFrames(list1.get(i), list2.get(i)));
        }
    }

    public void fillBackward() {
        for (int i = 0; i < list2.size(); i++) {
            forward.add(createFrames(list2.get(i), list1.get(i)));
        }
    }

    public ArrayList<Vector> createFrames(Vector sp, Vector ep) {
        Vector first = new Vector(sp.getStartPos(), sp.getEndPos());
        Vector last = new Vector(ep.getStartPos(), ep.getEndPos());
        ArrayList<Vector> retList = new ArrayList<Vector>();
        retList.add(first);
        if ( dialogValue > 2 ) {
            float sxdiff = last.getStartPos().x - first.getStartPos().x;
            float sydiff = last.getStartPos().y - first.getStartPos().y;
            float exdiff = last.getEndPos().x - first.getEndPos().x;
            float eydiff = last.getEndPos().y - first.getEndPos().y;
            sxdiff /= (dialogValue - 1);
            sydiff /= (dialogValue - 1);
            exdiff /= (dialogValue - 1);
            eydiff /= (dialogValue - 1);

            float retxs = first.getStartPos().x + sxdiff;
            float retys = first.getStartPos().y + sydiff;
            float retxe = first.getEndPos().x + exdiff;
            float retye = first.getEndPos().y + eydiff;

            for (int i = 0; i < (dialogValue-1); i++) {
                if ((int)retxs == first.getStartPos().x && (int)retys == first.getEndPos().y){
                    break;
                }
                retList.add(new Vector(
                        new Point((int)(retxs), (int)(retys)),
                        new Point((int)(retxe), (int)(retye)))
                );
                retxs += sxdiff;
                retys += sydiff;
                retxe += exdiff;
                retye += eydiff;
            }
        }
        return retList;
    }
}
