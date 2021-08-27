using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Turret_Logic_Script : MonoBehaviour
{
    public Color enemyColor;
    public GameObject deathParticleEmitter;
    public GameObject prefabToRespawnAs;
    
    public float attackRange;
    public GameObject bulletObject;
    public float rateOfFire;
    private float timeUntilFire;

    private Vector3 startPoint;

    private const string TAG_BULLET = "Unit Bullet";
    private const string TAG_UNIT = "Unit";

    // Start is called before the first frame update
    void Start()
    {
        this.transform.Find("Base").gameObject.GetComponent<Renderer>().material.color = enemyColor;
        this.transform.Find("Turret").Find("Turret Model").GetComponent<Renderer>().material.color = enemyColor;
        this.startPoint = this.gameObject.transform.position;
        this.startPoint.y = 0;

        this.timeUntilFire = rateOfFire;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject target = getClosestUnit();
        if((target.transform.position - this.gameObject.transform.position).magnitude < this.attackRange)
        {
            this.transform.Find("Turret").transform.LookAt(target.transform.position);
            this.transform.Find("Turret").transform.Rotate(new Vector3(45,0,0), Space.Self);

            if (timeUntilFire > 0)
            {
                timeUntilFire -= Time.deltaTime;
            }
            else
            {
                if (target != null)
                {
                    GameObject newBullet = Instantiate(bulletObject, this.gameObject.transform.Find("Turret").position, this.gameObject.transform.rotation);
                    newBullet.GetComponent<Enemy_Bullet_Logic>().targetPos = target.gameObject.transform.position;
                    timeUntilFire = rateOfFire;
                }
            }
        }
    }

    private GameObject getClosestUnit()
    {
        GameObject closestUnit = GameObject.FindGameObjectWithTag(TAG_UNIT);
        if (closestUnit == null)
        {
            return null;
        }

        foreach (GameObject aUnit in GameObject.FindGameObjectsWithTag(TAG_UNIT))
        {
            if ((aUnit.transform.position - this.gameObject.transform.position).magnitude < (closestUnit.transform.position - this.gameObject.transform.position).magnitude)
            {
                closestUnit = aUnit;
            }
        }
        return closestUnit;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == TAG_BULLET)
        {
            Vector3 respawnPoint = startPoint;
            respawnPoint.y = 10;
            Destroy(other.gameObject);
            Instantiate(deathParticleEmitter, this.gameObject.transform.position, new Quaternion());
            Instantiate(prefabToRespawnAs, respawnPoint, this.transform.rotation);
            Destroy(this.gameObject);
        }
    }

}
