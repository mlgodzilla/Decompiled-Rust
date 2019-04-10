// Decompiled with JetBrains decompiler
// Type: FlashlightBeam
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class FlashlightBeam : MonoBehaviour, IClientComponent
{
  public Vector2 scrollDir;
  public Vector3 localEndPoint;
  public LineRenderer beamRenderer;

  public FlashlightBeam()
  {
    base.\u002Ector();
  }
}
