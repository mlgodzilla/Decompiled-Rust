// Decompiled with JetBrains decompiler
// Type: FlameJet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class FlameJet : MonoBehaviour
{
  public LineRenderer line;
  public float tesselation;
  private float length;
  public float maxLength;
  public float drag;
  private int numSegments;
  public bool on;
  private Vector3[] lastWorldSegments;
  private Vector3[] currentSegments;
  public Color startColor;
  public Color endColor;
  public Color currentColor;

  private void Initialize()
  {
    this.currentColor = this.startColor;
  }

  private void Awake()
  {
    this.Initialize();
  }

  public void LateUpdate()
  {
    this.UpdateLine();
  }

  public void SetOn(bool isOn)
  {
    this.on = isOn;
  }

  private float curve(float x)
  {
    return x * x;
  }

  private void UpdateLine()
  {
    this.currentColor.a = (__Null) (double) Mathf.Lerp((float) this.currentColor.a, this.on ? 1f : 0.0f, Time.get_deltaTime() * 40f);
    this.line.SetColors(this.currentColor, this.endColor);
    this.tesselation = 0.1f;
    this.numSegments = Mathf.CeilToInt(this.maxLength / this.tesselation);
    float num1 = this.maxLength / (float) this.numSegments;
    Vector3[] vector3Array = new Vector3[this.numSegments];
    for (int index = 0; index < vector3Array.Length; ++index)
    {
      float num2 = 0.0f;
      float num3 = 0.0f;
      if (this.lastWorldSegments != null && Vector3.op_Inequality(this.lastWorldSegments[index], Vector3.get_zero()))
      {
        Vector3 vector3_1 = ((Component) this).get_transform().InverseTransformPoint(this.lastWorldSegments[index]);
        float num4 = (float) index / (float) vector3Array.Length;
        Vector3 zero = Vector3.get_zero();
        double num5 = (double) Time.get_deltaTime() * (double) this.drag;
        Vector3 vector3_2 = Vector3.Lerp(Vector3.get_zero(), Vector3.Lerp(vector3_1, zero, (float) num5), Mathf.Sqrt(num4));
        num2 = (float) vector3_2.x;
        num3 = (float) vector3_2.y;
      }
      if (index == 0)
        num2 = num3 = 0.0f;
      Vector3 vector3;
      ((Vector3) ref vector3).\u002Ector(num2, num3, (float) index * num1);
      vector3Array[index] = vector3;
      if (this.lastWorldSegments == null)
        this.lastWorldSegments = new Vector3[this.numSegments];
      this.lastWorldSegments[index] = ((Component) this).get_transform().TransformPoint(vector3);
    }
    this.line.SetVertexCount(this.numSegments);
    this.line.SetPositions(vector3Array);
  }

  public FlameJet()
  {
    base.\u002Ector();
  }
}
