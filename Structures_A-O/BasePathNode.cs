// Decompiled with JetBrains decompiler
// Type: BasePathNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class BasePathNode : MonoBehaviour
{
  public List<BasePathNode> linked;
  public float maxVelocityOnApproach;
  public bool straightaway;

  public void OnDrawGizmosSelected()
  {
  }

  public BasePathNode()
  {
    base.\u002Ector();
  }
}
