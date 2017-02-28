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
	//Game Piece Class:
	Enums.Color getColor();
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
	bool canUpgradeKnight (Dictionary<Enums.ResourceType, int> resources, 
	                      Dictionary<Enums.DevChartType, int> devChart, Vertex v);

	// Given the resources, and vertex, can a knight be upgraded
	bool canActivateKnight (Dictionary<Enums.ResourceType, int> resources, Vertex v);

	/* Given the commodities and development chart, can a development chart be upgraded
	 * in the given development area
	 * Note: a list of pieces must also be given to check if a city is on the board */
	bool canUpgradeDevChart (Enums.DevChartType dev, Dictionary<Enums.CommodityType, int> commodities, 
	                        List<GamePiece> pieces, Dictionary<Enums.DevChartType, int> devChart);

	/* The canBuild... methods check if a piece can be built at the given location, with the
	 * given resources and the piece list that player has */
	bool canBuildSettlement (Vertex location, Dictionary<Enums.ResourceType, int> resources,
	                        List<GamePiece> pieces, Enums.Color color);
	bool canBuildCity (Vertex location, Dictionary<Enums.ResourceType, int> resources,
	                  List<GamePiece> pieces, Enums.Color color);
	bool canBuildCityWall (Vertex location, Dictionary<Enums.ResourceType, int> resources,
	                      int cityWalls, Enums.Color color);
	bool canBuildKnight (Vertex location, Dictionary<Enums.ResourceType, int> resources,
	                    List<GamePiece> pieces, Enums.Color color);
	bool canBuildRoad (Edge location, Dictionary<Enums.ResourceType, int> resources,
	                  List<GamePiece> pieces, Enums.Color color);
	bool canBuildShip (Edge location, Dictionary<Enums.ResourceType, int> resources,
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
}
