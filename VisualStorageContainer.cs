// Decompiled with JetBrains decompiler
// Type: VisualStorageContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;

public class VisualStorageContainer : LootContainer
{
  public VisualStorageContainerNode[] displayNodes;
  public VisualStorageContainer.DisplayModel[] displayModels;
  public Transform nodeParent;
  public GameObject defaultDisplayModel;

  public override void ServerInit()
  {
    base.ServerInit();
  }

  public override void OnItemAddedOrRemoved(Item item, bool added)
  {
    base.OnItemAddedOrRemoved(item, added);
  }

  public override void PopulateLoot()
  {
    base.PopulateLoot();
    for (int slot1 = 0; slot1 < this.inventorySlots; ++slot1)
    {
      Item slot2 = this.inventory.GetSlot(slot1);
      if (slot2 != null)
      {
        DroppedItem component = (DroppedItem) ((Component) slot2.Drop(Vector3.op_Addition(((Component) this.displayNodes[slot1]).get_transform().get_position(), new Vector3(0.0f, 0.25f, 0.0f)), Vector3.get_zero(), ((Component) this.displayNodes[slot1]).get_transform().get_rotation())).GetComponent<DroppedItem>();
        if (Object.op_Implicit((Object) component))
        {
          this.ReceiveCollisionMessages(false);
          this.CancelInvoke(new Action(component.IdleDestroy));
          Rigidbody componentInChildren = (Rigidbody) ((Component) component).GetComponentInChildren<Rigidbody>();
          if (Object.op_Implicit((Object) componentInChildren))
            componentInChildren.set_constraints((RigidbodyConstraints) 10);
        }
      }
    }
  }

  public void ClearRigidBodies()
  {
    if (this.displayModels == null)
      return;
    foreach (VisualStorageContainer.DisplayModel displayModel in this.displayModels)
    {
      if (displayModel != null)
        Object.Destroy((Object) displayModel.displayModel.GetComponentInChildren<Rigidbody>());
    }
  }

  public void SetItemsVisible(bool vis)
  {
    if (this.displayModels == null)
      return;
    foreach (VisualStorageContainer.DisplayModel displayModel in this.displayModels)
    {
      if (displayModel != null)
      {
        LODGroup componentInChildren = (LODGroup) displayModel.displayModel.GetComponentInChildren<LODGroup>();
        if (Object.op_Implicit((Object) componentInChildren))
          componentInChildren.set_localReferencePoint(vis ? Vector3.get_zero() : new Vector3(10000f, 10000f, 10000f));
        else
          Debug.Log((object) ("VisualStorageContainer item missing LODGroup" + ((Object) displayModel.displayModel.get_gameObject()).get_name()));
      }
    }
  }

  public void ItemUpdateComplete()
  {
    this.ClearRigidBodies();
    this.SetItemsVisible(true);
  }

  public void UpdateVisibleItems(ItemContainer msg)
  {
    for (int index = 0; index < this.displayModels.Length; ++index)
    {
      VisualStorageContainer.DisplayModel displayModel = this.displayModels[index];
      if (displayModel != null)
      {
        Object.Destroy((Object) displayModel.displayModel);
        this.displayModels[index] = (VisualStorageContainer.DisplayModel) null;
      }
    }
    if (msg == null)
      return;
    using (List<Item>.Enumerator enumerator = ((List<Item>) msg.contents).GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        Item current = enumerator.Current;
        ItemDefinition itemDefinition = ItemManager.FindItemDefinition((int) current.itemid);
        GameObject gameObject = itemDefinition.worldModelPrefab == null || !itemDefinition.worldModelPrefab.isValid ? (GameObject) Object.Instantiate<GameObject>((M0) this.defaultDisplayModel) : itemDefinition.worldModelPrefab.Instantiate((Transform) null);
        if (Object.op_Implicit((Object) gameObject))
        {
          gameObject.get_transform().set_position(Vector3.op_Addition(((Component) this.displayNodes[current.slot]).get_transform().get_position(), new Vector3(0.0f, 0.25f, 0.0f)));
          gameObject.get_transform().set_rotation(((Component) this.displayNodes[current.slot]).get_transform().get_rotation());
          M0 m0 = gameObject.AddComponent<Rigidbody>();
          ((Rigidbody) m0).set_mass(1f);
          ((Rigidbody) m0).set_drag(0.1f);
          ((Rigidbody) m0).set_angularDrag(0.1f);
          ((Rigidbody) m0).set_interpolation((RigidbodyInterpolation) 1);
          ((Rigidbody) m0).set_constraints((RigidbodyConstraints) 10);
          this.displayModels[current.slot].displayModel = gameObject;
          this.displayModels[current.slot].slot = (int) current.slot;
          this.displayModels[current.slot].def = itemDefinition;
          gameObject.SetActive(true);
        }
      }
    }
    this.SetItemsVisible(false);
    this.CancelInvoke(new Action(this.ItemUpdateComplete));
    this.Invoke(new Action(this.ItemUpdateComplete), 1f);
  }

  [Serializable]
  public class DisplayModel
  {
    public GameObject displayModel;
    public ItemDefinition def;
    public int slot;
  }
}
