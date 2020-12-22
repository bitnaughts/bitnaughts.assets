using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour {
    
    public GameObject Canvas;

    Camera camera;
    GameObject focus;

    GameObject cursor;
    GameObject drag;

    float speed = 1f;

    void Start() {
        Canvas.SetActive(true);

        camera = this.GetComponent<Camera>();
        focus = GameObject.Find("Ship");

        cursor = GameObject.Find("Cursor");
        drag = GameObject.Find("Drag");
    }

    // Visualized via marching cubes... On update, update cubes to any granular damage
    public float dragSpeed = .25f;
    private Vector3 dragOrigin;

    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0) && Input.mousePosition.x > Screen.width / 4 && Input.mousePosition.x < 3 * Screen.width / 4 )
        {
            dragOrigin = Input.mousePosition;
            cursor.SetActive(true);
            cursor.transform.position = Input.mousePosition;
            drag.GetComponent<RectTransform> ().sizeDelta = Vector2.zero;
            return;
        }
        if (!Input.GetMouseButton(0)) 
        {
            cursor.SetActive(false);
            return;
        }
        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        float magnitude = (Input.mousePosition - dragOrigin).magnitude;
        if (magnitude > 1) {
            drag.GetComponent<RectTransform> ().localPosition = (Input.mousePosition - dragOrigin) / 2f;
            drag.GetComponent<RectTransform> ().sizeDelta = new Vector2 (magnitude, 4f);
            drag.GetComponent<RectTransform> ().localRotation = Quaternion.Euler (
                new Vector3 (0, 0, Mathf.Rad2Deg * Mathf.Atan ((dragOrigin.y - Input.mousePosition.y) / (dragOrigin.x - Input.mousePosition.x)))
            );
        }
        if (cursor.activeSelf) transform.Translate(pos, Space.World);  
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