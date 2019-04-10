// Decompiled with JetBrains decompiler
// Type: SwapKeycard
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class SwapKeycard : MonoBehaviour
{
  public GameObject[] accessLevels;

  public void UpdateAccessLevel(int level)
  {
    foreach (GameObject accessLevel in this.accessLevels)
      accessLevel.SetActive(false);
    this.accessLevels[level - 1].SetActive(true);
  }

  public SwapKeycard()
  {
    base.\u002Ector();
  }
}
