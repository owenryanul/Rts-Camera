using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Logic_Script : MonoBehaviour
{
    public Color enemyColor;
    public GameObject deathParticleEmitter;
    public float unitSpeed;
    public GameObject prefabToRespawnAs;

    private Vector3 startPoint;
    private Vector3 targetPoint;
    private float timeBetweenChangeTarget;
    private float timeSinceTargetChange;

    private const string TAG_BULLET = "Unit Bullet";

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<Renderer>().material.color = enemyColor;
        this.startPoint = this.gameObject.transform.position;
        this.startPoint.y = 0;
        timeBetweenChangeTarget = 0;
        timeSinceTargetChange = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceTargetChange += Time.deltaTime;
        this.gameObject.GetComponent<Rigidbody>().MovePosition(this.transform.position + (Vector3.Normalize(targetPoint - this.transform.position) * unitSpeed * Time.deltaTime));
        if (timeSinceTargetChange >= timeBetweenChangeTarget)
        {
            timeSinceTargetChange = 0;
            timeBetweenChangeTarget = Random.Range(1, 20);
            targetPoint = startPoint + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == TAG_BULLET)
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
