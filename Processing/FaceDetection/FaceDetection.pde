import processing.video.*;
import gab.opencv.*;
import java.awt.Rectangle;

OpenCV opencv;
Capture video;
Rectangle[] faces;
boolean hasFace;
int scaler = 4;
Movie mov1, mov2;
PFont font;

void setup() {
  size(1280, 720);
  pixelDensity(2);
  //init Capture object
  video = new Capture(this, width/scaler, height/scaler);
  //init opencv object
  opencv = new OpenCV(this, width/scaler, height/scaler);
  //tell opencv we want face detection this time
  opencv.loadCascade(OpenCV.CASCADE_FRONTALFACE);
  //active video
  video.start();
  font = createFont("STSong", 300);
}

void draw() {
  background(0);
  if (video.available()) {
    video.read();
    opencv.loadImage(video);

    faces = opencv.detect();

    if (faces.length!=0) {
      hasFace = true;
    } else {
      hasFace = false;
    }
  }

  pushMatrix();
  scale(0.843, 1);
  image(video, 0, 0, video.width, video.height);
  for (int i=0; i<faces.length; i++) {
    //rectMode(CENTER);
    stroke(0, 255, 0);
    strokeWeight(4);
    noFill();
    rect(faces[i].x, faces[i].y, faces[i].width, faces[i].height);
  }
  popMatrix();
  textAlign(CENTER, CENTER);
  textFont(font);
  if (faces != null) {
    if (faces.length == 1) {
      text("有面儿", width/2, height/2);
    } else if (faces.length == 0) {
      text("没面儿", width/2, height/2);
    } else if (faces.length > 1) {
      text("有好多面儿", width/2, height/2);
    }
  }
}