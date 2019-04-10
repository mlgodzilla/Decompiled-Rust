// Decompiled with JetBrains decompiler
// Type: EffectRecycle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine.Serialization;

public class EffectRecycle : BaseMonoBehaviour, IClientComponent, IRagdollInhert, IEffectRecycle
{
  [ReadOnly]
  [FormerlySerializedAs("lifeTime")]
  public float detachTime;
  [FormerlySerializedAs("lifeTime")]
  [ReadOnly]
  public float recycleTime;
  public EffectRecycle.PlayMode playMode;
  public EffectRecycle.ParentDestroyBehaviour onParentDestroyed;

  public enum PlayMode
  {
    Once,
    Looped,
  }

  public enum ParentDestroyBehaviour
  {
    Detach,
    Destroy,
    DetachWaitDestroy,
  }
}
