using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Global
{
    public class GameStateQuery : MonoBehaviour
    {

        private GameObject[] _towers;

        #region PUBLIC METHODS
        
        // Return a list of towers ordered by how many units the towers have
        // 0 position is most units, .Length position is least units
        //
        // player is the players unit returned. i.e. ePlayer.Player1 returns all
        // the player 1 towers sorted by unit count
        public TowerState[] getTowersByUnit(ePlayer player)
        {
            Debug.LogError("getTowersByUnit not initilized");
            return null;
        }

        // Returns the number of units a player has on screen
        //
        // player is the players unit to be counted
        public int getUnitsOnScreenCount(ePlayer player) {
            Debug.LogError("getUnitsOnScreenCount not initilized");
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
            return null;
        }

        // Returns a list of towers that are of type 'type'
        //
        // player is the towers owner to look for.
        //
        // type is the tower type to look for
        public TowerState[] getTowersByType(ePlayer player, eTowerType type)
        {
            Debug.LogError("getTowersByType not initilized");
            return null;
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
            return null;
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
            return null;
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
            return null;
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
            return null;
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
            return null;
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
            return null;
        }
        #endregion

        #region PRIVATE METHODS
        private GameObject[] mtowers
        {
            get {
                if (null == _towers) {
                    var findtowers = GameObject.FindGameObjectsWithTag("Tower");
                    for (int i = 0; i < findtowers.Length; i++) {
                        _towers[i] = findtowers[i];
                    }
                }
                return _towers;
            }
        }

        private TowerState[] getTowerStates() {
            var towerStates = new TowerState[_towers.Length];
            for (int i = 0; i < _towers.Length; i++)
            {
                towerStates[i] = new TowerState();
                towerStates[i].mTower = _towers[i];
                towerStates[i].mUnits = _towers[i].GetComponent<Tower>().Units;
                towerStates[i].mPosition = _towers[i].transform.position;
                towerStates[i].mPlayer = _towers[i].GetComponent<Tower>().myOwner;
            }
            return towerStates;
        }
        #endregion
    }
}