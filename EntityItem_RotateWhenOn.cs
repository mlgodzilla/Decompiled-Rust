// Decompiled with JetBrains decompiler
// Type: EntityItem_RotateWhenOn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class EntityItem_RotateWhenOn : EntityComponent<BaseEntity>
{
  public BaseEntity.Flags targetFlag = BaseEntity.Flags.On;
  public EntityItem_RotateWhenOn.State on;
  public EntityItem_RotateWhenOn.State off;
  internal bool currentlyOn;
  internal bool stateInitialized;

  [Serializable]
  public class State
  {
    public float timeToTake = 2f;
    public AnimationCurve animationCurve = new AnimationCurve(new Keyframe[2]
    {
      new Keyframe(0.0f, 0.0f),
      new Keyframe(1f, 1f)
    });
    public string effectOnStart = "";
    public string effectOnFinish = "";
    public float movementLoopFadeOutTime = 0.1f;
    public Vector3 rotation;
    public float initialDelay;
    public SoundDefinition movementLoop;
    public SoundDefinition startSound;
    public SoundDefinition stopSound;
  }
}
