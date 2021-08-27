using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bullet_Logic : MonoBehaviour
{
    public Vector3 targetPos;
    public float bulletSpeed;
    public float blastRadius;
    public GameObject explosionEmitter;
    public float maxArcHeight = 5;
    private Vector3 startPos;

    private const string TAG_UNIT = "Unit";

    // Start is called before the first frame update
    void Awake()
    {
        targetPos = new Vector3(0, 0, 0);
        startPos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
        Vector3 nextPos = this.gameObject.transform.position + Vector3.Normalize(targetPos - this.gameObject.transform.position) * bulletSpeed * Time.deltaTime;

        Vector3 xzMovementFull = targetPos - startPos;
        xzMovementFull.y = 0;
        Vector3 xzMovementSoFar = nextPos - startPos;
        xzMovementSoFar.y = 0;
        Vector3 xzMovementLeft = targetPos - nextPos;
        xzMovementLeft.y = 0;


        float nextY = Mathf.Lerp(startPos.y, targetPos.y, xzMovementSoFar.magnitude / xzMovementFull.magnitude);
        float extraHeightFromArc = maxArcHeight * (xzMovementSoFar.magnitude * xzMovementLeft.magnitude) / (0.25f * (xzMovementFull.magnitude * xzMovementFull.magnitude));
        nextPos = new Vector3(nextPos.x, nextY + extraHeightFromArc, nextPos.z);


        // Rotate to face the next position, and then move there
        //transform.rotation = LookAt2D(nextPos - transform.position);
        transform.position = nextPos;

        if ((targetPos - this.gameObject.transform.position).magnitude < 0.05f)
        {
            explode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Terrian")
        {
            explode();
        }
    }

    public void explode()
    {
        Instantiate(explosionEmitter, this.gameObject.transform.position, this.gameObject.transform.rotation);
        foreach(GameObject aUnit in GameObject.FindGameObjectsWithTag(TAG_UNIT))
        {
            if((aUnit.transform.position - this.transform.position).magnitude < blastRadius)
            {
                aUnit.GetComponent<Unit_Logic_Script>().kill();
            }
        }
        Destroy(this.gameObject);
    }
}
