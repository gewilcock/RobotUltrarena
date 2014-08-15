using UnityEngine;
using System.Collections;

public class HUDThrottleScript : MonoBehaviour
{
    MechController mController;
    WeaponController wController;
    ProgressBarScript throttleBar;
    TextMesh speedText;
    public Transform throttleBase;
    public Color overheatColour;

    CharacterController cController;

    float speedTick;

    float lastSpeed;

    Color baseColour;

    // Use this for initialization
    void Start()
    {
        PlayerInputHandler myHandler = PlayerInputHandler.Instance;

        transform.rotation = Quaternion.LookRotation(transform.position - transform.parent.transform.position);
        transform.Rotate(transform.localEulerAngles.x, 0, 90f);

        mController = myHandler.mechController;
        wController = myHandler.weaponController;
        throttleBar = GetComponentInChildren<ProgressBarScript>();
        baseColour = throttleBase.renderer.material.color;

        cController = myHandler.transform.GetComponent<CharacterController>();

        speedText = GetComponentInChildren<TextMesh>();

        lastSpeed = cController.velocity.magnitude;
    }

    // Update is called once per frame
    void Update()
    {

        throttleBar.fillRatio = mController.GetThrottleLevel();

        if (wController.isOverheating)
        {
            throttleBase.renderer.material.color = overheatColour;
        }
        else
        {
            throttleBase.renderer.material.color = baseColour;
        }

        if (speedTick < Time.time)
        {

            float deltaV = cController.velocity.magnitude - lastSpeed;
            lastSpeed = cController.velocity.magnitude;

            float speed = Mathf.Round(cController.velocity.magnitude * 3.6f);
            speedText.text = speed.ToString() + " km/h";
            /*speedText.text+=" - dV: "+deltaV.ToString();*/
            speedTick = Time.time + 0.5f;


        }
    }

}
