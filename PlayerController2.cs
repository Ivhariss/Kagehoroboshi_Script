using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController2 : MonoBehaviour
{
    [SerializeField] private float playerMoveSpeed = 1.0f;
    private Rigidbody rb;
    private PlayerAction playerAction;
    [SerializeField] private float timeToDir = 0;
    private Animator animator;
    [SerializeField] private TextMeshProUGUI textMeshPro;
    private GameObject lightItem;
    [SerializeField] private bool isCameraLocked = false;
    private InputAction lockButton;
    private InputAction ChangeItemButton;
    [SerializeField] private Item item;
    [SerializeField] private Slot slot;
    [SerializeField] private ItemList itemList;
    [SerializeField] private Transform toolPos;
    [SerializeField] private ItemUI itemUI;
    [SerializeField] private ItemUIAnimation UIAnimation;
    [SerializeField] private BoxCollider pCollider;
    private GameObject Lighter;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerAction = new PlayerAction();
        playerAction.Enable();
        rb = this.GetComponent<Rigidbody>();
        lockButton = playerAction.FindAction("CameraLock");
        ChangeItemButton = playerAction.FindAction("ChangeItem");
    }

    private void Start()
    {
        slot.SetItem(0);
        itemList.InstantiateItem(0);
    }
    private void Update()
    {
        if (ChangeItemButton.WasReleasedThisFrame())
        {
            ItemChange();
            Debug.Log(UIResize.isItemChanged);
        }

    }
    //�J�����̌����Ă�����𐳖ʂƂ��Ĉړ�������
    private void FixedUpdate()
    {
        if (lockButton.IsPressed())
        {
            isCameraLocked = true;
        }
        else isCameraLocked = false;

        PlayerMove();
    }

    //�v���C���[�̈ړ�
    void PlayerMove()
    {
        //���͏��擾
        Vector3 controlDir = new Vector3(playerAction.Player.Move.ReadValue<Vector2>().x, 0, playerAction.Player.Move.ReadValue<Vector2>().y);
        //�J�����̕����iXZ�j��P�ʃx�N�g����
        Vector3 cameraDir = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1));
        //�ړ������̌���
        Vector3 moveDir = cameraDir * controlDir.z + Camera.main.transform.right * controlDir.x;
        rb.velocity = moveDir * playerMoveSpeed + new Vector3(0, rb.velocity.y, 0);

        //�U�����
        if (moveDir != Vector3.zero && isCameraLocked == false)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir), timeToDir * Time.deltaTime);
        }
        else if (moveDir != Vector3.zero && isCameraLocked == true)
        {
            transform.rotation = Quaternion.LookRotation(cameraDir);
        }

    }


    //�A�C�e���E������
    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.CompareTag("Item"))
        {
            textMeshPro.text = "PickUp";
            //���̌�ɏE�����͂��󂯎��A�莝���̓���X�g�ɓ����
            if (playerAction.Player.PickUp.WasPressedThisFrame() == true)
            {
                Lighter = toolPos.GetChild(0).gameObject;
                Lighter.SetActive(false);
                //�����ɏE���Ă���A�C�e���ԍ�������(Slot�̃A�C�e���X�v���C�g�z��ԍ��Q��)
                switch (other.gameObject.name)
                {
                    case "Flashlight":
                        slot.SetItem(1);
                        itemUI.GetItemData(other.gameObject.GetComponent<LightStatus>());
                        other.gameObject.SetActive(false);
                        textMeshPro.text = string.Empty;
                        itemList.InstantiateItem(1);
                        break;
                    case "Camera":
                        slot.SetItem(2);
                        itemUI.GetItemData(itemList.items[2].GetComponent<LightStatus>());
                        other.gameObject.SetActive(false);
                        textMeshPro.text = string.Empty;
                        itemList.InstantiateItem(2);
                        break;
                    case "Lantern":
                        slot.SetItem(3);
                        itemUI.GetItemData(itemList.items[3].GetComponent<LightStatus>());
                        other.gameObject.SetActive(false);
                        textMeshPro.text = string.Empty;
                        itemList.InstantiateItem(3);
                        break;
                    case "Hotaru":
                        slot.SetItem(4);
                        itemUI.GetItemData(itemList.items[4].GetComponent<LightStatus>());
                        other.gameObject.SetActive(false);
                        textMeshPro.text = string.Empty;
                        itemList.InstantiateItem(4);
                        break;
                }
                //lightStatus.isPicked = true;                
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Item")
        {
            textMeshPro.text = string.Empty;
        }
    }

    public void ItemChange()
    {
        if (UIResize.isItemChanged == true)
        {
            UIResize.isItemChanged = false;
            Lighter = toolPos.GetChild(0).gameObject;
            if (toolPos.childCount > 1)
            {
                lightItem = toolPos.GetChild(1).gameObject;
                lightItem.gameObject.SetActive(false);
            }
            Lighter.gameObject.SetActive(true);

        }
        else
        {
            UIResize.isItemChanged = true;
            Lighter.gameObject.SetActive(false);
            if (toolPos.childCount > 1) lightItem.gameObject.SetActive(true);
            else Lighter.gameObject.SetActive(true);
        }
    }

    
}
