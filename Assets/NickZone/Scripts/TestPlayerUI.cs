using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestPlayerUI : MonoBehaviour
{
    [SerializeField]
    private GameObject healthBar, harmonyChargeBar;
    [SerializeField]
    private GameObject[] nextMultiplierProgressNodes;
    [SerializeField]
    private Text multiplier;

    private float maxHealthBarLength, maxHarmonyChargeBarLength;

    private void Start()
    {
        maxHealthBarLength = healthBar.transform.localScale.x;
        maxHarmonyChargeBarLength = harmonyChargeBar.transform.localScale.x;
    }

    public void SetHealthBar(int currentHealth, int maxHealth)
    {
        float healthBarLength = ((float) currentHealth) / ((float) maxHealth);
        healthBar.transform.localScale = new Vector3(healthBarLength, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }

    public void SetHarmonyModeBar(int currentHarmonyCharge, int maxHarmonyCharge)
    {
        float healthBarLength = ((float) currentHarmonyCharge) / ((float) maxHarmonyCharge);
        healthBar.transform.localScale = new Vector3(healthBarLength, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
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
}
