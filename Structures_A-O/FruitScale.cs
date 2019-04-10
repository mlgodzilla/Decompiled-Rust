// Decompiled with JetBrains decompiler
// Type: FruitScale
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class FruitScale : MonoBehaviour, IClientComponent
{
  public void SetProgress(float progress)
  {
    ((Component) this).get_transform().set_localScale(Vector3.op_Multiply(Vector3.get_one(), progress));
  }

  public FruitScale()
  {
    base.\u002Ector();
  }
}
