ArrayList<Brush> brushes;
int number = 40;
float rotationAngle=0;

void setup() {
  size(900, 600, P3D);
  pixelDensity(2);
  background(255);
  colorMode(HSB);
  brushes = new ArrayList<Brush>();

  for (int i=0; i < number; i++) {
    brushes.add(new Brush());
  }
}

void draw() {
  translate(width/2, height/2);
  fill(100, 255, 255);
  noStroke();
  //ellipse(0, 0, 200, 200);
  rotate(rotationAngle);
  rotationAngle += 0.0001;
  for (int i = 0; i<brushes.size(); i++) { 
    brushes.get(i).update();
    brushes.get(i).display();

    if (brushes.get(i).age > brushes.get(i).lifeSpan) {
      brushes.remove(i);
      brushes.add(new Brush());
    }
  }
}


void keyPressed() {
  if (key == 's') {
    save(frameCount+".png");
  }
}