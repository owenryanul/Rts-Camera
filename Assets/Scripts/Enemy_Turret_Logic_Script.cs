using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script the handles the behaviour of Enemy Turrets
public class Enemy_Turret_Logic_Script : MonoBehaviour
{
    public GameObject deathParticleEmitter; //Particale Emitter spawned when the Turret dies
    public GameObject prefabToRespawnAs; //The gameobject to be spawned as a replace when the Turret dies
    
    public float attackRange; //The maximum range at which the turret will target and attack player units
    public GameObject bulletObject; //The projectile prefab fired by the turret
    public float timeBetweenAttacks; //The time the turret waits between each attack.
    private float timeUntilAttack; //The current time remaining until the turret next attacks.

    public float barrelVisualAngleOffset = 45.0f; //The angle at which the turret's barrel should be offset when looking at the player. This purely visual and has no mechanical effect.

    private Vector3 startPoint; //the starting position of the turret, used for respawning
    private bool isDying; //flag used to stop a tower's death logic from triggering more than once 

    private const string TAG_BULLET = "Unit Bullet"; //Constant for storing the tag used for bullets fired by the player's units
    private const string TAG_UNIT = "Unit"; //Constant for storing the tag used for player units

    // Start is called before the first frame update
    void Start()
    {
        this.startPoint = this.gameObject.transform.position;
        this.startPoint.y = 0;
        this.isDying = false;
        this.timeUntilAttack = timeBetweenAttacks;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject target = getClosestUnit();

        //If the nearest player unit is within range begin running attack logic
        if(target != null && (target.transform.position - this.gameObject.transform.position).magnitude < this.attackRange)
        {
            //Point the turret's barrel towards the target
            this.transform.Find("Turret").transform.LookAt(target.transform.position);
            this.transform.Find("Turret").transform.Rotate(new Vector3(barrelVisualAngleOffset,0,0), Space.Self);

            if (timeUntilAttack > 0)
            {
                timeUntilAttack -= Time.deltaTime;
            }
            else
            {
                //If time until attack is 0 or less, fire a bullet at the target's current position (the turret intentional does not account for the target's movement).
                GameObject newBullet = Instantiate(bulletObject, this.gameObject.transform.Find("Turret").position, this.gameObject.transform.rotation);
                newBullet.GetComponent<Enemy_Bullet_Logic>().targetPos = target.gameObject.transform.position;
                timeUntilAttack = timeBetweenAttacks;
            }
        }
    }

    //Returns the closest player controlled unit. Returns null if there are no player controlled units
    private GameObject getClosestUnit()
    {
        //Intialise closestUnit to any player controlled unit
        GameObject closestUnit = GameObject.FindGameObjectWithTag(TAG_UNIT);
        if (closestUnit == null)
        {
            return null;
        }

        //Interate through all player controlled units to find the closest one
        foreach (GameObject aUnit in GameObject.FindGameObjectsWithTag(TAG_UNIT))
        {
            if ((aUnit.transform.position - this.gameObject.transform.position).magnitude < (closestUnit.transform.position - this.gameObject.transform.position).magnitude)
            {
                closestUnit = aUnit;
            }
        }
        return closestUnit;
    }

    //If the turret is hit by a player unit's bullet then destroy the bullet and kill this turret
    private void OnTriggerEnter(Collider other)
    {
        if (!isDying && other.tag == TAG_BULLET)
        {
            Destroy(other.gameObject);
            this.kill();
        }
    }

    /*___________________________________________________________________________
    *                   =========Public Methods==========
    * ____________________________________________________________________________
    */

    //Destroys this turret after spawning a replacement and a death particle effect.
    public void kill()
    {
        if (!isDying)
        {
            this.isDying = true; //Set isDying to true to stop this code from being triggered by collisions occuring before the turret can be destroyed
            Instantiate(deathParticleEmitter, this.gameObject.transform.position, new Quaternion());
            Vector3 respawnPoint = startPoint;
            respawnPoint.y = 10;
            Instantiate(prefabToRespawnAs, respawnPoint, this.transform.rotation);
            Destroy(this.gameObject);
        }
    }
}
