using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script for controlling bullets fired by the player's units
public class Bullet_Logic_Script : MonoBehaviour
{
    public Vector3 targetLocation; //the point in world space the bullet is traveling towards
    public float bulletSpeed; //the speed the bullet travels at

    //Note: ====> For collision with enemies see the enemys' logic scripts, e.g. Enemy_Logic_Script.OnTriggerEnter().

    // Start is called before the first frame update
    void Awake()
    {
        targetLocation = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //Move the bullet towards the targetlocation
        this.gameObject.transform.position += Vector3.Normalize(targetLocation - this.gameObject.transform.position) * bulletSpeed * Time.deltaTime; 

        //If the bullet is within 0.02 units of the target, it is considered to have reached it's destination and is destroyed
        if((targetLocation - this.gameObject.transform.position).magnitude < 0.02f)
        {
            Destroy(this.gameObject);
        }
    }
}
