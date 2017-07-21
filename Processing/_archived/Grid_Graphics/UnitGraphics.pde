class UnitGraphics {

  float w;
  float h;
  float d;
  PVector center;
  int vn;

  ArrayList<PVector> points;
  color[] colors;
  FloatList angles;
  float rotateXAngle;
  float rotateYAngle;
  float rotateZAngle;
  float xRotateSpeed, yRotateSpeed, zRotateSpeed;
  boolean showCenter;
  float strokeWeight;

  UnitGraphics(PVector _c, float _w, float _h) {
    w = _w;
    h = _h;
    d = w;
    center = _c;
    vn = 20;
    points = new ArrayList<PVector>();
    angles = new FloatList();
    xRotateSpeed = random(0.001, 0.006);
    yRotateSpeed = random(0.001, 0.006);
    zRotateSpeed = random(0.001, 0.006);
    showCenter = true;
    strokeWeight = 2;
  }

  void init() {
    colors = new color[vn];
    for (int i=0; i<vn; i++) {
      points.add(new PVector(random(-w/2, w), random(-h/2, h/2), random(-d/2, d/2)));
      angles.append(random(0, PI));
      if (random(0, 1)>0.85) {
        colors[i] = color(random(255), random(255), random(255));
      } else {
        colors[i] = color(255);
      }
    }
  }

  void display() {
    rotateXAngle+=xRotateSpeed;
    rotateYAngle+=yRotateSpeed;
    rotateZAngle+=zRotateSpeed;
    pushMatrix();
    translate(center.x, center.y);
    scale(0.4);
    rotateX(rotateXAngle);    
    rotateY(rotateYAngle);    
    rotateZ(rotateZAngle);

    if (showCenter) {
      stroke(255, 0, 0);
      strokeWeight(8);
      point(0, 0);
    }

    strokeWeight(5);
    noFill();
    
    beginShape();
    for (int i=0; i<points.size(); i++) {
      stroke(colors[i]);
      curveVertex(points.get(i).x, points.get(i).y, points.get(i).z);
    }
    endShape();
    popMatrix();
  }
}