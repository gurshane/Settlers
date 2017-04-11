using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.Networking;

public class ProgressCards : NetworkBehaviour {

	MoveAuthorizer ma;
	ProgressAuthroizor pa;
	Graph graph;
	private NetworkIdentity objNetId;
	static public ProgressCards instance = null;

	void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ma = new MoveAuthorizer();
		graph = new Graph ();
    }

	// Move a knight from source to target
	public bool bishop(Hex target, Enums.Color color, bool server)
    {

		// Check if the knight can be moved
		if (!ma.canMoveRobber (target))
        {
			return false;
		}

		Player current = GameManager.instance.getCurrentPlayer();

		foreach (Vertex v in BoardState.instance.vertexPosition.Values) {

			bool nextToRobber = false;
			if(target.adjacentToVertex(v)) {
				if (!Object.ReferenceEquals(v.getOccupyingPiece(), null)) {
					if (v.getOccupyingPiece().getColor() != color) {
						if(v.getOccupyingPiece().getPieceType() == PieceType.CITY ||
							v.getOccupyingPiece().getPieceType() == PieceType.SETTLEMENT) {

							nextToRobber = true;
						}
					}
				}
			}

			if (nextToRobber) {
				int opp = (int)v.getOccupyingPiece().getColor();
				Player oppo  = GameManager.instance.getPlayer(opp);
				bool taken = false;
				for (int i = 0; i < 5; i++) {
					if(oppo.getResources()[i] > 0) {
						GameManager.instance.getPersonalPlayer().changeResource((ResourceType)i, -1, oppo.getID());
						taken = true;
						GameManager.instance.getPersonalPlayer().changeResource((ResourceType)i, 1, current.getID());
						break;
					}
				}
				if (!taken) {
					for (int i = 0; i < 3; i++) {
						if(oppo.getCommodities()[i] > 0) {
							GameManager.instance.getPersonalPlayer().changeCommodity((CommodityType)i, -1, oppo.getID());
							GameManager.instance.getPersonalPlayer().changeCommodity((CommodityType)i, 1, current.getID());
							break;
						}
					}
				}
			}
		}

		GameManager.instance.getPersonalPlayer().removeProgressCard(ProgressCardName.BISHOP, current.getID());

		assignAuthority(server);
        RpcBishop(target.transform.position);
		removeAuthority(server);

		return true;
	}

    [ClientRpc]
	void RpcBishop(Vector3 target)
    {

		Hex source = null;
		GamePiece piece = null;
		foreach (Hex h in BoardState.instance.hexPosition.Values) {
			piece = h.getOccupyingPiece();
			if (!Object.ReferenceEquals(piece, null)) {
				if (piece.getPieceType() == Enums.PieceType.ROBBER) {
					source = h;
					break;
				} else {
					piece = null;
				}
			}
		}

		Hex targetLocation = BoardState.instance.hexPosition[target];
		if (!Object.ReferenceEquals(piece, null)) {
			targetLocation.setOccupyingPiece(piece);
		} else {
			targetLocation.setOccupyingPiece(new Robber());
		}

		GameObject newRobber = Instantiate<GameObject>(PrefabHolder.instance.robber, target, Quaternion.identity);
        newRobber.transform.position += new Vector3(0f, 10f, 0f);
		BoardState.instance.spawnedObjects.Add(target, newRobber);

		if (!Object.ReferenceEquals(piece, null)) {
			source.setOccupyingPiece(null);
			Destroy (BoardState.instance.spawnedObjects [source.transform.position]);
			BoardState.instance.spawnedObjects.Remove(source.transform.position);
		}
    }

	public bool constitution()
    {

		Player current = GameManager.instance.getCurrentPlayer();

		GameManager.instance.getPersonalPlayer().changeVictoryPoints(1, current.getID());

		GameManager.instance.getPersonalPlayer().removeProgressCard(ProgressCardName.CONSTITUTION, current.getID());

		return true;
	}

	// Move a knight from source to target
	public bool diplomat(Edge source, Edge target, Enums.Color color, bool server)
    {

		if (!pa.canRoadMove (source, target, color))
        {
			return false;
		}
		assignAuthority(server);
        RpcDiplomat(source.transform.position, target.transform.position, color);

		removeAuthority(server);
		return true;
	}

    [ClientRpc]
	void RpcDiplomat(Vector3 source, Vector3 target, Enums.Color color)
    {

		Edge sourcePiece = BoardState.instance.edgePosition [source];
        Edge targetPiece = BoardState.instance.edgePosition [target];

		GameObject spawnedRoad = Instantiate<GameObject>(PrefabHolder.instance.road, target, Quaternion.identity);
		spawnedRoad.GetComponent<MeshRenderer>().material.SetColor("_Color", translateColor(color));
		Destroy (BoardState.instance.spawnedObjects [source]);

		BoardState.instance.spawnedObjects.Add(target, spawnedRoad);
		BoardState.instance.spawnedObjects.Remove (source);

        Road movedRoad = (Road)sourcePiece.getOccupyingPiece();

        // Move the knight
        sourcePiece.setOccupyingPiece(null);
        targetPiece.setOccupyingPiece(movedRoad);
    }

	// Displace a knight at target with knight at source
	public bool intrigue(Vertex target, Enums.Color color, bool server)
    {
		// Check if the knight can displace
		if (!pa.canIntrigueKnight (target, color))
        {
			return false;
		}

		Knight kTarget = (Knight)target.getOccupyingPiece ();
		int targetLevel = kTarget.getLevel ();

		bool gone = true;
		foreach (Vertex v in BoardState.instance.vertexPosition.Values)
        {
			if (graph.areConnectedVertices (v, target, kTarget.getColor()))
            {
				if (Object.ReferenceEquals (v.getOccupyingPiece (), null))
                {
					gone = false;
					break;
				}
			}
		}

		if (!gone) {
			int currTurn = GameManager.instance.getPlayerTurn();
			GameManager.instance.getPersonalPlayer().setOldTurn(currTurn);

			foreach (Player p in GameManager.instance.players) {
				GameManager.instance.getPersonalPlayer().setMoveType(MoveType.SPECIAL, p.getID());
			}

			Player opponent = GameManager.instance.getPlayer((int)kTarget.getColor());
			GameManager.instance.getPersonalPlayer().setSpecial(Special.KNIGHT_DISPLACED, opponent.getID());
			GameManager.instance.getPersonalPlayer().setI1(targetLevel, opponent.getID());
			GameManager.instance.getPersonalPlayer().setB1(kTarget.isActive(), opponent.getID());
			GameManager.instance.getPersonalPlayer().SetV1(target, opponent.getID());
			GameManager.instance.getPersonalPlayer().setSpecialTurn((int)kTarget.getColor());
		}
		assignAuthority(server);
		RpcIntrigue(target.transform.position, targetLevel, color);

		removeAuthority(server);
		return true;
	}

    [ClientRpc]
	void RpcIntrigue(Vector3 target, int targetLevel, Enums.Color color)
    {
        Vertex targetPiece = BoardState.instance.vertexPosition[target];
        Knight targetKnight = (Knight)targetPiece.getOccupyingPiece();

		Destroy (BoardState.instance.spawnedObjects [target]);
		BoardState.instance.spawnedObjects.Remove(target);
        
        targetPiece.setOccupyingPiece(null);
		targetKnight.takeOffBoard();

    }

	// Get a unity color from an enums color
	private UnityEngine.Color translateColor(Enums.Color color) {
		switch (color)
		{
		case Enums.Color.RED:
			return UnityEngine.Color.red;
		case Enums.Color.BLUE:
			return UnityEngine.Color.blue; 
		case Enums.Color.ORANGE:
			return UnityEngine.Color.green;
		case Enums.Color.WHITE:
			return UnityEngine.Color.white;
		default:
			return UnityEngine.Color.gray;
		}
	}

	// Assign client authority
	private void assignAuthority(bool server) {
        if (!server) {
			objNetId = this.GetComponent<NetworkIdentity> ();     
			objNetId.AssignClientAuthority (connectionToClient);   
		}
    }

	// Remove client authority
    private void removeAuthority(bool server) {
        if (!server) {
			objNetId.RemoveClientAuthority (connectionToClient); 
		}
    }


}
