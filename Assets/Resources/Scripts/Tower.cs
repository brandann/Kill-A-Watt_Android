using UnityEngine;
using System.Collections;
namespace Global{

    public delegate void ChangedOwner(Tower t, ePlayer switchedFromPlayer, ePlayer switchedToPlayer);

    public class Tower : MonoBehaviour
    {       
        
        public static event ChangedOwner OnOwnerChange;

        

        #region Other GameObjects
        private GameManager Manager;
        private Camera sceneCam; //Needed to draw GUI labels centered in world cordinates
        private SpriteRenderer myRender;
        private Transform myPlusTen; 
        #endregion

        #region Tower Properties
        public bool _visited;
        public ePlayer myOwner; //Player this tower belongs go
        private int units; //Number of garrisoned units should be set at runtime
        private bool selected = false;
        public GUIStyle GUIplayer1;
        public GUIStyle GUIplayer2;
        public GUIStyle GUIneutral;
        private bool magnetized = false;
        #endregion
        
        #region Unit Variables
        protected float MAXUNITS = 0;
        private GameObject Player1UnitPrefab = null;
        private GameObject Player2UnitPrefab = null;
        private float lastUnitGeneratedTime = 0;
        private float unitSpawnRate = .5f; //Time between unit spawns in seconds

        public float percentOfUnitsPerAttack;                                   
        public float unitIncrementRate;
        public int attackedDamage;
        
        
        //Used to add particle like unit spawning
        private int randIntervalMin;
        private int randIntervalMax;
        private float randIntervalNorm;
        private Sprite[] blinkSprites;
        private int blinkCount;

        public GameObject SelectionGO;
        private SpriteRenderer Selection;

        public GameObject UpgradeBtnObject;
        private bool UpgradeActive = false;
        
        private int unitsToSend; //Units left to send from last attack
        public Quaternion destination;
        
        #endregion

        AudioManager audioManager;

        #region Sprites 
        protected Sprite neutralSprite = null,
                        player1Sprite = null,
                        player2Sprite = null,
                        player1SelectdSprite = null,
                        player2SelectdSprite = null,
                        ShockRogueSprite,
                        TowerRogueSprite;
        #endregion	

        void Awake () {
            Init();
        }

        virtual protected void Init()
        {
            audioManager = GameObject.Find("Main Camera").GetComponent<AudioManager>();
            _visited = false;
            Manager = GameObject.Find("Main Camera").GetComponent<GameManager>();
            GameObject go = GameObject.FindGameObjectWithTag("MainCamera");
            sceneCam = go.GetComponent<Camera>();
            Player1UnitPrefab = Resources.Load("Prefabs/Player1Unit") as GameObject;
            Player2UnitPrefab = Resources.Load("Prefabs/Player2Unit") as GameObject;
            ShockRogueSprite = Resources.Load("Textures/Tower/ShockTowerRogue", typeof(Sprite)) as Sprite;
            TowerRogueSprite = Resources.Load("Textures/Tower/GeneratorRogue", typeof(Sprite)) as Sprite;
            blinkSprites = new Sprite[3];
            myRender = (SpriteRenderer)GetComponent<Renderer>();
            myPlusTen = transform.FindChild("TenPlusPlus");

            blinkSprites[0] = player1Sprite;
            blinkSprites[1] = player2Sprite;
            blinkSprites[2] = TowerRogueSprite;
            Selection = SelectionGO.GetComponent<SpriteRenderer>();
        }
    
        //-----------------------------------------------------------------------------------------------------------------------------------------

