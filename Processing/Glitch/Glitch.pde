import gab.opencv.*;

OpenCV opencv;
float rax, ray, raz;
float rno, gno, bno;
PGraphics original;
PImage r, g, b;
boolean glitchOn;

void setup() {
  size(600, 600, P3D);
  opencv = new OpenCV(this, get());
  rno=0;
  gno=323;
  bno=123;
}

void draw() {
  background(0);
  fill(255);
  text(frameRate, 20, 20);
  if (frameCount % 200 < 15) {
    glitchOn = true;
  } else {
    glitchOn = false;
  }


  rax+=0.001;
  ray+=0.001;
  raz+=0.001;
  pushMatrix();
  translate(width/2, height/2);
  rotateX(rax);
  rotateY(ray);
  rotateZ(raz);
  noFill();
  stroke(255);
  strokeWeight(1);
  if (glitchOn) {
    strokeWeight(random(1, 11));
  }
  box(width/3);
  popMatrix();


  if (glitchOn) {
    opencv.loadImage(get());
    r = opencv.getSnapshot(opencv.getR());
    g = opencv.getSnapshot(opencv.getG());
    b = opencv.getSnapshot(opencv.getB());  
    blendMode(SCREEN);
    tint(255, 0, 0);
    int channelOffset = 50;
    image(r, noise(rno)*channelOffset-channelOffset/2, 0);
    tint(0, 255, 0);
    image(g, noise(gno)*channelOffset-channelOffset/2, 0);
    tint(0, 0, 255);
    image(b, noise(bno)*channelOffset-channelOffset/2, 0);
    rno++;
    gno++;
    bno++;
  }

  //offset
  if (glitchOn) {
    int offsetBlockY = (int)random(0, height);
    int cutHeight = (int)random(50, 300);
    image( get(0, offsetBlockY, width, cutHeight), random(5, 10), offsetBlockY);
  }
}


void mousePressed() {
  save("a");
}