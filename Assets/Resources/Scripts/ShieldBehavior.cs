using UnityEngine;
using System.Collections;
namespace Global{
    public class ShieldBehavior : MonoBehaviour {
		
        GameObject myDeathRay;
        LineRenderer line;
        private	float lifeTime = 20;
        private float startTime;
        DeathRay deathRay;
        bool color1 = true;
        bool color2;
        Transform secondShield;
		
        void Start () {
            if (this.name == "ShieldPlayer1(Clone)") {
                waitfordeathray("ShieldPlayer1(Clone)", "ShieldYellowTwo");
            } else {
                waitfordeathray("DeathRayPlayer2(Clone)", "ShieldBlueTwo");
            }
            
        }

        private bool waiting = true;

        IEnumerator waitfordeathray(string str, string shield)
        {
            while(myDeathRay == null)
            {
                myDeathRay = GameObject.Find ("DeathRayPlayer1(Clone)");
                yield return null;
            }

            secondShield = this.transform.FindChild ("ShieldYellowTwo");
            line = this.GetComponent<LineRenderer> ();
            setShield ();
            secondShield.GetComponent<SpriteRenderer> ().enabled = false;
            AudioManager audioManager = GameObject.Find ("Main Camera").GetComponent<AudioManager> ();
            audioManager.playShield();
            waiting = false;
        }
		
        // Update is called once per frame
        void Update () {
            if(waiting)
            {
                return;
            }
            secondShield.GetComponent<SpriteRenderer> ().enabled = false;
            if (Time.realtimeSinceStartup > startTime + lifeTime) {
                if(Network.isServer)
                    Network.Destroy(this.gameObject);
            }
            animateLineColor ();
            getMinions ();
        }
		
        private void getMinions(){
            if (deathRay.myOwner == ownerShip.Player1) {
                GameObject[] P2Array = GameObject.FindGameObjectsWithTag ("Player2Unit");
                destroyMinions(P2Array);
            }
            if (deathRay.myOwner == ownerShip.Player2) {
                GameObject[] P1Array = GameObject.FindGameObjectsWithTag ("Player1Unit");
                destroyMinions(P1Array);
            }
        }
		
        private void destroyMinions(GameObject[] minionArray){
          Vector3 from = this.transform.position;
          foreach (GameObject minion in minionArray) {
              Vector3 to = minion.transform.position;
              float dist = Vector3.Magnitude(from - to);
              if(Mathf.Abs(dist) < 3){
                  if (Network.isServer){
                      secondShield.GetComponent<SpriteRenderer> ().enabled = true;
                      minion.GetComponent<unitBehavior>().makeBurst();
                      Network.Destroy (minion);
                  }
              }
          }
        }
		
        private void setShield(){
            startTime = Time.realtimeSinceStartup;
            deathRay = myDeathRay.GetComponent<DeathRay> ();
            Vector3 spriteCenter =  deathRay.rayGunSprite.GetComponent<SpriteRenderer>().bounds.center;
            //Vector3 spriteCenter = new Vector3(0,0,0); // hack to make game work
            line.SetPosition (0, spriteCenter);
            line.SetPosition(1, new Vector3(transform.position.x, transform.position.y, -1));
            line.SetColors (Color.grey, Color.grey);
        }
		
        private void animateLineColor(){
            if (color1) {
                line.SetColors (Color.black, Color.black);
                color2 = true;
                color1 = false;
            } else if (color2) {
                line.SetColors (Color.grey, Color.grey);
                color1 = true;
                color2 = false;
            }
        }
    }
}