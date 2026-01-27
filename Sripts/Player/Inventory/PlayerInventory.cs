using Items;
using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using System.Collections.Generic;

namespace Player.Inventory
{
    [Serializable]
    public class InventoryItem
    {
        [CanBeNull] public GameObject prefab;
        [HideInInspector] public GameObject instance;
        [HideInInspector] public InventoryItemAnimator instanceItemAnimator;
        // I use this with my character controller
        public bool slowsPlayer = true;
    }
    [DisallowMultipleComponent]
    public class PlayerInventory : MonoBehaviour
    {
        [Header("Player inventory")]
        public List<InventoryItem> inventoryItems = new();
        [SerializeField] private int inventoryMaxItems = 3;
        
        // These are added at start because the player starts with them
        [Header("Compass")]
        [SerializeField] private InventoryItem compass;
        [Header("Map")]
        [SerializeField] private InventoryItem map;
        [Header("Flashlight")]
        [SerializeField] private InventoryItem flashlight;

        [Header("Input")]
        public List<InputActionReference> InventorySlotBinds = new();
        public InputActionReference cycleInventory;

        private enum InventorySlotDirection
        {
            Up,
            Down
        }
        
        // Feel free to get rid of this
        private PlayerMovement _playerMovement;

        private void Awake()
        {
            // Add the base items
            AddItemToInventory(map);
            AddItemToInventory(compass);

            InventorySlotBinds.Capacity = inventoryMaxItems;
            
            for (int i = 0; i < InventorySlotBinds.Count; i++)
            {
                int index = i; // Capture current index
                InventorySlotBinds[i].action.performed += ctx => InventoryBindPressed(index);
            }
            
            cycleInventory.action.performed += ctx => CycleInventorySlot();
            
            _playerMovement = transform.root.GetComponentInChildren<PlayerMovement>();
        }

        public void AddItemToInventory(InventoryItem item)
        {
            if (inventoryItems.Count >= inventoryMaxItems)
            {
                // TODO: Popup saying inv full
                return;
            }
            item.instance = Instantiate(item.prefab, transform, true);
            item.instance.transform.position = Vector3.zero;
            item.instance.transform.localPosition = Vector3.zero;
            item.instance.SetActive(false);
            item.instanceItemAnimator = item.instance.GetComponentInChildren<InventoryItemAnimator>();
            inventoryItems.Add(item);
        }

        // Public just in case I want to use it for smth
        public void UnequipAll()
        {
            StartCoroutine(UnequipAllRoutine());
        }

        private IEnumerator UnequipAllRoutine()
        {
            foreach (var invItem in inventoryItems)
            {
                if (!invItem.instance.activeSelf) continue;
        
                InventoryItemAnimator animator = invItem.instanceItemAnimator;
                if (animator)
                {
                    if(animator.GetCurrentPlayingAnimationClip() != animator.unequipAnimation 
                       && animator.GetCurrentPlayingAnimationClip() != animator.equipAnimation) 
                        animator.StartUnequip(); 
                    
                    while (!animator.IsUnequipDone())
                    {
                        yield return null;
                    }
                }
        
                invItem.instance.SetActive(false);
            }

            _playerMovement.UnfreezeMovement();
        }

        private void EquipItem(InventoryItem item)
        {
            // Prevent cutting off the animations
            if (IsAnyItemEquipped())
            {
                InventoryItem preItem = GetCurrentItem();
                InventoryItemAnimator preAnimator = preItem.instanceItemAnimator;

                if (preAnimator.GetCurrentPlayingAnimationClip() == preAnimator.equipAnimation
                    || preAnimator.GetCurrentPlayingAnimationClip() == preAnimator.unequipAnimation) 
                { return; }
            }
            
            StartCoroutine(EquipItemRoutine(item));
        }
        
        private IEnumerator EquipItemRoutine(InventoryItem item)
        {
            if(IsAnyItemEquipped()) 
                yield return StartCoroutine(UnequipAllRoutine());
            
            item.instance.SetActive(true);
            item.instanceItemAnimator.StartEquipAnimation();
            
            if(item.slowsPlayer)
              _playerMovement.FreezeMovement();
        }

        private InventoryItem GetCurrentItem()
        {
            foreach (var item in inventoryItems)
            {
                if (item.instance.activeSelf)
                    return item;
            }

            return null;
        }

        private bool IsAnyItemEquipped()
        {
            return inventoryItems.Any(item => item.instance.activeSelf);
        }

        private void InventoryBindPressed(int i)
        {
            if (i >= inventoryItems.Count) return;
    
            InventoryItem associate = inventoryItems[i];
            
            if (IsAnyItemEquipped())
            {
                InventoryItem currentItem = GetCurrentItem();
                InventoryItemAnimator animator = currentItem.instanceItemAnimator;
                
                UnequipAll();
                return;
            }
    
            EquipItem(associate);
        }

        private int GetItemSlotNumber(InventoryItem item)
        {
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i] == item)
                {
                    return i;
                }
            }

            // 'Null'
            return -1;
        }

        // If the user cycles down / up to the last slot it will unequip everything                                                  
        private void CycleInventorySlot()
        {
            if (inventoryItems.Count == 0) return;

            float actionVal = cycleInventory.action.ReadValue<float>();
            InventorySlotDirection direction = Mathf.Approximately(actionVal, 1) 
                ? InventorySlotDirection.Up 
                : InventorySlotDirection.Down;

            InventoryItem currentItem = GetCurrentItem();
            int currentIndex = currentItem != null ? GetItemSlotNumber(currentItem) : -1;

            int nextIndex;
            if (currentIndex == -1)
            {
                nextIndex = direction == InventorySlotDirection.Up 
                    ? Mathf.Min(1, inventoryItems.Count - 1) 
                    : 0;
            }
            else
            {
                nextIndex = direction == InventorySlotDirection.Up 
                    ? currentIndex + 1 
                    : currentIndex - 1;

                if (nextIndex < 0 || nextIndex >= inventoryItems.Count)
                {
                    UnequipAll();
                    return;
                }
            }

            EquipItem(inventoryItems[nextIndex]);
        }
    }
}
