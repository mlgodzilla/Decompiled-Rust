// Decompiled with JetBrains decompiler
// Type: ParticleEmitFromParentObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ParticleEmitFromParentObject : MonoBehaviour
{
  public string bonename;
  private Bounds bounds;
  private Transform bone;
  private BaseEntity entity;
  private float lastBoundsUpdate;

  public ParticleEmitFromParentObject()
  {
    base.\u002Ector();
  }
}
