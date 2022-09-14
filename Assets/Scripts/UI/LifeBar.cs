using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TimeCounter;
using TMPro;

public class LifeBar : MonoBehaviour
{
    [SerializeField] Image _lifeBar;

    [SerializeField] Image _portrait;

    [SerializeField] float maxTime;

    [SerializeField] TMP_Text _heroName, _lifeValues;

    Timer t;


    void Start()
    {
        t = GetComponent<Timer>();
    }

    private void LateUpdate()
    {
        if (maxTime > 0)
        {
            t.CountDown();
        }
    }

    public void StayActive()
    {
        t.GetComponent<Timer>().SetTimer(maxTime, () => this.gameObject.SetActive(false));
    }

    public void SettingInfos(float maxLifeValue, float currentLifeValue, string heroName, Sprite heroPortrait)
    {
        _portrait.sprite = heroPortrait;

        _lifeBar.fillAmount = currentLifeValue / maxLifeValue;

        _heroName.text = heroName;

        _lifeValues.text = currentLifeValue + " / " + maxLifeValue;

    }

    public void UpdateLifeBar(float currentLife, float maxLife)
    {
        if (currentLife < 0)
        {
            _lifeBar.fillAmount = 0;
        }

        _lifeValues.text = currentLife + " / " + maxLife;

        float value = currentLife / maxLife;

        _lifeBar.fillAmount = value;
    }
}
