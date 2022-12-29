using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;
using System.Linq;
using TMPro;

public class BattleBehaviour : StateMachines
{
    public delegate float _onSettingAttackOrder(float spd);
    public static _onSettingAttackOrder OnSettingAttackOrder;

    public delegate void _onDead();
    public static _onDead Ondead;

    [SerializeField] GameObject _slashFx;

    GameObject[] actors;

    [SerializeField] List<GameObject> _players;
    [SerializeField] List<GameObject> _enemies;
    public List<GameObject> Enemies { get { return _enemies; } }

    [SerializeField] List<GameObject> orderAtk;

    [SerializeField] GameObject _atkPanel;
    [SerializeField] TMP_Text _txtTurn, _attackerTurn;

    [SerializeField] GameObject _txtList, _atkList;

    int _turns = 0;

    float _timer;

    private void Start()
    {
        OnNextBattleState?.Invoke(BattleStates.BEGINING);

        Ondead = FindAllActors;
    }

    private void Update()
    {
        BattleSequence();

        CurrentBattleState = BattleNextState;
    }

    void BattleSequence()
    {
        switch (CurrentBattleState)
        {
            case BattleStates.BEGINING:
                {
                    _atkPanel.SetActive(false);

                    actors = GameObject.FindGameObjectsWithTag("Player");

                    foreach (GameObject actor in actors)
                    {
                        _players.Add(actor);
                    }

                    actors = GameObject.FindGameObjectsWithTag("Enemy");

                    foreach (GameObject actor in actors)
                    {
                        _enemies.Add(actor);
                    }

                    OnNextBattleState?.Invoke(BattleStates.SET_ATTACK_ORDER);

                    break;
                }
            case BattleStates.SET_ATTACK_ORDER:
                {
                    _atkPanel.SetActive(false);

                    if (orderAtk.Count != 0)
                    {
                        orderAtk.Clear();
                    }

                    foreach (GameObject player in _players)
                    {
                        orderAtk.Add(player);
                    }

                    foreach (GameObject enemy in _enemies)
                    {
                        orderAtk.Add(enemy);
                    }

                    GameObject[] txtObjs = GameObject.FindGameObjectsWithTag("txtOrder");

                    if (txtObjs != null)
                    {
                        foreach (var item in txtObjs)
                        {
                            Destroy(item);
                        }
                    }

                    NextTurn();

                    orderAtk = orderAtk.OrderByDescending(e => e.GetComponent<AtributesManager>().GetSpeed()).ToList();

                    foreach (GameObject g in orderAtk)
                    {
                        GameObject temp = Instantiate(_txtList, _atkList.transform);
                        temp.GetComponentInChildren<TMP_Text>().text = g.name;
                    }

                    OnNextBattleState?.Invoke(BattleStates.START);

                    break;
                }
            case BattleStates.START:
                {
                    AttackerName(orderAtk.First().GetComponent<AtributesManager>().GetName);

                    OnNextBattleState?.Invoke(BattleStates.ATTACK);

                    break;
                }
            case BattleStates.ATTACK:
                {
                    if (orderAtk.Count <= 0)
                    {
                        OnNextBattleState?.Invoke(BattleStates.END_TURN);
                    }
                    else
                    {
                        AttackerName(orderAtk.First().GetComponent<AtributesManager>().GetName);

                        if (orderAtk.First().gameObject.CompareTag("Enemy"))
                        {
                            _atkPanel.SetActive(false);

                            EnemyAttack();
                        }
                        else
                        {
                            _atkPanel.SetActive(true);
                        }
                    }

                    break;
                }
            case BattleStates.NEXT_ATTACKER:
                {

                    GameObject[] txtObjs = GameObject.FindGameObjectsWithTag("txtOrder");

                    if (txtObjs != null)
                    {
                        foreach (var item in txtObjs)
                        {
                            Destroy(item);
                        }
                    }

                    orderAtk = orderAtk.OrderByDescending(e => e.GetComponent<AtributesManager>().GetSpeed()).ToList();

                    foreach (GameObject g in orderAtk)
                    {
                        GameObject temp = Instantiate(_txtList, _atkList.transform);
                        temp.GetComponentInChildren<TMP_Text>().text = g.name;
                    }

                    StartCoroutine(WaitToCallNextState(BattleStates.ATTACK));

                    break;
                }
            case BattleStates.END_TURN:
                {
                    if(_enemies.Count > 0 && _players.Count > 0)
                    {
                        if (Input.anyKeyDown)
                        {
                            OnNextBattleState?.Invoke(BattleStates.SET_ATTACK_ORDER);
                        }
                    }
                    else
                    {
                        OnNextBattleState?.Invoke(BattleStates.FINISHING_BATTLE);
                    }

                    break;
                }
            case BattleStates.FINISHING_BATTLE:
                {
                    _atkPanel.SetActive(false);
                    
                    AttackerName("Battle is Over!!");

                    OnNextBattleState?.Invoke(BattleStates.FINISHING_BATTLE);

                    break;
                }
        }       
    }


    #region -- Button Events
    public void RunFromBattle()
    {
        OnNextBattleState?.Invoke(BattleStates.FINISHING_BATTLE);
    }

    public void Attacker(GameObject actor)
    {
        if (orderAtk.Count > 0)
        {
            AttackerName(orderAtk.First().GetComponent<AtributesManager>().GetName);

            actor.GetComponent<AtributesManager>().SufferDamage(
            orderAtk.First().gameObject.GetComponent<AtributesManager>().PlayerAtributes.Attack);

            Instantiate(_slashFx, actor.transform);

            orderAtk.Remove(orderAtk.First());

            StartCoroutine(WaitToCallNextState(BattleStates.NEXT_ATTACKER));
        }
        else
        {
            StartCoroutine(WaitToCallNextState(BattleStates.END_TURN));
        }
    }

    #endregion

    public void EnemyAttack()
    {
        if (orderAtk.Count > 0)
        {
            AttackerName(orderAtk.First().GetComponent<AtributesManager>().GetName);

            ChoosePlayerToAttack();
        }
        else
        {
            StartCoroutine(WaitToCallNextState(BattleStates.END_TURN));
        }
    }
    void AttackerName(string s)
    {
        _attackerTurn.text = s;
    }

    int NextTurn()
    {
        _turns++;

        _txtTurn.text = "TURN " + _turns;

        return _turns;
    }

    IEnumerator WaitToCallNextState(BattleStates state)
    {
        yield return new WaitForSeconds(1);

        OnNextBattleState?.Invoke(state);

        StopCoroutine(WaitToCallNextState(state));
    }

    void ChoosePlayerToAttack()
    {
        _timer += Time.deltaTime;

        if(_timer >= 1)
        {
            GameObject[] p = _players.ToArray();

            int i = Random.Range(0, p.Length);

            p[i].GetComponent<AtributesManager>().SufferDamage(
            orderAtk.First().gameObject.GetComponent<AtributesManager>().PlayerAtributes.Attack);

            Instantiate(_slashFx, p[i].transform);

            orderAtk.Remove(orderAtk.First());

            _timer = 0;

            StartCoroutine(WaitToCallNextState(BattleStates.NEXT_ATTACKER));
        }    
    }

    void FindAllActors()
    {
        _players.Clear();
        Enemies.Clear();

        actors = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject actor in actors)
        {
            _players.Add(actor);
        }

        actors = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject actor in actors)
        {
            _enemies.Add(actor);
        }

        if(_players.Count == 0 || _enemies.Count == 0)
        {
            OnNextBattleState(BattleStates.FINISHING_BATTLE);
        }

    }
}
