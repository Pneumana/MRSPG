using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class LockOnSystem : MonoBehaviour
{
    public GameObject player;
    public GameObject closestTarget = null;
    public GameObject closestEnemy = null;

    public List<GameObject> enemies = new List<GameObject> ();
    public List<GameObject> targeters = new List<GameObject> ();

    public Sprite lockedSprite;
    public Sprite unlockedSprite;

    public float remainingTime = 2;
    public float cooldown = 0;
    public float useTime;
    public float cooldownTime;

    public RectTransform timeJuice;

    private void Start()
    {
        Debug.LogWarning("Screen size is " + Screen.width + "x"+Screen.height);
        UpdateEnemyList();
    }
    public void UpdateEnemyList()
    {
        enemies.Clear ();
        enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();
    }
    private void Update()
    {
        var ui = GameObject.Find("TargetingUI").transform;
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
        if (Input.GetKeyDown(KeyCode.E) && cooldown <= 0 || Gamepad.current.rightTrigger.wasPressedThisFrame && cooldown <= 0)
        {
            remainingTime = useTime;
            //foreach enemy,
            //create a UI element that has an image, then check the distance 
            foreach (GameObject enemy in enemies)
            {
                var newTargeter = new GameObject();
                newTargeter.name = enemy.name+"Target";
                newTargeter.transform.SetParent(ui);
                newTargeter.transform.position = Camera.main.WorldToScreenPoint(enemy.transform.position);
                newTargeter.AddComponent<Image>();
                newTargeter.GetComponent<Image>().sprite = unlockedSprite;
                newTargeter.GetComponent<Image>().color = Color.white;
                targeters.Add(newTargeter);
                //add Line of sight check here
            }
        }
        if (Input.GetKey(KeyCode.E) || Gamepad.current.rightTrigger.isPressed)
        {
            if (remainingTime >= 0)
                remainingTime -= Time.unscaledDeltaTime;
        }
        float closest = float.MaxValue;
        closestTarget = null;
        closestEnemy = null;
        if (targeters.Count <= 0)
            return;
        for (int i =0; i<enemies.Count; i++)
        {
            var enemyScreenPos = Camera.main.WorldToScreenPoint(enemies[i].transform.position);
            targeters[i].transform.position = Camera.main.WorldToScreenPoint(enemies[i].transform.position);
            targeters[i].GetComponent<Image>().color = Color.white;
            //add a case to check to sprite so it's not constantly setting the sprite, that might be bad for framerate
            targeters[i].GetComponent<Image>().sprite = unlockedSprite;
            if (enemyScreenPos.x > Screen.width || enemyScreenPos.x < 0 || enemyScreenPos.y < 0 || enemyScreenPos.y > Screen.height)
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
                if(hit.collider!=null)
                    Debug.DrawLine(player.transform.position, hit.collider.gameObject.transform.position, Color.white, Time.unscaledDeltaTime);
                if (enemies[i]!=hit.collider.gameObject)
                {
                    targeters[i].GetComponent<Image>().color = Color.clear;
                    //Debug.Log(targeters[i].name + " is obscured");
                    continue;
                    //Debug.DrawLine(playerStartPos, hit.point, Color.red, 15);
                }
            }
            //LOS check should be here
            var dist = Vector2.Distance(targeters[i].transform.position, ui.position + new Vector3(Screen.width/2,Screen.height/2));
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
            closestTarget.GetComponent<Image>().color = Color.white;
        }

        if (Input.GetKeyUp(KeyCode.E) && remainingTime > 0 && cooldown <= 0 || Gamepad.current.rightTrigger.wasReleasedThisFrame && remainingTime > 0 && cooldown <= 0)
        {
            //remove all targeters

            SwapPositions();

            foreach (GameObject targeter in targeters)
            {
                Destroy(targeter);
            }
            targeters.Clear();
            cooldown = cooldownTime;
            remainingTime = useTime;
        }
        if(remainingTime <= 0)
        {
            Debug.Log("timed out");
            cooldown = cooldownTime;
            remainingTime = useTime;
            foreach (GameObject targeter in targeters)
            {
                Destroy(targeter);
            }
            targeters.Clear();
        }

    }
    void SwapPositions()
    {
        if (closestTarget == null||player==null)
            return;
       
        Vector3 playerStartPos = player.transform.position;
        Vector3 targetedPos = closestEnemy.transform.position;

        


        Debug.DrawLine(playerStartPos, targetedPos, Color.cyan, 15);
        player.transform.position = targetedPos;
        closestEnemy.transform.position = playerStartPos;
    }
}
