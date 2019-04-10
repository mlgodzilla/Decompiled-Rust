// Decompiled with JetBrains decompiler
// Type: RandomStaticPrefab
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class RandomStaticPrefab : MonoBehaviour
{
  public uint Seed;
  public float Probability;
  public string ResourceFolder;

  protected void Start()
  {
    uint seed = SeedEx.Seed(((Component) this).get_transform().get_position(), World.Seed + this.Seed);
    if ((double) SeedRandom.Value(ref seed) > (double) this.Probability)
    {
      GameManager.Destroy((Component) this, 0.0f);
    }
    else
    {
      Prefab.LoadRandom("assets/bundled/prefabs/autospawn/" + this.ResourceFolder, ref seed, (GameManager) null, (PrefabAttribute.Library) null, true).Spawn(((Component) this).get_transform());
      GameManager.Destroy((Component) this, 0.0f);
    }
  }

  public RandomStaticPrefab()
  {
    base.\u002Ector();
  }
}
