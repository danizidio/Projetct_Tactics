using System.Collections;
using UnityEngine;

public class AtributesManager : MonoBehaviour
{
    [SerializeField] ClassAtributes _playerAtributes;
    public ClassAtributes PlayerAtributes { get { return _playerAtributes; } }

    [SerializeField] float _currentLife;

    [SerializeField] float _currentSpeed;
    public float currentSpeed { get { return _playerAtributes.Speed + _auxSpeed; } }

    float _auxSpeed;

    public int currentDef { get { return _playerAtributes.Defense + _auxDef; } }

    int _auxDef;


    [SerializeField] GameObject _placeInfo;

    GameObject _instantiatedInfo;

    public string GetName { get { return _playerAtributes.CharName; } }

    private void Start()
    {
        _currentLife = PlayerAtributes.Life;

        _currentSpeed = _auxSpeed + PlayerAtributes.Speed;

        if(this.gameObject.CompareTag("Player"))
        {
            GameObject heroGrid = GameObject.FindGameObjectWithTag("PlayerGrid");

            _instantiatedInfo = Instantiate(_placeInfo, heroGrid.transform);

            _instantiatedInfo.GetComponent<LifeBar>().SettingInfos(PlayerAtributes.Life, _currentLife, currentDef, PlayerAtributes.Speed,
                                                                                                   PlayerAtributes.Attack, GetName, PlayerAtributes.CharPortrait);
        }
        else
        {
            GameObject heroGrid = GameObject.FindGameObjectWithTag("EnemyGrid");

            _instantiatedInfo = Instantiate(_placeInfo, heroGrid.transform);

            _instantiatedInfo.GetComponent<LifeBar>().SettingInfos(PlayerAtributes.Life, _currentLife, currentDef, PlayerAtributes.Speed, 
                                                                                                    PlayerAtributes.Attack, GetName, PlayerAtributes.CharPortrait);
        }
    }

    public void CharactersTurn(bool b)
    {
        _instantiatedInfo.GetComponent<LifeBar>().ActiveTurn(b);
    }

    public int SufferDamage(float damage)
    {
        int calcDamage = (int)damage - currentDef;
        if(calcDamage < 0) calcDamage = 0;
        _currentLife -= calcDamage;

        if (_currentLife <= 0)
        {
            _currentLife = 0;

            StartCoroutine(EnemyDied());
        }

        _instantiatedInfo.GetComponent<LifeBar>().UpdateLifeBar(_currentLife, PlayerAtributes.Life);

        return calcDamage;
    }
    
    public void DefenseBoost()
    {
        _auxDef = PlayerAtributes.Defense / 2;

        _instantiatedInfo.GetComponent<LifeBar>().UpdateToolTip(currentDef, currentSpeed, PlayerAtributes.Attack);
        _instantiatedInfo.GetComponent<LifeBar>().DefIconEnabled(true);
    }

    void RemoveOnturnBuffs()
    {
        _auxDef = 0;
        _auxSpeed = 0;

        _instantiatedInfo.GetComponent<LifeBar>().UpdateToolTip(currentDef, currentSpeed, PlayerAtributes.Attack);
        _instantiatedInfo.GetComponent<LifeBar>().DefIconEnabled(false);
    }

    public float GetSpeed()
    {
        return _currentSpeed;
    }

    private void OnEnable()
    {
        BattleBehaviour.OnChangeTurn += RemoveOnturnBuffs;
    }

    IEnumerator EnemyDied()
    {
        yield return new WaitForSeconds(1);

        BattleBehaviour.Ondead?.Invoke(this.gameObject);
//        this.gameObject.SetActive(false);

        StopCoroutine(EnemyDied());
    }
}
