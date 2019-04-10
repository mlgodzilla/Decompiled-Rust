// Decompiled with JetBrains decompiler
// Type: ItemModConsumeChance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ItemModConsumeChance : ItemModConsume
{
  public float chanceForSecondaryConsume = 0.5f;
  public GameObjectRef secondaryConsumeEffect;
  public ItemModConsumable secondaryConsumable;

  private bool GetChance()
  {
    Random.State state = Random.get_state();
    Random.InitState(Time.get_frameCount());
    int num = (double) Random.Range(0.0f, 1f) <= (double) this.chanceForSecondaryConsume ? 1 : 0;
    Random.set_state(state);
    return num != 0;
  }

  public override ItemModConsumable GetConsumable()
  {
    if (this.GetChance())
      return this.secondaryConsumable;
    return base.GetConsumable();
  }

  public override GameObjectRef GetConsumeEffect()
  {
    if (this.GetChance())
      return this.secondaryConsumeEffect;
    return base.GetConsumeEffect();
  }
}
