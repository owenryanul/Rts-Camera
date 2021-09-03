using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script for handling player controlled Unit movement and attacks
public class Unit_Logic_Script : OrderableUnit
{
    public float unitSpeed = 1; //The speed at which this unit moves
    
    public GameObject bulletObject; //The prefab used for this unit's projectiles
    public float attackRange; //The maximum distance at which the unit will target enemies
    public float timeBetweenAttacks; //The time in seconds between each of the unit's attacks
    private float timeUntilNextAttack; //The time in seconds remaining until the unit's next attack

    public GameObject deathParticleEmitter; //Particle Emitter used for the unit's death effect.

    private Vector3 targetLocation; //The position the unit attempting to move to.

    private const string TAG_ENEMY = "Enemy Unit"; //Constant storing the tag for Enemy Units
    private const string TAG_ENEMY_BULLET = "Enemy Bullet"; //Constant storing the tag for Enemy units' bullets


    // Start is called before the first frame update
    void Start()
    {
        //If the unit doesn't have a target position set it to it's current position
        if (targetLocation == Vector3.zero) //Note: this could cause problems if a unit ever starts/spawns positioned at (0,0,0) 
        {
            targetLocation = this.transform.position;
            targetLocation.y = 0;
        }

        base.Start();
    }

    // Update is called once per frame
    void Update()
    {        
        //[Movement]
        //Move unit towards the target position at it's unit speed.
        this.gameObject.GetComponent<Rigidbody>().MovePosition(this.transform.position + (Vector3.Normalize(targetLocation - this.transform.position) * unitSpeed * Time.deltaTime) );
    
        //[Attack]
        if(timeUntilNextAttack > 0)
        {
            timeUntilNextAttack -= Time.deltaTime;
        }
        else
        {
            //If the next attack is ready then check for enemies in range
            GameObject targetEnemy = getClosestEnemy();
            float distanceToClosestEnemy = (targetEnemy.transform.position - this.gameObject.transform.position).magnitude;
            if (targetEnemy != null && distanceToClosestEnemy < attackRange)
            {
                //If the closest enemy is within attack range, fire a bullet at it
                GameObject newBullet = Instantiate(bulletObject, this.gameObject.transform.position, this.gameObject.transform.rotation);
                newBullet.GetComponent<Bullet_Logic_Script>().targetLocation = targetEnemy.gameObject.transform.position;
                timeUntilNextAttack = timeBetweenAttacks;
            }
        }
    }

    //If this unit collides with an enemy bullet, explode the enemy bullet and kill this unit
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == TAG_ENEMY_BULLET)
        {
            other.gameObject.GetComponent<Enemy_Bullet_Logic>().explode();
            kill();
        }
    }

    //Returns the closest enemy Unit or turret, returns null if there are no enemies
    private GameObject getClosestEnemy()
    {
        //Initialises closestEnemy as any enemy
        GameObject closestEnemy = GameObject.FindGameObjectWithTag(TAG_ENEMY);
        if(closestEnemy == null)
        {
            return null;
        }

        //Interates through all enemies to find the closest enemy
        foreach (GameObject anEnemy in GameObject.FindGameObjectsWithTag(TAG_ENEMY))
        {
            if ((anEnemy.transform.position - this.gameObject.transform.position).magnitude < (closestEnemy.transform.position - this.gameObject.transform.position).magnitude)
            {
                closestEnemy = anEnemy;
            }
        }
        return closestEnemy;
    }

    /*___________________________________________________________________________
    *                   =========Public Methods==========
    * ____________________________________________________________________________
    */

    //Called in Cursor_Logic_Script when the user right clicks while this unit is selected. Change's this unit's targetLocation to where the user clicked.
    public override void issueMoveOrder(Vector3 target)
    {
        targetLocation = target;
    }

    //Destroys this unit after automatically deselecting it and spawning a death particle effect
    public override void kill()
    {
        if (!isDying)
        {
            isDying = true;
            onDeath_DeSelect();
            Instantiate(deathParticleEmitter, this.gameObject.transform.position, new Quaternion());
            Destroy(this.gameObject);
        }
    }

}
