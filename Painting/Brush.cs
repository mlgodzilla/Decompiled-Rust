// Decompiled with JetBrains decompiler
// Type: Painting.Brush
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Painting
{
  [Serializable]
  public class Brush
  {
    public float spacing;
    public Vector2 brushSize;
    public Texture2D texture;
    public Color color;
    public bool erase;
  }
}
