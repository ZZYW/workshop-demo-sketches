import glitchP5.*;

import peasy.*;
import peasy.org.apache.commons.math.*;
import peasy.org.apache.commons.math.geometry.*;
UnitGraphics[][] ugs;

float ugWidth, ugHeight;
int horizontalN = 5;
int verticalN = 5;

GlitchP5 glitchP5;

void setup() {
  size(900, 900, P3D);
  glitchP5 = new GlitchP5(this);
  ortho();
  ugWidth = width/horizontalN;
  ugHeight = height/verticalN;
  ugs = new UnitGraphics[horizontalN][verticalN];

  for (int x=0; x < horizontalN; x++) {
    for (int y=0; y < verticalN; y++) {
      float tempx = x * ugWidth;
      float tempy = y * ugHeight;
      float tempz = tempx;

      ugs[x][y] = new UnitGraphics(new PVector(tempx, tempy, tempz), ugWidth, ugHeight);
      ugs[x][y].init();
    }
  }
}


void draw() {
  background(0);
  translate(ugWidth/2, ugHeight/2);
  strokeCap(SQUARE);
  strokeJoin(BEVEL);
  for (int x=0; x < horizontalN; x++) {
    for (int y=0; y < verticalN; y++) {
      ugs[x][y].display();
      if (random(0, 100)<0.05) {
        glitchP5.glitch((int)ugs[x][y].center.x, (int)ugs[x][y].center.y, 200, 400, (int)random(ugWidth/3, ugWidth*3), (int)random(ugHeight/3, ugHeight*2), (int)random(1, 5), random(0.5, 1.5), 10, 40);
      }
    }
  }
  glitchP5.run();
}

//void mousePressed() {
//  glitchP5.glitch(mouseX, mouseY, 200, 400, 200, (int)ugHeight, 3, 1.0f, 10, 40);
//}

void keyPressed() {
  if (key == 's') {
    save(frameCount+".png");
  }
}