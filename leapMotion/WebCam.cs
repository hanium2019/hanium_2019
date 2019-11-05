using UnityEngine;
using UnityEngine.UI;
using System.Collections;

 
public class WebCam : MonoBehaviour {
 
    float m_FieldOfView = 35;

    // Use this for initialization
    void Start () {
        //BackgroundTexture = gameObject.AddComponent<GUITexture>();
        //BackgroundTexture.pixelInset = new Rect(0,0,Screen.width,Screen.height);

        WebCamDevice[] devices = WebCamTexture.devices;

        // for debugging purposes, prints available devices to the console
        for(int i = 0; i < devices.Length; i++)
        {
            print("Webcam available: " + devices[i].name);
        }
        
        WebCamTexture web = new WebCamTexture(devices[2].name);//new WebCamTexture(1280,720,60);
        //GetComponent<MeshRenderer>().material.mainTexture = web;
        // GetComponent<RawImage>().texture = web;
        GetComponent<Image>().material.mainTexture = web;

        web.Play();
        //BackgroundTexture.texture = CameraTexture;
    }
    
    // Update is called once per frame
    void Update () {
        //Camera.main.fieldOfView = m_FieldOfView;
    }
}
