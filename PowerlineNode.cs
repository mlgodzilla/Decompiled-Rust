// Decompiled with JetBrains decompiler
// Type: PowerlineNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class PowerlineNode : MonoBehaviour
{
  public Material WireMaterial;
  public float MaxDistance;

  protected void Awake()
  {
    if (!Object.op_Implicit((Object) TerrainMeta.Path))
      return;
    TerrainMeta.Path.AddWire(this);
  }

  public PowerlineNode()
  {
    base.\u002Ector();
  }
}
