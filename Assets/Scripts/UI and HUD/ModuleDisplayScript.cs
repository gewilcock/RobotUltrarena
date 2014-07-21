using UnityEngine;
using System.Collections;

public class ModuleDisplayScript : MonoBehaviour
{
    ProgressBarScript refireDisplay;
    Transform backplate;
    TextMesh infoText;
    TextMesh nameText;
    public int moduleIndex;
    public AbilityModuleScript modControl;
    WeaponController wControl;
    public Material baseColor;
    public Material overheatColor;
    public Material cannotFireColor;
    // Use this for initialization
    void Start()
    {

        wControl = MechInputHandler.playerController.GetComponentInChildren<WeaponController>();

        modControl = null;

        if (moduleIndex < MechInputHandler.playerController.GetComponentInChildren<WeaponController>().moduleList.Length)
            modControl = MechInputHandler.playerController.GetComponentInChildren<WeaponController>().moduleList[moduleIndex];

        if (modControl == null)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(transform.position - transform.parent.transform.position);
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);

            refireDisplay = transform.FindChild("UIWRefireBar").GetComponent<ProgressBarScript>();
            backplate = transform.FindChild("UIWeaponDisplayBack").transform;

            infoText = transform.FindChild("UIWeaponInfoText").GetComponent<TextMesh>();
            nameText = transform.FindChild("UIWeaponNameText").GetComponent<TextMesh>();

            nameText.text = modControl.moduleName;
        }
    }

    // Update is called once per frame
    void Update()
    {
        refireDisplay.fillRatio = modControl.capacityRatio;

		backplate.renderer.material = baseColor;
        if (wControl.isOverheating)
        {
			backplate.renderer.material = overheatColor;
        }
        else if (modControl.abilityCapacity > 0 && modControl.capacityRatio <= 0)
        {
			backplate.renderer.material = cannotFireColor;
        }
        
        if (!modControl.isActive)
        {
			backplate.renderer.material.color =  new Color(backplate.renderer.material.color.r, backplate.renderer.material.color.g, backplate.renderer.material.color.b, 0.1f);
        }

        
        if (wControl.isOverheating)
        {
            infoText.text = "OVERHEATING";
        }
        else if (modControl.abilityCapacity > 0 && modControl.capacityRatio <= 0)
        {
            infoText.text = "DEPLETED";
        }
        else if (modControl.isCharging)
        {
            infoText.text = "RECHARGING";
        }
        else
        {
            infoText.text = "";
        }
    }
}
