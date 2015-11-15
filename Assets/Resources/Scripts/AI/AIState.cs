using UnityEngine;
using System.Collections;

public abstract class AIState : MonoBehaviour {

    private GameObject _owner;

    public abstract void StateUpdate();

    public abstract void StateStart();

    public abstract void StateEnd();

    public virtual void SetOwner(GameObject owner)
    {
        _owner = owner;
    }

    
  
        
}
