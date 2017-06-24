class UnitGraphics {

  float w;
  float h;
  float d;
  PVector center;
  int vn;

  ArrayList<PVector> points;
  FloatList angles;
  float rotateAngle;
  float rotateSpeed;
  boolean showCenter;
  PGraphics graphics;



  UnitGraphics(PApplet mainClass, PVector _c, float _w, float _h) {
    w = _w;
    h = _h;
    d = 100;
    center = _c;
    vn = 20;
    points = new ArrayList<PVector>();
    angles = new FloatList();
    rotateSpeed = random(0.001, 0.006);
    showCenter = true;
    graphics = createGraphics((int)w, (int)h, P3D);
  }

  void init() {
    for (int i=0; i<vn; i++) {
      points.add(new PVector(random(0, w), random(0, h), random(0, d) ));
      angles.append(random(0, PI));
    }
  }

  void display() {
    graphics.beginDraw();

    graphics.ortho();
    rotateAngle+=rotateSpeed;

    if (showCenter) {
      graphics.stroke(243, 3, 130);
      graphics.strokeWeight(7);
      graphics.point(w/2, h/2);
    }

    graphics.stroke(255);    
    graphics.strokeWeight(2);
    graphics.noFill();

    graphics.beginShape();
    for (PVector p : points) {
      graphics.vertex(p.x, p.y, p.z);
    }

    graphics.endShape();
    graphics.endDraw();
    pushMatrix();    
    translate(center.x, center.y);
    scale(0.6);
    rotate(rotateAngle);
    imageMode(CENTER);
    image(graphics, 0, 0);
    popMatrix();
  }
}