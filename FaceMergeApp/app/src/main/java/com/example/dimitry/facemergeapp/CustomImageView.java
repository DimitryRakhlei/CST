package com.example.dimitry.facemergeapp;

/**
 * Created by dimitry on 13/01/2016.
 */
import android.content.Context;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.graphics.Point;
import android.util.AttributeSet;
import android.widget.ImageView;

import java.util.ArrayList;

/**
 * Allows to draw rectangle on ImageView.
 *
 * @author Maciej Nux Jaros
 */
public class CustomImageView extends ImageView {
    public Paint currentPaint;
    public Paint outlinePaint;

    public ArrayList<Vector> list;
    public ArrayList<Paint> paintlist;

    public boolean intermediate = false;
    public boolean finalRedraw = false;
    public boolean movingPoint = false;

    public int[] movable;

    public float xf;
    public float yf;
    public float xe;
    public float ye;

    public CustomImageView(Context context, AttributeSet attrs) {
        super(context, attrs);

        currentPaint = new Paint();
        currentPaint.setDither(true);
        currentPaint.setColor(Color.WHITE);  // alpha.r.g.b
        currentPaint.setStyle(Paint.Style.STROKE);
        currentPaint.setStrokeJoin(Paint.Join.ROUND);
        currentPaint.setStrokeCap(Paint.Cap.ROUND);
        currentPaint.setStrokeWidth(10);

        outlinePaint = new Paint();
        outlinePaint.setDither(true);
        outlinePaint.setColor(Color.BLACK);  // alpha.r.g.b
        outlinePaint.setStyle(Paint.Style.STROKE);
        outlinePaint.setStrokeJoin(Paint.Join.ROUND);
        outlinePaint.setStrokeCap(Paint.Cap.ROUND);
        outlinePaint.setStrokeWidth(15);

        paintlist = new ArrayList<Paint>();
    }

    @Override
    protected void onDraw(Canvas canvas) {
        super.onDraw(canvas);

        if ( list != null ) {
            for (Vector v : list ) {
                canvas.drawLine(
                        v.getStartPos().x,
                        v.getStartPos().y,
                        v.getEndPos().x,
                        v.getEndPos().y,
                        outlinePaint
                );
                canvas.drawLine(
                        v.getStartPos().x,
                        v.getStartPos().y,
                        v.getEndPos().x,
                        v.getEndPos().y,
                        currentPaint
                );

                canvas.drawCircle(v.getStartPos().x, v.getStartPos().y, 10, outlinePaint);
                canvas.drawCircle(v.getStartPos().x, v.getStartPos().y, 10, outlinePaint);
                canvas.drawCircle(v.getEndPos().x, v.getEndPos().y, 10, outlinePaint);

                canvas.drawCircle(v.getStartPos().x, v.getStartPos().y, 10, currentPaint);
                canvas.drawCircle(v.getStartPos().x, v.getStartPos().y, 10, currentPaint);
                canvas.drawCircle(v.getEndPos().x, v.getEndPos().y, 10, currentPaint);
            }
        }

    }
}
