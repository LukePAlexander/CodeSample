PS8 - Snake Server
Paul C Carlson
Spenser Riches
12/8/16


REQUIREMENTS OVERVIEW:
________________________________________________________________________________________________
This assignment involved building a server for a Snake game which sends game info to all connected
players. The server allows network connections so one or more people can play the game Snake together.




ADDITIONAL FEATURES:
________________________________________________________________________________________________
Our implementation of the Snake server includes the following additional features:

Game Mode 0
	In the settings.xml file, set Game Mode to 0 for normal game play

Game Mode 1
	In the settings.xml file, set Game Mode to 1 for "maze" mode.
	DISCLAIMER: Mode works BEST with a world that is 200x200. Other world sizes will work, 
	but the scale may cause some walls to be thinner than other walls.
	With this setting, additional "snakes" are placed as "walls" in a maze. These walls are
	stationary and twice as thick as regular snakes.

Game Mode 2
	In the settings.xml file, set Game Mode to 2 for "bumper" mode.
	With this setting, walls are placed within the game play as snakes, but whenever a snake
	runs into one of these walls, the snakes direction is reversed and the snake "bumps" off
	the wall. Snakes will still die if they run into themselves or other players.



IMPLEMENTATION DECISIONS:
________________________________________________________________________________________________
Server:
	Our server allows players to connect to it using sockets. Multiple players can play Snake at
	once against each other, or 1 player may play by himself if he chooses. Server settings are
	stored in an XML file in the Resources folder named "settings.xml". These settings allow a
	user to change: Board width, board height, board offset, frame rate, food density, snake start
	length, snake recycle rate, and game mode.
	
	Board offset specifies the minimum distance a snake will spawn from a wall. For example, if
	board offset is set to 20, a snake will spawn at least 20 units away from an outer edge wall.

	The server has callback methods that perform tasks for when a client connects, sends their name,
	and when they send a direction during gameplay.

	We used a timer (System.Timers.Timer) to control the gameplay updates, which includes moving 
	each client snake by 1 unit, checking if snakes collide with themselves, other snakes, walls,
	or food. Snakes will die if they run into themselves, other snakes, or walls. Snakes will grow
	by one unit if they eat food. At the end of the timer tick, the server sends the newly updated
	world information to each client.

World:
	Additions to the World class includes functions to place food and snakes into the world in random
	locations. When this is done, checks are in place to make sure newly spawned items are not
	placed where another item already resides. Several helper methods were created to assist
	with the calculations to check if items intersect or collide. World also handles recycling
	snakes. This happens when a snake dies. Food bits replace the area where the snake died.
	The amount of food that is dispersed depends on the settings.xml file for recycle rate.

Snake:
	We let the snake handle moving itself, which includes keeping track of its current direction,
	and adjusting its vertices each time it moves. Helper methods were added to assist with
	the calculations to determine the current direction of travel. The snake also handles
	collission detection with walls, other snakes, and food.

Food:
	Food was left unchanged.

Point:
	Several methods were added in the Point class (located in the Resources folder):
	- Method IsHorizontal takes two points and determines if they make a horizontal line.
	- Method IsVertical takes two points and determines if they make a vertical line.
	- Method IsIntersection takes 4 points (2 for each "line") and checks if they intersect
		at any point. This method only works if the given points form either horizontal
		or vertical lines.
	- Method IsBetween takes 3 points. It checksto see if the third point lies inbetween
		the first two points.


BREAKDOWN OF TASKS:
________________________________________________________________________________________________
The majority of the project was done by peer programming. Paul created game mode 2 which deals
with the 'bumper' mode.



KNOWN BUGS / ISSUES:
________________________________________________________________________________________________
* No issues logged so far


TIME SPENT: HRS
________________________________________________________________________________________________

Paul:                5
Spenser:             3
Peer Programming:    21 * 2
____________________________
Total:               50



CHANGE LOG:
________________________________________________________________________________________________
Please see the github commit history at https://github.com/uofu-cs3500-fall16/01014696/commits/master/Snake
