// Decompiled with JetBrains decompiler
// Type: DeferredExtension
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[RequireComponent(typeof (CommandBufferManager))]
[RequireComponent(typeof (Camera))]
[ExecuteInEditMode]
public class DeferredExtension : MonoBehaviour
{
  public ExtendGBufferParams extendGBuffer;
  public SubsurfaceScatteringParams subsurfaceScattering;
  public Texture2D blueNoise;
  public float depthScale;
  public bool debug;

  public DeferredExtension()
  {
    base.\u002Ector();
  }
}