        void FixedUpdate()
        {
            //Increment garrisoned units on a constant interval
            if ((Time.realtimeSinceStartup - lastUnitGeneratedTime) > unitIncrementRate)
            {   
                if(myOwner != ePlayer.Neutral){
                    if(units < MAXUNITS)
                    units++;
                }
                lastUnitGeneratedTime = Time.realtimeSinceStartup;
            }

            if (!UpgradeActive && Units >= MAXUNITS)
            {
                if (myOwner == ePlayer.Player1)
                {
                    UpgradeBtnObject.SetActive(true);
                }
                UpgradeActive = true;
            }
            else if (UpgradeActive && Units < MAXUNITS)
            {
                UpgradeBtnObject.SetActive(false);
                UpgradeActive = false;
            }
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------

        //Left mouse must go down and up on same collider to call this; used to toggle tower selection
        void OnMouseUpAsButton()
        {
            if (myOwner == ePlayer.Player1)
            {
                ToggleSelect();
            }
            else
            {
                Manager.AttackToward(transform.position, ePlayer.Player1);
            } 
        }
       
        public void ToggleSelect()
        {
            // cannot select tower while magnetized
            if (magnetized)
            {
                return;
            }

            // toggle the selection
            selected = (selected == true) ? false : true;

            // only update the sprite if the player is player 1
            // player 2 and neutural do not show selections
            if (myOwner == ePlayer.Player1)
            {
                updateSprite();
            }
        }
        
        //-------------------------------------------------------------------------------------------------------------------------------------------------
        #region Magnetize
        public void Magnetize()
        {
            magnetized = true;
        }
        //-----------------------------------------------------------------------------------------------------------------------------------------------

        public void DeMagnetize()
        {
            magnetized = false;
        }

        public void SubUnit()
        {
            //print("subUnit");
            if (units > 0)
            {
                units--;
            }
        }

        public void Blink()
        {
            blinkCount++;
            myRender.sprite = blinkSprites[blinkCount % 3];
        }

        #endregion

        #region SelectionSprite

        public void updateSprite()
        {
            if (selected)
            {
                Selection.enabled = true;
                return;
            }
            Selection.enabled = false;
        }
        #endregion
		
        #region attack
		
        /// <summary>
        /// Spawns units from this tower and sends them toward the target position. This doesn't actually have to be an attack as reinforcements work the same way.
        /// This coroutine waits for unitSpawnRate seconds between spawning units. The number to spawn is determined by the number of units in the tower when called
        /// multiplied by percentOfUnitsPerAttack
        /// </summary>
        /// <param name="targetPos">Position to send units toward</param>
        /// <returns></returns>
        public IEnumerator SpawnAttack(Vector3 targetPos)
        {
            //Debug.Log("spawn Attack, selected: " + selected);

            GameObject prefabToSpawn;
            //Select the proper unit prefab to spawn
            prefabToSpawn = (myOwner == ePlayer.Player1) ? Player1UnitPrefab : Player2UnitPrefab;

            //Calculate the point at which the units should spawn (just outside the tower in the proper direction)
            Vector3 vecToTarget = targetPos - transform.position;                                 //line between source and target
            Vector3 spawnPoint = vecToTarget;                                                     //a point on the line to target just outside the collider of the tower and unit
            spawnPoint.Normalize();
            spawnPoint *= (transform.localScale.x + prefabToSpawn.transform.localScale.x);        //could be more efficient math here but this is what I came up with to scale         
            spawnPoint *= .5f;
            spawnPoint = new Vector3(spawnPoint.x + transform.position.x, spawnPoint.y + transform.position.y, 0);   //then translate

            int unitsToSend = (int)(units * percentOfUnitsPerAttack);                            //Calculate the number of units to spawn
            
            ePlayer myOwnerWhenStarted = myOwner;                                              //Ownership could change while SpawnAttack is sleeping           
           
            //Keep sending till all units are sent or the tower runs out of units or switches sides
            while (unitsToSend > 0 && units > unitsToSend && myOwner == myOwnerWhenStarted)
            {
                //Create a unit and decrement
                GameObject go = (GameObject)GameObject.Instantiate(prefabToSpawn, spawnPoint, Quaternion.LookRotation(Vector3.forward, vecToTarget));
                unitBehavior spawnedUnit = go.GetComponent<unitBehavior>(); //set the owner of the new unit
                spawnedUnit.destination = targetPos;
                spawnedUnit.myOwner = myOwner;
                unitsToSend--;
                units--;                
                yield return new WaitForSeconds(unitSpawnRate); //wait to spawn another
            }	
        }
        #endregion
      
        void OnTriggerEnter2D(Collider2D other) 
        {
            if (null == other || other.gameObject.tag.Contains("Untagged"))
            {
                return;
            }

            //Flip owner if hit by magnet ability
            if (other.gameObject.tag.Contains("Magnet"))
                return;

            //Used to check the destination of units; Units spawned by the magnet have no destination and should collide with any tower
            Vector3 target = other.gameObject.GetComponent<unitBehavior>().destination;
            bool isRogue = other.gameObject.GetComponent<unitBehavior>().rogue;

            if (!isRogue)
            {
                Vector3 here = this.transform.position;
                Vector3 distance = target - here;
                if (distance.magnitude > 2)
                    return;
            }
            ePlayer otherOwner = other.gameObject.GetComponent<unitBehavior>().myOwner;

            if (myOwner == otherOwner && otherOwner != ePlayer.Neutral)                
                units++;
            else
            {
                units -= attackedDamage;
                if (units < 0)  //Can happen when multiple units hit at same time; might watnt to use math.clamp
                    units = 0;
                if (units == 0 && otherOwner != ePlayer.Neutral)  //Switch control when all units are lost
                    SwitchOwner(otherOwner);
                other.gameObject.GetComponent<unitBehavior>().makeBurst();
            }

            if(other.gameObject.tag.Contains("Unit")){
                
                GameObject.Destroy(other.gameObject);
            }
        }
        
        //----------------------------------------------------------------------------------------------------------------------------------------------
        bool playsound = false;
        public void SwitchOwner(ePlayer switchTo)
        {
            lastUnitGeneratedTime = Time.realtimeSinceStartup;

            //Notifiy listenter that tower has changed
            if (null != OnOwnerChange)
                OnOwnerChange(this, myOwner, switchTo);

            //Selection can't carry over when tower switches owner
            selected = false;

            myOwner = switchTo;
            switch (switchTo)
            {
            case ePlayer.Neutral:
                myRender.sprite = neutralSprite;
                break;
            case ePlayer.Player1:
                if(playsound) 
                    audioManager.playTowerTakeover1();
                myRender.sprite = player1Sprite;
                break;
            case ePlayer.Player2:
                if(playsound) 
                    audioManager.playTowerTakeover2();
                myRender.sprite = player2Sprite;
                break;
            default:
                Debug.LogError("Switching to invalid owner type");
                break;
            }
            updateSprite();

            // this should prevent the sounds from playing at the begining.
            playsound = true;

            
        }

        //--------------------------------------------------------------------------------------------------------------------------------------
        #region EnergyGenAnimation

        /// <summary>
        /// Called from the server's gamemanager to start the plus ten energy animation
        /// </summary>
        public void PlayPlusTen()
        {
            //print("called plus ten");
            myPlusTen.GetComponent<Animator>().enabled = true;
            myPlusTen.GetComponent<SpriteRenderer>().enabled = true;
            StartCoroutine(turnOffAnimation());
        }
        IEnumerator turnOffAnimation()
        {
            yield return new WaitForSeconds(2f);
            myPlusTen.GetComponent<Animator>().enabled = false;
            myPlusTen.GetComponent<SpriteRenderer>().enabled = false;
        }
        #endregion

        //-------------------------------------------------------------------------------------------
        //Display # of garrisoned units above the tower
        void OnGUI()
        {
            Vector3 screenPos = sceneCam.WorldToScreenPoint(transform.position);
            switch (myOwner)
            {
                case (ePlayer.Neutral):
                    GUI.contentColor = Color.grey;
                    GUI.Label(new Rect(screenPos.x - 9, sceneCam.pixelHeight - screenPos.y - 70, 50, 75),  units.ToString(),GUIneutral);
                    break;
                case (ePlayer.Player1):
                    // GUI.contentColor = Color.yellow;
                    GUI.Label(new Rect(screenPos.x - 9, sceneCam.pixelHeight - screenPos.y - 70, 50, 75),  units.ToString(),GUIplayer1);
                    break;
                case (ePlayer.Player2):
                    //  GUI.contentColor = Color.magenta;
                    GUI.Label(new Rect(screenPos.x - 9, sceneCam.pixelHeight - screenPos.y - 70, 50, 75),  units.ToString(),GUIplayer2);
                    break;
            }
        }

        public bool Visited
        {
            get{return _visited;}
            set{_visited = value;}
        }

        public int Units
        {
            get { return units; }
            set { units = value; }
        }

        public bool Selected
        {
            get { return selected; }
            private set { selected = value; }
        }
    }
}