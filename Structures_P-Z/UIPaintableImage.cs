// Decompiled with JetBrains decompiler
// Type: UIPaintableImage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class UIPaintableImage : MonoBehaviour
{
  public RawImage image;
  public int texSize;
  public Color clearColor;
  public FilterMode filterMode;
  public bool mipmaps;

  public RectTransform rectTransform
  {
    get
    {
      return ((Component) this).get_transform() as RectTransform;
    }
  }

  public UIPaintableImage()
  {
    base.\u002Ector();
  }

  public enum DrawMode
  {
    AlphaBlended,
    Additive,
    Lighten,
    Erase,
  }
}
