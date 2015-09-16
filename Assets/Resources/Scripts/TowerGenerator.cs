using UnityEngine;
using System.Collections;

namespace Global
{
    public class TowerGenerator : Tower
    {
        // Use this for initialization
        void Awake()
        {
            Init();
        }

        override protected void Init()
        {
            neutralSprite = Resources.Load("Textures/Gear", typeof(Sprite)) as Sprite;
            player1Sprite = Resources.Load("Textures/Tower/GeneratorYellow", typeof(Sprite)) as Sprite;
            player2Sprite = Resources.Load("Textures/Tower/GeneratorBlue", typeof(Sprite)) as Sprite;
            player1SelectdSprite = Resources.Load("Textures/Tower/GeneratorYellowSelected", typeof(Sprite)) as Sprite;
            player2SelectdSprite = Resources.Load("Textures/Tower/GeneratorBlueSelected", typeof(Sprite)) as Sprite;

            base.Init();
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
