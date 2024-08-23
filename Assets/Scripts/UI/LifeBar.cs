using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using TimeCounter;
using TMPro;

public class LifeBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] bool _isActive;

    [SerializeField] Image _lifeBar;

    [SerializeField] Image _portrait;

    [SerializeField] Image _innerFrame;

    [SerializeField] Image _shieldIcon;

    [SerializeField] Color _inactiveColor;

    [SerializeField] Color _activeColor;

    [SerializeField] float maxTime;

    [SerializeField] TMP_Text _heroName, _lifeValues, _statsTxt;

    [SerializeField] GameObject _heroStats;

    Timer t;

    void Start()
    {
        t = GetComponent<Timer>();

        DefIconEnabled(false);

        _heroStats.SetActive(false);
    }

    private void LateUpdate()
    {
        if (maxTime > 0)
        {
            t.CountDown();
        }
    }

    public void DefIconEnabled(bool b)
    {
        _shieldIcon.enabled = b;
    }


    public void StayActive()
    {
        t.GetComponent<Timer>().SetTimer(maxTime, () => this.gameObject.SetActive(false));
    }

    public bool ActiveTurn(bool b)
    {
        if(b)
        {
            _innerFrame.color = _activeColor;
        }
        else
        {
            _innerFrame.color = _inactiveColor;
        }

        return _isActive = b;
    }

    public void SettingInfos(float maxLifeValue, float currentLifeValue, float defenseValue, float speedValue, int attackValue, string heroName, Sprite heroPortrait)
    {
        _portrait.sprite = heroPortrait;

        _lifeBar.fillAmount = currentLifeValue / maxLifeValue;

        _heroName.text = heroName;

        _lifeValues.text = currentLifeValue + " / " + maxLifeValue;

        _innerFrame.color = _inactiveColor;

        UpdateToolTip(defenseValue, speedValue, attackValue);
    }

    public void UpdateToolTip(float defenseValue, float speedValue, int attackValue)
    {
        _statsTxt.text = "Stats \n" +
                          "Attack:    " + attackValue + "\n" +
                          "Defense: " + defenseValue + "\n" +
                          "Speed:    " + speedValue;
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        _heroStats.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _heroStats.SetActive(false);
    }
}
