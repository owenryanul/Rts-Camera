using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script used to handle the player clicking on the world and selecting units with the mouse
public class Cursor_Script : MonoBehaviour
{
    public GameObject cursorObject; //GameObject used for making cursor pings
    public List<GameObject> selectedUnits; //a list of every unit currently selected
    public float timeNeededToTurnClickIntoDrag = 0.2f; //The time in seconds that left click must be held before it starts drawing a selection box

    private const string TAG_UNIT = "Unit"; //Constant used to reference the tag associated with player controlled units

    private Vector3 boxCorner1; //the top left corner of the selection box in screen space co-ordinates.
    private Vector3 boxCorner2; //the bottom right corner of the selection box in screen space co-ordinates.
    private Rect selectionBoxRect; //the rect used to select units
    private bool drawingSelectionBox; //whether or not the selection box is being drawn.
    
    private float timeSinceMouseDown; //the time in seconds since the left mouse button was pressed down

    // Start is called before the first frame update
    void Start()
    {
        drawingSelectionBox = false;
        selectedUnits = new List<GameObject>();
        timeSinceMouseDown = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            //when the left mouse button is pressed down store the mouse's position for later use
            boxCorner1 = Input.mousePosition;
        }
        else if(Input.GetMouseButton(0))
        {
            //while the left mouse button is held down increase the time since it was first pressed
            timeSinceMouseDown += Time.deltaTime;
            //if the time since the mouse was clicked exceeds the delay, set the bottom right corner of the selection box to the mouse's position and start drawing the box
            if (timeSinceMouseDown >= timeNeededToTurnClickIntoDrag)
            {
                boxCorner2 = Input.mousePosition;
                drawingSelectionBox = true;
                //====> for selection box drawing code see this.OnGui()
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            //When the left mouse button is released, if the selection box was being drawn select all units in the box otherwise select the current unit the mouse was over
            drawingSelectionBox = false;
            if(timeSinceMouseDown >= timeNeededToTurnClickIntoDrag)
            {
                selectUnitsInBox();
            }
            else
            {
                selectUnit();
            }
            
            timeSinceMouseDown = 0; //Reset timer
        }
        else if(Input.GetMouseButtonDown(1)) 
        {
            //When right mouse button clicked, place the animated cursor marker at the point and call issueMoveOrder on all currently selected units
            cursorObject.transform.position = getMousePointInWorld();
            cursorObject.GetComponent<Animator>().SetTrigger("Ping");

            foreach(GameObject aUnit in selectedUnits)
            {
                aUnit.GetComponent<OrderableUnit>().issueMoveOrder(cursorObject.transform.position);
            }
        }

    }

    void OnGUI()
    {
        //if the selectionBox is supposed to be displayed, draw the selection box
        if(drawingSelectionBox)
        {
            //Important Note: For reasons I have been unable to discern GUIUtility.ScreenToGUIPoint does convert screen space to gui space.
            //It just outputs the vector it takes in with no change.
            //So screen space co-ords must be manually converted to GUI space co-ords.

            //Convert screenspace co-ordinates into gui-space co-ordinates
            Vector2 guiCorner1 = boxCorner1; 
            guiCorner1.y = Screen.height - guiCorner1.y;
            Vector2 guiCorner2 = boxCorner2;
            guiCorner2.y = Screen.height - guiCorner2.y;

            //Calculate and assemble the selectionBox Rect
            Vector2 rectSize = guiCorner2 - guiCorner1;
            Rect aRect = new Rect(guiCorner1, rectSize);
            selectionBoxRect = aRect;

            //Draw the Selection Box
            GUI.Box(aRect, "");
        }
    }

    //Raycasts from the mouse's current position and returns the point in the world the mouse currently is hovering over.
    //Returns (0,0,0) if the mouse's ray trace hits nothing.
    private Vector3 getMousePointInWorld()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    //Returns the first unit that the mouse is hovering over.
    //Returns null if the mouse is not hovering over a unit
    private GameObject getUnitAlongMouseRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.tag == TAG_UNIT)
            {
                return hit.transform.gameObject;
            }
        }
        return null;
    }

    //Selects the unit the player clicked on.
    private void selectUnit()
    {
        //If shift was not held down, deselect all previously selected units
        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
        {
            foreach (GameObject aUnit in selectedUnits)
            {
                aUnit.GetComponent<OrderableUnit>().OnDeSelected();
            }

            selectedUnits = new List<GameObject>();
        }

        
        GameObject bUnit = getUnitAlongMouseRay();
        if (bUnit != null)
        {
            //If a unit was clicked on and it wasn't selected already then select it, otherwise deselect it
            if (isUnitAlreadySelected(bUnit))
            {
                removeUnitFromSelection(bUnit);
            }
            else
            {
                addUnitToSelection(bUnit);
            }
        }

        
    }

    //Selects all units in the selection box.
    private void selectUnitsInBox()
    {
        //Deselect all previously selected units
        foreach (GameObject aUnit in selectedUnits)
        {
            aUnit.GetComponent<OrderableUnit>().OnDeSelected();
        }
        selectedUnits = new List<GameObject>();


        Rect box = selectionBoxRect; //Use the rect created in OnGui

        //Check every player unit to see if it is within the selection box
        //Warning: This may not be very scalable
        foreach (GameObject aUnit in GameObject.FindGameObjectsWithTag(TAG_UNIT))
        {
            //Convert the Unit's On Screen Pos to GUI.
            //We are not converting the selection box Rect from GUI to screen space because doing so was causing bizare issues.
            Vector2 unitOnScreenGUIPos = Camera.main.WorldToScreenPoint(aUnit.transform.position);
            unitOnScreenGUIPos.y = Screen.height - unitOnScreenGUIPos.y;
            
            //if the selection box rect contains the unit add them to the selection.
            if (box.Contains(unitOnScreenGUIPos, true))
            {
                addUnitToSelection(aUnit);
                aUnit.GetComponent<OrderableUnit>().OnSelected();
            }
        }

    }

    //Takes in a unit and returns true if inUnit is already selected, otherwise returns false
    private bool isUnitAlreadySelected(GameObject inUnit)
    {
        foreach(GameObject aUnit in selectedUnits)
        {
            if(inUnit == aUnit)
            {
                return true;
            }
        }
        return false;
    }

     /* ___________________________________________________________________________
     *                   =========Public Methods==========
     * ____________________________________________________________________________
     */

    //Adds a Unit to the current selection and calls their OnSelected() method.
    public void addUnitToSelection(GameObject unitIn)
    {
        selectedUnits.Add(unitIn);
        unitIn.GetComponent<OrderableUnit>().OnSelected();
    }

    //Removes a Unit from the current selection and calls their OnDeselected() method.
    public void removeUnitFromSelection(GameObject unitIn)
    {
        selectedUnits.Remove(unitIn);
        unitIn.GetComponent<OrderableUnit>().OnDeSelected();
    }
}
