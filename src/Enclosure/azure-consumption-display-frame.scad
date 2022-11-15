x = 119;
y = 42;

x2 = 101;
y2 = 19.25;


x3 = 113;
y3 = 39;

rad = 3.4/2;
$fn = 25;

difference(){
    square([x,y], center=true);
    square([x2,y2], center=true);

    translate([-x3/2+3,-y3/2+3])
            circle(r=rad);
        translate([x3/2-3,y3/2-3])
            circle(r=rad);
        translate([-x3/2+3,y3/2-3])
            circle(r=rad);
        translate([x3/2-3,-y3/2+3])
            circle(r=rad);
}