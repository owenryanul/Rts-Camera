using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script for handling the player controlled unit spawner's selection and spawning of new player controlled units.
public class Unit_Spawner_Logic_Script : OrderableUnit
{

    public GameObject prefabToSpawn; //The unit prefab this spawner spawns
    public float spawnForce; //The initial force with which a unit spawned by this spawner spawns with

    private const string TAG_CAMERA = "MainCamera"; //Constant containing the tag for the player's camera

    // Start is called before the first frame update
    void Start()
    {
        isDying = false;

        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Called in Cursor_Logic_Script when the user right clicks while this unit is selected.
    //Makes this spawner spawn a new player controlled unit and orders that unit to move to where the player right-clicked.
    public override void issueMoveOrder(Vector3 target)
    {
        //Spawn the unit
        Vector3 spawnPoint = this.gameObject.transform.position;
        spawnPoint.y = 2;
        GameObject unit = Instantiate(prefabToSpawn, spawnPoint, this.gameObject.transform.rotation);
        //Automatically issue a move order to the unit
        unit.GetComponent<OrderableUnit>().issueMoveOrder(target);

        //Launch the spawned unit with some amount of force towards its target position.
        Vector3 spawnVector = (target - spawnPoint);
        unit.GetComponent<Rigidbody>().AddForce((spawnVector.normalized + new Vector3(0, 10, 0)) * spawnForce); //* unit.GetComponent<Rigidbody>().mass);
    }

}
