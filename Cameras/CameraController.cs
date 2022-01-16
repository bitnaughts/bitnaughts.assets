using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour {
    
    Camera camera;
    GameObject focus;

    GameObject cursor;

    float speed = 1f;

    void Start() {

        camera = this.GetComponent<Camera>();
        focus = GameObject.Find("Ship");

        cursor = GameObject.Find("Cursor");
    }

    // Visualized via marching cubes... On update, update cubes to any granular damage
    public float dragSpeed = .25f;
    private Vector3 dragOrigin;
    private Vector3 oldPos;
    void LateUpdate()
    {
        if (Input.mousePosition.x > Screen.width / 2) //&& Input.mousePosition.x < 3 * Screen.width / 4 ) 
        {
            GetComponent<Camera>().orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * GetComponent<Camera>().orthographicSize / 5;
            if (GetComponent<Camera>().orthographicSize < 5) GetComponent<Camera>().orthographicSize = 5; 
            if (GetComponent<Camera>().orthographicSize > 800) GetComponent<Camera>().orthographicSize = 800;
            if (Input.GetMouseButtonDown(0))
            {
                dragOrigin = this.GetComponent<Camera>().ScreenToViewportPoint(Input.mousePosition);
                cursor.SetActive(true);
                cursor.transform.position = Input.mousePosition;
                
                oldPos = transform.position;
                return;
            }
        }
        if (!Input.GetMouseButton(0)) 
        {
            cursor.SetActive(false);
            return;
        }

        // Vector3 change = Input.mousePosition - dragOrigin;
        // change.x += Screen.width / 2;
        Vector3 pos = this.GetComponent<Camera>().ScreenToViewportPoint(Input.mousePosition) - dragOrigin;
        if (cursor.activeSelf) {
            // pos.x *= dragSpeed;
            // pos.y *= dragSpeed;   
            transform.position = oldPos + -pos * GetComponent<Camera>().orthographicSize * 2f;
        }
        // dragOrigin = Input.mousePosition;
    }

    public void ZoomIn() {
        
        if (camera.orthographicSize > 20) camera.orthographicSize -= 20;
    }
    public void ZoomOut() {
        camera.orthographicSize += 20;
    }
    public void ReloadScene() {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public float GetAngle(Vector2 pointA, Vector2 pointB)
    {
        var target = pointB - pointA;
        var angle = Vector2.Angle(pointA, pointB);
        var orientation = Mathf.Sign(pointA.x*target.y - pointA.y*target.x);
        return (360 - orientation*angle)%360;
    }
}