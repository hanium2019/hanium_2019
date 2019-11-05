using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Leap.Unity;
using Leap;

public class HandWatcher : MonoBehaviour
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
    
    
    public int leftHandState; // extend : 0, pinch : 1, grasp : 2, push : 3, pull : 4
    public int rightHandState;
    public int windowState; // on : 1, off : 0

    public float pinchStrengthForce = 0.85f;
    public float grabStrengthForce = 0.15f;
    public float onoffVelocity = 0.6f;
    public Vector3 offset;
    

    public TrailRenderer rightHandTrail;
    public GameObject windowBase;
    public GameObject[] windows;
    private IEnumerator rightHandCoroutine;
    private IEnumerator leftHandCoroutine;
    public CaptureWindowProcess captureManager;    

    // Start is called before the first frame update
    void Start()
    {
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
                if (rightHandState != 0 && rightHand.PinchStrength >= pinchStrengthForce) {
                    //Debug.Log("Right Pinching");
                    rightHandState = 1;
                    rightHandTrail.emitting = true;
                    
                } else if (rightHand.GrabStrength <= grabStrengthForce) {
                    //Debug.Log("Right AllExtend");
                    rightHandState = 2;
                    rightHandTrail.emitting = false;
                } else {
                    offset = Vector3.zero;
                    rightHandState = 0;
                }

                if (rightHandState == 2) {
                    if (rightHand.PalmVelocity.z >= onoffVelocity) {
                        //Debug.Log("Right Window TurnOff");
                        rightHandState = 3;
                    } else if (rightHand.PalmVelocity.z <= -onoffVelocity) {
                        //Debug.Log("Right Window TurnOn");
                        rightHandState = 4;
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
                
                if (leftHandState != 0 && leftHand.PinchStrength >= pinchStrengthForce) {
                    //Debug.Log("Left Pinching");
                    leftHandState = 1;
                } else if (leftHand.GrabStrength <= grabStrengthForce) {
                    //Debug.Log("Left AllExtend");
                    leftHandState = 2;
                } else {
                    offset = Vector3.zero;
                    leftHandState = 0;
                }

                if (leftHandState == 2) {
                    if (leftHand.PalmVelocity.z >= onoffVelocity) {
                        //Debug.Log("Left Window TurnOff");
                        leftHandState = 3;
                    } else if (leftHand.PalmVelocity.z <= -onoffVelocity) {
                        //Debug.Log("Left Window TurnOn");
                        leftHandState = 4;
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
        
        if (rightHandState == 4 && leftHandState == 4) {
            if (windowState == 0) {
                captureManager.MakeAll();
                captureManager.ShowAll();
                windowState = 1;
            }
        }
        else if (rightHandState == 3 && leftHandState == 3) {
            if (windowState == 1) {
                captureManager.HideAll();
                windowState = 0;
            }
        }
        
    }
}
