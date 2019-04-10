// Decompiled with JetBrains decompiler
// Type: Deployable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class Deployable : PrefabAttribute
{
  public Vector3 guideMeshScale = Vector3.get_one();
  public bool guideLights = true;
  public Mesh guideMesh;
  public bool wantsInstanceData;
  public bool copyInventoryFromItem;
  public bool setSocketParent;
  public bool toSlot;
  public BaseEntity.Slot slot;
  public GameObjectRef placeEffect;

  protected override System.Type GetIndexedType()
  {
    return typeof (Deployable);
  }
}
