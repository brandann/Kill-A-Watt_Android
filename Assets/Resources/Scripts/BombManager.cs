﻿using UnityEngine;
using System.Collections;

namespace Global{
    public class BombManager : MonoBehaviour {

        private GameObject bombPrefab1;
        private GameObject bombPrefab2;
        private float COUNTMAX = 10;
        private const float COOLDOWNMAX = 10;
        private float count = 15;
        private float cooldown = 0;
        private bool started = false;
        public ePlayer myOwner = ePlayer.Neutral;

        // Use this for initialization
        void Start () {
            AudioManager audioManager = GameObject.Find ("Main Camera").GetComponent<AudioManager> ();
            audioManager.playBomb();
            // load the bombparticle prefab
            bombPrefab1 = Resources.Load("Prefabs/BombParticlePlayer1") as GameObject;
            bombPrefab2 = Resources.Load("Prefabs/BombParticlePlayer2") as GameObject;
        }
        
        // Update is called once per frame
        void Update () {
            if (!started)
                return;
            // check duration of running particle effect
            if (count >= COUNTMAX){
                Destroy(this.gameObject);
            }
            //ceate particle
            makeBombPoint ();
            //count up particle duration
            count++;
        }

        private void makeBombPoint() {
            //instantiate and deploy particle
            GameObject e = null;
            if (myOwner == ePlayer.Player1) {
                e = GameObject.Instantiate (bombPrefab1, this.transform.position, Quaternion.LookRotation (Vector3.forward, Vector3.forward)) as GameObject;
            } else if(myOwner == ePlayer.Player2) {
                e = GameObject.Instantiate (bombPrefab2, this.transform.position, Quaternion.LookRotation (Vector3.forward, Vector3.forward)) as GameObject;
            }
        }

        public void changeOwner(ePlayer owner) {
            myOwner = owner;
            startBomb();
        }

        public void startBomb() {
            //reset values
            count = 0;
            started = true;
        }
    }
}