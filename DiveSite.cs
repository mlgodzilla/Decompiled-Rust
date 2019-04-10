// Decompiled with JetBrains decompiler
// Type: DiveSite
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class DiveSite : JunkPile
{
  public Transform bobber;

  public override float TimeoutPlayerCheckRadius()
  {
    return 40f;
  }
}
