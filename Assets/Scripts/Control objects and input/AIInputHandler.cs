using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
public class AIInputHandler : MonoBehaviour
{

    const float MAX_AIM_DISTANCE = 1000f;
    TerrainData levelTerrain;

    //control items
    MechController mechController;
    WeaponController weaponController;
    CharacterController characterController;
    public GameObject MechType;

    //AI control
    bool hasMoveAITicked;
    public float minMoveTickTime = 5f;
    public float maxMoveTickTime = 10f;
    float nextMoveTick;

    bool hasCombatAITicked;
    public float minCombatTickTime = 1f;
    public float maxCombatTickTime = 3f;
    float nextCombatTick;

    public float targetingTimeoutPeriod = 10f;
    float targetingTimeout = 0;

    //movement and steering variables
    Seeker mySeeker;
    public Path myPath;
    bool recalcWalkTarget = true;
    private int currentWaypoint = 0;
    Vector3 targetLocation;
    public float arrivalRadius = 20f;

    public float relativeAngle;
    public float absoluteAngle;
    float distanceToTarget;
    public float collisionScanDistance = 100f;

    Vector3 steerLocation;

    int collMask; //Mask for general AI visibility casts. Initialised in Start().
    int terrainMask = (1 << 12); //set up bitmask for terrain scan only



    //Weapons Targeting and combat
    public float minTargetAttackRadius = 10f;
    public float maxTargetAttackRadius = 500f;
    MechController myTarget;
    Ray aimRay;
    RaycastHit targetHit;
    public float defaultAggression = 0.8f;
    public float aggression = 0.8f;
    GlobalTargetList targetList;

    Vector3 aimDriftTarget;
    Vector3 aimDrift;
    Vector3 targetVector;
    public float myAccuracyDrift = 2f;

    bool hit;

    void Start()
    {

        //		GameObject newMech=(GameObject)Instantiate (MechType,transform.position,transform.rotation);
        //		newMech.transform.parent=transform;

        collMask = (1 << 9) | (1 << 13) | (1 << 15); //set up mask to ignore HUD, weapon, and aimcast-permeable collision layers.
        collMask = ~collMask;

        mechController = GetComponent<MechController>();
        weaponController = GetComponent<WeaponController>();
        characterController = GetComponent<CharacterController>();

        targetLocation = transform.position;
        levelTerrain = GameObject.Find("Terrain").GetComponent<Terrain>().terrainData;

        mySeeker = (Seeker)gameObject.AddComponent("Seeker");
        mySeeker.startEndModifier.useRaycasting = true;
        /*mySeeker.startEndModifier.useGraphRaycasting = true;*/
        mySeeker.startEndModifier.exactEndPoint = StartEndModifier.Exactness.Interpolate;
        mySeeker.startEndModifier.exactStartPoint = StartEndModifier.Exactness.Original;

        gameObject.AddComponent("FunnelModifier");

        mechController.cockpit.gameObject.SetActive(false);

        GlobalTargetList.targetList.AddMech(mechController);

        populateMinMaxRanges();
        AITick();
    }

    // Update is called once per frame
    void Update()
    {
        if (mechController != null)
        {
            if (!mechController.isDead)
            {
                AITick();
                ScanForTargets();
                CalculateAim();
                CalculateShootyBang();
                DetermineWalkTarget();
                //AvoidObstacles();
                CalculateSteering();
            }
            else
            {
                var players = GameDataManagerScript.Instance.players;
                foreach (var player in players)
                {
                    if (player.callSign == mechController.CallSign)
                    {
                        player.isDead = true;
                    }
                }
            }
        }

    }

    void populateMinMaxRanges()
    {
        foreach (var weapon in weaponController.weaponList)
        {
            if (weapon == null)
                continue;

            if (minTargetAttackRadius > weapon.weaponSafetyRange)
            {
                minTargetAttackRadius = weapon.weaponSafetyRange;
            }

            if (maxTargetAttackRadius > weapon.weaponRange)
            {
                minTargetAttackRadius = weapon.weaponRange;
            }
        }
    }

