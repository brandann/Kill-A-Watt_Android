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
    private Coroutine _actingRoutine;
    private List<Tower> _attackers;
    private List<Tower> _targets;
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
                    _planningRoutine = StartCoroutine("CreatePlan");
                    return;
                }
                else
                    return;
            case State.Acting:
                if (null == _planningRoutine)
                {
                    _actingRoutine = StartCoroutine("TakeAction");
                    return;
                }
                else
                    return;
            default:
                Debug.LogError("Unhandled AI state");
                break;
        }


    }

    IEnumerator DetermineGameState()
    {
        yield return new WaitForSeconds(0.5f);
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

    IEnumerator CreatePlan()
    {
        _targets = new List<Tower>();
        _attackers = new List<Tower>();

        yield return new WaitForSeconds(0.5f);
        int numberAttackers = 2;
        int selcted = 0;

        List<Tower> myTowers = gameManager.GetPlayer2Towers();
        for (int i; i < myTowers.Count; ++i)
        {
            if (numberAttackers == selcted)
                break;

            if (myTowers[i].Units > 2)
            {
                _attackers.Add(myTowers[i]);
                selcted++;
            }
        }

        if (_weakestNeutral != null)
            _targets.Add(_weakestNeutral);

        if (_targets.Count == 0 || _attackers.Count == 0)
        {
            _aiState = State.Idle;
        }
        else
        {
            _aiState = State.Acting;
        }
    }

    IEnumerator TakeAction()
    {

        yield return new WaitForSeconds(1);
        for (int i = 0; i < _attackers.Count; ++i)
        {
            if (false == _attackers[i].Selected)
            {
                _attackers[i].ToggleSelect();
            }
        }

        gameManager.AttackToward(_weakestNeutral.transform.position, ePlayer.Player2);
        _aiState = State.Idle;


    }
}
