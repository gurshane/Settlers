//!OpenSCAD
//Replacement pieces for Settlers of Catan
//By John Stäck

//Licensed under Creative Commons Attribution-ShareAlike
//http://creativecommons.org/licenses/by-sa/3.0/

// ROAD
road_length = 25.2;
road_width = 5.0;

module road()
{
	cube([road_length, road_width, road_width]);
}


// HOUSE
house_width = 10.0;
house_height = 11.5;
house_roof = 7.0; //Where roof starts
house_length = 14.0;
house_pt = [	[0,0],
			[house_width,0],
			[house_width,house_roof],
			[house_width/2, house_height],
			[0, house_roof]
           ];
house_ps = [[0,1,2,3,4]];

module house()
{
	rotate([90,0,90]) linear_extrude(height=house_length, convexity=2) polygon(points=house_pt, paths=house_ps);
}

// CITY
city_width = 19.0;
city_tower_width = 9.9; //Width of the tower part
city_height = 9.7;
city_tower_height = 19.0;
city_roof = 15.0; //Where roof starts on tower part
city_length = 10.0;

city_pt = [	[0,0],
			[city_width,0],
			[city_width,city_height],
			[city_tower_width,city_height],
			[city_tower_width,city_roof],
			[city_tower_width/2,city_tower_height],
			[0,city_roof]
		 ];

city_ps = [[0,1,2,3,4,5,6]];
module city()
{
	rotate([90,0,90]) linear_extrude(height=city_length, convexity=2) polygon(points=city_pt, paths=city_ps);
}


//Make a whole kit!!
n_road1 = 8;
n_road2 = 7;
n_houses = 5;
n_cities = 4;
space=road_width;
for(i=[0:n_road1-1]) translate([0, (road_width+space)*i, 0]) road();
for(i=[0:n_road2-1]) translate([road_length+space,,(road_width+space)*i, 0]) road();
for(i=[0:n_houses-1]) translate([road_length+road_length+space,(house_width+space)*i,0]) house();
for(i=[0:n_cities-1]) translate([road_length+road_length+house_length+space*2,(city_width+space)*i, 0]) city();