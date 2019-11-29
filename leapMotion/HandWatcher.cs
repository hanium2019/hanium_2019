using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Leap.Unity.Examples;
using Leap.Unity;
using Leap;

public class HandWatcher : MonoBehaviour
{
    public GameObject mainCamera;
    public HandModelBase rightHandModel;
    public HandModelBase leftHandModel;
    public Leap.Hand rightHand;
    public Leap.Hand leftHand;
    
    public int leftHandState; // extend : 0, pinch : 1, grasp : 2, push : 3, pull : 4, left : 5, right : 6
    public int rightHandState;
    public int windowState = 0; // off : 0, normal(collapsed) : 1, spread : 2

    public float pinchStrengthForce = 0.85f;
    public float grabStrengthForce = 0.15f;
    public float onoffVelocity = 0.6f;
    public float buttonDistance = 0.025f;
    public Vector3 offset;
    
    public GameObject dynamicUI;
    public TrailRenderer rightHandTrail;
    public ParticleSystem rightPalmParticle;
    public ParticleSystem leftPalmParticle;
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

    private bool workstationActivated = false;

    private IEnumerator rightHandWatcher() {
      while (true){
        if (rightHandModel != null){
            rightHand = rightHandModel.GetLeapHand();
            if (rightHand != null){
                if (rightHand.Fingers[1].IsExtended) {
                    if (leftHand != null) {
                        if ( Vector3.Distance(leftHand.PalmPosition.ToVector3(), rightHand.Fingers[1].TipPosition.ToVector3()) < buttonDistance ) {
                            LeanTween.move(dynamicUI, leftHand.PalmPosition.ToVector3(), 0.2f);
                            if (!workstationActivated) {
                                dynamicUI.GetComponent<WorkstationBehaviourExample>().ActivateWorkstation();
                                workstationActivated = true;
                            }
                        }
                    }                    
                }

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
                    rightHandTrail.emitting = false;
                }

                if (rightHandState == 2) {
                    if (rightHand.PalmVelocity.z >= onoffVelocity) {
                        //Debug.Log("Right Window TurnOff");
                        rightHandState = 3;
                    } else if (rightHand.PalmVelocity.z <= -onoffVelocity) {
                        //Debug.Log("Right Window TurnOn");
                        rightHandState = 4;
                    }
                    if (rightHand.PalmVelocity.x <= -onoffVelocity) {
                        rightHandState = 5;
                        //Debug.Log("Left");
                    } else if (rightHand.PalmVelocity.x >= onoffVelocity) {
                        rightHandState = 6;
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
                    if (workstationActivated) {
                        dynamicUI.GetComponent<WorkstationBehaviourExample>().DeactivateWorkstation();
                        workstationActivated = false;
                    }
                }

                if (leftHandState == 2) {
                    if (leftHand.PalmVelocity.z >= onoffVelocity) {
                        //Debug.Log("Left Window TurnOff");
                        leftHandState = 3;
                    } else if (leftHand.PalmVelocity.z <= -onoffVelocity) {
                        //Debug.Log("Left Window TurnOn");
                        leftHandState = 4;
                    }
                    if (leftHand.PalmVelocity.x <= -onoffVelocity) {
                        leftHandState = 5;
                        //Debug.Log("Left");
                    } else if (leftHand.PalmVelocity.x >= onoffVelocity) {
                        leftHandState = 6;
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.1f);
      }
    }

    public void ParticleControl(int flag, bool isRight) {
        if (flag == 0) {
            if (isRight)
                rightPalmParticle.Stop();
            else 
                leftPalmParticle.Stop();
        }   
        else {
            if (isRight)
                rightPalmParticle.Play();
            else 
                leftPalmParticle.Play();
        }

        switch(flag) {
            case 0:
                break;
            case 1:
                rightPalmParticle.startColor = new Color(1f, 0f, 0f, 0.35f);
                leftPalmParticle.startColor = new Color(1f, 0f, 0f, 0.35f);
                break;
            case 2:
                rightPalmParticle.startColor = new Color(0f, 1f, 0f, 0.35f);
                leftPalmParticle.startColor = new Color(0f, 1f, 0f, 0.35f);
                break;
            case 3:
                rightPalmParticle.startColor = new Color(0f, 0f, 1f, 0.35f);
                leftPalmParticle.startColor = new Color(0f, 0f, 1f, 0.35f);
                break;

        }
    }

    public void TrailOn() {
        rightHandTrail.enabled = true;
    }
    
    public void TrailOff() {
        rightHandTrail.enabled = false;
    }

    public void TrailErase() {
        rightHandTrail.time = 0;
        Invoke("TrailErase2", 0.2f);
    }

    private void TrailErase2() {
        rightHandTrail.time = 600;
        Debug.Log("erased");
    }
    

    // Update is called once per frame
    void Update()
    {
        
        if (rightHandState == 4 && leftHandState == 4) {
            if (windowState == 0) {
                //captureManager.MakeAll();
                captureManager.ShowAll();
                windowState = 1;
            }
        }
        else if (rightHandState == 3 && leftHandState == 3) {
            if (windowState != 0) {
                captureManager.HideAll();
                windowState = 0;
            }
        } 
        else if (rightHandState == 6 && leftHandState == 5) {
            if (windowState != 0) {
                captureManager.SpreadAll();
                windowState = 2;
            }
        } 
        else if (rightHandState == 5 && leftHandState == 6) {
            if (windowState == 1) {
                captureManager.CollapseAll();
                windowState = 1;
            }
        } 
        
        
    }
}
