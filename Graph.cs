// Decompiled with JetBrains decompiler
// Type: Graph
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public abstract class Graph : MonoBehaviour
{
  public Material Material;
  public int Resolution;
  public Vector2 ScreenFill;
  public Vector2 ScreenOrigin;
  public Vector2 Pivot;
  public Rect Area;
  internal float CurrentValue;
  private int index;
  private float[] values;
  private float max;

  protected abstract float GetValue();

  protected abstract Color GetColor(float value);

  protected Vector3 GetVertex(float x, float y)
  {
    return new Vector3(x, y, 0.0f);
  }

  protected void Update()
  {
    if (this.values == null || this.values.Length != this.Resolution)
      this.values = new float[this.Resolution];
    this.max = 0.0f;
    for (int index = 0; index < this.values.Length - 1; ++index)
      this.max = Mathf.Max(this.max, this.values[index] = this.values[index + 1]);
    this.max = Mathf.Max(this.max, this.CurrentValue = this.values[this.values.Length - 1] = this.GetValue());
  }

  protected void OnGUI()
  {
    if (Event.get_current().get_type() != 7 || this.values == null || this.values.Length == 0)
      return;
    float num1 = Mathf.Max(((Rect) ref this.Area).get_width(), (float) this.ScreenFill.x * (float) Screen.get_width());
    float num2 = Mathf.Max(((Rect) ref this.Area).get_height(), (float) this.ScreenFill.y * (float) Screen.get_height());
    float num3 = (float) ((double) ((Rect) ref this.Area).get_x() - this.Pivot.x * (double) num1 + this.ScreenOrigin.x * (double) Screen.get_width());
    float num4 = (float) ((double) ((Rect) ref this.Area).get_y() - this.Pivot.y * (double) num2 + this.ScreenOrigin.y * (double) Screen.get_height());
    GL.PushMatrix();
    this.Material.SetPass(0);
    GL.LoadPixelMatrix();
    GL.Begin(7);
    for (int index = 0; index < this.values.Length; ++index)
    {
      float num5 = this.values[index];
      float num6 = num1 / (float) this.values.Length;
      float num7 = num2 * num5 / this.max;
      float num8 = num3 + (float) index * num6;
      float num9 = num4;
      GL.Color(this.GetColor(num5));
      GL.Vertex(this.GetVertex(num8 + 0.0f, num9 + num7));
      GL.Vertex(this.GetVertex(num8 + num6, num9 + num7));
      GL.Vertex(this.GetVertex(num8 + num6, num9 + 0.0f));
      GL.Vertex(this.GetVertex(num8 + 0.0f, num9 + 0.0f));
    }
    GL.End();
    GL.PopMatrix();
  }

  protected Graph()
  {
    base.\u002Ector();
  }
}
