class Brush {
  float x, y, r;
  float xc, yc, rc;
  color c;

  float xcSpeed, ycSpeed, zcSpeed;
  float xspeed, yspeed, zspeed;

  float lifeSpan;
  float age;


  Brush() {
    x = random(-width/2, width/2);
    y = random(-height/2, height/2);


    c = color(random(0, 255), random(0, 255), random(0, 255));
    r = random(0.1, 2);
    rc = random(0, 10);
    xc = random(0, 10);
    yc = random(0, 10);

    xcSpeed = random(0.01, 0.05);
    ycSpeed = random(0.01, 0.05); 
    zcSpeed = random(0.01, 0.05);

    xspeed = random(2, 9);
    yspeed = random(2, 9);
    zspeed = random(2, 9);

    lifeSpan = random(30, 2000);
  }


  void update() {
    x += (noise(xc) - 0.5) * xspeed;
    y += (noise(yc) - 0.5) * yspeed;
    r += (random(0, 1) - 0.5);
    c = color((noise(xc)*255), 255, 0, 200);
    xc += xcSpeed;
    yc += ycSpeed;
    rc += 0.01;
    age++;
  }


  void display() {
    pushMatrix();
    translate(x, y);
    noStroke();
    fill(c);
    ellipse(0, 0, r, r);
    popMatrix();
  }
}