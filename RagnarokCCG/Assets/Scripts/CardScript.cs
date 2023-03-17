using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Camera _mainCamera;
    Vector3 _offset;
    public Transform DefaultParent, DefaultTempCardParent;
    GameObject _tempCardGO;

    private void Awake()
    {
        _mainCamera = Camera.allCameras[0];
        _tempCardGO = GameObject.Find("TempCardGO");
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
       _offset = transform.position - _mainCamera.ScreenToWorldPoint(eventData.position);

        DefaultParent = DefaultTempCardParent = transform.parent;

        _tempCardGO.transform.SetParent(DefaultParent);
        _tempCardGO.transform.SetSiblingIndex(transform.GetSiblingIndex());

        transform.SetParent(DefaultParent.parent);

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 newPos = _mainCamera.ScreenToWorldPoint(eventData.position);
        newPos.z = 0;
        transform.position = newPos + _offset;

        if(_tempCardGO.transform.parent != DefaultTempCardParent)
            _tempCardGO.transform.SetParent(DefaultTempCardParent);

        CheckPosition();
    }
    public void OnEndDrag(PointerEventData eventData)
    {
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
