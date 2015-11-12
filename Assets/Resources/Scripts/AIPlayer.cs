using UnityEngine;
using System.Collections;

public class AIPlayer : MonoBehaviour {

    private enum State { Idle, Sensing, Planning, Acting};
    private State _aiState;

    private float _lastMoveTime;
    private Coroutine _sensingRoutine;
    public Global.GameManager gameManager;

    //Seconds between AI Move
    public float SecondsBetweenMoves = 5;
	// Use this for initialization
	void Start () {

        _lastMoveTime = Time.time;

	
	}
	
	// Update is called once per frame
	void Update () {

        switch (_aiState)
        {
            case State.Idle:
                if (Time.time > _lastMoveTime + SecondsBetweenMoves)
                {
                    _lastMoveTime = Time.time;
                    _aiState = State.Sensing;
                    return;
                }
                else
                    return;
            case State.Sensing:
                if (null == _sensingRoutine)
                {
                    _sensingRoutine = StartCoroutine("DetermineGameState");
                    return;

                }
                else
                    return;
            case State.Planning:
                break;



        }


    }

    IEnumerator DetermineGameState()
    {


    }
}
