using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class LockOnSystem : MonoBehaviour
{
    public CharacterController characterController;
    public GameObject player;
    public GameObject closestTarget = null;
    public GameObject closestEnemy = null;

    public Image lockon;

    public List<GameObject> enemies = new List<GameObject> ();
    public List<GameObject> targeters = new List<GameObject> ();

    public GameObject enemyTracker;
    public GameObject trackedEnemy;
    public GameObject leftTrackedEnemy;
    public GameObject rightTrackedEnemy;

    public Sprite lockedSprite;
    public Sprite unlockedSprite;

    public float remainingTime = 2;
    public float cooldown = 0;
    public float useTime;
    public float cooldownTime;

    public float minTimeScale;
    public float maxTimeScale = 1;
    public float scaleSpeed;
    public float targetTime = 1;

    public RectTransform timeJuice;
    Transform ui;

    public Controller controller;

    bool freeAim = true;
    float swapTargetCD;
    //Separate the closestTarget object from the whole loop thing. same thing with closestEnemy
    //yep

    private void Start()
    {
        Debug.LogWarning("Screen size is " + Screen.width + "x"+Screen.height);
        UpdateEnemyList();
    }
    public void UpdateEnemyList()
    {
        enemies.Clear ();
        enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        ui = GameObject.Find("TargetingUI").transform;
    }
    private void Update()
    {
        if(swapTargetCD>0)
            swapTargetCD-=Time.deltaTime;
        if (trackedEnemy != null)
        {
            var trackedScreenPos = Camera.main.WorldToScreenPoint(trackedEnemy.transform.position);
            lockon.transform.position = trackedScreenPos;
        }

        Time.timeScale = Mathf.Lerp(Time.timeScale, targetTime, Time.unscaledDeltaTime * scaleSpeed);
        if (cooldown > 0)
        {
            cooldown -= Time.unscaledDeltaTime;
            timeJuice.localScale = new Vector2(1 -(cooldown / cooldownTime), 1);
            if(remainingTime!=useTime)
                remainingTime = useTime;
        }
        else
        {
            timeJuice.localScale = new Vector2(remainingTime / useTime, 1);
        }
        timeJuice.GetComponent<Image>().color = Color.Lerp(new Color(1f, 0f, 0), new Color(0f, 1f, 0), timeJuice.localScale.x);

        if (controller.controls.Gameplay.LookAtTarget.WasPressedThisFrame())
        {
            if (freeAim)
            {
                LockOn();
                freeAim = false;
                GameObject.Find("PlayerCam").GetComponent<CinemachineInputProvider>().enabled = false;
            }
            else
            {
                StopLockOn();
                freeAim = true;
                GameObject.Find("PlayerCam").GetComponent<CinemachineInputProvider>().enabled = true;
            }
        }

        InputEventStartLockOn();
        InputEventStayLockOn();
        
        if (closestTarget != null)
        {
            closestTarget.GetComponent<Image>().sprite = lockedSprite;
            closestTarget.GetComponent<Image>().color = Color.clear;
        }
        
        InputEventEndLockOn();
        if (!freeAim)
        {
            GetTargetedEnemy();
            LookAtTarget();
        }
        if(remainingTime <= 0)
        {
            Debug.Log("timed out");
            cooldown = cooldownTime;
            remainingTime = useTime;
            targetTime = maxTimeScale;
            foreach (GameObject targeter in targeters)
            {
                //i guess keep the closestTarget?
                    Destroy(targeter);
            }
            targeters.Clear();
            //targeters.Add(closestTarget);
        }
        if(lockon != null && trackedEnemy != null)
        {
            lockon.transform.position = Camera.main.WorldToScreenPoint(trackedEnemy.transform.position);
            var view = Camera.main.WorldToViewportPoint(trackedEnemy.transform.position);
            bool displayTarget = false;

                    if (view.z >= 0)
                    {
                        //Debug.Log("in veiw z");
                        displayTarget = true;
                    }
                    else
                    {
                        displayTarget = false;
                    }
            if(lockon.GetComponent<Image>().enabled!=displayTarget)
                lockon.GetComponent<Image>().enabled = displayTarget;
        }



    }

    void SwapPositions()
    {
        if (closestTarget == null||player==null)
            return;
        Vector3 playerStartPos = player.transform.position;
        Vector3 targetedPos = closestEnemy.transform.position;

        //disable closest enemy nav mesh agent
        try
        {
            closestEnemy.GetComponent<NavMeshAgent>().enabled = false;
        }
        catch { }


        if (Camera.main.WorldToViewportPoint(targetedPos).z < 0)
            return;


        Debug.DrawLine(playerStartPos, targetedPos, Color.cyan, 15);
        //Disable character controller movement override to swap position with enemy.
        characterController.enabled = false;
        player.transform.position = targetedPos;
        characterController.enabled = true;
        closestEnemy.transform.position = playerStartPos;

        try
        {
            closestEnemy.GetComponent<NavMeshAgent>().enabled = true;
        }
        catch { }
    }


    void InputEventStartLockOn()
    {

            if (controller.controls.Gameplay.Slowdown.WasPerformedThisFrame() && cooldown <= 0)
            {
                remainingTime = useTime;
                //foreach enemy,
                //create a UI element that has an image, then check the distance 
                foreach (GameObject enemy in enemies)
                {
                    var newTargeter = new GameObject();
                    newTargeter.name = enemy.name + "Target";
                    newTargeter.transform.SetParent(ui);
                    newTargeter.transform.position = Camera.main.WorldToScreenPoint(enemy.transform.position);
                    newTargeter.AddComponent<Image>();
                    newTargeter.GetComponent<Image>().sprite = unlockedSprite;
                    newTargeter.GetComponent<Image>().color = Color.white;
                    targeters.Add(newTargeter);
                    //add Line of sight check here
                }
            }
        
    }

    void LookAtTarget()
    {
        if (trackedEnemy == null)
            return;
        if(swapTargetCD <= 0)
        {
            if (controller.lookInput.x > 0)
            {
                Debug.Log("looking right");
                //right
                if (rightTrackedEnemy != null)
                    trackedEnemy = rightTrackedEnemy;
            }
            else if (controller.lookInput.x < 0)
            {
                Debug.Log("looking left");
                //left
                if (leftTrackedEnemy != null)
                    trackedEnemy = leftTrackedEnemy;
            }
            swapTargetCD = 0.1f;
        }
        

        var cam = Camera.main.gameObject;
        var freeLook = GameObject.Find("PlayerCam").GetComponent<CinemachineFreeLook>();

        var dir = trackedEnemy.transform.position - player.transform.position;
        //
        dir = Vector3.Normalize(dir);
        

        Debug.DrawLine(player.transform.position, trackedEnemy.transform.position, Color.red);

        //if (controller.controls.Gameplay.LookAtTarget.WasPressedThisFrame())
        //{
            //Debug.Log(dir);
            Debug.DrawLine(player.transform.position, player.transform.position + dir, Color.cyan, 10);


            var xangle = Mathf.Rad2Deg * (Mathf.Atan2(player.transform.position.x - (player.transform.position.x + dir.x), player.transform.position.z - (player.transform.position.z + dir.z)));
            freeLook.m_XAxis.Value = xangle - 180;
            freeLook.m_YAxis.Value = -dir.y;
            //cam.transform.LookAt((trackedEnemy.transform.position + player.transform.position) / 2);
        /*}
        if (controller.controls.Gameplay.LookAtTarget.WasReleasedThisFrame())
        {

        }*/
    }

    void InputEventStayLockOn()
    {
        if(cooldown > 0)
        {
            return;
        }
        
            if (controller.controls.Gameplay.Slowdown.IsPressed() && remainingTime > 0)
            {
                if (remainingTime >= 0)
                    remainingTime -= Time.unscaledDeltaTime;
                targetTime = minTimeScale;
            }
            else
            {
                targetTime = maxTimeScale;
            }
        
    }
    public void InputEventEndLockOn(bool dontSwapPositions = false)
    {
        
            if (controller.controls.Gameplay.Slowdown.WasReleasedThisFrame() && remainingTime > 0 && cooldown <= 0)
            {
            Debug.Log("end input");
                //remove all targeters
                if (!dontSwapPositions)
                    SwapPositions();

                foreach (GameObject targeter in targeters)
                {
                        Destroy(targeter);
                }
                targeters.Clear();
                //targeters.Add(closestTarget);
                cooldown = cooldownTime;
                remainingTime = useTime;
                targetTime = maxTimeScale;
            }
        
    }
    void GetTargetedEnemy()
    {
        
        float closest = float.MaxValue;
        closestTarget = null;
        closestEnemy = null;
        if (targeters.Count <= 0)
            return;
        List<int> validEnemies = new List<int>();
        for (int i = 0; i < enemies.Count; i++)
        {
            var enemyScreenPos = Camera.main.WorldToScreenPoint(enemies[i].transform.position);
            targeters[i].transform.position = Camera.main.WorldToScreenPoint(enemies[i].transform.position);
            targeters[i].GetComponent<Image>().color = Color.white;
            //add a case to check to sprite so it's not constantly setting the sprite, that might be bad for framerate
            targeters[i].GetComponent<Image>().sprite = unlockedSprite;
            if (enemyScreenPos.x > Screen.width || enemyScreenPos.x < 0 || enemyScreenPos.y < 0 || enemyScreenPos.y > Screen.height || Camera.main.WorldToViewportPoint(enemies[i].transform.position).z < 0)
            {
                targeters[i].GetComponent<Image>().color = Color.clear;
                //Debug.Log(targeters[i].name + " is offscreen with pos " + enemyScreenPos);
                //offscreen
                continue;
            }
            else
            {
                RaycastHit hit;
                var vector = enemies[i].transform.position - player.transform.position;
                Physics.Raycast(player.transform.position, vector, out hit, Mathf.Infinity);
                if (hit.collider != null)
                    Debug.DrawLine(player.transform.position, hit.collider.gameObject.transform.position, Color.white, Time.unscaledDeltaTime);
                if (enemies[i] != hit.collider.gameObject)
                {
                    targeters[i].GetComponent<Image>().color = Color.clear;
                    //Debug.Log(targeters[i].name + " is obscured");
                    continue;
                    //Debug.DrawLine(playerStartPos, hit.point, Color.red, 15);
                }
            }
            validEnemies.Add(i);
            //LOS check should be here
            var dist = Vector2.Distance(targeters[i].transform.position, ui.position + new Vector3(Screen.width / 2, Screen.height / 2));
            if (dist < closest)
            {
                closest = dist;
                closestTarget = targeters[i];

                closestEnemy = enemies[i];
                
                //Debug.Log(closestEnemy.name + " @ " + closestEnemy.transform.position + " is the closest target");
            }
        }
        if (trackedEnemy != closestEnemy && closestEnemy != null)
        {
            //enemyTracker = closestTarget;
            if (freeAim)
                trackedEnemy = closestEnemy;
        }
        //get valid enemies and get the closest one on the left and the right
        float left = float.MinValue;
        float right = float.MaxValue;
        foreach (int i in validEnemies)
        {
            if (targeters[i] == closestTarget)
                continue;
            var check = targeters[i].transform.position.x - closestTarget.transform.position.x;
            if(check > 0)
            {
                //right
                if (check < right)
                {
                    right = check;
                    rightTrackedEnemy = enemies[i];
                }
            }
            else if(check < 0)
            {
                //left
                if(check > left)
                {
                    left = check;
                    leftTrackedEnemy = enemies[i];
                }
            }
        }
    }
    public void LockOn()
    {
        foreach (GameObject enemy in enemies)
        {
            var newTargeter = new GameObject();
            newTargeter.name = enemy.name + "Target";
            newTargeter.transform.SetParent(ui);
            newTargeter.transform.position = Camera.main.WorldToScreenPoint(enemy.transform.position);
            newTargeter.AddComponent<Image>();
            newTargeter.GetComponent<Image>().sprite = unlockedSprite;
            newTargeter.GetComponent<Image>().color = Color.white;
            targeters.Add(newTargeter);
            //add Line of sight check here
        }
        GetTargetedEnemy();
    }

    public void StopLockOn()
    {
        foreach (GameObject targeter in targeters)
        {
            Destroy(targeter);
        }
        targeters.Clear();
        //targeters.Add(closestTarget);
        //cooldown = cooldownTime;
        //remainingTime = useTime;
        targetTime = maxTimeScale;
    }

    //ADD LOCK ON DARK SOULS STYLE



}
