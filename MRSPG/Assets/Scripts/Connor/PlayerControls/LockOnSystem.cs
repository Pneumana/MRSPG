using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
//using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;
using UnityEngine.UI;
using static Cinemachine.CinemachineTargetGroup;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;
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

    Quaternion noTweaking;

    Energy energy;
    public Quaternion eul;

    public bool freeAim = true;

    bool wasFreeAiming;

    float swapTargetCD;

    GameObject backpoint;

    [SerializeField] Mesh sphere;
    [SerializeField] Material rangeMaterial;
    //Separate the closestTarget object from the whole loop thing. same thing with closestEnemy
    //yep
    public static LockOnSystem LOS;

    Coroutine subtractEnergy;

    private void Awake()
    {
        if (LOS == null)
        {
            LOS = this;
        }
        else
        {
            if (LOS != this)
                Destroy(gameObject);
        }
    }
    private void Start()
    {
        //Debug.LogWarning("Screen size is " + Screen.width + "x"+Screen.height);
        energy = FindFirstObjectByType<Energy>();
        try
        {
            lockOnAssist = GameObject.Find("LockOnAssist").transform;
        }
        catch { }
        
        UpdateEnemyList();
        if (lockOnAssist == null)
        {
            lockOnAssist = new GameObject().transform;
            lockOnAssist.gameObject.name = "LockOnAssist";
            //lockOnAssist.AddComponent<CinemachineTargetGroup>();
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

        backpoint = new GameObject();
        backpoint.name = "Backpoint";
        enemyTracker.GetComponent<Image>().color = Color.clear;
    }
    public void UpdateEnemyList()
    {
        enemies.Clear ();
        enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        var removelist = new List<GameObject>();
        foreach(GameObject enemy in enemies)
        {
            if (!enemy.activeInHierarchy)
                removelist.Add(enemy);
        }
        foreach(GameObject go in removelist)
        {
            enemies.Remove(go);
        }
        ui = GameObject.Find("TargetingUI").transform;
    }

    private void LateUpdate()
    {
        if (trackedEnemy != null)
        {
            if (trackedEnemy.activeInHierarchy)
            {
                var trackedScreenPos = Camera.main.WorldToScreenPoint(trackedEnemy.transform.position);
                lockon.transform.position = trackedScreenPos;
                if (!freeAim)
                {
                    Metronome.inst.transform.position = trackedScreenPos;
                    //lockon.color = new Color(1, 1, 1, 1);
                }
                else
                {

                }

            }
            else
            {
                lockon.transform.position = new Vector2(-100, -100);
                Metronome.inst.transform.localPosition = Metronome.inst.startPos;
            }
        }
        else
        {
            lockon.transform.position = new Vector2(-100, -100);
            Metronome.inst.transform.localPosition = Metronome.inst.startPos;
        }
        //Debug.Log(Camera.main.gameObject.transform.rotation.eulerAngles.x);
    }

    private void Update()
    {
        if(swapTargetCD>0)
            swapTargetCD-=Time.deltaTime;
        
        if(!paused || energy.currentEnergy == 0)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, targetTime, Time.unscaledDeltaTime * scaleSpeed);
        }
        if (cooldown > 0)
        {
            cooldown -= Time.unscaledDeltaTime;
            timeJuice.GetComponent<Image>().fillAmount = 1 -(cooldown / cooldownTime);
            if(remainingTime!=useTime)
                remainingTime = useTime;
        }
        else
        {

            //timeJuice.GetComponent<Image>().fillAmount = remainingTime / useTime;
        }
        //timeJuice.GetComponent<Image>().color = Color.Lerp(new Color(1f, 0f, 0), new Color(0f, 1f, 0), timeJuice.localScale.x);

        //Debug.Log(Quaternion.LookRotation((Camera.main.transform.position - player.transform.position).normalized, Vector3.up).eulerAngles.x + " x, " + Quaternion.LookRotation((Camera.main.transform.position - player.transform.position).normalized, Vector3.up).eulerAngles.z  + " z");

        if (controller.controls.Gameplay.LookAtTarget.WasPressedThisFrame())
        {
            if (freeAim)
            {
                StartLockOn();
                
            }
            else
            {
                
                StopLockOn();
                //HideTargets();
                /*                if(!controller.controls.Gameplay.Slowdown.WasPressedThisFrame())
                                    StopLockOn();*/
                
                
            }
        }
        /*        if (!freeAim)
                {
                Vector3 dirRot = trackedEnemy.transform.position - (player.transform.position);
                Debug.DrawLine(player.transform.position, player.transform.position + dirRot, Color.black);
                eul = Quaternion.LookRotation(dirRot);
                }*/
        if (trackedEnemy != null)
        {
            if (!trackedEnemy.activeInHierarchy)
            {
                RefindTracked();
            }
        }
        else
        {
            RefindTracked();
        }
        if (!freeAim)
        {
            if (trackedEnemy != null )
            {
                if (trackedEnemy.activeInHierarchy)
                {

                    var midpoint = (trackedEnemy.transform.position + player.transform.position) / 2;
                    var a = player.transform.position;
                    a.y = 0;
                    var b = trackedEnemy.transform.position;
                    b.y = 0;
                    var dist = Vector3.Distance(a, b);
                    var influence = Mathf.Clamp01(dist / 5);

                    var playerSpeed = player.GetComponentInParent<InputControls>().velocity;
                    var mag = playerSpeed.magnitude;
                    if (player.GetComponentInParent<InputControls>().dashing)
                    {
                        mag += player.GetComponentInParent<InputControls>().dashSpeed;
                    }
                    var playerCam = GameObject.Find("PlayerCam");
                    var freeLook = playerCam.GetComponent<CinemachineFreeLook>();

                    var assistDir = lockOnAssist.transform.position - player.transform.position;
                    var distanceScaler = Mathf.Clamp((dist / 5) - 0.5f, 0, 1);

                    var dir = trackedEnemy.transform.position - player.transform.position;

                    //Debug.Log("horizontal dampening should be " + distanceScaler);
                    if (Mathf.Abs(trackedEnemy.transform.position.x - player.transform.position.x) < 0.25f && Mathf.Abs(trackedEnemy.transform.position.z - player.transform.position.z) < 0.25f)
                    {
                        //player is basically on top of the entity and the dir should not be updated;
                        //dir = lockOnAssist.transform.forward;
                        Debug.LogWarning(Mathf.Abs(trackedEnemy.transform.position.x - player.transform.position.x) + "x or " + Mathf.Abs(trackedEnemy.transform.position.z - player.transform.position.z) + "z is below 0.25 which means the player is over the enemy");
                        StopLockOn();
                        return;
                    }
                    else
                    {
                        //lockOnAssist.transform.forward = dir;
                    }
                    if (freeLook != null)
                    {
                        if (dist < 2)
                        {
                            //lockOnAssist.position = Vector3.Lerp(lockOnAssist.position, player.transform.position + (dir * 0.1f), distanceScaler);
                            Debug.Log("start using assistDir");
                            //lockOnAssist.position = player.transform.position + (player.transform.forward * 0.1f);
                            var end = new Vector3(player.transform.position.x + player.transform.forward.x * 0.1f, midpoint.y, player.transform.position.z + player.transform.forward.z * 0.1f);
                            var camDir = Camera.main.transform.forward;
                            camDir.y = 0;
                            player.transform.rotation = Quaternion.LookRotation(camDir, Vector3.up);
                            lockOnAssist.position = end;

                        }
                        else
                        {

                            lockOnAssist.position = Vector3.Lerp(lockOnAssist.position, midpoint, ((5 + Mathf.Abs(mag)) * Time.deltaTime));
                            player.transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
                            noTweaking = player.transform.rotation;
                            
                        }
                        freeLook.m_LookAt = lockOnAssist;
                        var start = freeLook.m_XAxis;
                    }

                    //get the direction that the camera is facing
                    Vector3 camdir = Camera.main.transform.position - player.transform.position;
                    //convert look direction to quaternion
                    var cameraRot = Quaternion.LookRotation(-camdir.normalized, Vector3.up);

                    //direction that is "behind" the player if they are looking at the lock on assist
                    //Vector3 dir = lockOnAssist.position - player.transform.position;



                    backpoint.transform.position = -dir * 1;
                    var look = Quaternion.LookRotation(-dir.normalized, Vector3.up);
                    //keep dir horizontal
                    dir.y = 0;
                    //make the player look at the lock on assist
                   
                    

                    //really gross conversion of angle to the stupid 0 to 1 thing that the free look camera uses
                    var d = look.eulerAngles.x;
                    var sample = look.eulerAngles.x;

                    if(d < 100)
                    {
                        if (d > 4.004173f)
                        {
                            d = Remap(d, 0.0134f, 4.004173f, 0.45f, 0.5f);
                        }
                        else
                        {
                            d = Remap(d, 4.004173f, 51.34019f, 0.5f, 1f);
                        }
                            
                            //remap this value 0 to 0.397372946f
                    }
                    else
                    {
                        //Debug.Log("remap 300 range");
                        //if()
                        d = Remap(d, 359.1992f, 311.5497f , 0.44f, 0f);
                    }
                    Debug.DrawLine(player.transform.position, trackedEnemy.transform.position, Color.magenta);
                    //make the camera look in a direction
                    if (freeLook != null)
                    {
                        //if (dist > 1)
                        //var lerp = Mathf.Lerp(freeLook.m_XAxis.Value, freeLook.m_XAxis.Value, distanceScaler);
                        freeLook.m_XAxis.Value = look.eulerAngles.y - 180;
                        //d = Mathf.Clamp(d, 0, 1);
                        //Debug.Log(d + " is the Y axis value, true angle is " + sample);
                        freeLook.m_YAxis.Value = 1 - d;
                    }
                    //remap function
                    float Remap(float value, float from1, float from2, float to1, float to2)
                    {
                        var fromAbs = value - from1;
                        var fromMaxAbs = from2 - from1;

                        var normal = fromAbs / fromMaxAbs;

                        var toMaxAbs = to2 - to1;
                        var toAbs = toMaxAbs * normal;

                        var to = toAbs + to1;

                        return to;
                        //return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
                    }
                    //if the target is too far away, break the lock on
                    if(Vector3.Distance(player.transform.position,trackedEnemy.transform.position) >= range)
                    {
                        StopLockOn();
                        StartLockOn();
                    }
                }
                else
                {
                    
                    RefindTracked();
                }
                //(value - from1) / (to1 - from1) * (to2 - from2) + from2;
            }

            //lockOnAssist.rotation = eul;
            /*freeLook.m_XAxis.Value = lockOnAssist.rotation.eulerAngles.y;
            freeLook.m_YAxis.Value = 1 -  (lockOnAssist.rotation.eulerAngles.x);*/

        }
        else
        {
            
            //RefindTracked();
        }

        InputEventStartSlowDown();
        InputEventStaySlowDown();
        
        /*if (closestTarget != null)
        {
            closestTarget.GetComponent<Image>().sprite = lockedSprite;
            closestTarget.GetComponent<Image>().color = Color.clear;
        }*/
        
        InputEventEndSlowDown();
        if (!freeAim)
        {
            UpdateTargetUI();
            LookAtTarget();
        }
        /*if(energy.currentEnergy <= 0)
        {
            Debug.Log("timed out");
            rangeFinder.SetActive(false);
            cooldown = cooldownTime;
            //remainingTime = useTime;
            if (subtractEnergy != null)
                StopCoroutine(subtractEnergy);
            targetTime = maxTimeScale;
            HideTargets();
            //targeters.Add(closestTarget);
        }*/
        if(lockon != null && trackedEnemy != null  )
        {
            if (trackedEnemy.activeInHierarchy)
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
        //Sounds.instance.PlaySFX ( "Teleport" );

        try
        {
            closestEnemy.GetComponent<NavMeshAgent>().enabled = true;
        }
        catch { }
        StopLockOn();
    }


    void InputEventStartSlowDown()
    {

            if (controller.controls.Gameplay.Slowdown.WasPressedThisFrame())
            {
            //Debug.Log("start input");
                //remainingTime = useTime;
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
        if(controller.controls.Gameplay.Fire.WasPressedThisFrame() && energy.currentEnergy >= 50)
        {
            remainingTime = useTime;
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
            swapTargetCD = 0.1f;
            if (controller.lookInput.x > 0)
            {
                //Debug.Log("looking right");
                //right
                if (rightTrackedEnemy != null)
                    trackedEnemy = rightTrackedEnemy;
            }
            else if (controller.lookInput.x < 0)
            {
                //Debug.Log("looking left");
                //left
                if (leftTrackedEnemy != null)
                    trackedEnemy = leftTrackedEnemy;
            }
            
        }
        

        var cam = Camera.main.gameObject;
        var freeLook = GameObject.Find("PlayerCam").GetComponent<CinemachineFreeLook>();

        var dir = trackedEnemy.transform.position - (player.transform.position + new Vector3(0, 5, 0));
        dir = Vector3.Normalize(dir);
        

        //Debug.DrawLine(player.transform.position, trackedEnemy.transform.position, Color.red);
           // Debug.DrawLine(player.transform.position, player.transform.position + dir, Color.cyan, 10);
        var xangle = Mathf.Rad2Deg * (Mathf.Atan2(player.transform.position.x - (player.transform.position.x + dir.x), player.transform.position.z - (player.transform.position.z + dir.z)));
        if (freeLook != null)
        {
            var xLerp = Mathf.MoveTowards(freeLook.m_XAxis.Value, xangle - 180, 0.5f);
        var yLerp = Mathf.MoveTowards(freeLook.m_YAxis.Value, -dir.y, 0.5f);

        }

        //Vector3 dir = target - transform.position;
        //dir.y = 0; // keep the direction strictly horizontal
        /*Quaternion rot = Quaternion.LookRotation(dir);
        // slerp to the desired rotation over time
        var prev = Quaternion.Euler(new Vector3(freeLook.m_XAxis.Value, freeLook.m_YAxis.Value));
        var eul = Quaternion.Slerp(prev, rot, 0.5f).eulerAngles;*/


        //freeLook.m_LookAt = trackedEnemy.transform;
    }
    IEnumerator DrainEnergy()
    {
        float t = 0;

        while (t < 1)
        {
            t += Time.unscaledDeltaTime;
            if(t > 0.25f)
            {
                t = 0;
                if(energy.currentEnergy > 0)
                {
                    energy.LoseEnergy(1);
                }
                else
                {
                    subtractEnergy = null;
                    yield break;
                }
            }
            yield return new WaitForSecondsRealtime(0);
        }

        yield return null;
    }
    void InputEventStaySlowDown()
    {
        if(cooldown > 0)
        {
            return;
        }
        //GUN IS FUCKY WITH THE SUBTRACT ENERGY SYSTEM
        if (controller.controls.Gameplay.Slowdown.IsPressed())
        {
            if(subtractEnergy == null && energy.currentEnergy > 0)
            {
                subtractEnergy = StartCoroutine(DrainEnergy());
            }
                /*if (remainingTime >= 0)
                    remainingTime -= Time.unscaledDeltaTime;*/
                if(energy.currentEnergy > 0)
                    targetTime = minTimeScale;
                else
                    targetTime = maxTimeScale;
                UpdateTargetUI();
            }
            else
            {
            targetTime = maxTimeScale;
            if (subtractEnergy != null)
                    StopCoroutine(subtractEnergy);
        }
        if(controller.controls.Gameplay.Fire.IsPressed() && remainingTime > 0 && energy.currentEnergy >= 50)
        {
            if (remainingTime >= 0)
                remainingTime -= Time.unscaledDeltaTime;
                targetTime = minTimeScale;
        }
    }
    //Specifically ran for the time slowdown ending
    public void InputEventEndSlowDown(bool dontSwapPositions = false)
    {
        
            if (controller.controls.Gameplay.Slowdown.WasReleasedThisFrame() && cooldown <= 0)
            {
            //Debug.Log("end input");
            //if (!dontSwapPositions)
            if (subtractEnergy != null)
            {
                StopCoroutine(subtractEnergy);
                subtractEnergy = null;
            }
                    
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
        if (closestTarget != null)
        {
            closestTarget.GetComponent<Image>().sprite = lockedSprite;
            if(!freeAim)
                closestTarget.GetComponent<Image>().color = Color.clear;
        }

        if (trackedEnemy != closestEnemy && closestEnemy != null)
        {
            if (freeAim)
                if (closestEnemy.GetComponent<EnemyBody>()._enemy.type != EnemyType.Crystal)
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
                    if (enemies[i].gameObject.GetComponent<EnemyBody>()._enemy.type == EnemyType.Crystal)
                        continue;
                        right = check;
                        rightTrackedEnemy = enemies[i];
                }
            }
            else if(check < 0)
            {
                //left
                if(check > left)
                {

                    if (enemies[i].gameObject.GetComponent<EnemyBody>()._enemy.type == EnemyType.Crystal)
                        continue;
                    leftTrackedEnemy = enemies[i];
                    left = check;
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

    public void HideTargets()
    {
        foreach (GameObject targeter in targeters)
        {
            Destroy(targeter);
        }
        targeters.Clear();
    }

    public void StopLockOn(bool overridePrev = false)
    {
        Metronome.inst.transform.localPosition = Metronome.inst.startPos;
        lockon.color = new Color(1, 1, 1, 0);

        var cam = GameObject.Find("PlayerCam").GetComponent<CinemachineFreeLook>();

        var x = cam.m_XAxis.Value;
        var y = cam.m_YAxis.Value;

        var rot = Camera.main.transform.rotation;
        var pos = Camera.main.transform.position;
        player.transform.localRotation = Quaternion.Euler(0, 0, 0);


        //hides all targeters and lets the camera be controlled as normal.
        //Debug.Log("stopping lock on");
        HideTargets();
        targetTime = maxTimeScale;
        freeAim = true;
        //rangeFinder.SetActive(false);
        cam.m_LookAt = player.transform;



        cam.m_XAxis.Value = x;
        cam.m_YAxis.Value = y;

        GameObject.Find("PlayerCam").GetComponent<CinemachineInputProvider>().enabled = true;
        //cam.ForceCameraPosition(pos, rot);
        //GameObject.Find("PlayerCam").GetComponent<CinemachineCameraOffset>().m_Offset = Vector3.zero;
    }

    public void StartLockOn()
    {
        //StartLockOn
        HideTargets();
        CreateTargeters();
        UpdateTargetUI();
        //Vector3 dirRot = trackedEnemy.transform.position - (player.transform.position);
        //eul = Quaternion.LookRotation(dirRot);
        var enemyScreenPos = lockon.transform.position;
        if (enemyScreenPos.x > Screen.width || enemyScreenPos.x < 0 || enemyScreenPos.y < 0 || enemyScreenPos.y > Screen.height)
        {
            StopLockOn();
        }
        else
        {
            freeAim = false;
            GameObject.Find("PlayerCam").GetComponent<CinemachineInputProvider>().enabled = false;
            var freeLook = GameObject.Find("PlayerCam").GetComponent<CinemachineFreeLook>();
            //lockOnAssist.position = player.transform.position + Camera.main.transform.forward;
            if (freeLook != null)
                freeLook.m_LookAt = lockOnAssist;

            var dir = player.transform.position - trackedEnemy.transform.position;
            lockOnAssist.position = player.transform.position - dir;
        }
    }

    void RefindTracked()
    {
        //Debug.Log("refinding a tracked enemy");
        HideTargets();
        CreateTargeters();
        bool swap = false;


        //used to get a new target if the one the player is locked on to dies
        if (!freeAim)
        {
            freeAim = true;
            swap = true;
        }

        UpdateTargetUI();
        if (swap)
            freeAim = false;
        if (!freeAim)
        {
            GameObject.Find("PlayerCam").GetComponent<CinemachineInputProvider>().enabled = false;
            var freeLook = GameObject.Find("PlayerCam").GetComponent<CinemachineFreeLook>();
            var dir = player.transform.position - trackedEnemy.transform.position;
            lockOnAssist.position = player.transform.position + dir;
            if (freeLook != null)
                freeLook.m_LookAt = lockOnAssist;

        }
        else
        {
            HideTargets();
        }
    }

}
