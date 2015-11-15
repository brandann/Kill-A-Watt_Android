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
    private Tower _weakestPlayer1;
    

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
                if (null == _actingRoutine)
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
        _weakestNeutral = null;
        _weakestPlayer1 = null;

        
        _sensingRoutine = null;
        List<Tower> neutralTowers = gameManager.GetTowersByPlayer(ePlayer.Neutral);
        List<Tower> player1Towers = gameManager.GetTowersByPlayer(ePlayer.Player1);

        _weakestNeutral = FindWeakest(neutralTowers);
        _weakestPlayer1 = FindWeakest(player1Towers);
        
        _aiState = State.Planning;   

        _planningRoutine = null;
        yield break;    

    }

    private Tower FindWeakest(List<Tower> towers)
    {
        if(null == towers || 0 == towers.Count)
            return null;

        Tower weakest = towers[0];
        for (int i = 1; i < towers.Count; ++i)
        {
            if (towers[i].Units < weakest.Units)
                weakest = towers[i];
        }

        return weakest;
    }

    IEnumerator CreatePlan()
    {
        _targets = new List<Tower>();
        _attackers = new List<Tower>();
        
        int numberAttackers = 3;
        int selcted = 0;

        List<Tower> myTowers = gameManager.GetTowersByPlayer(ePlayer.Player2);                
        

        if (_weakestNeutral != null)
        {
            _targets.Add(_weakestNeutral);
        }
        else
        {
            if (_weakestPlayer1 != null)
                _targets.Add(_weakestPlayer1);

        }

        if( 0 == _targets.Count)
        {
            _aiState = State.Idle;
            yield break;

        }

        for (int i = 0; i < myTowers.Count; ++i)
        {
            if (numberAttackers == selcted)
                break;

            if (myTowers[i].Units > 2)
            {
                _attackers.Add(myTowers[i]);
                selcted++;
            }
        }

        if (0 == _attackers.Count)
        {
            _aiState = State.Idle;
            yield break;

        }
        else
        {
            _aiState = State.Acting;
        }

        yield break;
    }

    IEnumerator TakeAction()
    {

        
        if(null == _attackers)
        {
            _aiState = State.Idle;
            _actingRoutine = null;
            StopCoroutine("TakeAction");
        }

        for (int i = 0; i < _attackers.Count; ++i)
        {
            if (false == _attackers[i].Selected)
            {
                _attackers[i].ToggleSelect();
            }
        }

        if(0 < _targets.Count )
        {
            gameManager.AttackToward(_targets[0].transform.position, ePlayer.Player2);

        }
        
        _aiState = State.Idle;
        _actingRoutine = null;
        yield break;


    }
}
