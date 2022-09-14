using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtributesManager : MonoBehaviour
{
    [SerializeField] ClassAtributes _playerAtributes;
    public ClassAtributes PlayerAtributes { get { return _playerAtributes; } }

    [SerializeField] float _currentLife;

    [SerializeField] float _currentSpeed;
    [SerializeField] float _auxSpeed;

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

            _instantiatedInfo.GetComponent<LifeBar>().SettingInfos(PlayerAtributes.Life, _currentLife,
                GetName, PlayerAtributes.CharPortrait);
        }
        else
        {
            GameObject heroGrid = GameObject.FindGameObjectWithTag("EnemyGrid");

            _instantiatedInfo = Instantiate(_placeInfo, heroGrid.transform);

            _instantiatedInfo.GetComponent<LifeBar>().SettingInfos(PlayerAtributes.Life, _currentLife,
               GetName, PlayerAtributes.CharPortrait);
        }
    }

    public void SufferDamage(float damage)
    {
        float calcDamage = damage - PlayerAtributes.Defense;
        _currentLife -= calcDamage;

        _instantiatedInfo.GetComponent<LifeBar>().UpdateLifeBar(_currentLife, PlayerAtributes.Life);

        if (_currentLife <= 0)
        {
            this.gameObject.SetActive(false);

            BattleBehaviour.Ondead?.Invoke();
        }
    }
    
    public float GetSpeed()
    {
        return _currentSpeed;
    }
}
