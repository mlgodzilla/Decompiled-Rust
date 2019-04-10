// Decompiled with JetBrains decompiler
// Type: ConditionalModel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class ConditionalModel : PrefabAttribute
{
  public bool onClient = true;
  public bool onServer = true;
  public GameObjectRef prefab;
  [NonSerialized]
  public ModelConditionTest[] conditions;

  protected override void AttributeSetup(
    GameObject rootObj,
    string name,
    bool serverside,
    bool clientside,
    bool bundling)
  {
    base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
    this.conditions = (ModelConditionTest[]) ((Component) this).GetComponentsInChildren<ModelConditionTest>(true);
  }

  public bool RunTests(BaseEntity parent)
  {
    for (int index = 0; index < this.conditions.Length; ++index)
    {
      if (!this.conditions[index].DoTest(parent))
        return false;
    }
    return true;
  }

  public GameObject InstantiateSkin(BaseEntity parent)
  {
    if (!this.onServer && this.isServer)
      return (GameObject) null;
    GameObject prefab = this.gameManager.CreatePrefab(this.prefab.resourcePath, ((Component) parent).get_transform(), false);
    if (Object.op_Implicit((Object) prefab))
    {
      prefab.get_transform().set_localPosition(this.worldPosition);
      prefab.get_transform().set_localRotation(this.worldRotation);
      prefab.AwakeFromInstantiate();
    }
    return prefab;
  }

  protected override System.Type GetIndexedType()
  {
    return typeof (ConditionalModel);
  }
}
