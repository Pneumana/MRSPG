using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class LockOnSystem : MonoBehaviour
{
    List<GameObject> enemies = new List<GameObject> ();
    List<GameObject> targeters = new List<GameObject> ();
    private void Start()
    {
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
                newTargeter.GetComponent<Image>().color = new Color(0, 1, 1, 0.1f);
                targeters.Add(newTargeter);
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
            foreach(GameObject targeter in targeters)
            {
                Destroy(targeter);
            }
            targeters.Clear();
        }
        float closest = float.MaxValue;
        GameObject closestTarget = null;
        if (targeters.Count <= 0)
            return;
        for (int i =0; i<enemies.Count-1; i++)
        {
            targeters[i].transform.position = Camera.main.WorldToScreenPoint(enemies[i].transform.position);
            targeters[i].GetComponent<Image>().color = new Color(0, 1, 1, 0.1f);
            var dist = Vector2.Distance(targeters[i].transform.position, ui.position);
            if (dist < closest)
            {
                closest = dist;
                closestTarget = targeters[i];
            }
        }
        if (closestTarget != null)
        {
            closestTarget.GetComponent<Image>().color = new Color(0, 1, 1, 1f);
        }
    }
}
