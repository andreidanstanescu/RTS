using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class Input : MonoBehaviour
{
    private Player jucator;

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
            Roteste();
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
        if(currx <= totalx && currx > totalx - GameService.GetSenzitivity()) 
            coords.x += GameService.ScrollDim;
        

        if(currx >= 0 && currx < GameService.GetSenzitivity())
            coords.x -= GameService.ScrollDim;

        if(curry <= totaly && curry > totaly - GameService.GetSenzitivity()) 
            coords.z += GameService.ScrollDim;
        

        if(curry >= 0 && curry < GameService.GetSenzitivity())
            coords.z -= GameService.ScrollDim;

        coords = Camera.main.transform.TransformDirection(coords);
        coords.y = 0;

        //coords.y -= GameService.ScrollDim*UnityEngine.Input.GetAxis("Mouse ScrollWheel");

        //pentru zoom in/zoom out

        if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (Camera.main.fieldOfView <= 125)
                Camera.main.fieldOfView += 2;
            if (Camera.main.orthographicSize <= 20)
                Camera.main.orthographicSize += 0.5f;
 
        }

        if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (Camera.main.fieldOfView > 2)
                Camera.main.fieldOfView -= 2;
            if (Camera.main.orthographicSize >= 1)
                Camera.main.orthographicSize -= 0.5f;
        }

        Vector3 cadru = Camera.main.transform.position;
        Vector3 dest = cadru;
        dest.x += coords.x;
        dest.y += coords.y;
        dest.z += coords.z;

        //limitez rezolutia pe ecran ca in jocurile din anii '90
        if(dest.y > GameService.MaxCameraHeight) {
            dest.y = GameService.MaxCameraHeight;
        } else if(dest.y < GameService.MinCameraHeight) {
            dest.y = GameService.MinCameraHeight;
        }

        if(dest != cadru)
            Camera.main.transform.position = Vector3.MoveTowards(cadru, dest, Time.deltaTime * GameService.ScrollDim);

    }

    //daca il conving pe bogdan sa-l facem 3D 
    private void Roteste(){
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
    }
}
