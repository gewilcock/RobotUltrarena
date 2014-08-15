using UnityEngine;
using System.Collections;

public class PlayerInputHandler : MonoBehaviour
{
    public static PlayerInputHandler Instance { get; private set; }

    public float deadZoneRange = 0.15f;
    public float mouseStrength;

    public float cameraPitchMax = 15f;
    public float cameraPitch = 0;

    public bool absThrottle = false;    

    public const float MAX_AIM_DISTANCE = 1000f;
    public float throttleAdjustRate = 0.75f;
    public MechController mechController;
    public WeaponController weaponController;
    GameObject mechPrefab;

    Ray aimRay;
    RaycastHit targetHit;
    int playerMask;
    int weaponPermeableMask;

    public GameObject HUDObject;
    GameObject deathCam;
    GameObject radarCam;

    float[] wepToggleButton = new float[2];

    public bool isShootyTarget { get; protected set; }

    public bool isTargeting { get; protected set; }

    bool deathRoutineDone = false;

    bool isCockpitView = false;

    // Use this for initialization
    void Awake()
    {

        Instance = this;
        deathCam = GameObject.Find("DeathCamera");

        Screen.showCursor = false;

        Screen.lockCursor = true;

        radarCam = GameObject.Find("RadarCamera");

    }


    void Start()
    {
        gameObject.layer = 8;

        mechController = GetComponent<MechController>();
        weaponController = GetComponent<WeaponController>();

        Camera.main.transform.parent = mechController.torsoBone;
        Camera.main.transform.localPosition = new Vector3(-8f, 0, -20);
        Camera.main.transform.localEulerAngles = new Vector3(0f, 0f, 90f);

        radarCam.transform.position = new Vector3(transform.position.x, radarCam.transform.position.y, transform.position.z);
        radarCam.transform.parent = mechController.torsoBone;

        transform.FindChild("RadarBlip").renderer.material.color = Color.green;

        deathCam.GetComponent<DeathCameraScript>().playerMech = mechController.AIAimPoint;
        deathCam.SetActive(false);

        mechController.cockpit.gameObject.SetActive(isCockpitView);

        playerMask = 1 << 8;
        weaponPermeableMask = 1 << 13;
        playerMask = playerMask | weaponPermeableMask | (1 << 15);
        playerMask = ~playerMask;

        GlobalTargetList.targetList.AddMech(mechController);

        SetCameraPosition();

        wepToggleButton[0] = -1;
        wepToggleButton[1] = -1;

        Screen.lockCursor = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (mechController != null)
        {
            if (!mechController.isDead)
            {
                GetSteeringInput();
                MouseTurretAdjustment();
                MouseCameraPitch();
                MouseAim();

                GetWeaponInput();
            }
            else
            {
                if (!deathRoutineDone)
                {

                    deathCam.transform.position = Camera.main.transform.position;

                    foreach (Camera c in Camera.allCameras) 
                    { 
                        c.gameObject.SetActive(false); 
                    }

                    deathCam.SetActive(true);
                    deathCam.transform.LookAt(mechController.transform.position);
                    deathRoutineDone = true;
                }
            }
        }

    }

    void GetSteeringInput()
    {

        if (!absThrottle)
        {
            mechController.setThrottle(mechController.throttleLevel + (Input.GetAxis("Throttle") * throttleAdjustRate * Time.deltaTime));
        }
        else { mechController.setThrottle(Input.GetAxis("Throttle")); }



        mechController.rotateMech(Input.GetAxis("Steering"));

        if (Input.GetButton("AllStop")) { mechController.allStop(); }

    }

