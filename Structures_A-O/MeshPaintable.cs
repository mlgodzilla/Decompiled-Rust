// Decompiled with JetBrains decompiler
// Type: MeshPaintable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class MeshPaintable : MonoBehaviour, IClientComponent
{
  public string replacementTextureName;
  public int textureWidth;
  public int textureHeight;
  public Color clearColor;
  public Texture2D targetTexture;
  public bool hasChanges;

  public MeshPaintable()
  {
    base.\u002Ector();
  }
}
