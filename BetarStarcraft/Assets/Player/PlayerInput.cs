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
            Misca();

            MouseCapture();
            //Roteste();

            MouseControl();
        }
    }

    private void Misca(){
        //detectez coordonatele curente ale mouselui 
        float currx = UnityEngine.Input.mousePosition.x;
        float curry = UnityEngine.Input.mousePosition.y;
        float totalx = Screen.width;
        float totaly = Screen.height;

        //creez un nou vector de coordonate
        //care reprezinta cu cat vreau sa ma deplasez pe mapa
        Vector3 coords = new Vector3(0,0,0);

        //y reprezinta inaltimea

        // DE REZOLVAT DIFERENTA DE VITEZA INTRE MISCAREA CAMEREI STANGA DREAPTA SI SUS JOS
        if(currx <= totalx && currx > totalx - GameService.GetSenzitivity()) {
            //Debug.Log("misca");
            coords.x += GameService.SCROLL_DIM;
        }
        else if(currx >= 0 && currx < GameService.GetSenzitivity())
            coords.x -= GameService.SCROLL_DIM;

        //fiind 2D template-ul, la noi va fi coords.y in loc de z (am setat camera pe orthographic)

        if(curry <= totaly + GameService.GetSenzitivity() && curry > totaly - GameService.GetSenzitivity()) 
            coords.y += GameService.SCROLL_DIM;
        
        else if(curry >= 0 && curry < GameService.GetSenzitivity())
            coords.y -= GameService.SCROLL_DIM;

        coords = Camera.main.transform.TransformDirection(coords);
        //coords.y = 0;

        //coords.y -= GameService.SCROLL_DIM*UnityEngine.Input.GetAxis("Mouse ScrollWheel");

        //pentru zoom in/zoom out

        if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (Camera.main.fieldOfView <= GameService.MAX_FOV)
                Camera.main.fieldOfView += GameService.DELTA_FOV;
            if (Camera.main.orthographicSize <= GameService.MAX_ORTOGRAPHIC_SIZE)
                Camera.main.orthographicSize += GameService.DELTA_ORTOGRAPHIC_SIZE;
 
        }

        if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (Camera.main.fieldOfView > GameService.MIN_FOV)
                Camera.main.fieldOfView -= GameService.DELTA_FOV;
            if (Camera.main.orthographicSize >= GameService.MIN_ORTOGRAPHIC_SIZE)
                Camera.main.orthographicSize -= GameService.DELTA_ORTOGRAPHIC_SIZE;
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
    }


    private void MouseLeft(){

        if(jucator.hud.InMouse()){
            //aflu obiectul si punctul selectat
            Vector3 point;
            point = GetCollisionPoint();
            GameObject gotoObject = GetCurrentObject();
            if(point != GameService.OutOfBounds && gotoObject != null){
                if(jucator.SelectedObject)
                    jucator.SelectedObject.SelectedDo(gotoObject, point, jucator);
                if(gotoObject.name == "Ground")
                    return;
                World worldObject = gotoObject.transform.root.GetComponent< World >();
                if(worldObject == null)
                    return;
                jucator.SelectedObject = worldObject;
                worldObject.SetSelection(true);
            }
        }

    }

    private void MouseRight(){

        if(jucator.hud.InMouse()){
            Vector3 point = GetCollisionPoint();
            if(jucator.SelectedObject){
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
