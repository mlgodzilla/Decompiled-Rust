// Decompiled with JetBrains decompiler
// Type: ConstructionGrade
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionGrade : PrefabAttribute
{
  [NonSerialized]
  public Construction construction;
  public BuildingGrade gradeBase;
  public GameObjectRef skinObject;
  internal List<ItemAmount> _costToBuild;

  public float maxHealth
  {
    get
    {
      if (!Object.op_Implicit((Object) this.gradeBase) || !(bool) ((PrefabAttribute) this.construction))
        return 0.0f;
      return this.gradeBase.baseHealth * this.construction.healthMultiplier;
    }
  }

  public List<ItemAmount> costToBuild
  {
    get
    {
      if (this._costToBuild != null)
        return this._costToBuild;
      this._costToBuild = new List<ItemAmount>();
      foreach (ItemAmount itemAmount in this.gradeBase.baseCost)
        this._costToBuild.Add(new ItemAmount(itemAmount.itemDef, Mathf.Ceil(itemAmount.amount * this.construction.costMultiplier)));
      return this._costToBuild;
    }
  }

  protected override System.Type GetIndexedType()
  {
    return typeof (ConstructionGrade);
  }
}
