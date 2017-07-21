import processing.video.*;
import gab.opencv.*;
import java.awt.Rectangle;

OpenCV opencv;
Capture video;

Rectangle[] faces;
boolean hasFace;
int scaler = 4;
Movie walkingmov, lyingmov;
PImage type;
PFont font;

void setup() {
  size(1280, 720);
  //init Capture object
  video = new Capture(this, width/scaler, height/scaler);
  //init opencv object
  opencv = new OpenCV(this, width/scaler, height/scaler);
  //tell opencv we want face detection this time
  opencv.loadCascade(OpenCV.CASCADE_FRONTALFACE);
  //load image
  type = loadImage("type.png");
  //active video
  video.start();
  font = createFont("STSong", 300);
  walkingmov = new Movie(this, "cameragirlwalking.mp4");
  lyingmov = new Movie(this, "cameragirlmastur.mp4");
}

void draw() {
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

  if (faces != null) {
    if (faces.length > 0) {
      lyingmov.pause();
      walkingmov.loop();
      image(walkingmov, 0, 0);
    } else if (faces.length == 0) {
      walkingmov.pause();
      lyingmov.loop();
      image(lyingmov, 0, 0);
    }
  }
  image(type, 0, 0);
  pushMatrix();
  scale(0.843/2, 1/2);
  image(video, 0, 0, video.width, video.height);
  if (faces!=null) {
    for (int i=0; i < faces.length; i++) {
      stroke(0, 255, 0);
      strokeWeight(4);
      noFill();
      rect(faces[i].x, faces[i].y, faces[i].width, faces[i].height);
    }
  }
  popMatrix();
}
// Called every time a new frame is available to read
void movieEvent(Movie m) {
  m.read();
}