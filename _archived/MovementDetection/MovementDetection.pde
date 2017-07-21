import gab.opencv.*;
import processing.video.*;

Capture video;
OpenCV opencv;
Movie movie;
float movSpeed = 1;
float previousMovSpeed;

float movingPixelSum = 0;

void setup() {
  size(834, 464);
  video = new Capture(this);
  movie = new Movie(this, "thepianist.mp4");
  opencv = new OpenCV(this, 720, 480);
  opencv.startBackgroundSubtraction(5, 3, 0.5);
  video.start();
  movie.loop();
}

void draw() {


  if (movingPixelSum > 0) {
    movingPixelSum *= 0.95;
  }


  if (video.available()) {
    video.read();

    //image(video, 0, 0, video.width/4, video.height/4);  
    opencv.loadImage(video);
    opencv.updateBackground();
    opencv.dilate();
    opencv.erode();
    for (Contour contour : opencv.findContours()) {
      movingPixelSum += contour.area();
    }
  }

  image(movie, 0, 0);
  float speedFactor = movingPixelSum/3000;

  noStroke();
  fill(255, 0, 0);
  rect(30, 30, speedFactor, 8);

  movSpeed = map(speedFactor, 0, width, 0.3, 1);
  //if (speedFactor > 70) {
  //  movSpeed = 1;
  //} else {
  //  movSpeed = 0.5;
  //}


  movie.speed(movSpeed);
}

void movieEvent(Movie m) {
  m.read();
}