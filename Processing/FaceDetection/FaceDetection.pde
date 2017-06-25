import processing.video.*;
import gab.opencv.*;
import java.awt.Rectangle;

OpenCV opencv;
Capture video;
Rectangle[] faces;
boolean hasFace;
int scaler = 4;
Movie mov1, mov2;

void setup() {
  size(1280, 720);
  //init Capture object
  video = new Capture(this, width/scaler, height/scaler);
  //init opencv object
  opencv = new OpenCV(this, width/scaler, height/scaler);
  //tell opencv we want face detection this time
  opencv.loadCascade(OpenCV.CASCADE_FRONTALFACE);
  //active video
  video.start();
}

void draw() {
  //scale(scaler);
  if (video.available()) {
    video.read();
    opencv.loadImage(video);
    image(video, 0, 0, video.width, video.height);
    faces = opencv.detect();

    if (faces.length!=0) {
      hasFace = true;
      //noStroke();
      //fill(255, 255, 0);
      //for (int i = 0; i < faces.length; i++) {
      //  ellipseMode(CORNER);
      //  ellipse(faces[i].x, faces[i].y, faces[i].width, faces[i].height);
      //}
    } else {
      hasFace = false;
    }
  }
}