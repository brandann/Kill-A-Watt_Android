using UnityEngine;
using System.Collections;

namespace Global {

    public class AudioManager : MonoBehaviour {

        #region system
        void Start () {
            playStartMenu();
        }

        // Update is called once per frame
        void Update () {

        }
        #endregion

        #region BackGround Music
        #region AudioClips
        // music tracks
        public AudioClip musicLightBackground;
        public AudioClip musicMediumBackground;
        public AudioClip musicHardBackground;
        #endregion

        #region volume
        // volume levels
        private float volumeLight = .1f;
        private float volumeMedium = .3f;
        private float currentVolume;
        private float setVolume;
        private float volumeIncrement = .001f;
        #endregion

        // switch to end game background music
        private void playEndGame(){
            // what is this state used for? during the death ray shot?
        }

        // switch to exit sound effect
        private void playExit(){
            // what iis this state used for?
        }

        // switch to game background music
        private void playInGame(){
            // play medium background music @ 50% during gameplay
            GetComponent<AudioSource>().clip = musicHardBackground;
            setVolume = volumeMedium;
            currentVolume = 0;
            GetComponent<AudioSource>().Play();
        }

        // switch to pause menu background music
        private void playPause(){
            // play medium background music @ small% during pause
        }

        // switch to quit sound effect
        private void playQuit(){
            // what is this state used for?
        }

        // switch to splash screen background music
        private void playSplashScreen(){
            // play light background music at 50% during the splash screen
        }

        // switch to start menu background music
        private void playStartMenu(){
            // if the menu is entered from any other screen, change the music to
            // light background music @ 50%
            GetComponent<AudioSource>().clip = musicLightBackground;
            setVolume = volumeLight;
            GetComponent<AudioSource>().Play();
        }
        #endregion
        
        #region SoundClips
        #region soundClips
        public AudioClip clipMinionCollision;
        public AudioClip clipTower0TakeOver;
        public AudioClip clipTower1TakeOver;
        public AudioClip clipTower2TakeOver;
        public AudioClip clipMagnet;
        public AudioClip clipBomb;
        public AudioClip clipShield;
        public AudioClip clipShockTower;
        public AudioClip clipGUI;
        #endregion

        #region GUIclick
        public void playGUI() {
            GetComponent<AudioSource>().PlayOneShot (clipGUI);
        }
        #endregion

        #region ShockTower
        public void playShockTower() {
            GetComponent<AudioSource>().PlayOneShot(clipShockTower);
        }
        #endregion

        #region Shield
        public void playShield() {
            GetComponent<AudioSource>().PlayOneShot(clipShield);
        }
        #endregion

        #region Bomb
        public void playBomb() {
            GetComponent<AudioSource>().PlayOneShot(clipBomb);
        }
        #endregion

        #region Magnet
        public void playMagnet() {
            GetComponent<AudioSource>().PlayOneShot(clipMagnet);
        }
        #endregion

        #region MinionCollision
        //-----------------------------------------------------------------------------
        // play a sound  when 2 minions collide
        public void playMinionCollision() {
            GetComponent<AudioSource>().PlayOneShot(clipMinionCollision);
        }
        #endregion

        #region TowerTakeOver
        //-----------------------------------------------------------------------------
        // play a sound  when the tower becomes neutural controlled
        public void playTowerTakeover0(){
            GetComponent<AudioSource>().PlayOneShot(clipTower0TakeOver);
        }
        
        //-----------------------------------------------------------------------------
        // play a sound when the tower becomes player1 controlled
        public void playTowerTakeover1(){
            GetComponent<AudioSource>().PlayOneShot(clipTower1TakeOver);
        }
      
        //-----------------------------------------------------------------------------
        // play a sound when the tower becomes player2 controlled
        public void playTowerTakeover2(){
            GetComponent<AudioSource>().PlayOneShot(clipTower2TakeOver);
        }
        #endregion
        #endregion
    }
}