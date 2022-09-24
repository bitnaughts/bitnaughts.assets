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
    // GameObject focus;
    // GameObject cursor;
    GameObject example;

    public Interactor Interactor;
    public OverlayInteractor OverlayInteractor;

    float speed = 1f;
    Vector3 panOrigin;
    bool bDragging;

    void Start() {

        camera = this.GetComponent<Camera>();
        // focus = GameObject.Find("Station");
        // cursor = GameObject.Find("Cursor");
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

    bool CheckInsideEdge() {
        return (Input.mousePosition.y > 60 && Input.mousePosition.y < Screen.height - 60 && Input.mousePosition.x > Screen.width / 2 && Input.mousePosition.x < Screen.width - 60);
    }
    int component = 0;
    public void ToggleView() {
        string[] components = Interactor.Ship.GetInteractiveComponents();
        component = (component + 1) % components.Length;
        Interactor.RenderComponent(components[component]);
        OverlayInteractor.gameObject.SetActive(true);
        for (int i = 0; i < OverlayInteractor.OverlayDropdown.options.Count; i++) {
            if (OverlayInteractor.OverlayDropdown.options[i].text == components[component]) OverlayInteractor.OverlayDropdown.value = i; 
        }
        this.transform.SetParent(GameObject.Find(components[component]).transform);
        this.transform.localPosition = new Vector3(0, 0, -200);
        this.transform.localEulerAngles = new Vector3(0, 0, 0);
        OverlayInteractor.OnDropdownChange();
        Interactor.Sound("Toggle");
    }

    void LateUpdate()
    {
        if (CheckInsideEdge())
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0) {
                if (GameObject.Find("Dropdown List") == null) { // && EventSystem.current.currentSelectedGameObject == null
                    GetComponent<Camera>().orthographicSize = Mathf.Clamp(GetComponent<Camera>().orthographicSize - Input.GetAxis("Mouse ScrollWheel") * GetComponent<Camera>().orthographicSize, 6f, 880f);
                    if (OverlayInteractor.gameObject.activeSelf) OverlayInteractor.Resize();
                    
                    Interactor.PanTutorial();
                }
            }
            if (Input.touchCount == 2) {
                Vector2[] newPositions = new Vector2[]{Input.GetTouch(0).position, Input.GetTouch(1).position};
                if (!wasZoomingLastFrame) {
                    wasZoomingLastFrame = true;
                    lastZoomPositions = newPositions;
                } else {
                    float offset = Vector2.Distance(newPositions[0], newPositions[1]) - Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
                    this.GetComponent<Camera>().orthographicSize = Mathf.Clamp(GetComponent<Camera>().orthographicSize - (offset/10f), 6f, 880f);
                    if (OverlayInteractor.gameObject.activeSelf) OverlayInteractor.Resize();
                    lastZoomPositions = newPositions;
                    Interactor.PanTutorial();
                }
            } else {
                wasZoomingLastFrame = false;
                if(Input.GetMouseButtonDown(0) && OverlayInteractor.gameObject.activeSelf == false)
                {
                    bDragging = true;
                    oldPos = transform.position;
                    //Get the ScreenVector the mouse clicked
                    //https://answers.unity.com/questions/827834/click-and-drag-camera.html#:~:text=Click%20and%20Drag%20Camera%20-%20Unity%20Answers%20void,%2F%2FGet%20the%20ScreenVector%20the%20mouse%20clicked%20%7D%20if%28Input.GetMouseButton%280%29%29
                    panOrigin = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                }
                if(Input.GetMouseButton(0) && OverlayInteractor.gameObject.activeSelf == false && bDragging)
                {
                    //Get the difference between where the mouse clicked and where it moved
                    Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition) - panOrigin;    
                    //Move the position of the camera to simulate a drag, speed * 10 for screen to worldspace conversion
                    transform.position = new Vector3(oldPos.x + -pos.x * GetComponent<Camera>().orthographicSize * 2f, oldPos.y + -pos.y * GetComponent<Camera>().orthographicSize * 2f, -10f);   
                }
                if(Input.GetMouseButtonUp(0) && bDragging)
                {
                    Interactor.PanTutorial();
                    bDragging = false;
                }
            }
        }
    }
    public void ZoomIn() {
        if (camera.orthographicSize > 20) camera.orthographicSize -= 20;
    }
    public void ZoomOut() {
        camera.orthographicSize += 20;
    }
    public void OnPanUp() {
        Interactor.Sound("Click");
        Interactor.PanTutorial();
        transform.Translate(new Vector3(0, GetComponent<Camera>().orthographicSize * .33f, 0)); 
        if (OverlayInteractor.gameObject.activeSelf) OverlayInteractor.Resize();
    }
    public void OnPanLeft() {
        Interactor.Sound("Click");
        Interactor.PanTutorial();
        transform.Translate(new Vector3(-GetComponent<Camera>().orthographicSize * .33f, 0, 0)); 
        if (OverlayInteractor.gameObject.activeSelf) OverlayInteractor.Resize();
    }
    public void OnPanRight() {
        Interactor.Sound("Click");
        Interactor.PanTutorial();
        transform.Translate(new Vector3(GetComponent<Camera>().orthographicSize * .33f, 0, 0)); 
        if (OverlayInteractor.gameObject.activeSelf) OverlayInteractor.Resize();
    }
    public void OnPanDown() {
        Interactor.Sound("Click");
        Interactor.PanTutorial();
        transform.Translate(new Vector3(0, -GetComponent<Camera>().orthographicSize * .33f, 0)); 
        if (OverlayInteractor.gameObject.activeSelf) OverlayInteractor.Resize();
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