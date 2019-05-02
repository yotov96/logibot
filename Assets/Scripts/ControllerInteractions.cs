using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.GrabAttachMechanics;

public class ControllerInteractions : MonoBehaviour {

    private VRTK_ControllerEvents events;
    private VRTK_InteractGrab grab;
    private VRTK_InteractTouch touch;
    private VRTK_Pointer pointer;
    private VRTK_StraightPointerRenderer pointerRenderer;
    private VRTK_InteractUse interactUse;

    //swipe detection stuff
    private bool isTrackingSwipe = false;
    private Vector2 startAxis;
    private Vector2 endAxis;
    private float startTime;

    [SerializeField]
    [Tooltip("Minimum velocity threshold for swipe detection")]
    float minimumSwipeVelocity = 5f;

    [SerializeField]
    [Tooltip("Minimum distance threshold for swipe detection")]
    float minimumSwipeDistance = 0.2f;

    public void Start()
    {
        events = GetComponent<VRTK_ControllerEvents>();
        grab = GetComponent<VRTK_InteractGrab>();
        pointer = GetComponent<VRTK_Pointer>();
        pointerRenderer = GetComponent<VRTK_StraightPointerRenderer>();
        touch = GetComponent<VRTK_InteractTouch>();
        interactUse = GetComponent<VRTK_InteractUse>();

        //swipe detection
        events.TouchpadTouchStart += TouchPadTouched;
        events.TouchpadAxisChanged += TouchPadAxisChanged;
        events.TouchpadTouchEnd += TouchPadReleased;

        //grab events
        //grab.ControllerGrabInteractableObject += Grab;
        //grab.ControllerUngrabInteractableObject += UnGrab;
    }

    private void UnGrab(object sender, ObjectInteractEventArgs e)
    {
        pointerRenderer.enabled = true;
    }

    private void Grab(object sender, ObjectInteractEventArgs e)
    {
        pointerRenderer.enabled = false;
    }

    private void TouchPadReleased(object sender, ControllerInteractionEventArgs e)
    {
        isTrackingSwipe = false;
    }

    public void Update()
    {
        DetectSwipe();
    }

    //record when the axis changes
    private void TouchPadAxisChanged(object sender, ControllerInteractionEventArgs e)
    {
        if (isTrackingSwipe)
            endAxis = e.touchpadAxis;
    }

    //record the start of the touch
    private void TouchPadTouched(object sender, ControllerInteractionEventArgs e)
    {
        isTrackingSwipe = true;
        startAxis = e.touchpadAxis;
        startTime = Time.time;
    }

    //detect which direction we've swiped in
    private void DetectSwipe()
    {
        float tolerance = 30f;
        //diff between start of touch and now
        float deltaTime = Time.time - startTime;
        Vector2 swipeVector = endAxis - startAxis;
        float velocity = swipeVector.magnitude / deltaTime;

        //axes for working out the angles of swiping
        Vector2 X = new Vector2(1, 0);
        Vector2 Y = new Vector2(0, 1);

        if((velocity > minimumSwipeVelocity) && (swipeVector.magnitude > minimumSwipeDistance))
        {
            //now detect which direction we're swiping in
            swipeVector.Normalize();
            float angle = Vector2.Dot(swipeVector, Y);
            angle = Mathf.Acos(angle) * Mathf.Rad2Deg;

            //up or down swipe
            //Up swipe
            if (angle < tolerance)
            {
                //Debug.Log("Up Swipe!!");
                SlideObject(velocity);
            }
            //down swipe
            else if ((180.0f - angle) < tolerance)
            {
                //Debug.Log("Down Swipe!!");
                SlideObject(-velocity);
            }
        }
        
    }

    //move the grabbed object once we've detected a swipe
    private void SlideObject(float velocity)
    {
        float mult = 0.005f;
        float distanceToSlide = mult * velocity;
        GameObject grabbed;
        if (grabbed = grab.GetGrabbedObject())
        {
            //work out the direction from the grabbed object to the controller.
            Vector3 heading = grabbed.transform.position - VRTK_DeviceFinder.GetControllerRightHand(true).transform.position;
            float distance = heading.magnitude;
            Vector3 direction = heading / distance;

            //don't want to move if it's going to be too close to the controller
            if(distance + distanceToSlide > 0.1)
                //move in direction at (velocity*multiplier)
                grabbed.transform.Translate(direction*distanceToSlide, Space.World);
        }
    }
}