    void GetWeaponInput()
    {

        if (Input.GetKey(KeyCode.Escape))
            mechController.armourLevel = 0;

        if (Input.GetButtonDown("HeavyFire1"))
        {
            weaponController.setHeavyWeaponTrigger(true, 0);
        }

        if (Input.GetButtonUp("HeavyFire1"))
        {
            weaponController.setHeavyWeaponTrigger(false, 0);
        }

        if (Input.GetButtonDown("HeavyFire2"))
        {
            weaponController.setHeavyWeaponTrigger(true, 1);
        }

        if (Input.GetButtonUp("HeavyFire2"))
        {
            weaponController.setHeavyWeaponTrigger(false, 1);
        }

        if (Input.GetButton("Fire1"))
        {
            weaponController.SetWeaponTriggers(true, 0);
        }
        else
        {
            weaponController.SetWeaponTriggers(false, 0);
        }

        if (Input.GetButton("Fire2"))
        {
            weaponController.SetWeaponTriggers(true, 1);
        }
        else
        {
            weaponController.SetWeaponTriggers(false, 1);
        }

        if (Input.GetButtonDown("ToggleWepGroup0"))
        {

            wepToggleButton[0] = Time.time + 0.3f;

        }

        if (Input.GetButtonDown("ToggleWepGroup1"))
        {
            wepToggleButton[1] = Time.time + 0.3f;

        }

        if (Input.GetButton("ToggleWepGroup0"))
        {
            if (wepToggleButton[0] <= Time.time)
            {
                weaponController.SetGroupFire(0);
            }
        }

        if (Input.GetButton("ToggleWepGroup1"))
        {
            if (wepToggleButton[1] <= Time.time)
            {
                weaponController.SetGroupFire(1);
            }
        }


        if (Input.GetButtonUp("ToggleWepGroup0"))
        {
            if (wepToggleButton[0] > Time.time)
            {
                weaponController.ToggleActiveWeapons(0);
            }
        }

        if (Input.GetButtonUp("ToggleWepGroup1"))
        {
            if (wepToggleButton[1] > Time.time)
            {
                weaponController.ToggleActiveWeapons(1);
            }
        }

        if (Input.GetButtonDown("TargetUnderReticle"))
        {
            isTargeting = true;
        }
        else { isTargeting = false; }

        if (Input.GetKeyUp(KeyCode.F))
        {

            weaponController.toggleModule(false, 0);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {

            weaponController.toggleModule(true, 0);
        }

        if (Input.GetKeyUp(KeyCode.G))
        {

            weaponController.toggleModule(false, 1);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {

            weaponController.toggleModule(true, 1);
        }

        if (Input.GetKeyUp(KeyCode.C))
        {

            isCockpitView = !isCockpitView;
            SetCameraPosition();
        }

    }

    void MouseAim()
    {

        aimRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hit = Physics.Raycast(aimRay, out targetHit, MAX_AIM_DISTANCE, (playerMask));
        isShootyTarget = false;

        if (hit)
        {
            weaponController.aimPoint = targetHit.point;
            if (targetHit.transform.CompareTag("DamageObject")) { isShootyTarget = true; }
        }
        else
        {
            weaponController.aimPoint = aimRay.GetPoint(MAX_AIM_DISTANCE);
        }

    }

    void MouseTurretAdjustment()
    {

        Vector3 targetVector = weaponController.aimPoint - transform.position;
        float relativeAngle = Quaternion.FromToRotation(mechController.transform.rotation * Vector3.forward, targetVector).eulerAngles.y;


        mouseStrength = 2 * Mathf.Abs(((Input.mousePosition.x - (Screen.width / 2)) / Screen.height));
        mouseStrength = Mathf.Clamp01(mouseStrength - deadZoneRange);

        if (relativeAngle < 180)
        {
            relativeAngle += mechController.torsoRotation;
            if (relativeAngle < 0)
            {
                mechController.rotateTorso(mouseStrength);
            }
            else if (relativeAngle > 0)
            {
                mechController.rotateTorso(-mouseStrength);
            }
        }
        else if (relativeAngle > 180)
        {
            relativeAngle += mechController.torsoRotation;
            if (relativeAngle > 360)
            {
                mechController.rotateTorso(-mouseStrength);
            }
            else if (relativeAngle < 360)
            {
                mechController.rotateTorso(mouseStrength);
            }
        }
    }

    void MouseCameraPitch()
    {

        float direction = Mathf.Clamp(Input.mousePosition.y - (Screen.height / 2), -1, 1);
        mouseStrength = 2 * Mathf.Abs(((Input.mousePosition.y - (Screen.height / 2)) / Screen.height));
        mouseStrength = Mathf.Clamp01(mouseStrength - deadZoneRange);

        cameraPitch = Mathf.Clamp(cameraPitch - mouseStrength * direction, -cameraPitchMax, cameraPitchMax);

        Camera.main.transform.localEulerAngles = new Vector3(0, cameraPitch, 90);

    }

    void SetCameraPosition()
    {
        if (isCockpitView)
        {
            mechController.cockpit.gameObject.SetActive(true);
            Camera.main.transform.localPosition = mechController.cockpit.transform.localPosition;
        }
        else
        {
            mechController.cockpit.gameObject.SetActive(false);
            Camera.main.transform.localPosition = mechController.cockpit.localPosition + new Vector3(-4, 0, -20);
        }
    }

}
