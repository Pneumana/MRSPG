using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
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
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            foreach (GameObject enemy in enemies)
            {
                if (enemy.GetComponent<Rigidbody>() != null)
                {
                    var x = Random.Range(-1, 1);
                    var y = Random.Range(-1, 1);
                    var z = Random.Range(-1, 1);
                    var vel = Random.Range(10, 10);
                    enemy.GetComponent<Rigidbody>().AddForce(new Vector3(x, y, z) * vel, ForceMode.Impulse);
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            //remove all targeters
            SwapPositions();
            foreach(GameObject targeter in targeters)
            {
                Destroy(targeter);
            }
            targeters.Clear();
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
                Debug.Log(targeters[i].name + " is offscreen with pos " + enemyScreenPos);
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
                    Debug.Log(targeters[i].name + " is obscured");
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
