using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardMovementScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Camera _mainCamera;
    Vector3 _offset;
    public Transform DefaultParent, DefaultTempCardParent;
    GameObject _tempCardGO;
    public GameManagerScript GameManager;
    public bool IsDraggable;

    private void Awake()
    {
        _mainCamera = Camera.allCameras[0];
        _tempCardGO = GameObject.Find("TempCardGO");
        GameManager = FindObjectOfType<GameManagerScript>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        _offset = transform.position - _mainCamera.ScreenToWorldPoint(eventData.position);

        DefaultParent = DefaultTempCardParent = transform.parent;

        IsDraggable = (DefaultParent.GetComponent<DropPlaceScript>().Type == FieldType.PLAYER_HAND ||
                       DefaultParent.GetComponent<DropPlaceScript>().Type == FieldType.PLAYER_FIELD) &&
                       GameManager.IsPlayerTurn;                       ;

        if (!IsDraggable)
            return;

        _tempCardGO.transform.SetParent(DefaultParent);
        _tempCardGO.transform.SetSiblingIndex(transform.GetSiblingIndex());

        transform.SetParent(DefaultParent.parent);

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsDraggable)
            return;

        Vector3 newPos = _mainCamera.ScreenToWorldPoint(eventData.position);
        transform.position = newPos + _offset;

        if(_tempCardGO.transform.parent != DefaultTempCardParent)
            _tempCardGO.transform.SetParent(DefaultTempCardParent);


        if(DefaultParent.GetComponent<DropPlaceScript>().Type != FieldType.PLAYER_FIELD)
            CheckPosition();
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsDraggable)
            return;

        transform.SetParent(DefaultParent);
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        transform.SetSiblingIndex(_tempCardGO.transform.GetSiblingIndex());


        _tempCardGO.transform.SetParent(GameObject.Find("Canvas").transform);
        _tempCardGO.transform.localPosition = new Vector3(2800, 0);
    }

    void CheckPosition()
    {
        int newIndex = DefaultTempCardParent.childCount;

        for(int i = 0; i < DefaultTempCardParent.childCount; i++)
        {
            if(transform.position.x < DefaultTempCardParent.GetChild(i).position.x)
            {
                newIndex = i;

                if (_tempCardGO.transform.GetSiblingIndex() < newIndex)
                    newIndex--;

                break;
            }
        }

        _tempCardGO.transform.SetSiblingIndex(newIndex);
    }
}
