// Decompiled with JetBrains decompiler
// Type: UIPaintBox
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Painting;
using System;
using UnityEngine;
using UnityEngine.Events;

public class UIPaintBox : MonoBehaviour
{
  public UIPaintBox.OnBrushChanged onBrushChanged;
  public Brush brush;

  public void UpdateBrushSize(int size)
  {
    this.brush.brushSize = Vector2.op_Multiply(Vector2.get_one(), (float) size);
    this.brush.spacing = Mathf.Clamp((float) size * 0.1f, 1f, 3f);
    this.OnChanged();
  }

  public void UpdateBrushTexture(Texture2D tex)
  {
    this.brush.texture = tex;
    this.OnChanged();
  }

  public void UpdateBrushColor(Color col)
  {
    this.brush.color.r = col.r;
    this.brush.color.g = col.g;
    this.brush.color.b = col.b;
    this.OnChanged();
  }

  public void UpdateBrushAlpha(float a)
  {
    this.brush.color.a = (__Null) (double) a;
    this.OnChanged();
  }

  public void UpdateBrushEraser(bool b)
  {
    this.brush.erase = b;
  }

  private void OnChanged()
  {
    this.onBrushChanged.Invoke(this.brush);
  }

  public UIPaintBox()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class OnBrushChanged : UnityEvent<Brush>
  {
    public OnBrushChanged()
    {
      base.\u002Ector();
    }
  }
}
