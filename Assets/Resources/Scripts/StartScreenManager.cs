using UnityEngine;
using System.Collections;

namespace Global
{
    public class StartScreenManager : MonoBehaviour
    {

        public GameObject StartScreenCanvas;
        public GameObject CreditsCanvas;
        public GameObject SettingsCanvas;
        public GameObject TutorialCanvas;

        public enum Canvasname { Start, Credits, Settings, Tutorial }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ShowCanvas(Canvasname canvas)
        {
            StartScreenCanvas.SetActive(false);
            CreditsCanvas.SetActive(false);
            SettingsCanvas.SetActive(false);
            TutorialCanvas.SetActive(false);

            if (canvas == Canvasname.Start)
            {
                StartScreenCanvas.SetActive(true);
            }
            else if (canvas == Canvasname.Credits)
            {
                CreditsCanvas.SetActive(true);
            }
            else if (canvas == Canvasname.Settings)
            {
                SettingsCanvas.SetActive(true);
            }
            else if (canvas == Canvasname.Tutorial)
            {
                TutorialCanvas.SetActive(true);
            }
        }

        public void ShowCanvas(GameObject go)
        {
            StartScreenCanvas.SetActive(false);
            CreditsCanvas.SetActive(false);
            SettingsCanvas.SetActive(false);
            TutorialCanvas.SetActive(false);

            go.SetActive(true);
        }
    }
}