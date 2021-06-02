using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class PlayerInput : MonoBehaviour
{
    private Player jucator;
    private bool lockMousePressed = false;

    // Start is called before the first frame update
    void Start()
    {
        //functie din Unity care se duce la obiectul Player creat
        //si il referentiaza
        jucator = transform.root.GetComponent< Player >();
    }

    // Update is called once per frame
    void Update()
    {
        //cand joaca o persoana reala(nu un bot)
        //vrem sa putem misca camera liber
        if(jucator.is_player){
            //Debug.Log("Jucator\n");
            if(Input.GetKeyDown(KeyCode.Escape)) 
                OpenPauseMenu();

            Misca();

            MouseCapture();
            //Roteste();

            MouseControl();
        }
    }

    private void OpenPauseMenu() {
        Time.timeScale = 0.0f;
        GetComponentInChildren< PauseMenu >().enabled = true;
        GetComponent< PlayerInput >().enabled = false;
        Cursor.visible = true;
        GameService.MenuOpen = true;
    }

    private void Misca(){
        //detectez coordonatele curente ale mouselui 
        float currx = UnityEngine.Input.mousePosition.x;
        float curry = UnityEngine.Input.mousePosition.y;
        float totalx = Screen.width;
        float totaly = Screen.height;
        int miscat = 0;

        //creez un nou vector de coordonate
        //care reprezinta cu cat vreau sa ma deplasez pe mapa
        Vector3 coords = new Vector3(0,0,0);

        //y reprezinta inaltimea

        // DE REZOLVAT DIFERENTA DE VITEZA INTRE MISCAREA CAMEREI STANGA DREAPTA SI SUS JOS
        if(currx <= totalx && currx > totalx - GameService.GetSenzitivity()) {
            //Debug.Log("misca");
            coords.x += GameService.SCROLL_DIM;
            GameService.changeCursor("misca");
            jucator.hud.SetCustomCursor();
            miscat = 1;
        }
        else if(currx >= 0 && currx < GameService.GetSenzitivity()){
            coords.x -= GameService.SCROLL_DIM;
            GameService.changeCursor("misca");
            jucator.hud.SetCustomCursor();
            miscat = 1;
        }

        //fiind 2D template-ul, la noi va fi coords.y in loc de z (am setat camera pe orthographic)

        if(curry <= totaly + GameService.GetSenzitivity() && curry > totaly - GameService.GetSenzitivity()) {
            coords.y += GameService.SCROLL_DIM;
            GameService.changeCursor("down");
            jucator.hud.SetCustomCursor();
            miscat = 2;
        }
        
        else if(curry >= 0 && curry < GameService.GetSenzitivity()){
            coords.y -= GameService.SCROLL_DIM;
            GameService.changeCursor("down");
            jucator.hud.SetCustomCursor();
            miscat = 2;
        }

        coords = Camera.main.transform.TransformDirection(coords);
        //coords.y = 0;

        //coords.y -= GameService.SCROLL_DIM*UnityEngine.Input.GetAxis("Mouse ScrollWheel");

        //pentru zoom in/zoom out

        if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            GameService.changeCursor("misca");
            jucator.hud.SetCustomCursor();
            if (Camera.main.fieldOfView <= GameService.MAX_FOV)
                Camera.main.fieldOfView += GameService.DELTA_FOV;
            if (Camera.main.orthographicSize <= GameService.MAX_ORTOGRAPHIC_SIZE)
                Camera.main.orthographicSize += GameService.DELTA_ORTOGRAPHIC_SIZE;
            GameService.changeCursor("misca");
            jucator.hud.SetCustomCursor();
            miscat = 1;
        }

        if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            GameService.changeCursor("misca");
            jucator.hud.SetCustomCursor();
            if (Camera.main.fieldOfView > GameService.MIN_FOV)
                Camera.main.fieldOfView -= GameService.DELTA_FOV;
            if (Camera.main.orthographicSize >= GameService.MIN_ORTOGRAPHIC_SIZE)
                Camera.main.orthographicSize -= GameService.DELTA_ORTOGRAPHIC_SIZE;
            GameService.changeCursor("misca");
            jucator.hud.SetCustomCursor();
            miscat = 1;
        }

        Vector3 cadru = Camera.main.transform.position;
        Vector3 dest = cadru;
        dest.x += coords.x;
        dest.y += coords.y;
        dest.z += coords.z;

        //limitez rezolutia pe ecran ca in jocurile din anii '90
        if(dest.y > GameService.MAX_CAMERA_HEIGHT) {
            dest.y = GameService.MAX_CAMERA_HEIGHT;
        } else if(dest.y < GameService.MIN_CAMERA_HEIGHT) {
            dest.y = GameService.MIN_CAMERA_HEIGHT;
        }

        if(dest != cadru){
            //Debug.Log("diferit");
            Camera.main.transform.position = Vector3.MoveTowards(cadru, dest, Time.deltaTime * 20);
            /*if(miscat == 1){
            GameService.changeCursor("misca");
            jucator.hud.SetCustomCursor();
            }
            //GameService.changeCursor("select");
            if(miscat == 2)
            {
                GameService.changeCursor("down");
                jucator.hud.SetCustomCursor();
            }*/
        }

        if(miscat == 0){
            GameService.changeCursor("select");
            jucator.hud.SetCustomCursor();
        }
        else{
            GameService.changeCursor("misca");
            jucator.hud.SetCustomCursor();
        }

    }
    
    private GameObject GetCurrentObject(){
        Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)) 
            return hit.collider.gameObject;
        return null;
    }

    private Vector3 GetCollisionPoint() {
        Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)) 
            return hit.point;
        return GameService.OutOfBounds;
    }

    private void MouseCapture() {

        bool lastPressed = lockMousePressed;
        if(UnityEngine.Input.GetKey(KeyCode.LeftControl) && UnityEngine.Input.GetKey(KeyCode.Mouse0)) {
            lastPressed = lockMousePressed;
            lockMousePressed = true;
        } else {
            lockMousePressed = false;
        }

        if(lastPressed == false && this.lockMousePressed == true) {
            if(Cursor.lockState == CursorLockMode.None) {
                Cursor.lockState = CursorLockMode.Confined;
            }
            else {
                Cursor.lockState = CursorLockMode.None;
            }
        }

    }

    private void MouseControl(){
        if(UnityEngine.Input.GetMouseButtonDown(0))
            MouseLeft();
        else if (UnityEngine.Input.GetMouseButtonDown(1))
            MouseRight();
        Flick();
    }

    private void Flick(){

        if(jucator.hud.InMouse()){
            if(jucator.IsFindingBuildingLocation()) {
                jucator.FindBuildingLocation();
            } else {
            //aflu obiectul si punctul selectat
                GameObject gotoObject = GetCurrentObject();
                if(gotoObject != null){
                    if(jucator.SelectedObject)
                        jucator.SelectedObject.SetFlick(gotoObject);
                    if(gotoObject.name == "Ground")
                        return;
                    Player? alt_jucator = null;
                    try{
                        alt_jucator = gotoObject.transform.parent.GetComponent< Player >();
                    }
                    catch(UnassignedReferenceException){
                        //Debug.Log("nimic selectat");
                    }
                    //aparent da eroare chiar daca e null
                    //o sa il adaug drept Child Component direct din Unity la final
                    if(alt_jucator == null || alt_jucator != jucator)
                        return;
                    Building b = gotoObject.transform.parent.GetComponent< Building >();
                    Debug.Log(b.name);
                    if(b){
                        GameService.changeCursor("select");
                        alt_jucator.hud.SetCustomCursor();
                    }
                }
            }
        }
    }


    private void MouseLeft(){

        if(jucator.hud.InMouse()){
            if(jucator.IsFindingBuildingLocation()) {
                if(jucator.CanPlaceBuilding()) jucator.StartConstruction();
            } else {
                //aflu obiectul si punctul selectat
                Vector3 point;
                point = GetCollisionPoint();
                GameObject gotoObject = GetCurrentObject();
                if(point != GameService.OutOfBounds && gotoObject != null){
                    if(jucator.SelectedObject)
                        jucator.SelectedObject.SelectedDo(gotoObject, point, jucator);
                    if(gotoObject.name == "Ground")
                        return;
                    World worldObject = gotoObject.transform.parent.GetComponent< World >();
                    if(worldObject == null)
                        return;
                    jucator.SelectedObject = worldObject;
                    worldObject.SetSelection(true);
                }
            }
        }
    }

    private void MouseRight(){
        if(jucator.hud.InMouse() && !Input.GetKey(KeyCode.LeftAlt) && jucator.SelectedObject) {
            if(jucator.IsFindingBuildingLocation()) {
                jucator.CancelBuildingPlacement();
            } else {
                jucator.SelectedObject.SetSelection(false);
                jucator.SelectedObject = null;
            }
        }
    }



        //daca il conving pe bogdan sa-l facem 3D 
        /*private void Roteste(){
            Vector3 cadru = Camera.main.transform.eulerAngles;
            Vector3 dest = cadru;

            if(UnityEngine.Input.GetKey(KeyCode.LeftShift)) {
                //Debug.Log("roteste");
                dest.x -= UnityEngine.Input.GetAxis("Mouse Y") * GameService.RotateDim;
                dest.y += UnityEngine.Input.GetAxis("Mouse X") * GameService.RotateDim;
            }
    
            //daca am rotit camera
            if(dest != cadru) {
                Camera.main.transform.eulerAngles = Vector3.MoveTowards(cadru, dest, Time.deltaTime * GameService.RotateSpeed);
            }
        }*/
    }
