using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : BeatTrackerObject
{
    public int maxHealth;
    public int health;
    public int attackDamage = 10;

    public float speed;
    public float gravity;
    public float rotateSpeed;

    public bool pursuePlayer = true;

    [SerializeField]
    private CharacterController characterController;
    [SerializeField]
    private TestPlayer player;
    [SerializeField]
    private GameObject attackBox;
    [SerializeField]
    private ParticleSystem getHitParticles;
    [SerializeField]
    private AudioSource getHitSound, charge1, charge2;

    [SerializeField]
    private TestEnemyHealthbar healthBar;

    //[SerializeField]
    private Material enemyMat;

    private float attackTimer, maxAttackTimer = 0.4f;
    //Which attack this enemy will perform. 1 means parryable attack, 2 means unparryable attack that must be dodged.
    private float attackType = 1;

    //Change color to telegraph attacks for now
    public Color windUpColor1, windUpColor2, preAttackColor;
    public Color defaultColor;

    private Vector3 moveDirection = Vector3.zero;
    private Vector3 moveDirectionNoGravity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        enemyMat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimers();

        if (pursuePlayer)
        {
            PursuePlayer();
        }

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

    void PursuePlayer()
    {
        moveDirection = (player.transform.position - transform.position).normalized * speed;

        moveDirectionNoGravity = moveDirection;

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        RotateEnemy(1.0f);
    }

    void RotateEnemy(float turnSpeedModifier)
    {
        //Rotate player to face movement direction
        if (moveDirectionNoGravity.magnitude > 0)
        {
            Vector3 targetPos = transform.position + moveDirectionNoGravity;
            Vector3 targetDir = targetPos - transform.position;

            // The step size is equal to speed times frame time.
            float step = rotateSpeed * turnSpeedModifier * Time.deltaTime;

            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
            Debug.DrawRay(transform.position, newDir, Color.red);

            // Move our position a step closer to the target.
            transform.rotation = Quaternion.LookRotation(newDir);
        }
        //Failsafe to ensure that x and z are always zero.
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    public bool IsAttacking()
    {
        return attackTimer > 0;
    }

    void EndAttack()
    {
        attackBox.SetActive(false);
    }

    public override void SixteenthNoteUpdate()
    {
        //Giant switch statements, the mark of a true prototype. We gotta come up with a more eloquent way to do beat based actions.
        switch (TestBeatTracker.instance.sixteenthNoteCount) {
            case 5:
                //Start telegraphing attack on the second beat of the measure
                attackType = 1;
                int ran = Random.Range(0,9);
                if (ran < 4)
                {
                    attackType = 2;
                }
                if (attackType == 1)
                {
                    enemyMat.color = windUpColor1;
                    charge1.Play();
                }
                else
                {
                    enemyMat.color = windUpColor2;
                    charge2.Play();
                }
                break;
            case 8:
                //Flash red RIGHT before the moment of attack. One 16th note away, to be precise.
                enemyMat.color = preAttackColor;
                break;
            case 9:
                enemyMat.color = defaultColor;
                //Attack right on the third beat of the measure
                if (attackType == 1)
                {
                    Attack(true);
                }
                else
                {
                    Attack(false);
                }
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

        healthBar.SetHealthBarSize(health, maxHealth);

        getHitParticles.Play();
        getHitSound.Play();

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
        bool wasAttackedOnBeat = WasAttackedOnBeat(true);
        if (wasAttackedOnBeat)
        {
            TakeDamage(damage);
        }
        return wasAttackedOnBeat;
    }

    void Die()
    {
        TestBeatTracker.instance.RemoveBeatTrackerAtIndex(beatTrackerIndex);
        Destroy(healthBar.gameObject);
        Destroy(gameObject);
    }

    //Allow a little bit of wiggle room both before and after the beat for determing whether or not the enemy was attacked on beat.
    bool WasAttackedOnBeat(bool debug = false)
    {
        return TestBeatTracker.instance.WasActionOnBeat(debug);
    }
}
