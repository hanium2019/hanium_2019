using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Leap.Unity;
using Leap;

public class Main : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject capsuleHandRight;
    public GameObject capsuleHandLeft;
    public HandModelBase rightHandModel;
    public HandModelBase leftHandModel;
    public Leap.Hand rightHand;
    public Leap.Hand leftHand;
    public PinchDetector rightPinchDetector;
    public ProximityDetector rightProximityDetector;
    public ExtendedFingerDetector rightExtendedFingerDetector;
    public FingerDirectionDetector rightFingerDirectionDetector;
    public PalmDirectionDetector rightPalmDirectionDetector;
    public PinchDetector leftPinchDetector;
    public ProximityDetector leftProximityDetector;
    public ExtendedFingerDetector leftExtendedFingerDetector;
    public FingerDirectionDetector leftFingerDirectionDetector;
    public PalmDirectionDetector leftPalmDirectionDetector;
    public Text text;
    public bool pinching;
    public bool leftWindow;
    public bool rightWindow;
    public Vector3 offset;
    public GameObject fire;
    public GameObject water;
    public int casting;
    public GameObject sword;

    // Start is called before the first frame update
    void Start()
    {
        rightHand = capsuleHandRight.GetComponent<CapsuleHand>().GetLeapHand();
        leftHand = capsuleHandLeft.GetComponent<CapsuleHand>().GetLeapHand();
        
        rightWindow = false;
        leftWindow = false;

        text = GameObject.Find("Text").GetComponent<Text>();
        sword = GameObject.Find("Sword");

        fire = GameObject.Find("Fire");
        water = GameObject.Find("Water");
    }


    // Update is called once per frame
    void Update()
    {
        if (capsuleHandRight.activeSelf) {
            // Leap.Hand rightHand가 지정되지 않았을 경우 지정해줌. 위의 Start에서 지정시 제대로 할당되지 않는 오류가 있어서 여기에 일단 둠
            if (rightHand == null) {
                rightHand = capsuleHandRight.GetComponent<CapsuleHand>().GetLeapHand();
                Debug.Log(rightHand);
            }
            
            if (rightPinchDetector.DidStartPinch) {
                if (rightProximityDetector.CurrentObject != null)
                    offset = rightProximityDetector.CurrentObject.transform.position - rightHand.GetThumb().TipPosition.ToVector3();
                text.text = offset.ToString();
            }
            if (rightPinchDetector.DidEndPinch) {
                offset = Vector3.zero;
            }
            if (rightPinchDetector.IsPinching && rightProximityDetector.CurrentObject != null && offset != Vector3.zero) {
                rightProximityDetector.CurrentObject.transform.position = rightHand.GetThumb().TipPosition.ToVector3() + offset;
            }

            //IsExtended 활용한 부분. 최적화는 나중으로 미루고 일단 구현부터
            if (casting == 0) {
                if (rightHand.Fingers[0].IsExtended && !rightHand.Fingers[1].IsExtended && rightHand.Fingers[2].IsExtended && !rightHand.Fingers[3].IsExtended && !rightHand.Fingers[4].IsExtended) {
                    casting = 1;
                    Debug.Log("Fire");
                    water.GetComponent<ParticleSystem>().Stop();
                    fire.GetComponent<ParticleSystem>().Play();
                } else if (rightHand.Fingers[0].IsExtended && !rightHand.Fingers[1].IsExtended && !rightHand.Fingers[2].IsExtended && !rightHand.Fingers[3].IsExtended && rightHand.Fingers[4].IsExtended) {
                    casting = 2;
                    Debug.Log("Water");
                    fire.GetComponent<ParticleSystem>().Stop();
                    water.GetComponent<ParticleSystem>().Play();
                } 
            } else {
                if (rightHand.Fingers[0].IsExtended && !rightHand.Fingers[1].IsExtended && !rightHand.Fingers[2].IsExtended && !rightHand.Fingers[3].IsExtended && !rightHand.Fingers[4].IsExtended) {
                    casting = 0;
                    Debug.Log("Cast End");
                    fire.GetComponent<ParticleSystem>().Stop();
                    water.GetComponent<ParticleSystem>().Stop();
                }
            }

            if (rightHand.Fingers[0].IsExtended && rightHand.Fingers[1].IsExtended && rightHand.Fingers[2].IsExtended && rightHand.Fingers[3].IsExtended && rightHand.Fingers[4].IsExtended) {
                rightWindow = true;
            } else {
                rightWindow = false;
            }

            switch(casting) {
                case 1: 
                    fire.transform.position = rightHand.PalmPosition.ToVector3();
                    fire.transform.LookAt(fire.transform.position + rightHand.PalmNormal.ToVector3());
                    break;
                case 2:
                    water.transform.position = rightHand.PalmPosition.ToVector3();
                    water.transform.LookAt(fire.transform.position + rightHand.PalmNormal.ToVector3());
                    break;
            }

            
        }
        
        if (capsuleHandLeft.activeSelf) {
            if (leftHand == null) {
                leftHand = capsuleHandLeft.GetComponent<CapsuleHand>().GetLeapHand();
                Debug.Log(leftHand);
            }
            
            if (leftPinchDetector.DidStartPinch) {
                if (leftProximityDetector.CurrentObject != null)
                    offset = leftProximityDetector.CurrentObject.transform.position - leftHand.GetThumb().TipPosition.ToVector3();
                text.text = offset.ToString();
            }
            if (leftPinchDetector.DidEndPinch) {
                offset = Vector3.zero;
            }
            if (leftPinchDetector.IsPinching && leftProximityDetector.CurrentObject != null && offset != Vector3.zero) {
                leftProximityDetector.CurrentObject.transform.position = leftHand.GetThumb().TipPosition.ToVector3() + offset;
            }

            if (leftHand.Fingers[0].IsExtended && leftHand.Fingers[1].IsExtended && leftHand.Fingers[2].IsExtended && leftHand.Fingers[3].IsExtended && leftHand.Fingers[4].IsExtended) {
                leftWindow = true;
            } else {
                leftWindow = false;
            }

        }

        if (rightWindow && leftWindow) {
            sword.GetComponent<MeshRenderer>().enabled = true;
            sword.transform.position = rightHand.PalmPosition.ToVector3();
            sword.transform.LookAt(sword.transform.position + rightHand.PalmNormal.ToVector3());
        } else {
            sword.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
