using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    [SerializeField]
    private GameObject attackBox;
    [SerializeField]
    private AudioSource metronomeSound;
    [SerializeField]
    private Material enemyMat;

    //Manually make the enemy act on beat by using the duration of our 16th note at 120 BPM as an update timer.
    public float sixteenthNoteDuration = 0.125f;
    private int sixteenthNoteCount = 1;
    //The enemy's logic cycle for now simply lasts 16 16th notes, which equals 4 beats, which equals 1 measure. Music, yeah!
    private int maxSixteenthNoteCount = 16;

    private float timeUntilNextSixteenthNote = 0;

    private float attackTimer, maxAttackTimer = 0.2f;

    //Change color to telegraph attacks for now
    public Color windUpColor, preAttackColor;
    private Color defaultColor;

    // Start is called before the first frame update
    void Start()
    {
        defaultColor = enemyMat.color;
        EightNoteUpdate();
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
            EightNoteUpdate();
        }
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

    bool IsAttacking()
    {
        return attackTimer > 0;
    }

    void EndAttack()
    {
        attackBox.SetActive(false);
    }

    void EightNoteUpdate()
    {
        if (sixteenthNoteCount % 4 == 1)
        {
            //Mark our rhythm by playing a metronome sound once per beat, which is four 16th notes.
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
                Attack();
                break;
        }
    }

    void Attack()
    {
        attackBox.SetActive(true);
        attackTimer = maxAttackTimer;
    }
}
