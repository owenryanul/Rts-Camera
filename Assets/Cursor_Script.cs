using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor_Script : MonoBehaviour
{
    public GameObject cursorObject;
    public List<GameObject> selectedUnits;
    public float timeNeededToTurnClickIntoDrag = 0.2f;

    private const string TAG_UNIT = "Unit";

    private Vector3 boxCorner1;
    private Vector3 boxCorner2;
    private bool drawSelectionBox;
    private Rect selectionBoxRect;
    private float timeSinceMouseDown;

    // Start is called before the first frame update
    void Start()
    {
        drawSelectionBox = false;
        timeSinceMouseDown = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            boxCorner1 = Input.mousePosition;
        }
        else if(Input.GetMouseButton(0))
        {
            timeSinceMouseDown += Time.deltaTime;
            if (timeSinceMouseDown >= timeNeededToTurnClickIntoDrag)
            {
                boxCorner2 = Input.mousePosition;
                drawSelectionBox = true;
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            drawSelectionBox = false;
            if(timeSinceMouseDown >= timeNeededToTurnClickIntoDrag)
            {
                selectUnitsInBox();
            }
            else
            {
                selectUnit();
            }
            
            timeSinceMouseDown = 0;
        }
        else if(Input.GetMouseButtonDown(1))
        {
            cursorObject.transform.position = getMousePointInWorld(Input.mousePosition);
            cursorObject.GetComponent<Animator>().SetTrigger("Ping");

            foreach(GameObject aUnit in selectedUnits)
            {
                aUnit.GetComponent<UnitOrder_Interface>().issueMoveOrder(cursorObject.transform.position);
            }
        }

    }

    void OnGUI()
    {
        if(drawSelectionBox)
        {
            //NB: GUIUtility.ScreenToGUIPoint does not work, must manually convert screen space co-ords to GUI space co-ords
            Vector2 guiCorner1 = boxCorner1; //(boxCorner1);
            guiCorner1.y = Screen.height - guiCorner1.y;
            Vector2 guiCorner2 = boxCorner2;
            guiCorner2.y = Screen.height - guiCorner2.y;
            Vector2 rectSize = guiCorner2 - guiCorner1;

            Rect aRect = new Rect(guiCorner1, rectSize);
            selectionBoxRect = aRect;
            Debug.Log("GuiBox = " + aRect.min + " : " + aRect.max);

            GUI.Box(aRect, "");
        }
    }

    private Vector3 getMousePointInWorld(Vector3 vector3)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        // You successfully hi
        if (Physics.Raycast(ray, out hit))
        {
            // Find the direction to move in
            return hit.point;
        }
        return Vector3.zero;
    }

    private void selectUnit()
    {
        //If shift wasn't held, deselect all previously selected units
        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
        {
            foreach (GameObject aUnit in selectedUnits)
            {
                aUnit.GetComponent<UnitOrder_Interface>().OnDeSelected();
            }

            selectedUnits = new List<GameObject>();
        }

        //If a unit was clicked on and it wasn't selected already select it, otherwise deselect it
        GameObject bUnit = getUnitAlongMouseRay();
        if (bUnit != null)
        {
            if (isUnitAlreadySelected(bUnit))
            {
                removeUnitFromSelection(bUnit);
                bUnit.GetComponent<UnitOrder_Interface>().OnDeSelected();
            }
            else
            {
                addUnitToSelection(bUnit);
                bUnit.GetComponent<UnitOrder_Interface>().OnSelected();
            }
        }

        
    }

    private GameObject getUnitAlongMouseRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        // You successfully hi
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.tag == TAG_UNIT)
            {
                return hit.transform.gameObject;
            }
        }
        return null;
    }

    private void selectUnitsInBox()
    {
        foreach (GameObject aUnit in selectedUnits)
        {
            aUnit.GetComponent<UnitOrder_Interface>().OnDeSelected();
        }

        selectedUnits = new List<GameObject>();

        //Vector3 boxCentre = (boxCorner1 + boxCorner2) / 2;
        //Vector3 boxSize = (boxCorner2 - boxCorner1) / 2;
        Rect box = selectionBoxRect;//new Rect(boxCentre, boxSize);

        Debug.Log("Box = " + box.min + " : " + box.max);
        foreach(GameObject aUnit in GameObject.FindGameObjectsWithTag(TAG_UNIT))
        {
            Vector2 unitOnScreenGUIPos = Camera.main.WorldToScreenPoint(aUnit.transform.position);
            //Convert the Unit's On Screen Pos to GUI.
            //Because trying to convert the selection box Rect from GUI to Screen was causing bizare issues.
            //Note to self, in future do no use Rect unless I have to, they are a nightmare of conflicting co-ordinate systems and min-max vector relations.
            unitOnScreenGUIPos.y = Screen.height - unitOnScreenGUIPos.y; 
            Debug.Log("aUnit position on screen= " + unitOnScreenGUIPos);
            if (box.Contains(unitOnScreenGUIPos, true))
            {
                addUnitToSelection(aUnit);
                aUnit.GetComponent<UnitOrder_Interface>().OnSelected();
            }
        }

    }

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

    public void addUnitToSelection(GameObject unitIn)
    {
        selectedUnits.Add(unitIn);
    }

    public void removeUnitFromSelection(GameObject unitIn)
    {
        selectedUnits.Remove(unitIn);
    }
}
