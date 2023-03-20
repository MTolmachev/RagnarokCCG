using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

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

        IsDraggable = GameManager.IsPlayerTurn &&
                      (
                        (DefaultParent.GetComponent<DropPlaceScript>().Type == FieldType.PLAYER_HAND &&
                        GameManager.PlayerMana >= GetComponent<CardInfoScript>().SelfCard.Manacost) ||
                        (DefaultParent.GetComponent<DropPlaceScript>().Type == FieldType.PLAYER_FIELD &&
                        GetComponent<CardInfoScript>().SelfCard.CanAtack)
                      );

        if (!IsDraggable)
            return;
        if (GetComponent<CardInfoScript>().SelfCard.CanAtack)
            GameManager.HighlightTargets(true);

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

        if (_tempCardGO.transform.parent != DefaultTempCardParent)
            _tempCardGO.transform.SetParent(DefaultTempCardParent);


        if (DefaultParent.GetComponent<DropPlaceScript>().Type != FieldType.PLAYER_FIELD)
            CheckPosition();
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsDraggable)
            return;

        GameManager.HighlightTargets(false);

        transform.SetParent(DefaultParent);
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        transform.SetSiblingIndex(_tempCardGO.transform.GetSiblingIndex());


        _tempCardGO.transform.SetParent(GameObject.Find("Canvas").transform);
        _tempCardGO.transform.localPosition = new Vector3(2800, 0);
    }

    void CheckPosition()
    {
        int newIndex = DefaultTempCardParent.childCount;

        for (int i = 0; i < DefaultTempCardParent.childCount; i++)
        {
            if (transform.position.x < DefaultTempCardParent.GetChild(i).position.x)
            {
                newIndex = i;

                if (_tempCardGO.transform.GetSiblingIndex() < newIndex)
                    newIndex--;

                break;
            }
        }

        _tempCardGO.transform.SetSiblingIndex(newIndex);
    }

    public void MoveToField(Transform field)
    {
        transform.SetParent(GameObject.Find("Canvas").transform);
        transform.DOMove(field.position, .5f);
    }

    public void MoveToTarget(Transform target)
    {
        
        StartCoroutine(MoveToTargetCor(target));
        
    }

    IEnumerator MoveToTargetCor(Transform target)
    {
        Vector3 pos = transform.position;
        Transform parent = transform.parent;
        int index = transform.GetSiblingIndex();

        transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = false;
        transform.SetParent(GameObject.Find("Canvas").transform);
        transform.DOMove(target.position, .25f);
        yield return new WaitForSeconds(.5f);
        transform.DOMove(pos, .25f);
        yield return new WaitForSeconds(.5f);
        transform.SetParent(parent);
        transform.SetSiblingIndex(index);
        transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = true;
    }
}
