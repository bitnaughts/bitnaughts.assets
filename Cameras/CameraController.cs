using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
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
        example = GameObject.Find("Example");
        Interactor = GameObject.Find("ScreenCanvas").GetComponent<Interactor>();
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
        return (Input.mousePosition.y > 114 && Input.mousePosition.y < Screen.height - 150 && Input.mousePosition.x > 114 && Input.mousePosition.x < Screen.width - 114)
        && !(Input.mousePosition.y < 535 && Input.mousePosition.y > 265 && Input.mousePosition.x < Screen.width - 265 && Input.mousePosition.x > Screen.width - 535)
        && !(Input.mousePosition.y < 535 && Input.mousePosition.y > 265 && Input.mousePosition.x > 265 && Input.mousePosition.x < 535); //175 to 725 from bottom left and right corners for Joystick/use weapon input for tutorial
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
        this.transform.position = new Vector3(0, -100, 0);
        this.transform.localEulerAngles = new Vector3(0, 0, 0);
        OverlayInteractor.OnDropdownChange("");
        Interactor.Sound("Toggle");
    }
    float delay_after_zoom = 0;
    void LateUpdate()
    {
        // if (Input.GetKeyDown(KeyCode.Space)) {
        //     Interactor.Sound("Toggle");
        //     this.transform.SetParent(GameObject.Find("World").GetComponentsInChildren<StructureController>()[0].transform);
        //     this.transform.position = new Vector3(0, -100, 0);
        //     this.transform.localEulerAngles = new Vector3(0, 0, 0);
        // }
        if (CheckInsideEdge())
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0 && Interactor.Stage != "MapInterface") {
                if (GameObject.Find("Dropdown List") == null) { // && EventSystem.current.currentSelectedGameObject == null
                    GetComponent<Camera>().orthographicSize = Mathf.Clamp(GetComponent<Camera>().orthographicSize - Input.GetAxis("Mouse ScrollWheel") * GetComponent<Camera>().orthographicSize, 6f, 100f);
                    if (OverlayInteractor.gameObject.activeSelf) OverlayInteractor.Resize();
                    
                    Interactor.PanTutorial();
                }
            }
            if (Input.touchCount == 2 && Interactor.Stage != "MapInterface") {
                delay_after_zoom = .1f;
                Vector2[] newPositions = new Vector2[]{Input.GetTouch(0).position, Input.GetTouch(1).position};
                if (!wasZoomingLastFrame) {
                    wasZoomingLastFrame = true;
                    lastZoomPositions = newPositions;
                } else {
                    float offset = Vector2.Distance(newPositions[0], newPositions[1]) - Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
                    this.GetComponent<Camera>().orthographicSize = Mathf.Clamp(GetComponent<Camera>().orthographicSize - (offset/10f), 6f, 100f);
                    if (OverlayInteractor.gameObject.activeSelf) OverlayInteractor.Resize();
                    lastZoomPositions = newPositions;
                    Interactor.PanTutorial();
                }
            } else {
                wasZoomingLastFrame = false;
                if (delay_after_zoom > 0) {
                    delay_after_zoom -= Time.deltaTime;
                    oldPos = transform.position;
                    panOrigin = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                } else {
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
                        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition) - panOrigin;// + new Vector3(example.GetComponent<StructureController>().translation.x * Time.deltaTime, example.GetComponent<StructureController>().translation.y * Time.deltaTime, 0);    
                        //Move the position of the camera to simulate a drag, speed * 10 for screen to worldspace conversion
                        // transform.position = new Vector3(oldPos.x + -pos.x * GetComponent<Camera>().orthographicSize * 2f, 100, oldPos.y + -pos.y * GetComponent<Camera>().orthographicSize * 2f);   
                        transform.Translate(new Vector3(Mathf.Clamp(-pos.x * GetComponent<Camera>().orthographicSize * 2f, -GetComponent<Camera>().orthographicSize * 2f, GetComponent<Camera>().orthographicSize * 2f), Mathf.Clamp(-pos.y * GetComponent<Camera>().orthographicSize * 2f, -GetComponent<Camera>().orthographicSize * 2f, GetComponent<Camera>().orthographicSize * 2f), 0)); 
                        panOrigin = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                    }
                    if(Input.GetMouseButtonUp(0) && bDragging)
                    {
                        Interactor.PanTutorial();
                        bDragging = false;
                    }
                }
            }
        }
    }
    int cycle_count = 1;
    public void CycleView() {
       this.transform.SetParent(GameObject.Find("World").GetComponentsInChildren<StructureController>()[cycle_count++ % GameObject.Find("World").GetComponentsInChildren<StructureController>().Length].transform);
       this.transform.localPosition = new Vector3(0, 0, -200);
       this.transform.localEulerAngles = new Vector3(0, 0, 0);
       GameObject.Find("BinocularToggleText").GetComponent<Text>().text = "⛭";
       Interactor.CycleTutorial();
    }
    public void BinocularView() {
        if (GameObject.Find("BinocularToggleText")) {
            if (GameObject.Find("BinocularToggleText").GetComponent<Text>().text == "⛯") {
                GameObject.Find("BinocularToggleText").GetComponent<Text>().text = "⛭";
                this.transform.SetParent(GameObject.Find("Example").transform);
                this.transform.localEulerAngles = new Vector3(0, 0, 0);
                this.transform.localPosition = new Vector3(0, 0, -200);
            } else {
                GameObject.Find("BinocularToggleText").GetComponent<Text>().text = "⛯";
                this.transform.SetParent(GameObject.Find("Example").transform.GetChild(0).transform);
                this.transform.localEulerAngles = new Vector3(0, 0, 0);
                this.transform.localPosition = new Vector3(0, 0, -200);
            }
        }
       Interactor.BinocularTutorial();
    }
    public void ZoomIn() {
        Interactor.MapZoom();
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize - 0.5f * GetComponent<Camera>().orthographicSize, 6f, 250f);
    }
    public void ZoomOut() {
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize + 0.5f * GetComponent<Camera>().orthographicSize, 6f, 250f);
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