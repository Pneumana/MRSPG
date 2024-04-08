using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;
using UnityEngine.UI;
using static Cinemachine.CinemachineTargetGroup;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI.Table;

public class LockOnSystem : MonoBehaviour
{
    public CharacterController characterController;
    public GameObject player;
    public GameObject closestTarget = null;
    public GameObject closestEnemy = null;

    public Image lockon;

    public List<GameObject> enemies = new List<GameObject> ();
    public List<GameObject> targeters = new List<GameObject> ();

    public LayerMask LOSMask;

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

    public bool paused;

    public RectTransform timeJuice;
    Transform ui;
    [SerializeField]Transform lockOnAssist;
    public GameObject rangeFinder;
    public float range;

    public Controller controller;

    Energy energy;
    public Quaternion eul;

    public bool freeAim = true;

    bool wasFreeAiming;

    float swapTargetCD;

    [SerializeField] Mesh sphere;
    [SerializeField] Material rangeMaterial;
    //Separate the closestTarget object from the whole loop thing. same thing with closestEnemy
    //yep

    private void Start()
    {
        Debug.LogWarning("Screen size is " + Screen.width + "x"+Screen.height);
        energy = FindFirstObjectByType<Energy>();
        //lockOnAssist = GameObject.Find("LockOnAssist").transform;
        UpdateEnemyList();
        if (lockOnAssist == null)
        {
            lockOnAssist = new GameObject().transform;
            lockOnAssist.gameObject.name = "LockOnAssist";
        }
        if (rangeFinder != null)
        {
            rangeFinder.transform.SetParent(player.transform);
            rangeFinder.SetActive(false);
        }
        else
        {
            rangeFinder = new GameObject();
            rangeFinder.name = "Rangefinder";
            rangeFinder.AddComponent<MeshFilter>();
            rangeFinder.GetComponent<MeshFilter>().mesh = sphere;
            rangeFinder.AddComponent<MeshRenderer>();
            rangeFinder.GetComponent<MeshRenderer>().material = rangeMaterial;
            rangeFinder.transform.SetParent(player.transform);
            rangeFinder.SetActive(false);
        }
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
        else
        {
            lockon.transform.position = new Vector2(-100, -100);
        }
        if(!paused)
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
                //StartLockOn
                HideTargets();
                CreateTargeters();
                UpdateTargetUI();
                //Vector3 dirRot = trackedEnemy.transform.position - (player.transform.position);
                //eul = Quaternion.LookRotation(dirRot);
                freeAim = false;
                GameObject.Find("PlayerCam").GetComponent<CinemachineInputProvider>().enabled = false;
                var freeLook = GameObject.Find("PlayerCam").GetComponent<CinemachineFreeLook>();
                lockOnAssist.position = player.transform.position + player.transform.forward;
                freeLook.m_LookAt = lockOnAssist;
            }
            else
            {
                StopLockOn();
                //HideTargets();
                /*                if(!controller.controls.Gameplay.Slowdown.WasPressedThisFrame())
                                    StopLockOn();*/
                freeAim = true;
                GameObject.Find("PlayerCam").GetComponent<CinemachineInputProvider>().enabled = true;
            }
        }
        /*        if (!freeAim)
                {
                Vector3 dirRot = trackedEnemy.transform.position - (player.transform.position);
                Debug.DrawLine(player.transform.position, player.transform.position + dirRot, Color.black);
                eul = Quaternion.LookRotation(dirRot);
                }*/
        if (trackedEnemy == null)
        {
            //Debug.Log("tracked enemy is null");
            HideTargets();
            CreateTargeters();
            bool swap = false;


            //used to get a new target if the one the player is locked on to dies
            if (!freeAim)
            {
                freeAim=true;
                swap = true;
            }
            
                UpdateTargetUI();
            if (swap)
                freeAim = false;
            if (!freeAim)
            {
                GameObject.Find("PlayerCam").GetComponent<CinemachineInputProvider>().enabled = false;
                var freeLook = GameObject.Find("PlayerCam").GetComponent<CinemachineFreeLook>();
                lockOnAssist.position = player.transform.position + player.transform.forward;
                freeLook.m_LookAt = lockOnAssist;

            }
            else
            {
                HideTargets();
            }

            //Vector3 dirRot = trackedEnemy.transform.position - (player.transform.position);
            //eul = Quaternion.LookRotation(dirRot);
            //HideTargets();
        }
        if (!freeAim)
        {

            //Vector3 dirRot = trackedEnemy.transform.position - (player.transform.position);
            //dirRot.y = 0;
            //dir.y = 0; // keep the direction strictly horizontal
            //Quaternion rot = Quaternion.LookRotation(dirRot, Vector3.up);
            // slerp to the desired rotation over time
            if (trackedEnemy != null)
            {
                var midpoint = (trackedEnemy.transform.position + player.transform.position) / 2;
                var influence = Mathf.Clamp01(Vector3.Distance(player.transform.position, trackedEnemy.transform.position) / 10f);
                lockOnAssist.position = Vector3.Lerp(lockOnAssist.position, midpoint, (5 * influence) * Time.deltaTime);

                var freeLook = GameObject.Find("PlayerCam").GetComponent<CinemachineFreeLook>();
                freeLook.m_LookAt = lockOnAssist;
                var start = freeLook.m_XAxis;


                //Vector3 dir = lockOnAssist.position - player.transform.position;
                Vector3 camdir = Camera.main.transform.position - player.transform.position;
                //dir.y = 0; // keep the direction strictly horizontal

                //Debug.DrawLine(player.transform.position, player.transform.position + (dir * 4), Color.white, Time.deltaTime);
                var quat = Quaternion.identity;
                //var look = Quaternion.LookRotation(-dir.normalized, Vector3.up);
                var cameraRot = Quaternion.LookRotation(-camdir.normalized, Vector3.up);

                //quat = Quaternion.Slerp(cameraRot, look, 5 * Time.deltaTime);

                //freeLook.m_XAxis.Value = quat.eulerAngles.y;

                if(Vector3.Distance(player.transform.position, trackedEnemy.transform.position) > 2)
                {

                    
                }
                Vector3 dir = lockOnAssist.position - player.transform.position;
                dir.y = 0;
                var look = Quaternion.LookRotation(-dir.normalized, Vector3.up);
                freeLook.m_XAxis.Value = look.eulerAngles.y - 180;





            }
            
            //lockOnAssist.rotation = eul;
            /*freeLook.m_XAxis.Value = lockOnAssist.rotation.eulerAngles.y;
            freeLook.m_YAxis.Value = 1 -  (lockOnAssist.rotation.eulerAngles.x);*/

        }

        InputEventStartSlowDown();
        InputEventStaySlowDown();
        
        if (closestTarget != null)
        {
            closestTarget.GetComponent<Image>().sprite = lockedSprite;
            closestTarget.GetComponent<Image>().color = Color.clear;
        }
        
        InputEventEndSlowDown();
        if (!freeAim)
        {
            UpdateTargetUI();
            LookAtTarget();
        }
        if(remainingTime <= 0)
        {
            Debug.Log("timed out");
            cooldown = cooldownTime;
            remainingTime = useTime;
            targetTime = maxTimeScale;
            HideTargets();
            //targeters.Add(closestTarget);
        }
        if(lockon != null && trackedEnemy != null)
        {
            lockon.transform.position = Camera.main.WorldToScreenPoint(trackedEnemy.transform.position);
            var view = Camera.main.WorldToViewportPoint(trackedEnemy.transform.position);
            bool displayTarget = false;

                    if (view.z >= 0)
                    {
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
    //swaps positions with target enemy
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


        //Debug.DrawLine(playerStartPos, targetedPos, Color.cyan, 15);
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


    void InputEventStartSlowDown()
    {

            if (controller.controls.Gameplay.Slowdown.WasPressedThisFrame() || controller.controls.Gameplay.Fire.WasPressedThisFrame() && energy.currentEnergy >= 50)
            {
            Debug.Log("start input");
                remainingTime = useTime;
            if(cooldown <= 0)
            {
                //StopLockOn();
                UpdateTargetUI();
                CreateTargeters();
                //show range finder object
                rangeFinder.SetActive(true);
                rangeFinder.transform.position = player.transform.position;
                rangeFinder.transform.localScale = new Vector3(range * 2 , range * 2 , range * 2 );
            }
            else
            {
                Debug.Log("input denied because not off cooldown");
            }

            }
        
    }

    void LookAtTarget()
    {
        if (trackedEnemy == null)
        {
            //StopLockOn();
            //HideTargets();

            UpdateTargetUI();
            CreateTargeters();

            //break lock on
            return;
        }

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

        var dir = trackedEnemy.transform.position - (player.transform.position + new Vector3(0, 5, 0));
        dir = Vector3.Normalize(dir);
        

        //Debug.DrawLine(player.transform.position, trackedEnemy.transform.position, Color.red);
           // Debug.DrawLine(player.transform.position, player.transform.position + dir, Color.cyan, 10);
        var xangle = Mathf.Rad2Deg * (Mathf.Atan2(player.transform.position.x - (player.transform.position.x + dir.x), player.transform.position.z - (player.transform.position.z + dir.z)));
        var xLerp = Mathf.MoveTowards(freeLook.m_XAxis.Value, xangle - 180, 0.5f);
        var yLerp = Mathf.MoveTowards(freeLook.m_YAxis.Value, -dir.y, 0.5f);

        //Vector3 dir = target - transform.position;
        //dir.y = 0; // keep the direction strictly horizontal
        /*Quaternion rot = Quaternion.LookRotation(dir);
        // slerp to the desired rotation over time
        var prev = Quaternion.Euler(new Vector3(freeLook.m_XAxis.Value, freeLook.m_YAxis.Value));
        var eul = Quaternion.Slerp(prev, rot, 0.5f).eulerAngles;*/


        //freeLook.m_LookAt = trackedEnemy.transform;
    }

    void InputEventStaySlowDown()
    {
        if(cooldown > 0)
        {
            return;
        }
        //ran while the slowdown is held
            if (controller.controls.Gameplay.Slowdown.IsPressed() && remainingTime > 0 || controller.controls.Gameplay.Fire.IsPressed() && remainingTime > 0 && energy.currentEnergy >= 50)
        {
                if (remainingTime >= 0)
                    remainingTime -= Time.unscaledDeltaTime;
                targetTime = minTimeScale;
                UpdateTargetUI();
            }
            else
            {
                targetTime = maxTimeScale;
            }

    }
    //Specifically ran for the time slowdown ending
    public void InputEventEndSlowDown(bool dontSwapPositions = false)
    {
        
            if (controller.controls.Gameplay.Slowdown.WasReleasedThisFrame() && remainingTime > 0 && cooldown <= 0)
            {
                Debug.Log("end input");
                //if (!dontSwapPositions)
                    SwapPositions();
                cooldown = cooldownTime;
                remainingTime = useTime;
                targetTime = maxTimeScale;
            //StopLockOn();
            HideTargets();

            rangeFinder.SetActive(false);
        }
        
    }
    public void UpdateTargetUI()
    {
        
        float closest = float.MaxValue;
        closestTarget = null;
        closestEnemy = null;
        //if there's no targeters, stop.
        if (targeters.Count <= 0)
            return;
        List<int> validEnemies = new List<int>();
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i]==null)
            {
                Debug.Log("current checked enemy is null, refreshing list");
                StopLockOn();
                HideTargets();
                UpdateEnemyList();
                CreateTargeters();
                return;
            }
            var enemyScreenPos = Camera.main.WorldToScreenPoint(enemies[i].transform.position);
            targeters[i].transform.position = Camera.main.WorldToScreenPoint(enemies[i].transform.position);
            targeters[i].GetComponent<Image>().color = Color.white;
            targeters[i].GetComponent<Image>().sprite = unlockedSprite;
            //If the targeter is offscreen, skip over it
            if (enemyScreenPos.x > Screen.width || enemyScreenPos.x < 0 || enemyScreenPos.y < 0 || enemyScreenPos.y > Screen.height || Camera.main.WorldToViewportPoint(enemies[i].transform.position).z < 0)
            {
                targeters[i].GetComponent<Image>().color = Color.clear;
                continue;
            }
            else
            {
                //Line of sight check from player to the enemy
                RaycastHit hit;
                var vector = enemies[i].transform.position - player.transform.position;
                Physics.Raycast(player.transform.position, vector, out hit, Mathf.Infinity, LOSMask);
                if (hit.distance > range)
                {
                    targeters[i].GetComponent<Image>().color = Color.clear;
                    //Debug.Log(hit.distance + " is greater than the range of " + range);
                    continue;
                }

                if (hit.collider == null)
                    continue;
                if (enemies[i] != hit.collider.gameObject)
                {
                    targeters[i].GetComponent<Image>().color = Color.clear;
                    //Debug.Log(hit.collider.gameObject.name + " stopped " + enemies[i] + " from being hit");
                    continue;
                }
            }
            validEnemies.Add(i);
            var dist = Vector2.Distance(targeters[i].transform.position, ui.position + new Vector3(Screen.width / 2, Screen.height / 2));
            if (dist < closest)
            {
                closest = dist;
                closestTarget = targeters[i];

                closestEnemy = enemies[i];
            }
        }
        if(closestTarget!=null)
            closestTarget.GetComponent<Image>().sprite = lockedSprite;
        if (trackedEnemy != closestEnemy && closestEnemy != null)
        {
            if (freeAim)
                trackedEnemy = closestEnemy;
        }
        float left = float.MinValue;
        float right = float.MaxValue;
        if(validEnemies.Count == 0)
        {
            //Debug.Log("stopping lock on, no targets");
            StopLockOn();
            
            return;
        }
        else
        {
            //freeAim = false;
            //GameObject.Find("PlayerCam").GetComponent<CinemachineInputProvider>().enabled = false;
            //GameObject.Find("PlayerCam").GetComponent<CinemachineCameraOffset>().m_Offset = Camera.main.gameObject.transform.right * 2 + Vector3.up * 1.5f - Camera.main.gameObject.transform.forward * 2;
        }
        foreach (int i in validEnemies)
        {
            if (targeters[i] == lockon)
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
    public void CreateTargeters()
    {
        //Debug.Log("creating targeters");
        //update enemy list
        UpdateEnemyList();
        //create targets for each enemy
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
        //get the target closest to the center of the screen
        UpdateTargetUI();
        
    }

    void HideTargets()
    {
        foreach (GameObject targeter in targeters)
        {
            Destroy(targeter);
        }
        targeters.Clear();
    }

    public void StopLockOn(bool overridePrev = false)
    {



        //hides all targeters and lets the camera be controlled as normal.
        //Debug.Log("stopping lock on");
        HideTargets();
        targetTime = maxTimeScale;
        freeAim = true;
        rangeFinder.SetActive(false);
        GameObject.Find("PlayerCam").GetComponent<CinemachineFreeLook>().m_LookAt = player.transform;
        GameObject.Find("PlayerCam").GetComponent<CinemachineInputProvider>().enabled = true;
        //GameObject.Find("PlayerCam").GetComponent<CinemachineCameraOffset>().m_Offset = Vector3.zero;
    }
}
