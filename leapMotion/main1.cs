using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Leap.Unity;
using Leap;

public class Main : MonoBehaviour
{
    public GameObject mainCamera;
    //public GameObject capsuleHandRight;
    //public GameObject capsuleHandLeft;
    public HandModelBase rightHandModel;
    public HandModelBase leftHandModel;
    public Leap.Hand rightHand;
    public Leap.Hand leftHand;
    public ProximityDetector rightProximityDetector;
    public ProximityDetector leftProximityDetector;
    public Text text;
    public bool rightPinch;
    public bool leftPinch;
    //public bool rightGrab;
    //public bool leftGrab;
    public bool rightExtend;
    public bool leftExtend;
    public bool rightWindow;
    public bool leftWindow;
    
    public bool windowOn;

    public Vector3 offset;
    
    public GameObject windowBase;
    public GameObject[] windows;
    public int index;
    private IEnumerator rightHandCoroutine;
    private IEnumerator leftHandCoroutine;
    

    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        rightHandCoroutine = rightHandWatcher();
        leftHandCoroutine = leftHandWatcher();
        //text = GameObject.Find("Text").GetComponent<Text>();
        
    }

    public void RightHandWatcherStart() {
        StartCoroutine(rightHandCoroutine);
    }

    public void RightHandWatcherStop() {
        StopCoroutine(rightHandCoroutine);
    }

    public void LeftHandWatcherStart() {
        StartCoroutine(leftHandCoroutine);
    }

    public void LeftHandWatcherStop() {
        StopCoroutine(leftHandCoroutine);
    }

    private IEnumerator rightHandWatcher() {
      while (true){
        if (rightHandModel != null){
            rightHand = rightHandModel.GetLeapHand();
            if (rightHand != null){
                //Debug.Log(rightProximityDetector.CurrentObject);
                //if (rightHand.GrabStrength >= 0.85f)  Debug.Log("Right Grabbing");
                if (rightHand.PinchStrength >= 0.85f) {
                    Debug.Log("Right Pinching");
                    if (!rightPinch && rightProximityDetector.CurrentObject != null) {
                        offset = rightProximityDetector.CurrentObject.transform.position - rightHand.PalmPosition.ToVector3();
                        rightPinch = true;
                    }
                    rightExtend = false;
                } else if (rightHand.GrabStrength <= 0.15f) {
                    Debug.Log("Right AllExtend");
                    rightPinch = false;
                    rightExtend = true;
                } else {
                    offset = Vector3.zero;
                    rightPinch = false;
                    rightExtend = false;
                }

                if (rightExtend) {
                    if (rightHand.PalmVelocity.z >= 0.6f) {
                        Debug.Log("Right Window TurnOff");
                        rightWindow = false;
                    } else if (rightHand.PalmVelocity.z <= -0.6f) {
                        Debug.Log("Right Window TurnOn");
                        rightWindow = true;
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.1f);
      }
    }

    private IEnumerator leftHandWatcher() {
      while (true){
        if (leftHandModel != null){
            leftHand = leftHandModel.GetLeapHand();
            if (leftHand != null){
                //if (leftHand.GrabStrength >= 0.85f)  Debug.Log("Left Grabbing");
                
                if (leftHand.PinchStrength >= 0.85f) {
                    Debug.Log("Left Pinching");
                    if (!leftPinch && leftProximityDetector.CurrentObject != null) {
                        offset = leftProximityDetector.CurrentObject.transform.position - leftHand.PalmPosition.ToVector3();
                        leftPinch = true;
                    }
                    leftExtend = false;
                } else if (leftHand.GrabStrength <= 0.15f) {
                    Debug.Log("Left AllExtend");
                    leftPinch = false;
                    leftExtend = true;
                } else {
                    offset = Vector3.zero;
                    leftPinch = false;
                    leftExtend = false;
                }

                if (leftExtend) {
                    if (leftHand.PalmVelocity.z >= 0.6f) {
                        Debug.Log("Left Window TurnOff");
                        leftWindow = false;
                    } else if (leftHand.PalmVelocity.z <= -0.6f) {
                        Debug.Log("Left Window TurnOn");
                        leftWindow = true;
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.1f);
      }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (index < 10 && rightWindow && leftWindow) {
            windowOn = true;
            index++;
            windows[index] = (GameObject) Instantiate(windowBase, new Vector3(0.2f*index-1.0f, 0.2f*index-1.0f, 0.5f), Quaternion.identity) as GameObject;
        }
        if (index >= 1 && !rightWindow && !leftWindow) {
            windowOn = false;
            for (index = 10; index > 0; index--)
                Destroy (windows[index], 0.0f);
            
        }
        
    }
}
