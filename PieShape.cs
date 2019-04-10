// Decompiled with JetBrains decompiler
// Type: PieShape
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class PieShape : Graphic
{
  [Range(0.0f, 1f)]
  public float outerSize;
  [Range(0.0f, 1f)]
  public float innerSize;
  public float startRadius;
  public float endRadius;
  public float border;
  public bool debugDrawing;

  protected virtual void OnPopulateMesh(VertexHelper vbo)
  {
    vbo.Clear();
    UIVertex simpleVert = (UIVertex) UIVertex.simpleVert;
    float startRadius = this.startRadius;
    float num1 = this.endRadius;
    if ((double) this.startRadius > (double) this.endRadius)
      num1 = this.endRadius + 360f;
    float num2 = Mathf.Floor((float) (((double) num1 - (double) startRadius) / 6.0));
    if ((double) num2 <= 1.0)
      return;
    float num3 = (num1 - startRadius) / num2;
    float num4 = startRadius + (float) (((double) num1 - (double) startRadius) * 0.5);
    Color color = this.get_color();
    Rect rect = this.get_rectTransform().get_rect();
    float num5 = ((Rect) ref rect).get_height() * 0.5f;
    Vector2 vector2 = Vector2.op_Multiply(new Vector2(Mathf.Sin(num4 * ((float) Math.PI / 180f)), Mathf.Cos(num4 * ((float) Math.PI / 180f))), this.border);
    int num6 = 0;
    for (float num7 = startRadius; (double) num7 < (double) num1; num7 += num3)
    {
      if (this.debugDrawing)
        color = !Color.op_Equality(color, Color.get_red()) ? Color.get_red() : Color.get_white();
      simpleVert.color = (__Null) Color32.op_Implicit(color);
      float num8 = Mathf.Sin(num7 * ((float) Math.PI / 180f));
      float num9 = Mathf.Cos(num7 * ((float) Math.PI / 180f));
      float num10 = num7 + num3;
      if ((double) num10 > (double) num1)
        num10 = num1;
      float num11 = Mathf.Sin(num10 * ((float) Math.PI / 180f));
      float num12 = Mathf.Cos(num10 * ((float) Math.PI / 180f));
      simpleVert.position = (__Null) Vector2.op_Implicit(Vector2.op_Addition(new Vector2(num8 * this.outerSize * num5, num9 * this.outerSize * num5), vector2));
      vbo.AddVert(simpleVert);
      simpleVert.position = (__Null) Vector2.op_Implicit(Vector2.op_Addition(new Vector2(num11 * this.outerSize * num5, num12 * this.outerSize * num5), vector2));
      vbo.AddVert(simpleVert);
      simpleVert.position = (__Null) Vector2.op_Implicit(Vector2.op_Addition(new Vector2(num11 * this.innerSize * num5, num12 * this.innerSize * num5), vector2));
      vbo.AddVert(simpleVert);
      simpleVert.position = (__Null) Vector2.op_Implicit(Vector2.op_Addition(new Vector2(num8 * this.innerSize * num5, num9 * this.innerSize * num5), vector2));
      vbo.AddVert(simpleVert);
      vbo.AddTriangle(num6, num6 + 1, num6 + 2);
      vbo.AddTriangle(num6 + 2, num6 + 3, num6);
      num6 += 4;
    }
  }

  public PieShape()
  {
    base.\u002Ector();
  }
}
