using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Global
{
    public class GameStateQuery : MonoBehaviour
    {

        private TowerState[] _towers;       

        #region PUBLIC METHODS
        
        // Return a list of towers ordered by how many units the towers have
        // 0 position is most units, .Length position is least units
        //
        // player is the players unit returned. i.e. ePlayer.Player1 returns all
        // the player 1 towers sorted by unit count
        public TowerState[] getTowersByUnit(ePlayer player)
        {
            // Code is not tested
            var towers = this.mtowers;
            var towerList = new List<TowerState>();
            for (int i = 0; i < towers.Length; i++ )
            {
                if (player == towers[i].mPlayer)
                {
                    towerList.Add(towers[i]);
                    towers[i].mVisited = true;
                }
            }

            return towerList.ToArray();
        }

        // Returns the number of units a player has on screen
        //
        // player is the players unit to be counted
        public int getUnitsOnScreenCount(ePlayer player) {
           
            Debug.LogError("getUnitsOnScreenCount not initilized");
            /*
             * Code is not tested
            var unitsOnScreen = GameObject.FindGameObjectsWithTag("Units");
            var unitCount = 0;
            for (int i = 0; i < unitsOnScreen.Length; i++ )
            {
                if(unitsOnScreen[i].GetComponent<unitBehavior>())
                {
                    unitCount++;
                }
            }
            */
            return -1;
        }

        // Returns a list of towers ordered by how close the tower is to the position.
        // towerstate[0] = closest tower
        // towerstate[towerstate.Length] = furthest tower
        //
        // player is the players tower to look for
        //
        // position is the position to compare each tower to
        public TowerState[] getTowersByLocation(ePlayer player, Vector3 position)
        {
            Debug.LogError("getTowersByLocation not initilized");
            return this.mtowers;
        }

        // Returns a list of towers that are of type 'type'
        //
        // player is the towers owner to look for.
        //
        // type is the tower type to look for
        public TowerState[] getTowersByType(ePlayer player, eTowerType type)
        {
            Debug.LogError("getTowersByType not initilized");
            return this.mtowers;
        }

        // Return a list of towers under attack
        //
        // player is the tower type to look for attacking state
        //
        // isAttacked sets the attack state to look for
        //      true returns a list of towers being attacked
        //      false returns a list of towers NOT being attacked
        public TowerState[] getTowersUnderAttack(ePlayer player, bool isAttacked)
        {
            Debug.LogError("getTowersUnderAttack not initilized");
            return this.mtowers;
        }

        // Returns a list of towers attacking another tower
        //
        // player is the tower owner to look for.
        //
        // isAttacking sets the attack state to look for
        //      true returns a list of towers that are attacking a tower
        //      false returns a list of towers that are NOT attacking a tower
        public TowerState[] getTowersAttacking(ePlayer player, bool isAttacking)
        {
            Debug.LogError("getTowersAttacking not initilized");
            return this.mtowers;
        }

        // Returns the players powerup state
        //
        // player is the player state to check
        //
        // true = player has an available powerup
        // false = player DOES NOT have an avaiable powerup
        public bool getPlayerPowerUpState(ePlayer player)
        {
            Debug.LogError("getPlayerPowerUpState not initilized");
            return false;
        }

        // Returns a list of towers that are selected
        //
        // player is the player to check for selection status
        //
        // isSelected sets the selection state to check for
        //      true returns all towers that are selected
        //      false returns all towers that are NOT selected
        public TowerState[] getTowersSelected(ePlayer player, bool isSelected)
        {
            Debug.LogError("getTowersSelected not initilized");
            return this.mtowers;
        }

        // Returns a list of towers that have an active shield
        //
        // player is the players shield state to check for
        //
        // hasShield set the shield state to check for
        //      true returns a list of towers that have an active shield
        //      false returns a list of towers that DO NOT have an active shield
        public TowerState[] getTowersWithShield(ePlayer player, bool hasShield)
        {
            Debug.LogError("getTowersWithShield not initilized");
            return this.mtowers;
        }

        // Returns a list of towers that have an active Magnet
        //
        // player is the players Magnet state to check for
        //
        // hasMagnet set the Magnet state to check for
        //      true returns a list of towers that have an active Magnet
        //      false returns a list of towers that DO NOT have an active Magnet
        public TowerState[] getTowersWithMagnet(ePlayer player, bool hasMagnet)
        {
            Debug.LogError("getTowersWithMagnet not initilized");
            return this.mtowers;
        }

        // Returns a list of towers that have an active Connection
        //
        // player is the players Connection state to check for
        //
        // hasMagnet set the Connection state to check for
        //      true returns a list of towers that have an active Connection
        //      false returns a list of towers that DO NOT have an active Connection
        public TowerState[] getTowersWithConnection(ePlayer player, bool hasConnection)
        {
            Debug.LogError("getTowersWithConnection not initilized");
            return this.mtowers;
        }
        #endregion

        #region PRIVATE METHODS
        private TowerState[] mtowers
        {
            get {

                // if the _towers is not instantiated then find all the objects
                if (null == _towers) {
                    
                    // get all the towers from the heirarchy and set them to _towers array
                    var findtowers = GameObject.FindGameObjectsWithTag("Tower");
                    _towers = new TowerState[findtowers.Length];

                    // set all tower refrences
                    for (int i = 0; i < findtowers.Length; i++) {
                        _towers[i] = new TowerState(); 
                        _towers[i].mTower = findtowers[i]; // gameobject never changes
                        _towers[i].mPosition = findtowers[i].transform.position; // position never changes
                    }
                }

                // update all the object
                // since the query is looking at a live game, a towerstate update
                // is required everytime this is called
                for (int i = 0; i < _towers.Length; i++)
                {
                    _towers[i].mUnits = _towers[i].mTower.GetComponent<Tower>().Units; // update the tower state unit count
                    _towers[i].mPlayer = _towers[i].mTower.GetComponent<Tower>().myOwner; // update the owner of the tower
                    _towers[i].mVisited = false; // set visited state to false
                }
                return _towers;
            }
        }
        #endregion
    }
}