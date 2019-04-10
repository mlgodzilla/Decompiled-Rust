// Decompiled with JetBrains decompiler
// Type: TreeMarker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using UnityEngine;

public class TreeMarker : BaseEntity
{
  public GameObjectRef hitEffect;
  public SoundDefinition hitEffectSound;
  public GameObjectRef spawnEffect;
  public DeferredDecal myDecal;
  private Vector3 initialPosition;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("TreeMarker.OnRpcMessage", 0.1f))
      ;
    return base.OnRpcMessage(player, rpc, msg);
  }
}
