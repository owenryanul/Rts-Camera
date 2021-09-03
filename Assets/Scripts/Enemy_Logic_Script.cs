using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script for handling the logic of wandering enemy units
public class Enemy_Logic_Script : MonoBehaviour
{
    public GameObject deathParticleEmitter; //The death particle effect emitter.
    public float unitSpeed; //the speed the unit moves at
    public GameObject prefabToRespawnAs; //the unit's prefab, used to respawn the unit when it is killed

    private bool isDying; //flag used to prevent death logic being run more than once.
    private Vector3 startPoint; //the position the unit started in
    private Vector3 targetPoint; //the position the unit is attempting to move to
    private float timeBetweenTargetChanges;  //the time the units must wait before changing targetPoints (is randomly set each time it changes target point)
    private float timeSinceTargetChange; //the time since the unit last changed it's target point.

    private const string TAG_BULLET = "Unit Bullet"; //Constant for storing the Unit Bullet's Tag

    // Start is called before the first frame update
    void Start()
    {
        this.isDying = false;
        this.startPoint = this.gameObject.transform.position; //set the unit's start point to it's starting position, but with y = 0
        this.startPoint.y = 0;
        timeBetweenTargetChanges = 0;
        timeSinceTargetChange = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Move the unit towards the current target point
        this.gameObject.GetComponent<Rigidbody>().MovePosition(this.transform.position + (Vector3.Normalize(targetPoint - this.transform.position) * unitSpeed * Time.deltaTime));
        
        //Update the time since the unit last changed target.
        timeSinceTargetChange += Time.deltaTime;
        if (timeSinceTargetChange >= timeBetweenTargetChanges)
        {
            //if enough time has past since the last time the unit changed it's target point, then change it again to a random point within range of it's starting point
            timeSinceTargetChange = 0;
            timeBetweenTargetChanges = Random.Range(1, 20);
            targetPoint = startPoint + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        //If the unit is not already dying and it collides with a bullet, kill it and the bullet.
        if(!this.isDying &&  other.tag == TAG_BULLET)
        {
            kill();
            Destroy(other.gameObject);
        }
    }

    /*___________________________________________________________________________
    *                   =========Public Methods==========
    * ____________________________________________________________________________
    */

    //Spawns a death particle effect and spawns a replacement enemy unit at this unit's original spawn point. Then destroys this unit.
    public void kill()
    {
        this.isDying = true;
        Vector3 respawnPoint = startPoint;
        respawnPoint.y = 10;
        Instantiate(deathParticleEmitter, this.gameObject.transform.position, new Quaternion());
        Instantiate(prefabToRespawnAs, respawnPoint, this.transform.rotation);
        Destroy(this.gameObject);
    }

}
