import gab.opencv.*;
import processing.video.*;

Capture video;
OpenCV opencv;

void setup() {
  size(720, 480);
  video = new Capture(this);
  opencv = new OpenCV(this, 720, 480);

  opencv.startBackgroundSubtraction(5, 3, 0.5);
  video.start();
}

void draw() {
  if (video.available()) {
    video.read();
    image(video, 0, 0, video.width/2, video.height/2);  
    opencv.loadImage(video);

    opencv.updateBackground();

    opencv.dilate();
    opencv.erode();

    noFill();
    stroke(255, 0, 0);
    strokeWeight(3);
    float sum = 0;
    for (Contour contour : opencv.findContours()) {
      sum += contour.area();
      //println(contour.area());
    }
    noStroke();
    fill(255,0,0);
    rect(30,30,sum/3000,8);
  }
}

void movieEvent(Movie m) {
  m.read();
}