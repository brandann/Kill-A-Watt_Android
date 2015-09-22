using UnityEngine;
using System.Collections;

namespace Global
{
    public class TowerShock : Tower
    {
        // Use this for initialization
        void Awake()
        {
            Init();
        }

        override protected void Init()
        {
            neutralSprite = Resources.Load("Textures/Gear", typeof(Sprite)) as Sprite;
            player1Sprite = Resources.Load("Textures/Tower/ShockTowerYellow", typeof(Sprite)) as Sprite;
            player2Sprite = Resources.Load("Textures/Tower/ShockTowerBlue", typeof(Sprite)) as Sprite;
            player1SelectdSprite = Resources.Load("Textures/Tower/ShockTowerYellowSelected", typeof(Sprite)) as Sprite;
            player2SelectdSprite = Resources.Load("Textures/Tower/ShockTowerBlueSelected", typeof(Sprite)) as Sprite;
            MAXUNITS = 25;

            base.Init();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
