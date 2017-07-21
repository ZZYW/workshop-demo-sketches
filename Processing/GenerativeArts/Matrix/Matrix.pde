import peasy.*;
import peasy.org.apache.commons.math.*;
import peasy.org.apache.commons.math.geometry.*;

PVector[] positions;
PeasyCam cam;

float rotatingAngle;

void setup () {
  size(1440, 900, P3D);
  pixelDensity(2);
  cam = new PeasyCam(this, 100);
  positions = new PVector[2000];  
  int range = 2000;

  for (int i=0; i<positions.length; i++) {
    positions[i] = new PVector(random(-range, range), random(-range, range), random(-range, range));
  }
  
  rotatingAngle = 0.1;
}

void draw() {
  
  rotate(rotatingAngle);
  rotatingAngle+=0.01;
  
  fill(200);
  directionalLight(220, 220, 220, 0, -0.2, -1);
  sphere(7000);
  for (int i=0; i<positions.length; i++) {
    pushMatrix();
    translate(positions[i].x, positions[i].y, positions[i].z);
    fill(255);
    noStroke();
    box(30);
    popMatrix();
  }
}

void keyPressed() {
  if (key == 's') {
    save(frameCount+".png");
  }
}