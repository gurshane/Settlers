using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Catalog {

	//----------------------------
	//Board Piece Class:
	GamePiece getOccupyingPiece ();
	Enums.TerrainType getTerrainType ();
	void setOccupyingPiece (GamePiece gamePiece);

	/* Vertex and Edge Class:
	 * Visited statuses:
	 * 0 = has not been visited
	 * 1 = has been visited
	 * 2 = reset mode */
	int getVisited ();          //get the visited status of an edge or vertex
	void setVisited ();         //set the status of an edge or vertex to 1
	void resetVisited ();       //set the status of an edge or vertex to 0
	void clearVisited ();       //set the status of an edge or vertex to 2

	//Edge Class:
	Vertex getLeftVertex();
	Vertex getRightVertex ();
	Hex getLeftHex ();
	Hex getRightHex ();
	void setLeftVertex (Vertex v);
	void setRightVertex(Vertex v);
	void setLeftHex(Hex v);
	void setRightHex(Hex v);

	//Hex Class:
	List<Vertex> getVertices();
	bool adjacentToVertex (Vertex v);
	Enums.HexType getHexType();
	int getHexNumber();
	void addVertex(Vertex v);
	void setHexNumber (int hexNumber);

	//Vertex Class:
	List<Edge> getNeighbouringEdges ();
	int getChits();
	Enums.PortType getPortType();
	bool getHasWall();              //returns true if there is a city with a city wall on the vertex
	void addWall();                 //sets the wall flag for cities on the vertex
	void removeWall();              //removes the wall flag for cities on the vertex
	bool isAdjacentToRobber();      //returns true if the vertex is adjacent to the robber
	void addRobber();               //sets adjacentToRobber to true
	void removeRobber();            //sets adjacentToRobber to false
	void addEdge (Edge e);
	//----------------------------


	//----------------------------
	// Game Piece and Player Class:
	Enums.Color getColor();

	//Game Piece Class:
	string getOwnerName();
	Enums.PieceType getPieceType();
	bool isOnBoard();            //returns true if piece is on board
	void putOnBoard();           //sets onBoard to true
	void takeOffBoard();         //sets onBoard to false

	//City Class:
	bool isMetropolis();                        //returns true if the city is a metropolis
	void makeMetropolis();                      //turns the city into a metropolis
	void removeMetropolis();                    //removes a metropolis from a city
	City getFreeCity (List<GamePiece> pieces);  //gets a city that isn't on the board from a list of game pieces

	//Knight Class:
	void updateLevel(int level);                   //sets level to the given level
	void upgrade();                                //ugrades the knight
	void activateKnight ();
	void deactivateKnight();
	int getLevel();
	bool isActive();
	bool wasUpgraded();                             //returns true if the knight was upgraded this turn
	bool wasActivatedThisTurn();                    //returns true if the knight was activated this turn
	void notActivatedThisTurn ();                   //sets activatedThisTurn to false
	void notUpgradedThisTurn();						//sets hasBeenUpgraded to false
	Knight getFreeKnight (List<GamePiece> pieces);  //gets a knight that isn't on the board from a list of game pieces

	//Road Class:
	bool getIsShip();                           //returns true if the piece is a ship
	bool getBuiltThisTurn ();                   //returns true if the piece was built this turn           
	void wasBuiltThisTurn ();                   //sets builtThisTurn to true
	void notBuiltThisTurn ();                   //sets builtThisTurn to false
	Road getFreeRoad (List<GamePiece> pieces);  //gets a road that isn't on the board from a list of game pieces
	Road getFreeShip (List<GamePiece> pieces);  //gets a ship that isn't on the board from a list of game pieces

	//Settlement Class:
	Settlement getFreeSettlement (List<GamePiece> pieces); //gets a settlement that isn't on the board from a list of game pieces
	//----------------------------


	//----------------------------
	//Graph Class (Static):

	//resets all vertices to visited status 0
	void vertexReset(Vertex v);

	/* Check if two vertices are legally connected via edges (Helpful for displacing knights)
	 * Note: Make sure to reset vertices before using this method
	 * Note: returns true if there is an enemy knight on v2
	 * Note: returns false if there are any other pieces on v2
	 * Note: assumes travel from road to ship is legal */
	bool areConnectedVertices(Vertex v1, Vertex v2, Enums.Color color);

	// Checks if a vertex is free to place a town-piece
	bool freeVertex (Vertex v);

	// The nextTo... methods check if an appropriate piece of the given color is next the given board piece
	bool nextToMyEdge (Vertex v, Enums.Color color);
	bool nextToMyRoad (Vertex v, Enums.Color color);
	bool nextToMyShip(Vertex v, Enums.Color color);
	bool nextToMyCityOrSettlement (Edge e, Enums.Color color);
	bool nextToMyPiece (Edge e, Enums.Color color);

	// Checks if an edge has a closed ship piece on it for a given color
	bool isClosedShip(Edge edge, Enums.Color color);

	// Resets all edges to visited status 0
	void edgeReset(Edge e);

	// Collect and return all edges with roads/ships of a given color connected to a given edge
	List<Edge> collectRouteEdges(Edge edge, Enums.Color color);

	// Get the longest route in a given collection of edges
	int longestRoute (List<Edge> edges, Enums.Color color);
	//----------------------------


	//----------------------------
	//Move Authorizer Class (Static):

	// Can a knight move from source to target
	bool canKnightMove(Vertex source, Vertex target, Enums.Color color);

	// Can a knight at source displace a knight at target
	bool canKnightDisplace (Vertex source, Vertex target, Enums.Color color);

	// Given the resources, development chart, and vertex, can a knight be upgraded
	bool canUpgradeKnight (int[] resources, int[] devChart, Vertex v);

	// Given the resources, and vertex, can a knight be upgraded
	bool canActivateKnight (int[] resources, Vertex v);

	/* Given the commodities and development chart, can a development chart be upgraded
	 * in the given development area
	 * Note: a list of pieces must also be given to check if a city is on the board */
	bool canUpgradeDevChart (Enums.DevChartType dev, int[] commodities, 
		List<GamePiece> pieces, int[] devChart);

	/* The canBuild... methods check if a piece can be built at the given location, with the
	 * given resources and the piece list that player has */
	bool canBuildSettlement (Vertex location, int[] resources,
	                        List<GamePiece> pieces, Enums.Color color);
	bool canBuildCity (Vertex location, int[] resources,
	                  List<GamePiece> pieces, Enums.Color color);
	bool canBuildCityWall (Vertex location, int[] resources,
	                      int cityWalls, Enums.Color color);
	bool canBuildKnight (Vertex location, int[] resources,
	                    List<GamePiece> pieces, Enums.Color color);
	bool canBuildRoad (Edge location, int[] resources,
	                  List<GamePiece> pieces, Enums.Color color);
	bool canBuildShip (Edge location, int[] resources,
	                  List<GamePiece> pieces, Enums.Color color);

	// Check if a ship can be moved from source to target
	bool canShipMove (Edge source, Edge target, Enums.Color color);

	// Check if a knight at source can chase the robber away
	bool canChaseRobber(Vertex source);
	bool canMoveRobber (Hex target);
	bool canMovePirate (Hex target);
	bool canPlaceMerchant(Hex target);

	// The canPlaceInitial... methods check if a piece can be placed on the given locations
	bool canPlaceInitialTownPiece(Vertex v);
	bool canPlaceInitialRoad(Edge e, Enums.Color color);
	bool canPlaceInitialShip (Edge e, Enums.Color color);
	//----------------------------


	//----------------------------
	//Move Manager Class (Static):

	// The placeIntial... methods place a piece from the given piece list at the given location
	bool placeInitialSettlement (Vertex v, List<GamePiece> pieces);
	bool placeInitialCity (Vertex v, List<GamePiece> pieces);
	bool placeInitialRoad (Edge e, Enums.Color color, List<GamePiece> pieces);
	bool placeInitialShip (Edge e, Enums.Color color, List<GamePiece> pieces);

	/* The build... methods will check to see if the request is valid,
	 * and then build the appropriate game piece adjusting all necessary
	 * game state */
	bool buidSettlement (Vertex location, int[] resources,
	                    List<GamePiece> pieces, Enums.Color color);
	bool buidCity (Vertex location, int[] resources,
	                    List<GamePiece> pieces, Enums.Color color);
	bool buildRoad (Edge location, int[] resources,
	               List<GamePiece> pieces, Enums.Color color);
	bool buildShip (Edge location, int[] resources,
	               List<GamePiece> pieces, Enums.Color color);

	bool moveKnight (Vertex source, Vertex target, Enums.Color color);
	bool displaceKnight (Vertex source, Vertex target, Enums.Color color);
	bool upgradeKnight (int[] resources, int[] devChart, Vertex v);
	bool activateKnight (int[] resources, Vertex v);
	bool upgradeDevChart (Enums.DevChartType dev, int[] commodities, 
	                     List<GamePiece> pieces, int[] devChart);
	//----------------------------


	//----------------------------
	// Game Manager Class (Static):

	/* The following four classes return the constant number in each of the categories
	 * Resources = 5 different resources
	 * Commodities = 3 different commodities
	 * DevChartType = 3 different Development Chart Types 
	 * ProgressCardLimit = maximum of 4 progress cards at the end of the turn per player */
	int getNumberResources ();
	int getNumberCommodities ();
	int getNumberDevChartType();
	int getProgressCardLimit ();

	/* These methods will conduct the way turns should go.
	 * If the passed value for first is true, it means it is the very first 
	 * time that the method is being called this game.
	 * Initial first turn will recursively call itself until all the initial
	 * first turns have been made. Then it will call initial second turn which 
	 * will recursively call itself until it calls begin turn for regular turns. 
	 * Begin turn sets up the turn and the the player will roll the dice. When
	 * a player hits end turn, the end turn function will be called. */
	void initialFirstTurn ();
	void initialSecondTurn(bool first);
	void beginTurn (bool first);
	void endTurn();

	List<string> getPlayerNames();    // Get all player names
	Player getCurrentPlayer ();	      // Get the  current player
	Player getPlayer (string name);   // Get a player from their name (returns null if player not in game)

	Enums.GamePhase getGamePhase();
	int getFirstDie();
	int getSecondDie();
	Enums.EventDie getEventDie();
	string getMerchantController();
	string getLongestRouteContoller ();
	Hex getPirateLocation ();
	Hex getRobberLocation();
	int getPointsToWin ();

	bool hasBarbarianAttacked();       // Returns true if barbarian has already attacked this game
	void barbarianAttackedThisGame();  // Sets barbarianHasAttacked to true

	/* Give a metropolis to the given player at the given vertex.
	 * Remove a metropolis from the player who currently controls it if anyone does.
	 * Returns true upon successful completion, false otherwise */
	bool giveMetropolis (string player, Enums.DevChartType met, Vertex city);

	// Not yet implemented
	void determineLongestRoute();

	void updateRobberLocation (Hex newLocation);
	void updatePirateLocation (Hex newLocation);
	void setMerchantController (Merchant m, string player);

	void rollDice();					// Rolls the dice and commences all appropriate actions
	void rollDice(int d1, int d2);      // Same as rollDice(), but sets dice for alchemist card

	// Given a hex type, the following two methods will return the appropriate resource or commodity
	Enums.ResourceType getResourceFromHex (Enums.HexType hType);
	Enums.CommodityType getCommodityFromHex (Enums.HexType hType);
	//----------------------------


	//----------------------------
	// Player Class:

	List<GamePiece> getNotOnBoardPiece (); 			  // Get a list of the pieces that aren't on the board
	List<GamePiece> getGamePieces ();                 // Get a list of all the game pieces
	string getUserName();

	int getVictoryCounts();
	bool decrementVictoryPoints(int num);
	void incrementVictoryPoints(int num);

	int getGoldCount();
	bool decrementGoldCount(int num);
	void incrementGoldCount(int num);

	Enums.Status getStatus();
	void setStatus(Enums.Status newStatus);

	// Upgrade the development chart for the given type
	void upgradeDevChart(Enums.DevChartType devChartType);
	int[] getDevFlipChart ();

	List<Enums.ProgressCardName> getProgressCards();
	void addProgressCard(Enums.ProgressCardName cardName);
	bool discardProgressCard(Enums.ProgressCardName cardName);

	int getSafeCardCount();
	void increaseSafeCardCount(int count);
	void decreaseSafeCardCount(int count);

	int getCityWallCount();
	void incrementCityWallCount ();
	bool decrementCityWallCount ();

	int[] getResourceRatios ();
	void updateResourceRatio (Enums.ResourceType resourceType, int newRatio);
	void updateResoureRatios(int [] newRatios);

	int[] getCommodityRatios ();
	void updateCommodityRatio(Enums.CommodityType commodity, int newRatio);
	void updateCommodityRatios(int [] newRatios);

	int[] getCommodities();
	void addCommodity(Enums.CommodityType commodityType, int numToAdd);
	bool discardCommodity(Enums.CommodityType commodityType, int numToRemove);

	int[] getResources();
	void addResource(Enums.ResourceType resourceType, int numToAdd);
	bool discardResource(Enums.ResourceType resource, int numToRemove);

	bool hasMovedRoad();
	void movesRoad();
	void roadNotMoved ();

	// Aqueduct is reached when a player has level 3 in science
	bool getAqueduct();
	void giveAqueduct();
	//----------------------------


	//----------------------------
	// Bank Class:

	int getResourceAmount (Enums.ResourceType res);
	int getCommodityAmount (Enums.CommodityType com);
	bool withdrawResource (Enums.ResourceType res, int amount);
	bool withdrawCommodity (Enums.CommodityType com, int amount);
	void depositResource(Enums.ResourceType res, int amount);
	void depositCommodity(Enums.CommodityType com, int amount);

	// Put the given progress card on the bottom of a progress card pile
	void depositProgressCard (Enums.DevChartType progressType, 
	                         Enums.ProgressCardName progressCard);

	// Draw and return a progress card from the requested pile
	Enums.ProgressCardName withdrawProgressCard (Enums.DevChartType progressType);

	// Check if a trade is valid for the bank given certain trade ratios
	bool isValidBankTrade(int[] resRatios, int[] comRatios, Trades trade);

	// Make a trade with the bank, returns false if invalid, true if it goes through
	bool tradeWithBank (int[] resRatios, int[] comRatios, Trades trade);
}
