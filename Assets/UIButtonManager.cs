using UnityEngine;
using System.Collections;

namespace Global
{
    public class UIButtonManager : MonoBehaviour
    {

        AudioManager audioManager;

        // Use this for initialization
        void Start()
        {
            audioManager = GameObject.Find("Main Camera").GetComponent<AudioManager>();
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
            Application.LoadLevel("Tutorial");
        }

        public void OnStartSettingsButtonPressed()
        {
            Debug.Log("Settings Button Pressed");
            audioManager.playGUI();
        }
    }
}