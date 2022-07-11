using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
 

public class CameraController : MonoBehaviour {
    Camera camera;
    GameObject focus;
    GameObject cursor;
    GameObject example;

    public OverlayInteractor OverlayInteractor;

    float speed = 1f;

    void Start() {

        camera = this.GetComponent<Camera>();
        focus = GameObject.Find("Station");
        cursor = GameObject.Find("Cursor");
        // example = GameObject.Find("Example");
        // example.SetActive(false);
    }

    // Visualized via marching cubes... On update, update cubes to any granular damage
    public float dragSpeed = .25f;
    private Vector3 dragOrigin;
    private Vector3 oldPos;
    private bool wasZoomingLastFrame; // Touch mode only
    private Vector2[] lastZoomPositions; // Touch mode only

    public float waitTime, waitTime2;
    float timer = 0f;



    void LateUpdate()
    {
       
    //     if (timer < 5.5f) {
    //         GetComponent<AudioSource>().volume = 0f;
    //         focus.GetComponent<AudioSource>().volume = 1f;
    //     }
    //     if (timer > 5.5f && timer < 6.5f) {
    //         // if (timer > .5f && timer < 5.5f) {
    //             // this.GetComponent<Camera>().orthographicSize = 64f - (int)((timer-.5f) * 1.325f) * 7.547f;
    //         // }
    //         GetComponent<AudioSource>().volume = Mathf.Lerp(0f, 1f, timer-5.5f);
    //         focus.GetComponent<AudioSource>().volume = Mathf.Lerp(1f, 0f, timer-5.5f);
    //     }
    //     if (timer > 20 && timer < 29) {
    //         this.GetComponent<Camera>().orthographicSize = 40f;
    //         transform.position = new Vector3(-20f + (int)(timer - 20) * 5, 240f - (int)(timer - 20) * 5, -200f);
    //     }

    //     if (timer > 30 && timer < 31) {
    //         this.GetComponent<Camera>().orthographicSize = 25f;
    //         transform.position = new Vector3(0, 0f, -200f);
    //     }
    //     if (timer > 28.5f && timer < 32.5f) {
    //         focus.GetComponent<AudioSource>().volume = Mathf.Lerp(0f, 0.5f, (timer - 28.5f)/4f);
    //         if (timer < 29.5f) GetComponent<AudioSource>().volume = Mathf.Lerp(1f, 0f, timer-28.5f);
    //     }
    //     if (timer > 45 && timer < 46) {
    //         example.SetActive(true);
    //     }
    //     if (timer > 70 && timer < 71) {
    //         focus.GetComponent<AudioSource>().volume = Mathf.Lerp(0.5f, 0.0f, timer - 70);
    //         GetComponent<AudioSource>().clip = IntroCallback;
    //         GetComponent<AudioSource>().volume = 1f;
    //         GetComponent<AudioSource>().Play();
    //     }
    //      if (timer > 76 && timer < 79) {
    //         this.GetComponent<Camera>().orthographicSize = 20f;// - (int)((timer-76) * 2);
    //         transform.position = new Vector3(20, 200f, -200f);
    //     }
    //      if (timer > 79 && timer < 82) {
    //         this.GetComponent<Camera>().orthographicSize = 20f;// - (int)((timer-76) * 2);
    //         transform.position = new Vector3(-20, 240f, -200f);
    //     }
    //     if (timer > 82 && timer < 83) {
    //         this.GetComponent<Camera>().orthographicSize = 240f;
    //         transform.position = new Vector3(0, 175f, -200f);
    //     }
    //     if (timer > 93 && timer < 98) {
    //         focus.GetComponent<AudioSource>().volume = Mathf.Lerp(0.00f, 0.5f, (timer - 93)/5f);
    //         this.GetComponent<Camera>().orthographicSize = 140f;
    //     }
    //     if (timer > 120 && timer < 135) {
    //         this.GetComponent<Camera>().orthographicSize = 140f + (int)((timer-120) * 1) * 20;
    //         focus.GetComponent<AudioSource>().volume = Mathf.Lerp(0.5f, 0f, (timer - 120)/15f);
    //     }
    //     if (Input.mousePosition.x > Screen.width / 2) //&& Input.mousePosition.x < 3 * Screen.width / 4 )  
    //     {
    //         if (Input.GetAxis("Mouse ScrollWheel") != 0) {
    //             if (GameObject.Find("Dropdown List") == null) { // && EventSystem.current.currentSelectedGameObject == null
    //                 GetComponent<Camera>().orthographicSize = Mathf.Clamp(GetComponent<Camera>().orthographicSize - Input.GetAxis("Mouse ScrollWheel") * GetComponent<Camera>().orthographicSize / 10, 6f, 800f);
    //                 if (OverlayInteractor.gameObject.activeSelf) OverlayInteractor.Resize();
    //             }
    //         }
    //         if (Input.touchCount == 2) {
    //             Vector2[] newPositions = new Vector2[]{Input.GetTouch(0).position, Input.GetTouch(1).position};
    //             if (!wasZoomingLastFrame) {
    //                 wasZoomingLastFrame = true;
    //                 lastZoomPositions = newPositions;
    //             } else {
    //                 float offset = Vector2.Distance(newPositions[0], newPositions[1]) - Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
    //                 this.GetComponent<Camera>().orthographicSize = Mathf.Clamp(GetComponent<Camera>().orthographicSize - offset, 6f, 800f);
    //                 if (OverlayInteractor.gameObject.activeSelf) OverlayInteractor.Resize();
    //                 lastZoomPositions = newPositions;
    //             }
    //         } else {
    //             wasZoomingLastFrame = false;
    //         }
    //         if (Input.GetMouseButtonDown(0) && OverlayInteractor.gameObject.activeSelf == false) { // && EventSystem.current.currentSelectedGameObject == null
    //             dragOrigin = this.GetComponent<Camera>().ScreenToViewportPoint(Input.mousePosition);
    //             cursor.SetActive(true);
    //             cursor.transform.position = Input.mousePosition;
                
    //             oldPos = transform.position;
    //             return;
    //         }
    //     }
    //     if (!Input.GetMouseButton(0)) 
    //     {
    //         cursor.SetActive(false);
    //         return;
    //     }

    //     // Vector3 change = Input.mousePosition - dragOrigin;
    //     // change.x += Screen.width / 2;
    //     // Vector3 pos = this.GetComponent<Camera>().ScreenToViewportPoint(Input.mousePosition) - dragOrigin;
    //     // if (cursor.activeSelf && !OverlayInteractor.gameObject.activeSelf) {
    //     //     // pos.x *= dragSpeed;
    //     //     // pos.y *= dragSpeed;   
    //     //     transform.position = oldPos + -pos * GetComponent<Camera>().orthographicSize * 2f;
    //     // }

    //     // dragOrigin = Input.mousePosition;
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