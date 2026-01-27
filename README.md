# Inventory System

### Usage
This was developed for a personal project and the remenant of that is `private PlayerMovement _playerMovement;` with code that freezes the player's movement `_playerMovement.FreezeMovement();` if this is something you don't want simply remove it. (If you need help ask me)

### Code
Every item in your inventory follows the class:
```cs
 [Serializable]
    public class InventoryItem
    {
        [CanBeNull] public GameObject prefab;
        [HideInInspector] public GameObject instance;
        [HideInInspector] public InventoryItemAnimator instanceItemAnimator;

        // I use this to slow the player when they hold this item
        public bool slowsPlayer = true;
    }
```
Your inventory's items are managed in the list `inventoryItems`
You must use `InventoryItemAnimator` on your animator with equip and unequip animations, it is easy to remove this logic though.

Your basic inventory bindings (other than cycling up and down the inventory) are stored in a list also `InventorySlotBinds` 
This allows you to allow your players to assign their own hotkeys for important slots 
This parameter is automatically capacitated to `inventoryMaxItems` on awake.

#### Cycle logic works using a postive/negative action binding so make sure to assign one.

## Modularity 

### Equipping Items
```cs
// This takes in type InventoryItem
EquipItem(itemToEquip);
```
### Unequpping Items
```cs
UnequipAll();
```

#### Get current item
```cs
// This returns type Inventory Item
GetCurrentItem();
```
#### Get item slot number 
This can be used for auto organization if you want a minecraft style inventory
```cs
// This takes in type InventoryItem
GetItemSlotNumber(itemToCheck);
```

etc etc...

