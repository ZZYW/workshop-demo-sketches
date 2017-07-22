float x1;
float y1;
float z1;

float x2;
float y2;
float z2;

float x3;
float y3;
float z3;

void setup(){
  background(0, 0, 0);
  size(600, 600, P3D);
  x1 = width/2;
  y1 = height/2;
  z1 = 0;
  
  x2 = width/2 - 100;
  y2 = height/2 + 100;
  z2 = 100;
  
  x3 = width/2 + 100;
  y3 = height/2 - 100;
  z3 = -100;
}

void draw(){
  noStroke();
  fill(255, 0, 0);
  rect(x1, y1, 10, 10);
  x1 += random(-10, 10);
  y1 += random(-10, 10);
  z1 += random(-10, 10);
  x1 = constrain(x1, 0, width);
  y1 = constrain(y1, 0, width);
  z1 = constrain(z1, 0, width);
  
  fill(0, 255, 0);
  rect(x2, y2, 10, 10);
  x2 += random(-10, 10);
  y2 += random(-10, 10);
  z2 += random(-10, 10);
  x2 = constrain(x2, 0, width);
  y2 = constrain(y2, 0, width);
  z2 = constrain(y2, 0, width);
  
  fill(0, 0, 255);
  rect(x3, y3, 10, 10);
  x3 += random(-10, 10);
  y3 += random(-10, 10);
  z3 += random(-10, 10);
  x3 = constrain(x3, 0, width);
  y3 = constrain(y3, 0, width);
  z3 = constrain(y3, 0, width);
  
}
