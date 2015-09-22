using UnityEngine;
using System.Collections;

namespace Global
{
    public class UIButtonManager : MonoBehaviour
    {

        AudioManager audioManager;

        public GameObject startScreenManager;
        private StartScreenManager _startManager;

        // Use this for initialization
        void Start()
        {
            audioManager = GameObject.Find("Main Camera").GetComponent<AudioManager>();
            _startManager = startScreenManager.GetComponent<StartScreenManager>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnStartPlayButtonPress()
        {
            Debug.Log("Play Button Pressed");
            audioManager.playGUI();
            Application.LoadLevel("Game");
        }

        public void OnStartTutorialButtonPress()
        {
            Debug.Log("Tutorial Button Pressed");
            audioManager.playGUI();
            _startManager.ShowCanvas(StartScreenManager.Canvasname.Tutorial);
            //Application.LoadLevel("Tutorial");
        }

        public void OnStartSettingsButtonPressed()
        {
            Debug.Log("Settings Button Pressed");
            audioManager.playGUI();
            _startManager.ShowCanvas(StartScreenManager.Canvasname.Settings);
        }

        public void OnStartCreditsButtonPressed()
        {
            Debug.Log("Credits Button Pressed");
            audioManager.playGUI();
            _startManager.ShowCanvas(StartScreenManager.Canvasname.Credits);
        }
    }
}