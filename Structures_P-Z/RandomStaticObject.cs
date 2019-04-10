// Decompiled with JetBrains decompiler
// Type: RandomStaticObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class RandomStaticObject : MonoBehaviour
{
  public uint Seed;
  public float Probability;
  public GameObject[] Candidates;

  protected void Start()
  {
    uint num1 = SeedEx.Seed(((Component) this).get_transform().get_position(), World.Seed + this.Seed);
    if ((double) SeedRandom.Value(ref num1) > (double) this.Probability)
    {
      for (int index = 0; index < this.Candidates.Length; ++index)
        GameManager.Destroy(this.Candidates[index], 0.0f);
      GameManager.Destroy((Component) this, 0.0f);
    }
    else
    {
      int num2 = SeedRandom.Range(num1, 0, ((Component) this).get_transform().get_childCount());
      for (int index = 0; index < this.Candidates.Length; ++index)
      {
        GameObject candidate = this.Candidates[index];
        if (index == num2)
          candidate.SetActive(true);
        else
          GameManager.Destroy(candidate, 0.0f);
      }
      GameManager.Destroy((Component) this, 0.0f);
    }
  }

  public RandomStaticObject()
  {
    base.\u002Ector();
  }
}
