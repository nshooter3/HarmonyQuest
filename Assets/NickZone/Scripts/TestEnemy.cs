using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    public int maxHealth;
    public int health;
    public int attackDamage = 10;

    [SerializeField]
    private GameObject attackBox;
    [SerializeField]
    private ParticleSystem getHitParticles;
    [SerializeField]
    private AudioSource metronomeSound, getHitSound;

    [SerializeField]
    private GameObject healthBar;
    private Vector3 maxHealthBarScale;

    //[SerializeField]
    private Material enemyMat;

    //Manually make the enemy act on beat by using the duration of our 16th note at 120 BPM as an update timer.
    public float sixteenthNoteDuration = 0.125f;
    private int sixteenthNoteCount = 1;
    //The enemy's logic cycle for now simply lasts 16 16th notes, which equals 4 beats, which equals 1 measure. Music, yeah!
    private int maxSixteenthNoteCount = 16;

    private float timeUntilNextSixteenthNote = 0;

    private float attackTimer, maxAttackTimer = 0.4f;

    //Change color to telegraph attacks for now
    public Color windUpColor, preAttackColor;
    public Color defaultColor;

    float beatCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        maxHealthBarScale = healthBar.transform.localScale;
        health = maxHealth;
        enemyMat = GetComponent<Renderer>().material;
        SixteenthNoteUpdate();
        timeUntilNextSixteenthNote = sixteenthNoteDuration;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimers();

        timeUntilNextSixteenthNote -= Time.deltaTime;
        if (timeUntilNextSixteenthNote <= 0)
        {
            timeUntilNextSixteenthNote = sixteenthNoteDuration;
            //Once we pass our max 16th note count, reset to 1 instead of 0.
            sixteenthNoteCount = Mathf.Max((sixteenthNoteCount + 1) % (maxSixteenthNoteCount + 1), 1);
            SixteenthNoteUpdate();
        }

        beatCount += Time.deltaTime;

        /*(if (WasAttackedOnBeat())
        {
            enemyMat.color = windUpColor;
        }
        else
        {
            enemyMat.color = defaultColor;
        }*/
    }

    void UpdateTimers()
    {
        if (IsAttacking())
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                EndAttack();
            }
        }
    }

    public bool IsAttacking()
    {
        return attackTimer > 0;
    }

    void EndAttack()
    {
        attackBox.SetActive(false);
    }

    void SixteenthNoteUpdate()
    {
        if (sixteenthNoteCount % 4 == 1)
        {
            //Mark our rhythm by playing a metronome sound once per beat, which is four 16th notes.
            metronomeSound.pitch = 1;
            metronomeSound.Play();
            //print("BEAT COUNT: " + beatCount);
            beatCount = 0;
        }
        else
        {
            metronomeSound.pitch = 0.7f;
            metronomeSound.Play();
        }

        //Giant switch statements, the mark of a true prototype. We gotta come up with a more eloquent way to do beat based actions.
        switch (sixteenthNoteCount) {
            case 5:
                //Start telegraphing attack on the second beat of the measure
                enemyMat.color = windUpColor;
                break;
            case 8:
                //Flash red RIGHT before the moment of attack. One 16th note away, to be precise.
                enemyMat.color = preAttackColor;
                break;
            case 9:
                enemyMat.color = defaultColor;
                //Attack right on the third beat of the measure
                Attack(true);
                break;
        }
    }

    void Attack(bool parryable = true)
    {
        attackBox.SetActive(true);
        attackTimer = maxAttackTimer;
        Collider boxCol = attackBox.GetComponent<BoxCollider>();
        Collider[] cols = Physics.OverlapBox(boxCol.bounds.center, boxCol.bounds.extents, boxCol.transform.rotation, LayerMask.GetMask("Player"));
        for (int i = 0; i < cols.Length; i++)
        {
            TestPlayer player = cols[i].GetComponent<TestPlayer>();
            if (player != null)
            {
                print("ATTACK HIT PLAYER!");
                player.ReceiveAttack(attackDamage, this, parryable);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health = Mathf.Max(0, health - damage);
        //print("Enemy " + damage + " damage taken! Health is currently " + health);

        getHitParticles.Play();
        getHitSound.Play();

        float healthPercentage = (float)health / maxHealth;
        //print("HEALTH PERCENTAGE IS " + healthPercentage);
        healthBar.transform.localScale = new Vector3(healthPercentage * maxHealthBarScale.x, maxHealthBarScale.y, maxHealthBarScale.z);

        if (health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Make this enemy take damage
    /// </summary>
    /// <param name="damage"> How much damage to take </param>
    /// <returns> Whether or not the player attacked the enemy on beat </returns>
    public bool TakeDamageAndCheckForOnBeatAttack(int damage)
    {
        TakeDamage(damage);
        return WasAttackedOnBeat(true);
    }

    void Die()
    {
        Destroy(gameObject);
    }

    //Allow a little bit of wiggle room both before and after the beat for determing whether or not the enemy was attacked on beat.
    bool WasAttackedOnBeat(bool debug = false)
    {
        //An attack within a 32nd note before the beat will count as on beat.
        bool attackedWithinRangeBeforeBeat = sixteenthNoteCount % 4 == 0 && (timeUntilNextSixteenthNote) <= sixteenthNoteDuration / 2.0f;
        //An attack within a 32nd note after the beat will count as on beat.
        bool attackedWithinRangeAfterBeat = (sixteenthNoteCount % 4 == 1) || sixteenthNoteCount % 4 == 2 && (timeUntilNextSixteenthNote >= sixteenthNoteDuration / 2.0f);
        if (debug)
        {
            if (attackedWithinRangeBeforeBeat)
            {
                print("BEFORE BEAT, GOOD TIMING");
            }
            else if (attackedWithinRangeAfterBeat)
            {
                print("AFTER BEAT, GOOD TIMING");
            }
            else
            {
                print("OFF BEAT, BAD TIMING");
            }
        }
        return attackedWithinRangeBeforeBeat || attackedWithinRangeAfterBeat;
    }
}
