// Decompiled with JetBrains decompiler
// Type: RandomDynamicPrefab
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class RandomDynamicPrefab : MonoBehaviour, IClientComponent, ILOD
{
  public uint Seed;
  public float Distance;
  public float Probability;
  public string ResourceFolder;

  public RandomDynamicPrefab()
  {
    base.\u002Ector();
  }
}
