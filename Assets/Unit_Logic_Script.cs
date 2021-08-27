using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Logic_Script : MonoBehaviour, UnitOrder_Interface
{

    //TODO: Reorganise code, rename variables and generally make code more presentable

    public Color defaultColor;
    public Color selectedColor;
    public float unitSpeed = 1;
    public GameObject bulletObject;
    public float rateOfFire;
    public float attackRange;
    private float timeUntilFire;

    public GameObject deathParticleEmitter;

    private Vector3 targetLocation;

    private const string TAG_ENEMY = "Enemy Unit";
    private const string TAG_ENEMY_BULLET = "Enemy Bullet";
    private const string TAG_CAMERA = "MainCamera";

    private Cursor_Script cursorScript;



    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<Renderer>().material.color = defaultColor;
        if (targetLocation == Vector3.zero) //Note: this could cause problems if a unit ever starts/spawns positioned at (0,0,0) 
        {
            targetLocation = this.transform.position;
            targetLocation.y = 0;
        }

        this.cursorScript = GameObject.FindGameObjectWithTag(TAG_CAMERA).GetComponent<Cursor_Script>();
    }

    // Update is called once per frame
    void Update()
    {        
        //Move
        this.gameObject.GetComponent<Rigidbody>().MovePosition(this.transform.position + (Vector3.Normalize(targetLocation - this.transform.position) * unitSpeed * Time.deltaTime) );
    
        //Attack
        if(timeUntilFire > 0)
        {
            timeUntilFire -= Time.deltaTime;
        }
        else
        {
            GameObject targetEnemy = getClosestEnemy();
            float distanceToClosestEnemy = (targetEnemy.transform.position - this.gameObject.transform.position).magnitude;
            if (targetEnemy != null && distanceToClosestEnemy < attackRange)
            {
                GameObject newBullet = Instantiate(bulletObject, this.gameObject.transform.position, this.gameObject.transform.rotation);
                newBullet.GetComponent<Bullet_Logic_Script>().targetLocation = targetEnemy.gameObject.transform.position;
                timeUntilFire = rateOfFire;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == TAG_ENEMY_BULLET)
        {
            other.gameObject.GetComponent<Enemy_Bullet_Logic>().explode();
            OnDeath();
        }
    }


    private GameObject getClosestEnemy()
    {
        GameObject closestEnemy = GameObject.FindGameObjectWithTag(TAG_ENEMY);
        if(closestEnemy == null)
        {
            return null;
        }

        foreach (GameObject anEnemy in GameObject.FindGameObjectsWithTag(TAG_ENEMY))
        {
            if ((anEnemy.transform.position - this.gameObject.transform.position).magnitude < (closestEnemy.transform.position - this.gameObject.transform.position).magnitude)
            {
                closestEnemy = anEnemy;
            }
        }
        return closestEnemy;
    }

    public void OnSelected()
    {
        this.gameObject.GetComponent<Renderer>().material.color = selectedColor;
    }

    public void OnDeSelected()
    {
        this.gameObject.GetComponent<Renderer>().material.color = defaultColor;
    }

    public void issueMoveOrder(Vector3 target)
    {
        targetLocation = target;
    }

    private void OnDeath()
    {
        onDeath_DeSelect();
        Instantiate(deathParticleEmitter, this.gameObject.transform.position, new Quaternion());
        Destroy(this.gameObject);
    }

    public void kill()
    {
        this.OnDeath();
    }

    public void onDeath_DeSelect()
    {
        cursorScript.removeUnitFromSelection(this.gameObject);
    }
}
