// Decompiled with JetBrains decompiler
// Type: ImpostorAsset
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class ImpostorAsset : ScriptableObject
{
  public ImpostorAsset.TextureEntry[] textures;
  public Vector2 size;
  public Vector2 pivot;
  public Mesh mesh;

  public Texture2D FindTexture(string name)
  {
    foreach (ImpostorAsset.TextureEntry texture in this.textures)
    {
      if (texture.name == name)
        return texture.texture;
    }
    return (Texture2D) null;
  }

  public ImpostorAsset()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class TextureEntry
  {
    public string name;
    public Texture2D texture;

    public TextureEntry(string name, Texture2D texture)
    {
      this.name = name;
      this.texture = texture;
    }
  }
}
