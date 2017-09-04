package com.example.dimitry.facemergeapp;

import android.app.Activity;
import android.content.Intent;
import android.graphics.Bitmap;
import android.net.Uri;
import android.provider.MediaStore;
import android.view.View;
import android.widget.ImageView;

import java.io.IOException;

/**
 * Created by Dimitry on 24/01/2016.
 */

public class CameraHandler {

    public final static int PICK_PHOTO_CODE = 1046;

    Activity a;
    ImageView iv1;
    ImageView iv2;
    boolean which = true;

    public CameraHandler(ImageView i1, ImageView i2, Activity activity) {
        a = activity;
        iv1 = i1;
        iv2 = i2;
    }

    // Trigger gallery selection for a photo
    public void onPickPhoto() {
        // Create intent for picking a photo from the gallery
        Intent intent = new Intent(Intent.ACTION_PICK,
                MediaStore.Images.Media.EXTERNAL_CONTENT_URI);

        // If you call startActivityForResult() using an intent that no app can handle, your app will crash.
        // So as long as the result is not null, it's safe to use the intent.
        if (intent.resolveActivity(a.getPackageManager()) != null) {
            // Bring up gallery to select a photo
            which = !which;
            a.startActivityForResult(intent, PICK_PHOTO_CODE);
        }
    }


}
