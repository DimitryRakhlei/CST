package com.example.dimitry.facemergeapp;

import android.app.Activity;
import android.app.DialogFragment;
import android.graphics.Bitmap;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.RelativeLayout;
import android.widget.Toast;

import java.util.ArrayList;

/**
 * Created by Dimitry on 28/01/2016.
 */
public class NumberPicker extends DialogFragment implements View.OnClickListener {
    Button yes;
    DataCarrier courier;
    android.widget.NumberPicker picker;
    RelativeLayout scroll;

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.activity_dialog, null);
        yes = (Button)view.findViewById(R.id.okbutton);
        picker = (android.widget.NumberPicker)view.findViewById(R.id.numberPicker);
        scroll = courier.getProgress();
        picker.setMaxValue(50);
        picker.setMinValue(4);
        picker.setWrapSelectorWheel(true);
        picker.setOnValueChangedListener(new android.widget.NumberPicker.OnValueChangeListener() {
            @Override
            public void onValueChange(android.widget.NumberPicker picker, int oldVal, int newVal) {
                Integer temp = newVal;
                String s = temp.toString();
                //picker.setDisplayedValues();
                picker.setValue(newVal);
            }
        });
        yes.setOnClickListener(this);
        setCancelable(false);
        return view;
    }

    public void setCarrier(DataCarrier c) {
        courier = c;
    }

    @Override
    public void onStop() {
        ArrayList<Bitmap> maplist = new ArrayList<Bitmap>();
        Activity main = courier.getMainprog();
        ArrayList<ArrayList<Vector>> forward = courier.getForward();
        ArrayList<ArrayList<Vector>> backward = courier.getBackward();

        for (int counter = 0; counter < forward.size(); counter++) {
            maplist.add(courier.getMod().calcMap(forward, courier, counter));
            counter++;
        }

        scroll.setVisibility(View.GONE);

        RelativeLayout playLayout = courier.getPlayView();

        ImageView mergeview = (ImageView)main.findViewById(R.id.imageView2);
        mergeview.setImageBitmap(maplist.get(2));

        playLayout.setVisibility(View.VISIBLE);
        courier.setDatamaps(maplist);
        super.onStop();
    }

    @Override
    public void onClick(View v) {
        if ( v.getId() == R.id.okbutton) {
            courier.setDialogValue(picker.getValue());
            courier.fillForward();
            courier.fillBackward();
            dismiss();
        }
    }
}
