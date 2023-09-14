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

    float speed = 1f;
    Vector3 panOrigin;
    public bool bDragging;
    public string orientation = "Horizontal";
    // Attach this script to a camera, this will make it render in wireframe
    // void OnPreRender()
    // {
    //     GL.wireframe = true;
    // }

    // void OnPostRender()
    // {
    //     GL.wireframe = false;
    // }
    void Start() {

        camera = this.GetComponent<Camera>();
        // focus = GameObject.Find("Station");
        // cursor = GameObject.Find("Cursor");
        example = GameObject.Find("Example");
        Interactor = GameObject.Find("ScreenCanvas").GetComponent<Interactor>();
        // example.SetActive(false);
    }

    // Visualized via marching cubes... On update, update cubes to any granular damage
    public float dragSpeed = .125f;
    private Vector3 dragOrigin;
    private Vector3 oldPos;
    private bool wasZoomingLastFrame; // Touch mode only
    private Vector2[] lastZoomPositions; // Touch mode only

    public float waitTime, waitTime2;
    float timer = 0f;

    bool CheckInsideEdge() {
        // print (orientation);
        if (orientation == "Horizontal" && Input.mousePosition.x > Screen.width / 2) return false;
        if (orientation == "Verticle" && Input.mousePosition.y < Screen.height / 2) return false;
        return (Input.mousePosition.y > 114 && Input.mousePosition.y < Screen.height - 150 && Input.mousePosition.x > 114 && Input.mousePosition.x < Screen.width - 114)
        && !(Input.mousePosition.y < 535 && Input.mousePosition.y > 265 && Input.mousePosition.x < Screen.width - 265 && Input.mousePosition.x > Screen.width - 535)
        && !(Input.mousePosition.y < 535 && Input.mousePosition.y > 265 && Input.mousePosition.x > 265 && Input.mousePosition.x < 535); //175 to 725 from bottom left and right corners for Joystick/use weapon input for tutorial
    }
    int component = 0;
    public void ToggleView() {
        string[] components = Interactor.Ship.GetInteractiveComponents();
        component = (component + 1) % components.Length;
        Interactor.RenderComponent(components[component]);
        Interactor.OverlayInteractor.gameObject.SetActive(true);
        for (int i = 0; i < Interactor.OverlayInteractor.OverlayDropdown.options.Count; i++) {
            if (Interactor.OverlayInteractor.OverlayDropdown.options[i].text == components[component]) Interactor.OverlayInteractor.OverlayDropdown.value = i; 
        }
        this.transform.SetParent(GameObject.Find(components[component]).transform);
        this.transform.position = new Vector3(0, -100, 0);
        this.transform.localEulerAngles = new Vector3(0, 0, 0);
        Interactor.OverlayInteractor.OnDropdownChange(0);
        Interactor.Sound("Toggle");
    }
    float delay_after_zoom = 0;
    void LateUpdate()
    {
        if (Screen.height > Screen.width) {
            // // choose the margin randomly
            // float margin = UnityEngine.Random.Range(0.0f, 0.3f);
            // // setup the rectangle
            // GetComponent<Camera>().rect = new Rect(margin, 0.0f, 1.0f - margin * 2.0f, 1.0f);
            GameObject.Find("WorldPanel").GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f);
            GameObject.Find("WorldPanel").GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
            GameObject.Find("InterpreterPanel").GetComponent<RectTransform>().anchorMin = new Vector2(0, 0f);
            GameObject.Find("InterpreterPanel").GetComponent<RectTransform>().anchorMax = new Vector2(1f, 0.5f);
            GameObject.Find("WorldPanel").GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            GameObject.Find("WorldPanel").GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            GameObject.Find("InterpreterPanel").GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            GameObject.Find("InterpreterPanel").GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            GetComponent<Camera>().rect = new Rect(0.0f, 0.5f, 1f, 0.5f);
            orientation = "Verticle";
        }
        else {
            // // choose the margin randomly
            // float margin = UnityEngine.Random.Range(0.0f, 0.3f);
            // // setup the rectangle
            // GetComponent<Camera>().rect = new Rect(margin, 0.0f, 1.0f - margin * 2.0f, 1.0f);
            GetComponent<Camera>().rect = new Rect(0.0f, 0.0f, 0.5f, 1f);
            GameObject.Find("WorldPanel").GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            GameObject.Find("WorldPanel").GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1f);
            GameObject.Find("InterpreterPanel").GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
            GameObject.Find("InterpreterPanel").GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
            GameObject.Find("WorldPanel").GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            GameObject.Find("WorldPanel").GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            GameObject.Find("InterpreterPanel").GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            GameObject.Find("InterpreterPanel").GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            orientation = "Horizontal";    
        }
        if (Interactor.Stage == "MapZoom") {
            return;
        }
        // if (Input.GetKeyDown(KeyCode.Space)) {
        //     Interactor.Sound("Toggle");
        //     this.transform.SetParent(GameObject.Find("World").GetComponentsInChildren<StructureController>()[0].transform);
        //     this.transform.position = new Vector3(0, -100, 0);
        //     this.transform.localEulerAngles = new Vector3(0, 0, 0);
        // }
        if (CheckInsideEdge() || Interactor.Stage == "MapInterface")
        {
            // if (Input.GetAxis("Mouse ScrollWheel") != 0 && Interactor.Stage != "MapInterface") {
            //     if (GameObject.Find("Dropdown List") == null) { // && EventSystem.current.currentSelectedGameObject == null
            //         GetComponent<Camera>().orthographicSize = Mathf.Clamp(GetComponent<Camera>().orthographicSize - Input.GetAxis("Mouse ScrollWheel") * GetComponent<Camera>().orthographicSize, 6f, 100f);
                    
            //         if (Interactor.OverlayInteractor.gameObject.activeSelf) Interactor.OverlayInteractor.Resize();
                    
                    
            //         // Interactor.PanTutorial();
            //     }
            // }
            // else if (Input.GetAxis("Mouse ScrollWheel") > 0 && Interactor.Stage == "MapInterface") Interactor.MapZoom();
            if (Input.touchCount == 2 && Interactor.Stage != "MapInterface") {
                delay_after_zoom = .1f;
                Vector2[] newPositions = new Vector2[]{Input.GetTouch(0).position, Input.GetTouch(1).position};
                if (!wasZoomingLastFrame) {
                    wasZoomingLastFrame = true;
                    lastZoomPositions = newPositions;
                } else {
                    float offset = Vector2.Distance(newPositions[0], newPositions[1]) - Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
                    this.GetComponent<Camera>().orthographicSize = Mathf.Clamp(GetComponent<Camera>().orthographicSize - (offset/10f), 6f, 100f);
                    if (Interactor.OverlayInteractor.gameObject.activeSelf) Interactor.OverlayInteractor.Resize();
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
                    if(Input.GetMouseButtonDown(0) && Interactor.OverlayInteractor.gameObject.activeSelf == false)
                    {
                        bDragging = true;
                        oldPos = transform.position;
                        //Get the ScreenVector the mouse clicked
                        //https://answers.unity.com/questions/827834/click-and-drag-camera.html#:~:text=Click%20and%20Drag%20Camera%20-%20Unity%20Answers%20void,%2F%2FGet%20the%20ScreenVector%20the%20mouse%20clicked%20%7D%20if%28Input.GetMouseButton%280%29%29
                        panOrigin = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                    }
                    if(Input.GetMouseButton(0) && Interactor.OverlayInteractor.gameObject.activeSelf == false && bDragging)
                    {
                        //Get the difference between where the mouse clicked and where it moved
                        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition) - panOrigin;// + new Vector3(example.GetComponent<StructureController>().translation.x * Time.deltaTime, example.GetComponent<StructureController>().translation.y * Time.deltaTime, 0);    
                        //Move the position of the camera to simulate a drag, speed * 10 for screen to worldspace conversion
                        // transform.position = new Vector3(oldPos.x + -pos.x * GetComponent<Camera>().orthographicSize * 2f, 100, oldPos.y + -pos.y * GetComponent<Camera>().orthographicSize * 2f);   
                        transform.Translate(new Vector3(
                            Mathf.Clamp(-pos.x * GetComponent<Camera>().orthographicSize * 4f, -GetComponent<Camera>().orthographicSize * 2f, GetComponent<Camera>().orthographicSize * 2f),
                            Mathf.Clamp(-pos.y * GetComponent<Camera>().orthographicSize * 2f, -GetComponent<Camera>().orthographicSize * 2f, GetComponent<Camera>().orthographicSize * 2f), 0)); 
                                        
                        // if (Interactor.Stage == "MapInterface") {
                        //     Vector2 BL_Edge = Camera.main.View(Input.mousePosition);
                        //         print (BL_Edge.ToString());
                        //     if (BL_Edge.x < -5) {
                        //         print ("-x");
                        //         transform.Translate(new Vector3(Mathf.Clamp(pos.x * GetComponent<Camera>().orthographicSize * 2f, -GetComponent<Camera>().orthographicSize * 2f, GetComponent<Camera>().orthographicSize * 2f), 0, 0));
                        //     }
                        //     if (BL_Edge.x > 95) {
                        //         print ("+x");
                        //         transform.Translate(new Vector3(Mathf.Clamp(pos.x * GetComponent<Camera>().orthographicSize * 2f, -GetComponent<Camera>().orthographicSize * 2f, GetComponent<Camera>().orthographicSize * 2f), 0, 0));
                        //     }
                        // }
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
        if (Interactor.Stage == "MapInterface") {
            // if (transform.localPosition.y > 35 + (Screen.height / 1000)) {
            //     transform.localPosition = new Vector3(transform.localPosition.x, 35, 0);
            // }
            // if (transform.localPosition.y < -75 - (Screen.height / 1000)) {
            //     transform.localPosition = new Vector3(transform.localPosition.x, -75, 0);
            // }
            // if (transform.localPosition.x > 95 + (Screen.width / 1000)) {
            //     transform.localPosition = new Vector3(100, transform.localPosition.y, 0);
            // }
            // if (transform.localPosition.x < -5 - (Screen.width / 1000)) {
            //     transform.localPosition = new Vector3(-5, transform.localPosition.y, 0);
            // }
        }
    }
    int cycle_count = 1;
    public void CycleView() {
        this.transform.SetParent(GameObject.Find("World").GetComponentsInChildren<StructureController>()[cycle_count++ % GameObject.Find("World").GetComponentsInChildren<StructureController>().Length].transform);
        this.transform.localPosition = new Vector3(0, 0, -200);
        this.transform.localEulerAngles = new Vector3(0, 0, 0);
        Interactor.CycleTutorial();
        Interactor.SetBinocular("⛭");
    }
    public void BinocularView() {
        if (Interactor.GetBinocular() == "⛯") {
            Interactor.SetBinocular("⛭");
            this.transform.SetParent(GameObject.Find("Example").transform);
            this.transform.localEulerAngles = new Vector3(0, 0, 0);
            this.transform.localPosition = new Vector3(0, 0, -200);
        } else {
            Interactor.SetBinocular("⛯");
            this.transform.SetParent(GameObject.Find("Example").transform.GetChild(0).transform);
            this.transform.localEulerAngles = new Vector3(0, 0, 0);
            this.transform.localPosition = new Vector3(0, 0, -200);
        }
    }
    public void ZoomIn() {
        if (Interactor.Stage == "MapInterface" || Interactor.Stage == "MapZoom") Interactor.MapZoom();
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize - 0.5f * GetComponent<Camera>().orthographicSize, 6f, 248f);
        if (Interactor.OverlayInteractor.gameObject.activeSelf) Interactor.OverlayInteractor.Resize();
    }
    public void ZoomOut() {
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize + 0.5f * GetComponent<Camera>().orthographicSize, 6f, 248f);
        Interactor.MapUnzoom();
        if (Interactor.OverlayInteractor.gameObject.activeSelf) Interactor.OverlayInteractor.Resize();
    }
    public void OnPanUp() {
        Interactor.Sound("Click");
        Interactor.PanTutorial();
        transform.Translate(new Vector3(0, GetComponent<Camera>().orthographicSize * .33f, 0)); 
        if (Interactor.OverlayInteractor.gameObject.activeSelf) Interactor.OverlayInteractor.Resize();
    }
    public void OnPanLeft() {
        Interactor.Sound("Click");
        Interactor.PanTutorial();
        transform.Translate(new Vector3(-GetComponent<Camera>().orthographicSize * .33f, 0, 0)); 
        if (Interactor.OverlayInteractor.gameObject.activeSelf) Interactor.OverlayInteractor.Resize();
    }
    public void OnPanRight() {
        Interactor.Sound("Click");
        Interactor.PanTutorial();
        transform.Translate(new Vector3(GetComponent<Camera>().orthographicSize * .33f, 0, 0)); 
        if (Interactor.OverlayInteractor.gameObject.activeSelf) Interactor.OverlayInteractor.Resize();
    }
    public void OnPanDown() {
        Interactor.Sound("Click");
        Interactor.PanTutorial();
        transform.Translate(new Vector3(0, -GetComponent<Camera>().orthographicSize * .33f, 0)); 
        if (Interactor.OverlayInteractor.gameObject.activeSelf) Interactor.OverlayInteractor.Resize();
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