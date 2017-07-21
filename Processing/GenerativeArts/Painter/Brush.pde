class Brush {
  //position
  float x, y, r;
  //noise index
  float xc, yc, rc;
  //color
  color c;
  //noise index increment step
  float xcstep, ycstep, zcstep;
  //moving step
  float xstep, ystep, zstep;
  
  
  float lifeSpan;
  
  float age;


  Brush() {
    x = random(-width/2, width/2);
    y = random(-height/2, height/2);

    c = color(random(0, 255), random(0, 255), random(0, 255));
    r = random(0.1, 1.5);
    rc = random(0, 10);
    xc = random(0, 10);
    yc = random(0, 10);

    xcstep = random(0.01, 0.05);
    ycstep = random(0.01, 0.05); 
    zcstep = random(0.01, 0.05);

    xstep = random(2, 9);
    ystep = random(2, 9);
    zstep = random(2, 9);

    lifeSpan = random(30, 2000);
  }


  void update() {
    x += (noise(xc) - 0.5) * xstep;
    y += (noise(yc) - 0.5) * ystep;
    r += (random(0, 1) - 0.5);
    c = color((noise(xc)*255), 255, 255, 200);
    xc += xcstep;
    yc += ycstep;
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