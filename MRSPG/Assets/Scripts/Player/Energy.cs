using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Energy : MonoBehaviour
{
    #region variables
    int _maxEnergy = 50;
    public int currentEnergy;

    [SerializeField]
    Image _energyImg, _gunRdy, _teleportRdy;

    [SerializeField]
    Sprite [ ] _energySprite;

    List<Image> cells = new List<Image>();

    #endregion

    private void Awake ( )
    {
        //Gets all Values needed for the start of the game
        currentEnergy = _maxEnergy;

        foreach(Transform child in _energyImg.transform.Find("Cells").GetComponentInChildren<Transform>())
        {
            if (child.GetComponent<Image>()!=null)
            {
                cells.Add(child.GetComponent<Image>());
            }
        }
        Debug.Log("found " + cells.Count + " energy cells");
        UIUpdateEnergy();


    }

    private void Update ( )
    {
        UIUpdateEnergy ( );
        if (Input.GetKey(KeyCode.RightArrow))
        {
            GainEnergy(1);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            GainEnergy(-1);
        }
    }

    public void LoseEnergy (int amount )
    {
        currentEnergy -= amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, _maxEnergy);
        UIUpdateEnergy();
    }
      
    //adds energy after every enemy killed
    private void OnCollisionExit ( Collision enemy )
    {
        if ( enemy.collider.name == "StandardEnemy")
        {
            if (Metronome.inst.IsOnBeat()) { GainEnergy(10); }
            else { GainEnergy(5); }
            Debug.Log("Killed a standard enemy.");
            Destroy ( enemy.gameObject );
        }
      
        else if ( enemy.collider.name=="HeavyEnemy" )
        {
            if (Metronome.inst.IsOnBeat()) { GainEnergy(20); }
            else { GainEnergy(10); }
            Destroy ( enemy.gameObject );
        }
      
        else if ( enemy.collider.name=="RangedEnemy" )
        {
            if (Metronome.inst.IsOnBeat()) { GainEnergy(10); }
            else { GainEnergy(5); }
            Destroy ( enemy.gameObject );
        }
    }
    /// <summary>
    /// standard: 10/5, ranged:10,5, heavy: 20/10
    /// </summary>
    public void GainEnergy(int energy) 
    {
        currentEnergy += energy;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, _maxEnergy);
/*        if (currentEnergy > 50)
        {
            currentEnergy = 50;
        }*/
        UIUpdateEnergy();
    }

    [ContextMenu("UpdateUI")]
    void UIUpdateEnergy ( )
    {
        //Code was not updating the UI with this so it was returned to the original so that it will update the UI
        /*
        _energyImg.fillAmount = currentEnergy/50f;
        if(currentEnergy != 50)
            _gunRdy.gameObject.SetActive(false);
        else
        {
            _gunRdy.gameObject.SetActive(true);
        }
        if (currentEnergy >= 20)
            _teleportRdy.gameObject.SetActive(false);
        else
        {
            _teleportRdy.gameObject.SetActive(true);
        }
        */

        //there was a nullreference error from _gunRdy and _teleportRdy so i just made it use this to change the sprite rather that the if else checks
        //because i *really* didnt want to comment out each reference to _gunRdy and _teleportRdy
        // - Connor

        var index = Mathf.FloorToInt(currentEnergy / 10);
        
        index = Mathf.Clamp(index, 0, cells.Count - 1);

        float fill = (currentEnergy - (index * 10)) / 10f;
        cells[index].fillAmount = fill;
        Debug.Log("current cell fill amount is " + fill);
        for (int i = 0; i < index; i++)
        {
            cells[i].fillAmount = 1;
        }
        if(currentEnergy == 0)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                cells[i].fillAmount = 0;
            }
        }
        /*for (int x = end; end < cells.Count; end++)
        {
            cells[x].fillAmount = 0;
        }*/
        //_energyImg.sprite = _energySprite[_energySprite.Length - (1 + Mathf.FloorToInt(currentEnergy/5f))];

        

        /*if ( currentEnergy == 50 )
        {
            _energyImg.sprite = _energySprite [ 0 ];
            *//*_gunRdy.gameObject.SetActive ( true );
            _teleportRdy.gameObject.SetActive ( true );*//*
        }
        else if ( currentEnergy == 45 )
        {
            _energyImg.sprite = _energySprite [ 1 ];
            *//*_gunRdy.gameObject.SetActive(false);
            _teleportRdy.gameObject.SetActive ( true );*//*
        }
        else if ( currentEnergy == 40 )
        {
            _energyImg.sprite = _energySprite [ 2 ];
            *//*_gunRdy.gameObject.SetActive ( false );
            _teleportRdy.gameObject.SetActive ( true );*//*
        }
        else if ( currentEnergy == 35 )
        {
            _energyImg.sprite = _energySprite [ 3 ];
            *//*_gunRdy.gameObject.SetActive ( false );
            _teleportRdy.gameObject.SetActive ( true );*//*
        }
        else if ( currentEnergy == 30 )
        {
            _energyImg.sprite = _energySprite [ 4 ];
            _gunRdy.gameObject.SetActive ( false );
            _teleportRdy.gameObject.SetActive ( true );
        }
        else if ( currentEnergy == 25 )
        {
            _energyImg.sprite = _energySprite [ 5 ];
            _gunRdy.gameObject.SetActive ( false );
            _teleportRdy.gameObject.SetActive ( true );
        }
        else if ( currentEnergy == 20 )
        {
            _energyImg.sprite = _energySprite [ 6 ];
            _gunRdy.gameObject.SetActive ( false );
            _teleportRdy.gameObject.SetActive ( true );
        }
        else if ( currentEnergy == 15 )
        {
            _energyImg.sprite = _energySprite [ 7 ];
            _gunRdy.gameObject.SetActive ( false );
            _teleportRdy.gameObject.SetActive ( false );
        }
        else if ( currentEnergy == 10 )
        {
            _energyImg.sprite = _energySprite [ 8 ];
            _gunRdy.gameObject.SetActive ( false );
            _teleportRdy.gameObject.SetActive ( false );
        }
        else if ( currentEnergy == 5 )
        {
            _energyImg.sprite = _energySprite [ 9 ];
            _gunRdy.gameObject.SetActive ( false );
            _teleportRdy.gameObject.SetActive ( false );
        }
        else if ( currentEnergy == 0 )
        {
            _energyImg.sprite = _energySprite [ 10 ];
            _gunRdy.gameObject.SetActive ( false );
            _teleportRdy .gameObject.SetActive ( false );
        }
        else { return; }*/
    }

}