    public void DetermineWalkTarget()
    {
        if (hasMoveAITicked)
        {
            if (myTarget == null)
            {
                if (recalcWalkTarget)
                    targetLocation = new Vector3(Random.Range(0, levelTerrain.size.x), 0, Random.Range(0, levelTerrain.size.z));

            }
            else
            { //in combat
                if (Random.Range(0, 1) <= aggression)
                    targetLocation = myTarget.transform.position + myTarget.transform.rotation * Quaternion.AngleAxis(180, Vector3.up) * (Vector3.forward * maxTargetAttackRadius * (1 - aggression));

            }

            mySeeker.StartPath(transform.position, targetLocation, OnPathComplete);
            recalcWalkTarget = false;
        }


    }

    public void OnPathComplete(Path p)
    {
        Debug.Log("Path returned. Error: " + p.error);
        if (!p.error)
        {
            myPath = p;
            //Reset the waypoint counter
            currentWaypoint = 0;

        }
    }

    public virtual void targetReached()
    {
        recalcWalkTarget = true;
        hasMoveAITicked = true;
    }

    void FixedUpdate()
    {



        if (myPath == null)
        {
            //We have no path to move after yet
            return;
        }
        if (currentWaypoint >= myPath.vectorPath.Count)
        {

            targetReached();
            return;

        }

        //set steering location to next waypoint
        steerLocation = myPath.vectorPath[currentWaypoint];

        //Check if we are close enough to the next waypoint
        //If we are, proceed to follow the next waypoint
        if (Vector3.Distance(transform.position, myPath.vectorPath[currentWaypoint]) <= arrivalRadius)
        {
            currentWaypoint++;
            return;
        }



    }

    void CalculateSteering()
    {

        Quaternion relativeRot = Quaternion.FromToRotation(mechController.transform.rotation * Vector3.forward, steerLocation - mechController.transform.position);

        relativeAngle = relativeRot.eulerAngles.y;



        absoluteAngle = Quaternion.Angle(mechController.transform.rotation, mechController.transform.rotation * relativeRot);

        if (myTarget == null)
        {
            if ((absoluteAngle > 30f))
            {

                mechController.setThrottle(0.1f);

            }
            else { mechController.setThrottle(0.8f); }
        }
        else
        {
            mechController.setThrottle(0.5f);
        }

        if (relativeAngle < 180f)
        {
            mechController.rotateMech(1);
        }
        else if (relativeAngle > 180f)
        {
            mechController.rotateMech(-1);
        }



    }


    void ScanForTargets()
    {
        if ((hasCombatAITicked) || ((myTarget == null) && (mechController.hasTakenDamage)))
        {
            //Debug.Log ("Scanning tick");
            bool findtarget = false;

            if (myTarget != null)
            {
                //Debug.Log ("I already have a target");

                if (myTarget.isDead)
                    findtarget = true;

                if (Physics.Raycast(mechController.AIAimPoint.position, targetVector, targetVector.magnitude, terrainMask))
                {
                    //Debug.Log ("My view of it is blocked");

                    if (targetingTimeout == 0)
                    {
                        //Debug.Log ("Setting obstruction timeout");
                        targetingTimeout = Time.time + targetingTimeoutPeriod;
                    }
                    else
                    {

                        if (targetingTimeout <= Time.time)
                        {
                            //Debug.Log ("View obstruction means target has timed out");
                            targetingTimeout = 0;
                            findtarget = true;
                        }
                    }
                }
                else
                {
                    //Debug.Log ("I can see my target. Resetting timeout.");
                    targetingTimeout = 0;
                }
            }
            else
            {

                findtarget = true;
            }

            if (findtarget)
            {
                myTarget = null;
                //Debug.Log ("Scanning for target.");
                myTarget = GlobalTargetList.targetList.GetClosestTarget(mechController.AIAimPoint);

                //if(myTarget!=null){Debug.Log ("Got a target!");}

            }

        }
    }


