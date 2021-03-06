﻿using UnityEngine;
using System.Collections;

namespace Global {
    public class ScientistAbility : MonoBehaviour {

        #region prefabs
        private GameObject bombManagerPrefab;
        private GameObject shieldP1;
        private GameObject shieldP2;
        private GameObject p1DeathRay;
        private GameObject p2DeathRay;
        private GameObject magnetPrefab;
        public GUIStyle Crosshairs;
        public GUIStyle GreenCrosshairs;
        public GUIStyle BombCrosshairs;
        public GUIStyle RedCrosshairs;
        GameManager gameManager;
        #endregion

        private bool active = false;
        public enum ability{
            none,
            ability0,
            ability1,
            ability2
        };
		
        public ability currentAbility = ability.none;
        
        #region Ability2Variables
        bool fireMag;
        float clickTime = 0;
        Vector3 throwMagFrom;
        const float magTorque = 1500f;
        const float magForce = 20;
        ePlayer magThrower;
        #endregion
        
        // Use this for initialization
        void Start () {
            cam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>();
            gameManager = cam.GetComponent<GameManager> ();
            bombManagerPrefab = Resources.Load("Prefabs/BombManager") as GameObject;
            shieldP1 = Resources.Load("Prefabs/ShieldPlayer1") as GameObject;
            shieldP2 = Resources.Load("Prefabs/ShieldPlayer2") as GameObject;
            magnetPrefab = Resources.Load("Prefabs/Magnet") as GameObject;
        }

        void OnGUI(){
            switch (currentAbility) {
                case ability.none:
                    Cursor.visible = true;
                    break;
                case ability.ability0:
                    Cursor.visible = false;
                    if(overMyTower)
                    GUI.Box(new Rect(Input.mousePosition.x - 25, Screen.height - (Input.mousePosition.y) - 25, 50, 50),"", GreenCrosshairs);
                    else
                    GUI.Box(new Rect(Input.mousePosition.x - 25, Screen.height - (Input.mousePosition.y) - 25, 50, 50),"", Crosshairs);
                    break;
                case ability.ability1:
                    Cursor.visible = false;
                    GUI.Box(new Rect(Input.mousePosition.x - 25, Screen.height - (Input.mousePosition.y) - 25,75, 75),"", BombCrosshairs);
                    break;
                case ability.ability2:
                    Cursor.visible = false;
                    if(overEnemyTower)
                      GUI.Box(new Rect(Input.mousePosition.x - 25, Screen.height - (Input.mousePosition.y) - 25, 50, 50),"", RedCrosshairs);
                    else
                      GUI.Box(new Rect(Input.mousePosition.x - 25, Screen.height - (Input.mousePosition.y) - 25, 50, 50),"", Crosshairs);
                    break;
            }
        }
		
        // Update is called once per frame
        void Update () {
            switch (currentAbility) {
                case ability.none:
                    break;
                case ability.ability0:
                    ability0();
                    break;
                case ability.ability1:
                    ability1();
                    break;
                case ability.ability2:
                    ability2();
                    break;
            }
        }

        #region Ability0
        Camera cam;
        private bool overMyTower;
        private ePlayer shieldOwner;
        
        public void setAbility0(ePlayer owner) {
            currentAbility = ability.ability0;
            shieldOwner = owner;
        }
		
        public void ability0() { // erick
            print ("ability0");
            overMyTower = false;
            Tower tower = null;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
            Vector3 pos = new Vector3 (mousePos.x, mousePos.y, 0);
            RaycastHit2D hit = Physics2D.Raycast(mousePos,Vector2.zero);
            if (hit.collider != null) {
                tower = hit.collider.gameObject.GetComponent<Tower> ();
                if(tower != null && shieldOwner == tower.myOwner)
                    overMyTower = true;
            }
            if (Input.GetMouseButton (1)) {
                currentAbility = ability.none;
                return;
            }
            if (Input.GetMouseButton (0)) {
                if (tower != null) {
                    if (shieldOwner == ePlayer.Player1 && tower.myOwner == ePlayer.Player1) {
                        GameObject one = (GameObject) Instantiate (shieldP1, tower.transform.position, Quaternion.Euler (0, 0, 0));
                        currentAbility = ability.none;
                        gameManager.resetScore ();
                        tower = null;
                        overMyTower = false;
                    } else if (shieldOwner == ePlayer.Player2 && tower.myOwner == ePlayer.Player2) {
                        GameObject two = (GameObject) Instantiate (shieldP2, tower.transform.position, Quaternion.Euler (0, 0, 0));
                        currentAbility = ability.none;
                        gameManager.resetScore ();
                        tower = null;
                        overMyTower = false;
                    }
                }
            }
        }
		
