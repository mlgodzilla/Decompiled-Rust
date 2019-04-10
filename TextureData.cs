// Decompiled with JetBrains decompiler
// Type: TextureData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public struct TextureData
{
  public int width;
  public int height;
  public Color32[] colors;

  public TextureData(Texture2D tex)
  {
    if (Object.op_Inequality((Object) tex, (Object) null))
    {
      this.width = ((Texture) tex).get_width();
      this.height = ((Texture) tex).get_height();
      this.colors = tex.GetPixels32();
    }
    else
    {
      this.width = 0;
      this.height = 0;
      this.colors = (Color32[]) null;
    }
  }

  public Color32 GetColor(int x, int y)
  {
    return this.colors[y * this.width + x];
  }

  public int GetShort(int x, int y)
  {
    return (int) BitUtility.DecodeShort(this.GetColor(x, y));
  }

  public int GetInt(int x, int y)
  {
    return BitUtility.DecodeInt(this.GetColor(x, y));
  }

  public float GetFloat(int x, int y)
  {
    return BitUtility.DecodeFloat(this.GetColor(x, y));
  }

  public float GetHalf(int x, int y)
  {
    return BitUtility.Short2Float(this.GetShort(x, y));
  }

  public Vector4 GetVector(int x, int y)
  {
    return BitUtility.DecodeVector(this.GetColor(x, y));
  }

  public Vector3 GetNormal(int x, int y)
  {
    return BitUtility.DecodeNormal(Color32.op_Implicit(this.GetColor(x, y)));
  }

  public Color32 GetInterpolatedColor(float x, float y)
  {
    float num1 = x * (float) (this.width - 1);
    float num2 = y * (float) (this.height - 1);
    int x1 = Mathf.Clamp((int) num1, 1, this.width - 2);
    int y1 = Mathf.Clamp((int) num2, 1, this.height - 2);
    int x2 = Mathf.Min(x1 + 1, this.width - 2);
    int y2 = Mathf.Min(y1 + 1, this.height - 2);
    Color color1 = Color32.op_Implicit(this.GetColor(x1, y1));
    Color color2 = Color32.op_Implicit(this.GetColor(x2, y1));
    Color color3 = Color32.op_Implicit(this.GetColor(x1, y2));
    Color color4 = Color32.op_Implicit(this.GetColor(x2, y2));
    float num3 = num1 - (float) x1;
    float num4 = num2 - (float) y1;
    Color color5 = color2;
    double num5 = (double) num3;
    return Color32.op_Implicit(Color.Lerp(Color.Lerp(color1, color5, (float) num5), Color.Lerp(color3, color4, num3), num4));
  }

  public int GetInterpolatedInt(float x, float y)
  {
    return this.GetInt(Mathf.Clamp(Mathf.RoundToInt(x * (float) (this.width - 1)), 1, this.width - 2), Mathf.Clamp(Mathf.RoundToInt(y * (float) (this.height - 1)), 1, this.height - 2));
  }

  public int GetInterpolatedShort(float x, float y)
  {
    return this.GetShort(Mathf.Clamp(Mathf.RoundToInt(x * (float) (this.width - 1)), 1, this.width - 2), Mathf.Clamp(Mathf.RoundToInt(y * (float) (this.height - 1)), 1, this.height - 2));
  }

  public float GetInterpolatedFloat(float x, float y)
  {
    float num1 = x * (float) (this.width - 1);
    float num2 = y * (float) (this.height - 1);
    int x1 = Mathf.Clamp((int) num1, 1, this.width - 2);
    int y1 = Mathf.Clamp((int) num2, 1, this.height - 2);
    int x2 = Mathf.Min(x1 + 1, this.width - 2);
    int y2 = Mathf.Min(y1 + 1, this.height - 2);
    double num3 = (double) this.GetFloat(x1, y1);
    float num4 = this.GetFloat(x2, y1);
    float num5 = this.GetFloat(x1, y2);
    float num6 = this.GetFloat(x2, y2);
    float num7 = num1 - (float) x1;
    float num8 = num2 - (float) y1;
    double num9 = (double) num4;
    double num10 = (double) num7;
    return Mathf.Lerp(Mathf.Lerp((float) num3, (float) num9, (float) num10), Mathf.Lerp(num5, num6, num7), num8);
  }

  public float GetInterpolatedHalf(float x, float y)
  {
    float num1 = x * (float) (this.width - 1);
    float num2 = y * (float) (this.height - 1);
    int x1 = Mathf.Clamp((int) num1, 1, this.width - 2);
    int y1 = Mathf.Clamp((int) num2, 1, this.height - 2);
    int x2 = Mathf.Min(x1 + 1, this.width - 2);
    int y2 = Mathf.Min(y1 + 1, this.height - 2);
    double half1 = (double) this.GetHalf(x1, y1);
    float half2 = this.GetHalf(x2, y1);
    float half3 = this.GetHalf(x1, y2);
    float half4 = this.GetHalf(x2, y2);
    float num3 = num1 - (float) x1;
    float num4 = num2 - (float) y1;
    double num5 = (double) half2;
    double num6 = (double) num3;
    return Mathf.Lerp(Mathf.Lerp((float) half1, (float) num5, (float) num6), Mathf.Lerp(half3, half4, num3), num4);
  }

  public Vector4 GetInterpolatedVector(float x, float y)
  {
    float num1 = x * (float) (this.width - 1);
    float num2 = y * (float) (this.height - 1);
    int x1 = Mathf.Clamp((int) num1, 1, this.width - 2);
    int y1 = Mathf.Clamp((int) num2, 1, this.height - 2);
    int x2 = Mathf.Min(x1 + 1, this.width - 2);
    int y2 = Mathf.Min(y1 + 1, this.height - 2);
    Vector4 vector1 = this.GetVector(x1, y1);
    Vector4 vector2 = this.GetVector(x2, y1);
    Vector4 vector3 = this.GetVector(x1, y2);
    Vector4 vector4 = this.GetVector(x2, y2);
    float num3 = num1 - (float) x1;
    float num4 = num2 - (float) y1;
    Vector4 vector4_1 = vector2;
    double num5 = (double) num3;
    return Vector4.Lerp(Vector4.Lerp(vector1, vector4_1, (float) num5), Vector4.Lerp(vector3, vector4, num3), num4);
  }

  public Vector3 GetInterpolatedNormal(float x, float y)
  {
    float num1 = x * (float) (this.width - 1);
    float num2 = y * (float) (this.height - 1);
    int x1 = Mathf.Clamp((int) num1, 1, this.width - 2);
    int y1 = Mathf.Clamp((int) num2, 1, this.height - 2);
    int x2 = Mathf.Min(x1 + 1, this.width - 2);
    int y2 = Mathf.Min(y1 + 1, this.height - 2);
    Vector3 normal1 = this.GetNormal(x1, y1);
    Vector3 normal2 = this.GetNormal(x2, y1);
    Vector3 normal3 = this.GetNormal(x1, y2);
    Vector3 normal4 = this.GetNormal(x2, y2);
    float num3 = num1 - (float) x1;
    float num4 = num2 - (float) y1;
    Vector3 vector3 = normal2;
    double num5 = (double) num3;
    return Vector3.Lerp(Vector3.Lerp(normal1, vector3, (float) num5), Vector3.Lerp(normal3, normal4, num3), num4);
  }
}
