// Decompiled with JetBrains decompiler
// Type: MapMarkerExplosion
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class MapMarkerExplosion : MapMarker
{
  private float duration = 10f;

  public void SetDuration(float newDuration)
  {
    this.duration = newDuration;
    if (this.IsInvoking(new Action(this.DelayedDestroy)))
      this.CancelInvoke(new Action(this.DelayedDestroy));
    this.Invoke(new Action(this.DelayedDestroy), this.duration * 60f);
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (!info.fromDisk)
      return;
    Debug.LogWarning((object) "Loaded explosion marker from disk, cleaning up");
    this.Invoke(new Action(this.DelayedDestroy), 3f);
  }

  public void DelayedDestroy()
  {
    this.Kill(BaseNetworkable.DestroyMode.None);
  }
}
