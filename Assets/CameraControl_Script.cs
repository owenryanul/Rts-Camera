using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl_Script : MonoBehaviour
{
    [Header("Camera Speeds")]
    public float panSpeed = 1;
    public float verticaltiltSpeed = 1;
    public float horizontaltiltSpeed = 1;

    public float maxZoomLevel = 30;
    public float minZoomLevel = 0.1f;

    private Vector3 centreOfRotation;
    private Vector3 mousePosLastFrame;
    private bool rotating;

    // Start is called before the first frame update
    void Start()
    {
        rotating = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Camera Panning
        float zoomPanModifier = this.gameObject.transform.position.y / maxZoomLevel; 
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            Vector3 panDirection = Vector3.Normalize(new Vector3(-this.transform.right.x, 0, -this.transform.right.z));
            this.transform.position += panDirection * zoomPanModifier * panSpeed;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            Vector3 panDirection = Vector3.Normalize(new Vector3(this.transform.right.x, 0, this.transform.right.z));
            this.transform.position +=  panDirection * zoomPanModifier * panSpeed;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            Vector3 panDirection = Vector3.Normalize(new Vector3(this.transform.forward.x, 0, this.transform.forward.z));
            this.transform.position += panDirection * zoomPanModifier * panSpeed;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            Vector3 panDirection = Vector3.Normalize(new Vector3(-this.transform.forward.x, 0, -this.transform.forward.z));
            this.transform.position += panDirection * zoomPanModifier * panSpeed;
        }

        //Camera Rotation
        if(Input.GetMouseButtonDown(2))
        {
            rotating = true;
            centreOfRotation = getMousePointInWorld(Input.mousePosition);
            Debug.Log("Centre of Rotation: " + centreOfRotation);
        }

        if(Input.GetMouseButtonUp(2))
        {
            rotating = false;
        }

        if (rotating)
        {
            Vector3 difference = mousePosLastFrame - Input.mousePosition;
            difference.y = difference.y * verticaltiltSpeed;
            difference.x = difference.x * horizontaltiltSpeed;

            float totalAngle = this.gameObject.transform.rotation.eulerAngles.x + difference.y;
            if (totalAngle > 90 && totalAngle < 180)
            {
                Debug.Log("Over");
                difference.y = 90 - this.gameObject.transform.rotation.eulerAngles.x;
            }
            else if(totalAngle < 10)
            {
                Debug.Log("Under");
                difference.y = 10 - this.gameObject.transform.rotation.eulerAngles.x;
            }
 
            this.gameObject.transform.RotateAround(centreOfRotation, Vector3.up, difference.x);
            this.gameObject.transform.RotateAround(centreOfRotation, this.transform.right, difference.y);
        }
        
        //Camera Zooming
        if(Input.mouseScrollDelta.y != 0)
        {
            Vector3 targetZoomedPosition =  this.gameObject.transform.position + (this.gameObject.transform.forward * Input.mouseScrollDelta.y);
            if((targetZoomedPosition.y > this.gameObject.transform.position.y) && (targetZoomedPosition.y < maxZoomLevel))
            {
                //If end position of camera is higher than current position and not higher than max zoom height, then move camera.
                this.gameObject.transform.position = targetZoomedPosition;
            }
            else if ((targetZoomedPosition.y < this.gameObject.transform.position.y) && targetZoomedPosition.y > minZoomLevel)
            {
                //If end position of camera is lower than current position and not higher than min zoom height, then move camera.
                this.gameObject.transform.position = targetZoomedPosition ;
            }
        }



        //Update Data of where the mouse was last frame to setup for next update().
        mousePosLastFrame = Input.mousePosition;
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
}
