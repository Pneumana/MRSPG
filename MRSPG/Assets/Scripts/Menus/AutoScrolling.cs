using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoScrolling : MonoBehaviour, ISelectHandler
{
    #region Variables
    ScrollRect _scrollRect;

    float _scrollPos = 1;

    #endregion

    private void Start ( )
    {
        _scrollRect = GetComponentInParent<ScrollRect>(true);
        int childCount = _scrollRect.content.transform.childCount - 1;
        int childIndex = transform.GetSiblingIndex ( );

        childIndex = childIndex < ( ( float ) childCount / 2 ) ? childIndex - 1 : childIndex;

        _scrollPos = 1 - ( ( float ) childIndex / childCount );
    }

    public void OnSelect ( BaseEventData eventData )
    {
        if ( _scrollRect )
        {
            _scrollRect.verticalScrollbar.value = _scrollPos;
        }
        
    }

}
