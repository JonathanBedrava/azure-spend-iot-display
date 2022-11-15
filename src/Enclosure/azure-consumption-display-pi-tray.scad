$fn = 30;
r = 3.4/2;
screwDistY = 97.5;

difference() {
    union(){
        square([72,62.5]);
        translate([28.5,-screwDistY/2+18])
        difference(){
            square([15,106.5]);
            translate([7.5,4])
                circle(r = r);
            translate([7.5,screwDistY+5])
                circle(r = r);
        }
    }
    translate([2,3])
        PiHoles();
    translate([0,31])
        circle(r=12);
}

module PiHoles()
{
    translate([3.5,3.5])
        circle(r=r);
    translate([3.5,52.5])
        circle(r=r);
    translate([61.5, 52.5])
        circle(r=r);
    translate([61.5,3.5])
        circle(r=r);
}