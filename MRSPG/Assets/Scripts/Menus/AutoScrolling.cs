using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoScrolling : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    #region Variables

    [SerializeField]
    bool _canAutoScroll=true;
    [SerializeField]
    float _autoScrollSpeed = .21f, _setTimetoScroll = 3, _setTimetoReturnToStart = 3;

    float _timeToScroll, _timeToReturntoStart;

    [SerializeField]
    ScrollRect _autoScroll;

    #endregion

    private void Start ( )
    {

        if ( _autoScroll == null )
            _autoScroll = GetComponent<ScrollRect> ( );

        _timeToScroll = _setTimetoScroll;
        _timeToReturntoStart = _setTimetoReturnToStart;
    }

    private void Update ( )
    {
        AutoScroll ( );
    }

    void AutoScroll ( )
    {
        if ( _canAutoScroll )
        {
            if ( ReachedEnd ( ) )
            {
                if ( _timeToReturntoStart > 0 )
                {
                    _timeToReturntoStart-= Time.deltaTime;
                }
                else
                {
                    _autoScroll.verticalScrollbar.value = 1;
                    _timeToReturntoStart = _setTimetoReturnToStart;
                }
            }
            else
            {
                if ( _timeToScroll > 0 )
                {
                    _timeToScroll -= Time.deltaTime;
                }
                else
                {
                    _autoScroll.verticalScrollbar.value -= _autoScrollSpeed * Time.deltaTime;
                }
            }
        }
        else
        {
            _timeToScroll = _setTimetoScroll;
        }
    }

    bool ReachedEnd ( )
    {
        return _autoScroll.verticalScrollbar.value <= 0;
    }

    public void OnBeginDrag ( PointerEventData eventData )
    {
        _canAutoScroll = false;
    }

    public void OnEndDrag ( PointerEventData eventData )
    {
        _canAutoScroll= true;
    }
}
