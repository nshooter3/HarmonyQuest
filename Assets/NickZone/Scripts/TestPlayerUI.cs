using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestPlayerUI : MonoBehaviour
{
    [SerializeField]
    private GameObject healthBar, harmonyChargeBar, harmonyModeBar, normalModeUI, harmonyModeUI;
    [SerializeField]
    private GameObject[] nextMultiplierProgressNodes;
    [SerializeField]
    private Text multiplier, healingItems;
    [SerializeField]
    private Image multiplierBackground;

    private bool isInHarmonyMode = false;

    private float maxHealthBarLength, maxHarmonyChargeBarLength, maxHarmonyModeBarLength;

    private void Start()
    {
        maxHealthBarLength = healthBar.transform.localScale.x;
        maxHarmonyChargeBarLength = harmonyChargeBar.transform.localScale.x;
        maxHarmonyModeBarLength = harmonyModeBar.transform.localScale.x;
    }

    public void SetHealthBar(int currentHealth, int maxHealth)
    {
        float healthBarLength = maxHealthBarLength * ((float) currentHealth) / ((float) maxHealth);
        healthBar.transform.localScale = new Vector3(healthBarLength, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }

    public void SetHealingItems(int currentHealingItems)
    {
        healingItems.text = "" + currentHealingItems;
    }

    public void SetHarmonyChargeBar(float currentHarmonyCharge, float maxHarmonyCharge)
    {
        float harmonyChargeBarLength = maxHarmonyChargeBarLength * currentHarmonyCharge / maxHarmonyCharge;
        harmonyChargeBar.transform.localScale = new Vector3(harmonyChargeBarLength, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
        SetHarmonyModeBar(currentHarmonyCharge, maxHarmonyCharge);
    }

    private void SetHarmonyModeBar(float currentHarmonyModeDuration, float maxHarmonyModeDuration)
    {
        float harmonyModeDurationBarLength = maxHarmonyModeBarLength * currentHarmonyModeDuration / maxHarmonyModeDuration;
        harmonyModeBar.transform.localScale = new Vector3(harmonyModeDurationBarLength, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }

    public void SetMultiplierProgress(int multiplierValue, int nextMultiplierProgress)
    {
        multiplier.text = multiplierValue + "x";
        for (int i = 0; i < nextMultiplierProgressNodes.Length; i++)
        {
            bool isNodeActive = i + 1 <= nextMultiplierProgress;
            nextMultiplierProgressNodes[i].SetActive(isNodeActive);
        }
    }

    public void ToggleHarmonyMode(bool isHarmonyModeOn)
    {
        isInHarmonyMode = isHarmonyModeOn;
        normalModeUI.SetActive(!isHarmonyModeOn);
        harmonyModeUI.SetActive(isHarmonyModeOn);
        if (isHarmonyModeOn)
        {
            multiplierBackground.color = Color.cyan;
            multiplier.color = Color.blue;
        }
        else
        {
            multiplierBackground.color = Color.white;
            multiplier.color = Color.black;
        }
    }
}
