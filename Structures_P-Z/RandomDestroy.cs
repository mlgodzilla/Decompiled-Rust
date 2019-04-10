// Decompiled with JetBrains decompiler
// Type: RandomDestroy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class RandomDestroy : MonoBehaviour
{
  public uint Seed;
  public float Probability;

  protected void Start()
  {
    uint num = SeedEx.Seed(((Component) this).get_transform().get_position(), World.Seed + this.Seed);
    if ((double) SeedRandom.Value(ref num) > (double) this.Probability)
      GameManager.Destroy((Component) this, 0.0f);
    else
      GameManager.Destroy(((Component) this).get_gameObject(), 0.0f);
  }

  public RandomDestroy()
  {
    base.\u002Ector();
  }
}
