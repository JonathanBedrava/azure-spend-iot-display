x = 101;
x2 = 113;
y = 19.25;
y2 = 27;
y3 = 39;
z = 8-3.175;
rad = 3/2;
$fn = 25;

difference()
{
    base();
    cube([x,y,z+1],center=true);
}

module base(){
    translate([-x2/2,-y3/2,-z/2])
    difference(){
        cube([x2,y3,z]);
        translate([3,3,-.5])
            cylinder(r=rad,h=z+1);
        translate([x2-3,y3-3,-.5])
            cylinder(r=rad,h=z+1);
        translate([x2-3,3,-.5])
            cylinder(r=rad,h=z+1);
        translate([3,y3-3,-.5])
            cylinder(r=rad,h=z+1);
        
        translate([x2/4-5,-.5,-.5])
            cube([15,15,3]);
        translate([x2-(x2/4+5),-.5,-.5])
            cube([15,15,3]);
    }
}