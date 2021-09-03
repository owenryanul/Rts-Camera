using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base Class extended by all player controlled units, containing methods common to each player controlled unit.
//Contains base versions of OnSelected, OnDeselected, IssueMoveOrder and Kill methods.
public class OrderableUnit : MonoBehaviour
{
    protected bool isDying; //Flag to prevent death logic from being triggered more than once
    protected Color defaultColor; //Variable storing the unit's original color.
    public Color colorWhenSelected; //The color this unit switches to when it is selected by the player
    protected Cursor_Script cursorScript; //Reference to the script that handles the player issuing orders

    private const string TAG_CAMERA = "MainCamera"; //Constant storing the tag for the player's Camera.

    public virtual void Start()
    {
        this.cursorScript = GameObject.FindGameObjectWithTag(TAG_CAMERA).GetComponent<Cursor_Script>();
        isDying = false;
        defaultColor = this.gameObject.GetComponent<Renderer>().material.color;
    }

    //Called in Cursor_Logic_Script when the unit is Selected. Changes the unit's color to mark it as selected.
    public virtual void OnSelected()
    {
        this.gameObject.GetComponent<Renderer>().material.color = colorWhenSelected;
    }

    //Called in Cursor_Logic_Script when the unit is Deselected. Changes the unit's color back to normal.
    public virtual void OnDeSelected()
    {
        this.gameObject.GetComponent<Renderer>().material.color = defaultColor;
    }

    //Called in Cursor_Logic_Script when the user right clicks while this unit is selected. Change's this unit's targetLocation to where the user clicked.
    public virtual void issueMoveOrder(Vector3 target)
    {

    }

    //Destroys this unit after automatically deselecting it and spawning a death particle effect
    public virtual void kill()
    {
        if (!isDying)
        {
            isDying = true;
            onDeath_DeSelect();
            Destroy(this.gameObject);
        }
    }

    //Removes this unit from the list of selected units in Cursor_Logic_Script when this unit dies
    public void onDeath_DeSelect()
    {
        cursorScript.removeUnitFromSelection(this.gameObject);
    }
}
