// Decompiled with JetBrains decompiler
// Type: uiPlayerPreview
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class uiPlayerPreview : SingletonComponent<uiPlayerPreview>
{
  public Camera previewCamera;
  public PlayerModel playermodel;
  public ReflectionProbe reflectionProbe;
  public SegmentMaskPositioning segmentMask;

  public uiPlayerPreview()
  {
    base.\u002Ector();
  }
}