    void CalculateAim()
    {

        if (hasCombatAITicked)
        {
            aimDriftTarget = new Vector3(Random.Range(-myAccuracyDrift, myAccuracyDrift), Random.Range(-myAccuracyDrift, myAccuracyDrift), Random.Range(-myAccuracyDrift, myAccuracyDrift));

            if (!weaponController.inFireCone)
                hasMoveAITicked = true;
        }

        aimDrift = Vector3.Slerp(aimDrift, aimDriftTarget, Time.deltaTime);


        float relativeTargetAngle;

        if (myTarget != null)
        {
            targetVector = myTarget.AIAimPoint.position - mechController.AIAimPoint.position;
            Quaternion relativeTarget = Quaternion.FromToRotation(mechController.transform.rotation * Vector3.forward, targetVector);
            relativeTargetAngle = relativeTarget.eulerAngles.y;
        }
        else
        {
            targetVector = mechController.transform.forward;
            relativeTargetAngle = 0;
        }

        if (relativeTargetAngle < 180)
        {
            relativeTargetAngle += mechController.torsoRotation;
            if (relativeTargetAngle < 0)
            {
                mechController.rotateTorso(1);
            }
            else
            {
                mechController.rotateTorso(-1);
            }
        }
        else if (relativeTargetAngle > 180)
        {
            relativeTargetAngle += mechController.torsoRotation;
            if (relativeTargetAngle > 360)
            {
                mechController.rotateTorso(-1);
            }
            else
            {
                mechController.rotateTorso(1);
            }
        }

        RaycastHit targetHit;

        hit = Physics.Raycast(mechController.AIAimPoint.position, targetVector, out targetHit, MAX_AIM_DISTANCE, collMask);

        if (hit)
        {
            weaponController.aimPoint = targetHit.point + aimDrift;
        }
        else
        {
            weaponController.aimPoint = aimRay.GetPoint(MAX_AIM_DISTANCE);
        }        
    }



    void CalculateShootyBang()
    {
        if (myTarget == null)
        {
            weaponController.StopAllWeapons();
            return;
        }

        if (!hasCombatAITicked)
            return;

        weaponController.StopAllWeapons();

        if (weaponController.heatRatio < 0.8f)
        {
            if ((weaponController.inFireCone) && (hit))
            {
                float targetdistance = targetVector.magnitude;

                if (!Physics.Raycast(mechController.AIAimPoint.position, targetVector, targetdistance, terrainMask))
                {

                    for (int q = 0; q <= 2; q += 2)
                    {
                        for (int r = 0; r < 2; r++)
                        {
                            if (weaponController.weaponList[q + r] != null)
                            {
                                if ((weaponController.aimTag[q / 2] == "DamageObject") && (weaponController.aimRange[q / 2] <= weaponController.weaponList[q + r].weaponRange) && (weaponController.aimRange[q / 2] >= weaponController.weaponList[q + r].weaponSafetyRange))
                                    weaponController.weaponList[q + r].isActive = true;
                                else
                                    weaponController.weaponList[q + r].isActive = false;
                            }
                        }
                    }

                    weaponController.SetWeaponTriggers(true, 0);
                    weaponController.SetWeaponTriggers(true, 1);
                }

            }
        }
    }

    void AITick()
    {
        hasMoveAITicked = false;
        hasCombatAITicked = false;


        if (nextCombatTick <= Time.time)
        {
            //Debug.Log ("Tick!");
            nextCombatTick = Time.time + Random.Range(minCombatTickTime, maxCombatTickTime);
            hasCombatAITicked = true;
        }

        if (nextMoveTick <= Time.time)
        {
            //Debug.Log ("Movement tick");
            nextMoveTick = Time.time + Random.Range(minMoveTickTime, maxMoveTickTime);
            hasMoveAITicked = true;
        }

    }

    void OnDrawGizmos()
    {
        if (mechController != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(mechController.transform.position, steerLocation);
            Gizmos.DrawRay(mechController.transform.position, mechController.transform.forward);

            if (weaponController.inFireCone)
                Gizmos.color = Color.cyan;
            else
                Gizmos.color = Color.blue;
            Gizmos.DrawLine(mechController.AIAimPoint.position, weaponController.aimPoint);


            if (myTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(mechController.transform.position, myTarget.transform.position);
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(mechController.AIAimPoint.position, targetVector);

            }
            Gizmos.color = Color.white;
        }

    }


}
