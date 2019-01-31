PS7 - Snake Client
Paul C Carlson
Spenser Riches
11/22/16


REQUIREMENTS OVERVIEW:
________________________________________________________________________________________________
This assignment involved building a client GUI that gets information from a common server to display
the game "Snake" to the user. The client allows the user to specify the server name to connect to,
the client's name, and directional inputs. The server interprets this information and tells the
client what the state of the game is, so it can be drawn. The client displays player's names
and scores. When the player starts the game, they will see a zoomed in view until they get a high
enough score to slowly widen their view.



ADDITIONAL FEATURES:
________________________________________________________________________________________________
Our implementation of the Snake client includes the following additional features:

Game play will scale to fit the user's window size:
	If the user decides to change their window size, the game will scale so the user does not
	loose sight of the game.

High Scores:
	When the player dies, a high scores list will display. This list only keeps track of the scores
	that were played on that specific machine. This allows the user to see their highest scores.

Start Game panel:
	When the game is first launched, an animated window will display with the Snake title and a
	snake moving around. The user enters the server and player name data below.

Enter key to start game:
	In addition to the Start game button, the user can press the Enter key while focused in the 
	player name text box to start the game.

Play Again:
	When the user dies, the player name textbox will become enabled and a "Play Again" button will
	display. This allows the user to rejoin the game on the same server.




IMPLEMENTATION DECISIONS:
________________________________________________________________________________________________
Model:
	In our model, we made separate classes for Food, Snake, World, Network, SocketState,
	LocalScores, & Point.  World primarily handles keeping track of the Food, Snakes and 
	Scores of the world as well as some properties used to define the world space.

View:
	In our view, we made separate classes for SnakeWorldPanel, Form1, StartPanel, & ScorePanel.
	We originally had the drawing methods for the world in the world class, but we moved that
	code into the SnakeWorldPanel to keep the GUI code out of the model code. We also found
	that using a Pen to draw the Snakes (as opposed to rectangles) made the code simpler and
	we were able to get the round edges by changing a Pen property. As an extra feature, we
	wanted the SnakeWorldPanel to scale when the user resized the Form window. This ensures that
	the full game area can be displayed on any resolution monitor. 
	
	Since the original start screen only contained a couple labels and text boxes, we decided to 
	add an animated title screen to display when the game is launched. We also moved the labels and 
	text boxes to the top of the window when game play started, and provided a "Play Again" button 
	if the user dies.  The window displays a message to let the player know they have died and shows
	an ordered list of high scores that are locally stored.

	When creating the ScorePanel, we decided to use colored dots to associate the snake names and
	snakes since reading the colored text could be difficult.  Placing the score within the dots
	gave them a more concrete purpose and also made a clear distinction between names and scores.

Controller:
	We created a Controller class within the View project to handle the interaction between the
	UI, server, and model code.  The controller takes care of connecting with the server and handling
	the communication between the view, world and server.  We added code that displays a MessageBox
	with a relevant message if an error occurs when attempting to connect to the server and allows
	the player to try connecting again without crashing the game.




BREAKDOWN OF TASKS:
________________________________________________________________________________________________
Below lists the different components and features that each team member implemented:

Peer Programming
________________________________________
* Networking code
* View and controller
* Model code: Snake, Food, World
* ScorePanel


Spenser
________________________________________
* StartPanel
* Unit tests


Paul
________________________________________
* High Scores
* Scaling of game when window is resized





KNOWN BUGS / ISSUES:
________________________________________________________________________________________________
* Occasionaly a high score from a previous Snake is stored in the HighScores list instead of the
  current snake.
* Performance speed considerably decreases when the window is scaling and tracking the client's
  snake or if there are a large number of snakes in the server.



TIME SPENT: HRS
________________________________________________________________________________________________

Paul:                25
Spenser:             25
Peer Programming:    25 * 2
____________________________
Total:               100




CHANGE LOG:
________________________________________________________________________________________________
Please see the github commit history at https://github.com/uofu-cs3500-fall16/01014696/commits/master/Snake
