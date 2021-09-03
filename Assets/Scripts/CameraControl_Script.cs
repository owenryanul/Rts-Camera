using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script for handling player control of the game camera
public class CameraControl_Script : MonoBehaviour
{
    [Header("Camera Speeds")]
    public float panSpeed = 1; //the speed at which the camera can be panned with the arrow keys
    public float verticaltiltSpeed = 1; //the speed at which the camera can be rotated up or down
    public float horizontaltiltSpeed = 1; //the speed at which the camera can be rotated left or right
    public float zoomSpeed = 1;

    [Header("Camera Zoom Limits")]
    public float maxZoomLevel = 30; //the highest Y value the camera can be zoomed out to
    public float minZoomLevel = 0.1f; //the lowest Y value the camera can be zoomed in to

    //Internal Variables
    private Vector3 centreOfRotation; //the point around which the camera is rotated.
    private Vector3 mousePosLastUpdate; //the position of the mouse in pixal co-ordinates from the last time update was called.
    private bool isRotating; //whether or not the player is currently rotating the camera

    // Start is called before the first frame update
    void Start()
    {
        isRotating = false;
    }

    // Update is called once per frame
    void Update()
    {
        //[Camera Panning]
        panCamera();   

        //[Camera Rotation]
        //When the player holds down right click, set isRotating to true and set the centreOfRotation to where they clicked in the world.
        if(Input.GetMouseButtonDown(2))
        {
            isRotating = true;
            centreOfRotation = getMousePointInWorld();
        }

        //When the player releases right click, set isRotating to false
        if(Input.GetMouseButtonUp(2))
        {
            isRotating = false;
        }

        if (isRotating)
        {
            rotateCamera();
        }
        
        //[Camera Zooming]
        // if the mouse wheel has scrolled since the last update, zoom the camera
        if(Input.mouseScrollDelta.y != 0)
        {
            zoomCamera();
        }


        //Update Data of where the mouse was last frame to setup for next update().
        mousePosLastUpdate = Input.mousePosition;
    }

    //Handles the player using the arrow keys, wasd keys, or moving the mouse to the edge of the screen to move the camera in the x and z axis.
    private void panCamera()
    {
        //Multiplier that adjusts pan speed based on zoom level, reducing while zoomed in, increasing it while zoomed out
        float zoomPanModifier = this.gameObject.transform.position.y / maxZoomLevel;

        //Pan left and right
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) || Input.mousePosition.x <= (Screen.width * 0.05f))
        {
            Vector3 panDirection = Vector3.Normalize(new Vector3(-this.transform.right.x, 0, -this.transform.right.z));
            this.transform.position += panDirection * zoomPanModifier * panSpeed;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) || Input.mousePosition.x >= (Screen.width * 0.95f))
        {
            Vector3 panDirection = Vector3.Normalize(new Vector3(this.transform.right.x, 0, this.transform.right.z));
            this.transform.position += panDirection * zoomPanModifier * panSpeed;
        }

        //Pan Forward and back
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || Input.mousePosition.y >= (Screen.height * 0.95f))
        {
            Vector3 panDirection = Vector3.Normalize(new Vector3(this.transform.forward.x, 0, this.transform.forward.z));
            this.transform.position += panDirection * zoomPanModifier * panSpeed;
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) || Input.mousePosition.y <= (Screen.height * 0.05f))
        {
            Vector3 panDirection = Vector3.Normalize(new Vector3(-this.transform.forward.x, 0, -this.transform.forward.z));
            this.transform.position += panDirection * zoomPanModifier * panSpeed;
        }
    }

    //Rotates the camera around centreOfRotation by an amount relative to how far the player moved the mouse since the last update.
    private void rotateCamera()
    {
        //calculate the distance the mouse was moved since last update, then multiple the horizontial and vertical movement by their relative speed settings
        Vector3 difference = mousePosLastUpdate - Input.mousePosition;
        difference.y = difference.y * verticaltiltSpeed;
        difference.x = difference.x * horizontaltiltSpeed;

        //[Limit the angle of camera's vertical tilting]
        float totalAngle = this.gameObject.transform.rotation.eulerAngles.x + difference.y;
        //If the camera's new angle of rotation around the x axis would be greater than 90 and less 180, then limit the change of angle to keep the camera within those bounds
        if (totalAngle > 90 && totalAngle < 180) 
        {
            Debug.Log("Over");
            difference.y = 90 - this.gameObject.transform.rotation.eulerAngles.x;
        }
        else if (totalAngle < 10) //if the camera's new angle of rotation would be less than 10, then limit the change of angle to keep the camera within those bounds
        {
            Debug.Log("Under");
            difference.y = 10 - this.gameObject.transform.rotation.eulerAngles.x;
        }

        //[Rotate the camera]
        this.gameObject.transform.RotateAround(centreOfRotation, Vector3.up, difference.x);
        this.gameObject.transform.RotateAround(centreOfRotation, this.transform.right, difference.y);
    }

    //Zooms the camera in or out depending on which way the scrollwheel was rolled
    private void zoomCamera()
    {
        //Calculate the camera's new position by adding the camera's forward vector mulitplied by the amount the mouse wheel was scrolled and a scroll speed multiplier.
        Vector3 targetZoomedPosition = this.gameObject.transform.position + (this.gameObject.transform.forward * Input.mouseScrollDelta.y * zoomSpeed);

        //Apply upper and lower limits on watch position the camera can zoom to.
        if ((targetZoomedPosition.y > this.gameObject.transform.position.y) && (targetZoomedPosition.y < maxZoomLevel))
        {
            //If end position of camera is higher than current position and not higher than max zoom height, then move camera.
            this.gameObject.transform.position = targetZoomedPosition;
        }
        else if ((targetZoomedPosition.y < this.gameObject.transform.position.y) && targetZoomedPosition.y > minZoomLevel)
        {
            //If end position of camera is lower than current position and not higher than min zoom height, then move camera.
            this.gameObject.transform.position = targetZoomedPosition;
        }
    }

    //Returns the current position in World space that the mouse is hovering over.
    //Returns (0,0,0) if the mouse wasn't hovering over anything.
    private Vector3 getMousePointInWorld()
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
