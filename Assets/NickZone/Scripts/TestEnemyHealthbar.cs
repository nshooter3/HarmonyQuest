using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyHealthbar : MonoBehaviour
{
    public GameObject currentHealth;
    private Vector3 maxHealthBarScale;

    public Transform target;
    public Vector3 offset;

    private void Start()
    {
        maxHealthBarScale = currentHealth.transform.localScale;
    }

    private void Update()
    {
        transform.position = target.position + offset;
    }

    public void SetHealthBarSize(float health, float maxHealth)
    {
        float healthPercentage = (float)health / maxHealth;
        currentHealth.transform.localScale = new Vector3(healthPercentage * maxHealthBarScale.x, maxHealthBarScale.y, maxHealthBarScale.z);
    }
}