        #endregion

        #region Ability1
        private ePlayer bombOwner;

        // sets ability
        public void setAbility1(ePlayer owner) {
            if (currentAbility == ability.none) {
                bombOwner = owner;
                currentAbility = ability.ability1;
            }
        }

        // does ability
        public void ability1() {
            // reset the abiltyManabger if the user right clicks
            if (Input.GetMouseButton (1)) {
                currentAbility = ability.none;
                return;
            }
            if (Input.GetMouseButton (0)) {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
                GameObject e = Instantiate (bombManagerPrefab, new Vector3(mousePos.x,mousePos.y,0), Quaternion.LookRotation (Vector3.forward, Vector3.forward)) as GameObject;
                BombManager BM = e.GetComponent<BombManager> ();
                if (BM != null) {
                    BM.changeOwner (bombOwner);
                }
                currentAbility = ability.none;
                gameManager.resetScore();
            }
        }

        #endregion

        #region Ability3

        private bool overEnemyTower;
        public void setAbility2(ePlayer thrower)
        {
            //Cant initialize on start as towers aren't placed until game begins 
            if (p1DeathRay == null || p2DeathRay == null)
            {
                p1DeathRay = GameObject.Find("DeathRayPlayer1(Clone)");
                p2DeathRay = GameObject.Find("DeathRayPlayer2(Clone)");
            }          
            active = true;
            currentAbility = ability.ability2;
            throwMagFrom = (thrower == ePlayer.Player1) ? p1DeathRay.transform.position : p2DeathRay.transform.position;
            clickTime = Time.time;
            magThrower = thrower;            //Used for the ability2()
        }

        private void ability2() { // gary
            // returns if the users right clicks
            if (Input.GetMouseButton (1)) {
                currentAbility = ability.none;
                return;
            }
            // code to change the mouse sprite, yes, it is redundant in some ways
            // I just copied it from my own ability and altered it a little
            overEnemyTower = false;
            Tower tower = null;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
            Vector3 pos = new Vector3 (mousePos.x, mousePos.y, 0);
            RaycastHit2D hitTower = Physics2D.Raycast(mousePos,Vector2.zero);
            if (hitTower.collider != null) {
                tower = hitTower.collider.gameObject.GetComponent<Tower> ();
                if(tower != null && magThrower != tower.myOwner && tower.myOwner != ePlayer.Neutral)
                    overEnemyTower = true;
            }
            //end redundant code ------------------------------------
            if (clickTime < Time.time && Input.GetMouseButtonUp(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider == null)
                    return;
                Tower thingClicked = hit.collider.gameObject.GetComponent<Tower>();
                if (thingClicked == null)
                    return;
                if (thingClicked.myOwner == ePlayer.Neutral || thingClicked.myOwner == magThrower)
                    return;
                    GameObject mag = (GameObject) Instantiate(magnetPrefab, throwMagFrom, Quaternion.Euler(0, 0, 0));
                    Vector2 toMouse = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - throwMagFrom) * magForce;
                    mag.GetComponent<Rigidbody2D>().AddForce(toMouse);
                    mag.GetComponent<Rigidbody2D>().AddTorque(magTorque);
                    MagnetBehavior mb = mag.GetComponent<MagnetBehavior>();
                    mb.target = hit.collider;
                    active = false;
                    gameManager.resetScore();
                    currentAbility = ability.none;
            } 
        }

        #endregion
    }
}