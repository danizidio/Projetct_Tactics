using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class BattleBehaviour : StateMachines
{
    public delegate float _onSettingAttackOrder(float spd);
    public static _onSettingAttackOrder OnSettingAttackOrder;

    public delegate void _onDead(GameObject g);
    public static _onDead Ondead;

    public static Action OnChangeTurn;

    [SerializeField] GameObject _damageFx;
    [SerializeField] Button _defenseButton;

    GameObject[] actors;

    [SerializeField] List<GameObject> _players;
    [SerializeField] List<GameObject> _enemies;
    public List<GameObject> Enemies { get { return _enemies; } }

    [SerializeField] List<GameObject> _orderAtk;
    public List<GameObject> OrderAtk { get { return _orderAtk; } set{ _orderAtk = value; } }

    [SerializeField] GameObject _atkPanel;
    [SerializeField] TMP_Text _txtTurn, _attackerTurn;

    [SerializeField] GameObject _actorTurn, _latterActor;

    [SerializeField] GameObject _txtList, _atkList;

    int _turns = 0;

    float _timer;

    private void Start()
    {
        OnNextBattleState?.Invoke(BattleStates.BEGINING);

        Ondead = OnDyingUnit;
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
                        actor.GetComponent<AtributesManager>().CharactersTurn(false);
                        _players.Add(actor);
                    }

                    actors = GameObject.FindGameObjectsWithTag("Enemy");

                    foreach (GameObject actor in actors)
                    {
                        _enemies.Add(actor);
                    }

                    _defenseButton.GetComponent<Button>().onClick.AddListener(Defender);

                    OnNextBattleState?.Invoke(BattleStates.SET_ATTACK_ORDER);

                    break;
                }
            case BattleStates.SET_ATTACK_ORDER:
                {
                    _atkPanel.SetActive(false);

                    if (_orderAtk.Count != 0)
                    {
                        _orderAtk.Clear();
                    }

                    foreach (GameObject player in _players)
                    {
                        _orderAtk.Add(player);
                    }

                    foreach (GameObject enemy in _enemies)
                    {
                        _orderAtk.Add(enemy);
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

                    _orderAtk = _orderAtk.OrderByDescending(e => e.GetComponent<AtributesManager>().GetSpeed()).ToList();

                    foreach (GameObject g in _orderAtk)
                    {
                        GameObject temp = Instantiate(_txtList, _atkList.transform);
                        temp.GetComponentInChildren<TMP_Text>().text = g.name;
                    }

                    OnNextBattleState?.Invoke(BattleStates.START);

                    break;
                }
            case BattleStates.START:
                {
                    _actorTurn = _orderAtk.First();
                    _actorTurn.GetComponent<AtributesManager>().CharactersTurn(true);
                    AttackerName(_actorTurn.GetComponent<AtributesManager>().GetName);

                    OnNextBattleState?.Invoke(BattleStates.ATTACK);

                    break;
                }
            case BattleStates.ATTACK:
                {
                    AttackerName(_actorTurn.GetComponent<AtributesManager>().GetName);

                    if (_orderAtk.Count <= 0)
                    {
                        OnNextBattleState?.Invoke(BattleStates.END_TURN);
                    }
                    else
                    {
                        if (_actorTurn.CompareTag("Enemy"))
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

                    _latterActor = _actorTurn;
                    _actorTurn = null;

                    if(_latterActor != null)
                    _latterActor.GetComponent<AtributesManager>().CharactersTurn(false);

                    _orderAtk = _orderAtk.OrderByDescending(e => e.GetComponent<AtributesManager>().GetSpeed()).ToList();
                    
                    foreach (GameObject g in _orderAtk)
                    {
                        GameObject temp = Instantiate(_txtList, _atkList.transform);
                        temp.GetComponentInChildren<TMP_Text>().text = g.name;
                    }

                    try
                    {
                        _actorTurn = _orderAtk.First();
                        _actorTurn.GetComponent<AtributesManager>().CharactersTurn(true);

                        StartCoroutine(WaitToCallNextState(BattleStates.ATTACK));
                    }
                    catch
                    {
                        OnNextBattleState?.Invoke(BattleStates.SET_ATTACK_ORDER);
                    }

                    break;
                }
            case BattleStates.END_TURN:
                {
                    if (_enemies.Count > 0 && _players.Count > 0)
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

                    MainMessage("Battle is Over!!");

                    if (Input.anyKeyDown)
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    }

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
        if (_orderAtk.Count > 0)
        {
            int damageOutput = _actorTurn.GetComponent<AtributesManager>().PlayerAtributes.Attack - actor.GetComponent<AtributesManager>().currentDef;

            actor.GetComponent<AtributesManager>().SufferDamage(
            _actorTurn.GetComponent<AtributesManager>().PlayerAtributes.Attack);

            GameObject temp = Instantiate(_damageFx, actor.transform.position, Quaternion.identity, actor.transform);
            temp.GetComponent<Canvas>().worldCamera = Camera.main;
            temp.GetComponent<DamageOutput>().SetDamage(damageOutput);

            _orderAtk.Remove(_orderAtk.First());

            StartCoroutine(WaitToCallNextState(BattleStates.NEXT_ATTACKER));
        }
        else
        {
            StartCoroutine(WaitToCallNextState(BattleStates.END_TURN));
        }
    }

    public void Defender()
    {
        if (_orderAtk.Count > 0)
        {
            _actorTurn.GetComponent<AtributesManager>().DefenseBoost();

            _orderAtk.Remove(_orderAtk.First());

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
        if (_orderAtk.Count > 0)
        {
            _timer += Time.deltaTime;

            if (_timer >= 1)
            {
                GameObject[] p = _players.ToArray();

                int i = UnityEngine.Random.Range(0, p.Length);

                int damageOutput = p[i].GetComponent<AtributesManager>().SufferDamage(_actorTurn.GetComponent<AtributesManager>().PlayerAtributes.Attack);

                GameObject temp = Instantiate(_damageFx, p[i].transform.position, Quaternion.identity, p[i].transform);
                temp.GetComponent<Canvas>().worldCamera = Camera.main;
                temp.GetComponent<DamageOutput>().SetDamage(damageOutput);

                _orderAtk.Remove(_actorTurn);

                _timer = 0;

                StartCoroutine(WaitToCallNextState(BattleStates.NEXT_ATTACKER));
            }
            else
            {
                StartCoroutine(WaitToCallNextState(BattleStates.END_TURN));
            }

        }
    }

    void OnDyingUnit(GameObject actor)
    {
        _orderAtk.Remove(actor);
        actor.SetActive(false);

        FindAllActors();
    }

    void FindAllActors()
    {
        _players.Clear();
        _enemies.Clear();

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

    void MainMessage(string message)
    {
        _attackerTurn.text = message;
    }

    void AttackerName(string s)
    {
        _attackerTurn.text = s + "'s turn!";
    }

    int NextTurn()
    {
        _turns++;

        _txtTurn.text = "TURN " + _turns;

        OnChangeTurn?.Invoke();

        return _turns;
    }

    IEnumerator WaitToCallNextState(BattleStates state)
    {
        yield return new WaitForSeconds(1);

        OnNextBattleState?.Invoke(state);

        StopCoroutine(WaitToCallNextState(state));
    }

    private void OnDisable()
    {
        OnChangeTurn = null;
    }
}
