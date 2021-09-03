using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script for handling the lobbed bullets of Enemy Turrets
public class Enemy_Bullet_Logic : MonoBehaviour
{
    public Vector3 targetPos; //the bullet's intendend destination
    public float bulletSpeed; //the speed of the bullet
    public float maxArcHeight = 5; //the maximium height of the arc the bullet travels in.

    public float blastRadius; //the blast radius of the bullet when it explodes.
    public GameObject explosionEmitter; //the particle emiter for the bullet's explosion effect.
    
    private Vector3 startPos; //the bullet's starting position

    private const string TAG_UNIT = "Unit";

    // Start is called before the first frame update
    void Awake()
    {
        targetPos = new Vector3(0, 0, 0); //intialise the bullet's targetPos
        startPos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //[Move the Bullet]
        moveBulletAlongArc();

        //If the bullet is within 0.02 units of it's destination it has reached it and it explodes.
        if ((targetPos - this.gameObject.transform.position).magnitude < 0.02f)
        {
            explode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //If the bullet collides wtih the terrian, it explodes.
        if (other.tag == "Terrian")
        {
            explode();
        }

        //====> for Collisions with player units see Unit_Logic_Script.OnTriggerEnter();
    }

    //Calculates where the bullet should be moved to in the x and z axis, then calculates how high in it's arc the bullet should be, then moves the bullet to that position.
    private void moveBulletAlongArc()
    {
        //Calculate the bullet's next position along the x and z axises.
        Vector3 nextPos = this.gameObject.transform.position + Vector3.Normalize(targetPos - this.gameObject.transform.position) * bulletSpeed * Time.deltaTime;

        //Calculate the necessary vectors for the arc calculation
        Vector3 xzMovementFull = targetPos - startPos;
        xzMovementFull.y = 0;
        Vector3 xzMovementSoFar = nextPos - startPos;
        xzMovementSoFar.y = 0;
        Vector3 xzMovementRemainingToTarget = targetPos - nextPos;
        xzMovementRemainingToTarget.y = 0;

        //Calculate how much height would need to be added if the bullet was being fired straight at the target.
        //(This accounts for the target being at a different Y to the start point)
        float nextY = Mathf.Lerp(startPos.y, targetPos.y, xzMovementSoFar.magnitude / xzMovementFull.magnitude);
        //Calculate the extra height to add based on how far along it's tradjectory the bullet has travelled.
        float extraHeightFromArc = maxArcHeight * (xzMovementSoFar.magnitude * xzMovementRemainingToTarget.magnitude) / (0.25f * (xzMovementFull.magnitude * xzMovementFull.magnitude));
        
        //Move the bullet to the new position.
        nextPos = new Vector3(nextPos.x, nextY + extraHeightFromArc, nextPos.z);
        transform.position = nextPos;
    }

     /* ___________________________________________________________________________
     *                   =========Public Methods==========
     * ____________________________________________________________________________
     */

    //Explodes the bullet, destroying it, spawning an explosion particle effect and killing any player controlled unit within the blast radius.
    public void explode()
    {
        Instantiate(explosionEmitter, this.gameObject.transform.position, this.gameObject.transform.rotation);
        foreach(GameObject aUnit in GameObject.FindGameObjectsWithTag(TAG_UNIT))
        {
            //If aUnit was within the blastRadius, call it's kill() method.
            if((aUnit.transform.position - this.transform.position).magnitude < blastRadius)
            {
                aUnit.GetComponent<Unit_Logic_Script>().kill();
            }
        }
        Destroy(this.gameObject);
    }
}
