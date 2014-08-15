using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalTargetList : MonoBehaviour
{
    private List<MechController> mechList = new List<MechController>();

    public static GlobalTargetList targetList { get; private set; }

    Transform bT;
    Vector3 rangeVector;
    public float maxScanRange;

    void Awake()
    {
        targetList = this;
    }

    // Use this for initialization
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }

    public MechController GetClosestTarget(Transform baseTransform)
    {

        bT = baseTransform;

        MechController bestTarget = null;

        float lastRange = maxScanRange;

        for (int i = 0; i < mechList.Count; i++)
        {

            if (mechList[i] != null)
            {
                if (!mechList[i].isDead)
                {
                    Transform testTarget = mechList[i].AIAimPoint;

                    if (testTarget != baseTransform)
                    {
                        rangeVector = testTarget.position - baseTransform.position;
                        float testRange = rangeVector.magnitude;
                        //Debug.Log ("testTarget is at range "+testRange.ToString () + "out of maximum" + maxScanRange.ToString ());
                        if ((testRange < lastRange) && (testRange > 0.5))
                        {
                            //Debug.Log ("Test range is adequate.");

                            if (!Physics.Raycast(baseTransform.position, rangeVector, testRange, 1 << 12))
                            {
                                //Debug.Log ("I can see this target through the terrain layer");
                                bestTarget = mechList[i];
                                lastRange = testRange;
                            }
                            //else{Debug.Log ("Target failed the visibility test");}
                        }

                    }

                }
            }
        }

        return bestTarget;

    }

    public void AddMech(MechController newMech)
    {
        mechList.Add(newMech);
    }

    void OnDrawGizmos()
    {
        if (bT != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(bT.position, rangeVector);
        }
    }
}