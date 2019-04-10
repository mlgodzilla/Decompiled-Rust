// Decompiled with JetBrains decompiler
// Type: WaterCatcher
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class WaterCatcher : LiquidContainer
{
  public float maxItemToCreate = 10f;
  [Header("Outside Test")]
  public Vector3 rainTestPosition = new Vector3(0.0f, 1f, 0.0f);
  public float rainTestSize = 1f;
  [Header("Water Catcher")]
  public ItemDefinition itemToCreate;
  private const float collectInterval = 60f;

  public override void ServerInit()
  {
    base.ServerInit();
    this.AddResource(1);
    this.InvokeRandomized(new Action(this.CollectWater), 60f, 60f, 6f);
  }

  private void CollectWater()
  {
    if (this.IsFull())
      return;
    float num = 0.25f + Climate.GetFog(((Component) this).get_transform().get_position()) * 2f;
    if (this.TestIsOutside())
      num = num + Climate.GetRain(((Component) this).get_transform().get_position()) + Climate.GetSnow(((Component) this).get_transform().get_position()) * 0.5f;
    this.AddResource(Mathf.CeilToInt(this.maxItemToCreate * num));
  }

  private bool IsFull()
  {
    return this.inventory.itemList.Count != 0 && this.inventory.itemList[0].amount >= this.inventory.maxStackSize;
  }

  private bool TestIsOutside()
  {
    Matrix4x4 localToWorldMatrix = ((Component) this).get_transform().get_localToWorldMatrix();
    return !Physics.SphereCast(new Ray(((Matrix4x4) ref localToWorldMatrix).MultiplyPoint3x4(this.rainTestPosition), Vector3.get_up()), this.rainTestSize, 256f, 1101070337);
  }

  private void AddResource(int iAmount)
  {
    this.inventory.AddItem(this.itemToCreate, iAmount);
    this.UpdateOnFlag();
  }
}
