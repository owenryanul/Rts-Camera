using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Spawner_Logic_Script : MonoBehaviour, UnitOrder_Interface
{
    //TODO: Reorganise code, rename variables and generally make code more presentable

    public Color defaultColor;
    public Color selectedColor;
    public GameObject prefabToSpawn;
    public float spawnForce;

    private const string TAG_CAMERA = "MainCamera";

    private Cursor_Script cursorScript;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<Renderer>().material.color = defaultColor;
        this.cursorScript = GameObject.FindGameObjectWithTag(TAG_CAMERA).GetComponent<Cursor_Script>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        //When Unit Spawner is issued a move order, spawn a unit and launch in towards the target
        Vector3 spawnPoint = this.gameObject.transform.position;
        spawnPoint.y = 2;
        GameObject unit = Instantiate(prefabToSpawn, spawnPoint, this.gameObject.transform.rotation);
        unit.GetComponent<UnitOrder_Interface>().issueMoveOrder(target);
        Vector3 spawnVector = (target - spawnPoint);
        unit.GetComponent<Rigidbody>().AddForce((spawnVector.normalized + new Vector3(0, 10, 0)) * spawnForce); //* unit.GetComponent<Rigidbody>().mass);
    }

    private void OnDeath()
    {
        onDeath_DeSelect();
    }

    public void onDeath_DeSelect()
    {
        cursorScript.removeUnitFromSelection(this.gameObject);    
    }
}
