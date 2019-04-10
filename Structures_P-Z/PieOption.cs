// Decompiled with JetBrains decompiler
// Type: PieOption
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.UI;

public class PieOption : MonoBehaviour
{
  public PieShape background;
  public Image imageIcon;

  public void UpdateOption(
    float startSlice,
    float sliceSize,
    float border,
    string optionTitle,
    float outerSize,
    float innerSize,
    float imageSize,
    Sprite sprite)
  {
    if (Object.op_Equality((Object) this.background, (Object) null))
      return;
    Rect rect = this.background.get_rectTransform().get_rect();
    double num1 = (double) ((Rect) ref rect).get_height() * 0.5;
    float num2 = (float) (num1 * ((double) innerSize + ((double) outerSize - (double) innerSize) * 0.5));
    float num3 = (float) (num1 * ((double) outerSize - (double) innerSize));
    this.background.startRadius = startSlice;
    this.background.endRadius = startSlice + sliceSize;
    this.background.border = border;
    this.background.outerSize = outerSize;
    this.background.innerSize = innerSize;
    this.background.set_color(new Color(Random.Range(0.0f, 1f), Random.Range(0.0f, 1f), Random.Range(0.0f, 1f), 0.0f));
    double num4 = (double) startSlice + (double) sliceSize * 0.5;
    ((Transform) ((Graphic) this.imageIcon).get_rectTransform()).set_localPosition(new Vector3(Mathf.Sin((float) (num4 * (Math.PI / 180.0))) * num2, Mathf.Cos((float) (num4 * (Math.PI / 180.0))) * num2));
    ((Graphic) this.imageIcon).get_rectTransform().set_sizeDelta(new Vector2(num3 * imageSize, num3 * imageSize));
    this.imageIcon.set_sprite(sprite);
  }

  public PieOption()
  {
    base.\u002Ector();
  }
}
