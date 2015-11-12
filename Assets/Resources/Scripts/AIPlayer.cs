using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Global;

public class AIPlayer : MonoBehaviour {

    public Global.GameManager gameManager;

    private enum State { Idle, Sensing, Planning, Acting};
    private State _aiState;

    private float _lastMoveTime;
    private Coroutine _sensingRoutine;
    private Coroutine _planningRoutine;
    private Tower _weakestNeutral;
    

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
                if (null == _planningRoutine)
                {
                    _planningRoutine = StartCoroutine("TakeAction");
                    return;
                }
                else
                    return;




        }


    }

    IEnumerator DetermineGameState()
    {
        yield return new WaitForSeconds(1);
        _weakestNeutral = null;

        
        _sensingRoutine = null;
        List<Tower> neutralTowers = gameManager.GetNeutralTowers();

        if(0 == neutralTowers.Count)
        {
            _weakestNeutral = null;
            _aiState = State.Acting;
            StopCoroutine("DetermineGameState");

        }

        if(1 == neutralTowers.Count)
        {
            _weakestNeutral = neutralTowers[0];
            _aiState = State.Acting;
            StopCoroutine("DetermineGameState");
        }

        if(neutralTowers.Count > 1)
        {
            _weakestNeutral = neutralTowers[0];
            for(int i = 1; i < neutralTowers.Count; ++i)
            {
                if (neutralTowers[i].Units < _weakestNeutral.Units)
                    _weakestNeutral = neutralTowers[i];
            }

            _aiState = State.Acting;
            StopCoroutine("DetermineGameState");
        }

        

    }

    IEnumerator TakeAction()
    {
        yield return new WaitForSeconds(1);

    }
}
