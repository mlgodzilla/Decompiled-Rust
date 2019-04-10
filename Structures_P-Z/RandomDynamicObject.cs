// Decompiled with JetBrains decompiler
// Type: RandomDynamicObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class RandomDynamicObject : MonoBehaviour, IClientComponent, ILOD
{
  public uint Seed;
  public float Distance;
  public float Probability;
  public GameObject[] Candidates;

  public RandomDynamicObject()
  {
    base.\u002Ector();
  }
}
