// Decompiled with JetBrains decompiler
// Type: PostOpaqueDepth
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof (CommandBufferManager))]
public class PostOpaqueDepth : MonoBehaviour
{
  public RenderTexture postOpaqueDepth;

  public RenderTexture PostOpaque
  {
    get
    {
      return this.postOpaqueDepth;
    }
  }

  public PostOpaqueDepth()
  {
    base.\u002Ector();
  }
}
