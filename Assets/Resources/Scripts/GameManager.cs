using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Global {
    public enum ePlayer { Neutral, Player1, Player2 };
    public enum eTowerType { Neutral, Generator, Shock };
    
    public class GameManager : MonoBehaviour
    {
        #region LevelLayoutVariables
        
        //location and type of towermarkers placed on the scene in the heiarchy
        private Dictionary<Vector3, ePlayer> mappedTowers = new Dictionary<Vector3, ePlayer>();
        private Dictionary<Vector3, ePlayer> mappedShocks = new Dictionary<Vector3, ePlayer>();
        private Dictionary<Vector3, ePlayer> mappedRoots = new Dictionary<Vector3, ePlayer>();
        
        //List of actuall ingame towers and their positions
        public Dictionary<Vector3, Tower> towerLookup = new Dictionary<Vector3, Tower>();

        public List<Tower>[] _towersByPlayer = new List<Tower>[Enum.GetNames(typeof(ePlayer)).Length];
        
        //Default unit count for each of the 3 factions        
        private int NeutralStartingUnits = 5;
        private int Player1StartingUnits = 10;
        private int Player2StartingUnits = 10;
        
        public GameObject towerPrefab;
        public GameObject shockPrefab;
        public GameObject root1Prefab;
        public GameObject root2Prefab;
        public GameObject plusTenPrefab;
        public GameObject shieldP1;
        public GameObject shieldP2;
        #endregion

        //Each Player's score
        public float player1Score = 0;
        public float player2Score = 0;
        private float fequency = 5;
        private float lastCalc;
        public bool player1HasAllTowers = false;
        public bool player2HasAllTowers = false;
        StateManager stateManager;

        //Set from inspector to have selections cleared after attacks
        public bool ClearSelectionAfterAttack;
        
        void Start() {			

            GameObject manager = GameObject.FindGameObjectsWithTag("MainCamera")[0];
            stateManager = manager.GetComponent<StateManager>();
            
            //Find all the locations that towers should spawn at from markers
            BuildTowerLocations();
            SpawnTowers();
        }
      
        public void resetScore(){
            player1Score = 0;
            player2Score = 0;
        }

        private void winCondition(){
            int player1Count = 0;
            int player2Count = 0;
            foreach (var tower in towerLookup) {

                if(tower.Value.myOwner == ePlayer.Player1)
                    player1Count++;
                else if(tower.Value.myOwner == ePlayer.Player2)
                    player2Count++;
            }
            if (player1Count == 0 ) {
                player2HasAllTowers = true;
                stateManager.status = WorldGameState.EndGame;
                return;
            }
            if (player2Count == 0){
                player1HasAllTowers = true;
                stateManager.status = WorldGameState.EndGame;
            }
        }

        #region updateScore
        private Queue<GameObject> TowerQ = new Queue<GameObject>();
        private float regenSpeed = 1f;
        private float lastUpdate;

        public void calculateScore() {
            //int test = 0;
            GameObject[] rootArray = GameObject.FindGameObjectsWithTag("RootNode");

            if (Time.realtimeSinceStartup - lastCalc > fequency) {
                
                lastCalc = Time.realtimeSinceStartup;
                if (rootArray != null) {
                    BFS(rootArray[0]);
                    BFS(rootArray[1]);
                }

                GameObject[] towerArray = GameObject.FindGameObjectsWithTag("Tower");
                foreach (GameObject go in towerArray) {
                    go.GetComponent<Tower>().Visited = false;
                }
            }
        }

        public void BFS(GameObject root) {
            Dictionary<GameObject, LineRenderer> rootAdjacent = root.GetComponentInChildren<Connection>().connections;
            //todo reduce the .getComponent<> calls they are expensive
            foreach (var node in rootAdjacent) {
                if (root.GetComponent<DeathRay>().myOwner == node.Key.GetComponent<Tower>().myOwner) {
                    TowerQ.Enqueue(node.Key);
                    node.Key.GetComponent<Tower>()._visited = true;
                    if (root.GetComponent<DeathRay>().myOwner == ePlayer.Player1) {
                        //player1Score += 10; // this was the old version
                        player1Score += node.Key.GetComponent<Tower>().Units;
                        node.Key.GetComponent<Tower>().PlayPlusTen();
                    }
                }
            }

            while (TowerQ.Count != 0) {
                GameObject currentNode = TowerQ.Dequeue(); //remove the first element
                ePlayer myOwner = currentNode.GetComponent<Tower>().myOwner; // get the owner of current
                if (!currentNode.GetComponent<Tower>()._visited) {
                    if (root.GetComponent<DeathRay>().myOwner == ePlayer.Player1) {
                        player1Score += 10;
                        currentNode.GetComponent<Tower>().PlayPlusTen();
                    }
                    currentNode.GetComponent<Tower>()._visited = true;
                }
                Dictionary<GameObject, LineRenderer> adjacent = currentNode.GetComponentInChildren<Connection>().connections;
                foreach (var node in adjacent) {
                    ePlayer childOwner = node.Key.GetComponent<Tower>().myOwner; // get the owner of the child node
                    if (myOwner == childOwner && node.Key.GetComponent<Tower>()._visited == false) {
                        TowerQ.Enqueue(node.Key);
                    }
                }
            }
        }
        #endregion

        //Adds up all of the units in each players' towers to calculate score
        void FixedUpdate() {
            
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.E))
            {
                SelectAll(true); // select all player1
            }
            
			//---------------------------------------
			if (stateManager.status == WorldGameState.InGame)
			{
				calculateScore();
			}

            //---------------------------------------
            if (!player2HasAllTowers && !player1HasAllTowers)
            {
                return;
            }
            else
            {
                winCondition();
            }
                
            //---------------------------------------
            if (towerLookup.Count > 1)
            {
                winCondition();
            }
                
        }

        //Called from individual towers to notify all of the same player's towers to attack a certain location
        public void AttackToward(Vector3 targetPosition, ePlayer attackingPlayer) {
            foreach (KeyValuePair<Vector3, Tower> entry in towerLookup) {
                if (entry.Value.Selected && entry.Value.myOwner == attackingPlayer)
                    StartCoroutine(entry.Value.SpawnAttack(targetPosition));
            }
            if(ClearSelectionAfterAttack)
                DeselectTowers(attackingPlayer == ePlayer.Player1);
        }

        public void DeselectTowers(bool isPlayer1) {
            
            ePlayer playerToDeselect = (isPlayer1 == true) ? ePlayer.Player1 : ePlayer.Player2;

            foreach (KeyValuePair<Vector3, Tower> entry in towerLookup)
            {
                if (entry.Value.Selected && entry.Value.myOwner == playerToDeselect)
                {
                    entry.Value.ToggleSelect();
                    entry.Value.updateSprite();
                }
            }
           
        }

        //-----------------------------------------------------------------------------------------------------------------------------
        #region SelectAll

        //Called from individual towers to notify all of the same player's towers to deselect a certain location
        public void SelectAll(bool isPlayer1) {
            ePlayer playerToDeselect = (isPlayer1 == true) ? ePlayer.Player1 : ePlayer.Player2;
            foreach (KeyValuePair<Vector3, Tower> entry in towerLookup)
            {
                if (!entry.Value.Selected && entry.Value.myOwner == playerToDeselect)
                {
                    entry.Value.ToggleSelect();
                    entry.Value.updateSprite();
                }
            }
        }

        #endregion

        void BuildTowerLocations() {
            GameObject[] towerMarkers = GameObject.FindGameObjectsWithTag("towerMarker");
            foreach (GameObject tm in towerMarkers) {
                switch (tm.name) {
                    case "towerMarkerNeutral":
                        mappedTowers.Add(tm.transform.position, ePlayer.Neutral);
                        break;
                    case "towerMarkerPlayer1":
                        mappedTowers.Add(tm.transform.position, ePlayer.Player1);
                        break;
                    case "towerMarkerPlayer2":
                        mappedTowers.Add(tm.transform.position, ePlayer.Player2);
                        break;
                    case "shockMarkerPlayer1":
                        mappedShocks.Add(tm.transform.position, ePlayer.Player1);
                        break;
                    case "shockMarkerPlayer2":
                        mappedShocks.Add(tm.transform.position, ePlayer.Player2);
                        break;
                    case "shockMarkerPlayerNeutral":
                        mappedShocks.Add(tm.transform.position, ePlayer.Neutral);
                        break;
                    default:
                        Debug.LogError("Invalid towerMarker type in buildTowerLocations");
                        break;
                }
                Destroy(tm);
            }   

            GameObject[] rootMarkers = GameObject.FindGameObjectsWithTag("RootMarker");
            foreach (GameObject rm in rootMarkers)
            {
                if (rm.name == "RootMarkerPlayer1")
                    mappedRoots.Add(rm.transform.position, ePlayer.Player1);
                else
                    mappedRoots.Add(rm.transform.position, ePlayer.Player2);
                Destroy(rm);
            }
        }

        //Instanciates the towers in all the locations specified by BuildTowerLocations()
        public void SpawnTowers()
        {
            //Initialize _towersByPlayers
            for (int i = 0; i < _towersByPlayer.Length; ++i)
            {
                _towersByPlayer[i] = new List<Tower>();
            }
            
               
            foreach (KeyValuePair<Vector3, ePlayer> r in mappedRoots) {
                if(r.Value == ePlayer.Player1){
                    GameObject aRoot = (GameObject)GameObject.Instantiate(root1Prefab, r.Key, Quaternion.Euler(0, 0, 0));
                }
                else{
                    GameObject aRoot = (GameObject)GameObject.Instantiate(root2Prefab, r.Key, Quaternion.Euler(0, 0, 0));
                }
            }

            foreach (KeyValuePair<Vector3, ePlayer> entry in mappedTowers)
            {

                GameObject aTower = (GameObject)GameObject.Instantiate(towerPrefab, entry.Key, Quaternion.Euler(0, 0, 0));
                Tower tScript = aTower.GetComponent<Tower>();
                tScript.SwitchOwner(entry.Value);
                switch (entry.Value)
                {
                    case ePlayer.Neutral:
                        tScript.Units = NeutralStartingUnits;
                        break;
                    case ePlayer.Player1:
                        tScript.Units = Player1StartingUnits;
                        break;
                    case ePlayer.Player2:
                        tScript.Units = Player2StartingUnits;
                        break;
                    default:
                        Debug.LogError("Invalid Ownership type");
                        break;
                }
                towerLookup.Add(entry.Key, tScript);
                _towersByPlayer[(int)entry.Value].Add(tScript);
                Tower.OnOwnerChange += HandleTowerPlayerSwitch;



            }

            foreach (KeyValuePair<Vector3, ePlayer> entry in mappedShocks) {
                GameObject aTower = (GameObject)GameObject.Instantiate(shockPrefab, entry.Key, Quaternion.Euler(0, 0, 0));
                Tower tScript = aTower.GetComponent<Tower>();
                tScript.SwitchOwner(entry.Value);
                switch (entry.Value) {
                    case ePlayer.Neutral:
                        tScript.Units = NeutralStartingUnits * 3;
                        break;
                    case ePlayer.Player1:
                        tScript.Units = Player1StartingUnits;
                        break;
                    case ePlayer.Player2:
                        tScript.Units = Player2StartingUnits;
                        break;
                    default:
                        Debug.LogError("Invalid Ownership type");
                        break;
                }
                towerLookup.Add(entry.Key, aTower.GetComponent<Tower>());
            }
        }

        private void HandleTowerPlayerSwitch(Tower t, ePlayer from, ePlayer to)
        {
            _towersByPlayer[(int)from].Remove(t);
            _towersByPlayer[(int)to].Add(t);
        }
    }
}