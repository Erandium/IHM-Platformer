using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class CanvasScript : MonoBehaviour
{
    PlayerControler player;
    [SerializeField] InputField gravity;
    [SerializeField] InputField detectionMargin;
    [SerializeField] InputField frictionFactor;
    [SerializeField] InputField dashGranularity;
    [SerializeField] InputField mass;
    [SerializeField] InputField horizontalForce;
    [SerializeField] InputField walkMaxSpeed;
    [SerializeField] InputField sprintMaxSpeed;
    [SerializeField] InputField airControlFactor;
    [SerializeField] InputField jumpSpeed;
    [SerializeField] InputField jumpNumber;
    [SerializeField] InputField wallJumpAngle;
    [SerializeField] InputField variableJumpFactor;
    [SerializeField] InputField dashDistance;
    [SerializeField] InputField dashCooldown;
    [SerializeField] InputField dashDuration;
    [SerializeField] InputField dashNumber;

    private CanvasGroup canvasGroup;
    private bool isHidden;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControler>();
        canvasGroup = GetComponent<CanvasGroup>();
        isHidden = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (!isHidden)
            {
                isHidden = true;
                canvasGroup.alpha = 0f;
                canvasGroup.blocksRaycasts = false;
            }
            else
            {
                isHidden = false;
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
            }
        }
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene("Scene1");
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("Scene2");
    }
    public void setGravity()
    {
        player.setGravity(float.Parse(gravity.text));
    }
    public void setDetectionMargin()
    {
        player.setDetectionMargin(float.Parse(detectionMargin.text));
    }
    public void setFrictionAdjustmentFactor()
    {
        player.setFrictionAdjustmentFactor(float.Parse(frictionFactor.text));
    }
    public void setDashGranularity()
    {
        player.setDashGranularity(int.Parse(dashGranularity.text));
    }
    public void setMass()
    {
        player.setMass(float.Parse(mass.text));
    }
    public void setHorizontalForce()
    {
        player.setHorizontalForce(float.Parse(horizontalForce.text));
    }
    public void setWalkMaxSpeed()
    {
        player.setWalkMaxSpeed(float.Parse(walkMaxSpeed.text));
    }
    public void setSprintMaxSpeed()
    {
        player.setSprintMaxSpeed(float.Parse(sprintMaxSpeed.text));
    }
    public void setAirControlFactor()
    {
        player.setAirControlFactor(float.Parse(airControlFactor.text));
    }
    public void setJumpSpeed()
    {
        player.setJumpSpeed(float.Parse(jumpSpeed.text));
    }
    public void setJumpNumber()
    {
        player.setJumpNumber(int.Parse(jumpNumber.text));
    }
    public void setWallJumpAngleDegre()
    {
        player.setWallJumpAngleDegre(float.Parse(wallJumpAngle.text));
    }
    public void setVariableJumpFactor()
    {
        player.setVariableJumpFactor(float.Parse(variableJumpFactor.text));
    }
    public void setDashDistance()
    {
        player.setDashDistance(float.Parse(dashDistance.text));
    }
    public void setDashCooldown()
    {
        player.setDashCooldown(float.Parse(dashCooldown.text));
    }
    public void setDashDuration()
    {
        player.setDashDuration(float.Parse(dashDuration.text));
    }
    public void setDashNumber()
    {
        player.setDashNumber(int.Parse(dashNumber.text));
    }
}
