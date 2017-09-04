package com.example.dimitry.facemergeapp;

import android.Manifest;
import android.app.AlertDialog;
import android.app.FragmentManager;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Color;
import android.graphics.Matrix;
import android.graphics.Paint;
import android.graphics.Point;
import android.net.Uri;
import android.opengl.Visibility;
import android.os.Bundle;
import android.os.Environment;
import android.provider.MediaStore;
import android.support.design.widget.NavigationView;
import android.support.v4.content.ContextCompat;
import android.support.v4.view.GravityCompat;
import android.support.v4.widget.DrawerLayout;
import android.support.v7.app.ActionBarDrawerToggle;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.view.Menu;
import android.view.MenuItem;
import android.view.MotionEvent;
import android.view.View;
import android.view.View.OnTouchListener;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;

import com.google.android.gms.appindexing.Action;
import com.google.android.gms.appindexing.AppIndex;
import com.google.android.gms.common.api.GoogleApiClient;

import java.io.IOException;
import java.util.ArrayList;

import static java.lang.Math.abs;

public class MainActivity extends AppCompatActivity
        implements NavigationView.OnNavigationItemSelectedListener {

    private ArrayList<Vector> vectorList1;
    private ArrayList<Vector> vectorList2;

    private CameraHandler ch;
    private Vector curVector;
    private ImageView ivImage;
    private ImageView ivImage2;
    private boolean drawing1 = true;
    private int totalLines = 0;
    int [] val;

    DataCarrier courier;

    private Bitmap bm;
    private Bitmap bm2;

    static final int IMAGE_SELECT_NUM = 2;
    int currentImageSelect = 0;

    int REQUEST_CAMERA = 0, SELECT_FILE = 1;
    static final int REQUEST_TAKE_PHOTO = 1;

    int REQUEST_RCONTACT = 2;
    int REQUEST_WCONTACT = 3;

    RelativeLayout layout2;
    LinearLayout layout1;
    RelativeLayout swapLayout;
    /**
     * ATTENTION: This was auto-generated to implement the App Indexing API.
     * See https://g.co/AppIndexing/AndroidStudio for more information.
     */
    private GoogleApiClient client;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);


        final DrawerLayout drawer = (DrawerLayout) findViewById(R.id.drawer_layout);
        ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(
                this, drawer, toolbar, R.string.navigation_drawer_open, R.string.navigation_drawer_close);
        drawer.setDrawerListener(toggle);
        toggle.syncState();

        NavigationView navigationView = (NavigationView) findViewById(R.id.nav_view);
        navigationView.setNavigationItemSelectedListener(this);

        layout2 = (RelativeLayout)findViewById(R.id.relativeViewMain);
        layout1  = (LinearLayout)findViewById(R.id.layoutMainF);
        swapLayout = (RelativeLayout)findViewById(R.id.swapLayout);
        layout1.setVisibility(View.VISIBLE);

        vectorList1 = new ArrayList<Vector>();
        vectorList2 = new ArrayList<Vector>();

        courier = new DataCarrier();

        ivImage = (ImageView) findViewById(R.id.ivImage);
        ivImage2 = (ImageView) findViewById(R.id.ivImage2);

        ch = new CameraHandler(ivImage, ivImage2, this);

        ivImage.setOnTouchListener(new OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                CustomImageView drawView = (CustomImageView) v;
                CustomImageView drawView2 = (CustomImageView) ivImage2;

                Vector drawing;

                switch (event.getAction()) {
                    case MotionEvent.ACTION_DOWN: {
                        val = checkLineTouch(event, 1);
                        if (val == null) {
                            drawing = new Vector(
                                    new Point((int) event.getX(), (int) event.getY()),
                                    new Point((int) event.getX(), (int) event.getY())
                            );
                            vectorList1.add(drawing);
                            vectorList2.add(new Vector(drawing.getStartPos(),drawing.getEndPos()));
                            totalLines++;
                            drawView.list = vectorList1;
                            drawView2.list = vectorList2;
                            drawView.invalidate();
                            drawView2.invalidate();
                        } else {

                        }
                        break;
                    }
                    case MotionEvent.ACTION_UP: {
                        if (val==null) {
                            drawing = vectorList1.get(totalLines-1);
                            drawing.setEndPos(new Point((int)event.getX(), (int)event.getY()));
                            vectorList1.set(totalLines - 1, drawing);
                            vectorList2.set(totalLines - 1, new Vector(drawing.getStartPos(),drawing.getEndPos()));
                            drawView.list = vectorList1;
                            drawView2.list = vectorList2;
                            drawView.invalidate();
                            drawView2.invalidate();
                        }
                        break;
                    }
                    case MotionEvent.ACTION_MOVE: {
                        if (val==null) {
                            drawing = vectorList1.get(totalLines - 1);
                            drawing.setEndPos(new Point((int) event.getX(), (int) event.getY()));
                            vectorList1.set(totalLines - 1, drawing);
                            vectorList2.set(totalLines - 1, new Vector(drawing.getStartPos(),drawing.getEndPos()));
                            drawView.list = vectorList1;
                            drawView2.list = vectorList2;
                            drawView.invalidate();
                            drawView2.invalidate();
                        } else {
                            drawing = vectorList1.get(val[0]);
                            if (val[1] == 1) {
                                drawing.setEndPos(new Point((int) event.getX(), (int) event.getY()));
                            } else {
                                drawing.setStartPos(new Point((int) event.getX(), (int) event.getY()));
                            }
                            drawView.invalidate();
                            drawView2.invalidate();
                        }
                        break;
                    }
                }

                drawView.list = vectorList1;
                return true;
            }

        });


        ivImage2.setOnTouchListener(new OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                CustomImageView drawView = (CustomImageView) v;
                CustomImageView drawView2 = (CustomImageView) ivImage;

                Vector drawing;

                switch (event.getAction()) {
                    case MotionEvent.ACTION_DOWN: {
                        val = checkLineTouch(event, 1);
                        if (val == null) {
                            drawing = new Vector(
                                    new Point((int) event.getX(), (int) event.getY()),
                                    new Point((int) event.getX(), (int) event.getY())
                            );
                            vectorList2.add(drawing);
                            vectorList1.add(new Vector(drawing.getStartPos(),drawing.getEndPos()));
                            totalLines++;
                            drawView.list = vectorList2;
                            drawView2.list = vectorList1;
                            drawView.invalidate();
                            drawView2.invalidate();
                        } else {

                        }
                        break;
                    }
                    case MotionEvent.ACTION_UP: {
                        if (val==null) {
                            drawing = vectorList2.get(totalLines-1);
                            drawing.setEndPos(new Point((int)event.getX(), (int)event.getY()));
                            vectorList2.set(totalLines - 1, drawing);
                            vectorList1.set(totalLines - 1, new Vector(drawing.getStartPos(),drawing.getEndPos()));
                            drawView.list = vectorList2;
                            drawView2.list = vectorList1;
                            drawView.invalidate();
                            drawView2.invalidate();
                        }
                        break;
                    }
                    case MotionEvent.ACTION_MOVE: {
                        if (val==null) {
                            drawing = vectorList2.get(totalLines - 1);
                            drawing.setEndPos(new Point((int) event.getX(), (int) event.getY()));
                            vectorList2.set(totalLines - 1, drawing);
                            vectorList1.set(totalLines - 1, new Vector(drawing.getStartPos(),drawing.getEndPos()));
                            drawView.list = vectorList2;
                            drawView2.list = vectorList1;
                            drawView.invalidate();
                            drawView2.invalidate();
                        } else {
                            drawing = vectorList2.get(val[0]);
                            if (val[1] == 1) {
                                drawing.setEndPos(new Point((int) event.getX(), (int) event.getY()));
                            } else {
                                drawing.setStartPos(new Point((int) event.getX(), (int) event.getY()));
                            }
                            drawView.invalidate();
                            drawView2.invalidate();
                        }
                        break;
                    }
                }

                drawView.list = vectorList2;
                return true;
            }

        });


        // ATTENTION: This was auto-generated to implement the App Indexing API.
        // See https://g.co/AppIndexing/AndroidStudio for more information.
        client = new GoogleApiClient.Builder(this).addApi(AppIndex.API).build();
    }

    public static boolean InDistance(Point Old, Point Current, int distance)
    {
        int diffX = Math.abs(Old.x - Current.x);
        int diffY = Math.abs(Old.y - Current.y);
        return diffX <= distance && diffY <= distance;
    }


    public Point[] isSelected(CustomImageView iv) {
        int index = -1;
        for (Paint p: iv.paintlist) {
            if(p.getColor() == Color.GREEN) {
                index = iv.paintlist.indexOf(p);
            }
        }
        Point [] p = null;
        if ( index != -1) {
            p = iv.list.get(index).getLine();
        }
        return p;
    }

    public Paint newPaint() {
        Paint outlinePaint;
        outlinePaint = new Paint();
        outlinePaint.setDither(true);
        outlinePaint.setColor(Color.WHITE);  // alpha.r.g.b
        outlinePaint.setStyle(Paint.Style.STROKE);
        outlinePaint.setStrokeJoin(Paint.Join.ROUND);
        outlinePaint.setStrokeCap(Paint.Cap.ROUND);
        outlinePaint.setStrokeWidth(15);
        return outlinePaint;
    }

    public int[] checkLineTouch(MotionEvent event, int pos) {
        switch (pos) {
            case 1 :
                for (int i = 0; i < vectorList1.size(); i++) {
                    Point[] p = vectorList1.get(i).getLine();
                    if (abs(p[0].x - event.getX()) < 50 && abs(p[0].y - event.getY()) < 50) {
                        int []val = new int[3];
                        val[0] = i;
                        val[1] = 0;
                        return val;
                    }else if (abs(p[1].x - event.getX()) < 50 && abs(p[1].y - event.getY()) < 50) {
                        int []val = new int[3];
                        val[0] = i;
                        val[1] = 1;
                        return val;
                    } else if((
                            Math.abs(p[0].x + 0.5 * ( p[1].x - p[0].x ) - (event.getX())) <  50)
                            &&
                            Math.abs((p[0].y + 0.5 * ( p[1].y - p[0].y ) - (event.getY()))) < 50)
                    {
                        int []val = new int[3];
                        val[0] = i;
                        val[1] = 2;
                        return val;
                    }
                }
                break;
            case 2:
                for (int i = 0; i < vectorList2.size(); i++) {
                    Point[] p = vectorList2.get(i).getLine();
                    if (abs(p[0].x - event.getX()) < 50 && abs(p[0].y - event.getY()) < 50) {
                        int []val = new int[3];
                        val[0] = i;
                        val[1] = 0;
                        return val;
                    }else if (abs(p[1].x - event.getX()) < 50 && abs(p[1].y - event.getY()) < 50) {
                        int []val = new int[3];
                        val[0] = i;
                        val[1] = 1;
                        return val;
                    } else if((
                            Math.abs(p[0].x + 0.5 * ( p[1].x - p[0].x ) - (event.getX())) <  50)
                            &&
                            Math.abs((p[0].y + 0.5 * ( p[1].y - p[0].y ) - (event.getY()))) < 50)
                    {
                        int []val = new int[3];
                        val[0] = i;
                        val[1] = 2;
                        return val;
                    }
                }
                break;
        }

        return null;
    }


    private void selectImage() {
        final CharSequence[] items = {"Take Photo", "Choose from Library", "Cancel"};
        AlertDialog.Builder builder = new AlertDialog.Builder(MainActivity.this);
        builder.setTitle("Add Photo!");
        builder.setItems(items, new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int item) {
                if (items[item].equals("Take Photo")) {
                    ch.onPickPhoto();
                } else if (items[item].equals("Choose from Library")) {
                    ch.onPickPhoto();
                } else if (items[item].equals("Cancel")) {
                    dialog.dismiss();
                }
            }
        });
        builder.show();
    }



    @Override
    public void onRequestPermissionsResult(int requestCode,
                                           String permissions[], int[] grantResults) {
        switch (requestCode) {
            case 1: {
                // If request is cancelled, the result arrays are empty.
                if (grantResults.length > 0
                        && grantResults[0] == PackageManager.PERMISSION_GRANTED) {

                    // permission was granted, yay! Do the
                    // contacts-related task you need to do.

                } else {

                    // permission denied, boo! Disable the
                    // functionality that depends on this permission.
                }
                return;
            }

            // other 'case' lines to check for other
            // permissions this app might request\
        }
    }


    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {

        if (ContextCompat.checkSelfPermission(this, Manifest.permission.CAMERA)
                != PackageManager.PERMISSION_GRANTED) {

            // The permission is NOT already granted.
            // Check if the user has been asked about this permission already and denied
            // it. If so, we want to give more explanation about why the permission is needed.
            if (shouldShowRequestPermissionRationale(
                    Manifest.permission.CAMERA)) {
                // Show our own UI to explain to the user why we need to read the contacts
                // before actually requesting the permission and showing the default UI
            }

            // Fire off an async request to actually get the permission
            // This will show the standard permission request dialog UI
            requestPermissions(new String[]{Manifest.permission.CAMERA},
                    REQUEST_TAKE_PHOTO);
        }

        super.onActivityResult(requestCode, resultCode, data);
        if (resultCode == RESULT_OK) {
            if(requestCode == 1046) {

                Uri photoUri = data.getData();
                // Do something with the photo based on Uri
                Bitmap selectedImage = null;
                try {
                    selectedImage = MediaStore.Images.Media.getBitmap(this.getContentResolver(), photoUri);
                    selectedImage = getResizedBitmap(selectedImage, 500, 700);
                    bm = selectedImage.copy(selectedImage.getConfig(), true);
                } catch (IOException e) {
                    e.printStackTrace();
                }
                if (ch.which) {
                    ivImage.setImageBitmap(selectedImage);
                    bm = selectedImage;
                    courier.setMap1(bm);
                }else {
                    ivImage2.setImageBitmap(selectedImage);
                    bm2 = selectedImage;
                    courier.setMap2(bm2);
                }

            }else if (requestCode == REQUEST_RCONTACT) {
                System.out.println("got permission for reading");
            }else if (requestCode == SELECT_FILE) {


                Uri photoUri = data.getData();
                // Do something with the photo based on Uri
                Bitmap selectedImage = null;
                try {
                    selectedImage = MediaStore.Images.Media.getBitmap(this.getContentResolver(), photoUri);
                    bm = selectedImage.copy(selectedImage.getConfig(), true);
                } catch (IOException e) {
                    e.printStackTrace();
                }
                if (ch.which) {
                    ivImage.setImageBitmap(selectedImage);
                }else {
                    ivImage2.setImageBitmap(selectedImage);
                }

            }
        }
    }

    public Bitmap getResizedBitmap(Bitmap bm, int newWidth, int newHeight) {
        int width = bm.getWidth();
        int height = bm.getHeight();
        float scaleWidth = ((float) newWidth) / width;
        float scaleHeight = ((float) newHeight) / height;
        // CREATE A MATRIX FOR THE MANIPULATION
        Matrix matrix = new Matrix();
        // RESIZE THE BIT MAP
        matrix.postScale(scaleWidth, scaleHeight);

        // "RECREATE" THE NEW BITMAP
        Bitmap resizedBitmap = Bitmap.createBitmap(
                bm, 0, 0, width, height, matrix, false);
        return resizedBitmap;
    }

    @Override
    public void onBackPressed() {
        DrawerLayout drawer = (DrawerLayout) findViewById(R.id.drawer_layout);
        if (drawer.isDrawerOpen(GravityCompat.START)) {
            drawer.closeDrawer(GravityCompat.START);
        } else {
            super.onBackPressed();
        }
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.main, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();

        //noinspection SimplifiableIfStatement
        if (id == R.id.action_settings) {
            return true;
        }

        return super.onOptionsItemSelected(item);
    }



    @SuppressWarnings("StatementWithEmptyBody")
    @Override
    public boolean onNavigationItemSelected(MenuItem item) {
        // Handle navigation view item clicks here.
        int id = item.getItemId();

        String[] perms = {Manifest.permission_group.CAMERA};
        int permsRequestCode = 200;
        requestPermissions(perms, permsRequestCode);

        String[] wperms = {Manifest.permission_group.STORAGE};
        permsRequestCode = 300;
        requestPermissions(wperms, permsRequestCode);

        getExternalFilesDir(Environment.DIRECTORY_DOCUMENTS);

        if (id == R.id.nav_gallery) {
            for ( currentImageSelect = 0; currentImageSelect < IMAGE_SELECT_NUM; currentImageSelect++) {
                selectImage();
            }
        } else if (id == R.id.nav_merge) {
            ImageView ivMerge = (ImageView) findViewById(R.id.imageView2);
            courier.setMainprog(this);
            courier.setProgress(swapLayout);
            courier.setPlayView(layout2);
            ImageModifier mod = new ImageModifier(bm, ivImage, ivMerge);
            showDialogBox(mod);
            layout1.setVisibility(View.GONE);
            layout2.setVisibility(View.GONE);
            swapLayout.setVisibility(View.VISIBLE);



        } else if (id == R.id.nav_share) {
            vectorList1 = new ArrayList<Vector>();
            vectorList2 = new ArrayList<Vector>();
            CustomImageView iv1 = (CustomImageView) ivImage;
            CustomImageView iv2 = (CustomImageView) ivImage2;
            iv1.list = vectorList1;
            iv2.list = vectorList2;
            totalLines = 0;
            iv1.invalidate();
            iv2.invalidate();
        } else if (id == R.id.nav_send) {
            ImageView ivMerge = (ImageView) findViewById(R.id.imageView2);
            ImageModifier mod = new ImageModifier(bm, ivImage, ivMerge);
            showDialogBox(mod);
        }

        DrawerLayout drawer = (DrawerLayout) findViewById(R.id.drawer_layout);
        drawer.closeDrawer(GravityCompat.START);
        return true;
    }

    @Override
    public void onStart() {
        super.onStart();

        // ATTENTION: This was auto-generated to implement the App Indexing API.
        // See https://g.co/AppIndexing/AndroidStudio for more information.
        client.connect();
        Action viewAction = Action.newAction(
                Action.TYPE_VIEW, // TODO: choose an action type.
                "Main Page", // TODO: Define a title for the content shown.
                // TODO: If you have web page content that matches this app activity's content,
                // make sure this auto-generated web page URL is correct.
                // Otherwise, set the URL to null.
                Uri.parse("http://host/path"),
                // TODO: Make sure this auto-generated app deep link URI is correct.
                Uri.parse("android-app://com.example.dimitry.facemergeapp/http/host/path")
        );
        AppIndex.AppIndexApi.start(client, viewAction);
    }

    public void showDialogBox(ImageModifier mod) {
        FragmentManager manager = getFragmentManager();
        NumberPicker picker = new NumberPicker();
        courier.setMod(mod);
        courier.setList1(vectorList1);
        courier.setList2(vectorList2);
        picker.setCarrier(courier);
        picker.show(manager, "mydialog");
    }

    @Override
    public void onStop() {
        super.onStop();

        // ATTENTION: This was auto-generated to implement the App Indexing API.
        // See https://g.co/AppIndexing/AndroidStudio for more information.
        Action viewAction = Action.newAction(
                Action.TYPE_VIEW, // TODO: choose an action type.
                "Main Page", // TODO: Define a title for the content shown.
                // TODO: If you have web page content that matches this app activity's content,
                // make sure this auto-generated web page URL is correct.
                // Otherwise, set the URL to null.
                Uri.parse("http://host/path"),
                // TODO: Make sure this auto-generated app deep link URI is correct.
                Uri.parse("android-app://com.example.dimitry.facemergeapp/http/host/path")
        );
        AppIndex.AppIndexApi.end(client, viewAction);
        client.disconnect();
    }
}
