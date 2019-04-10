// Decompiled with JetBrains decompiler
// Type: DecayPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class DecayPoint : PrefabAttribute
{
  [Tooltip("If this point is occupied this will take this % off the power of the decay")]
  public float protection = 0.25f;
  public Socket_Base socket;

  public bool IsOccupied(BaseEntity entity)
  {
    return entity.IsOccupied(this.socket);
  }

  protected override System.Type GetIndexedType()
  {
    return typeof (DecayPoint);
  }
}
