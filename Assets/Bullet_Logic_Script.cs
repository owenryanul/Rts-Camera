using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Logic_Script : MonoBehaviour
{
    public Vector3 targetLocation;
    public float bulletSpeed;

    // Start is called before the first frame update
    void Awake()
    {
        targetLocation = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position += Vector3.Normalize(targetLocation - this.gameObject.transform.position) * bulletSpeed * Time.deltaTime; 

        if((targetLocation - this.gameObject.transform.position).magnitude < 0.05f)
        {
            Destroy(this.gameObject);
        }
    }
}
