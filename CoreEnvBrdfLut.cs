// Decompiled with JetBrains decompiler
// Type: CoreEnvBrdfLut
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class CoreEnvBrdfLut
{
  private static Texture2D runtimeEnvBrdfLut;

  [RuntimeInitializeOnLoadMethod]
  private static void OnRuntimeLoad()
  {
    CoreEnvBrdfLut.PrepareTextureForRuntime();
    CoreEnvBrdfLut.UpdateReflProbe();
  }

  private static void PrepareTextureForRuntime()
  {
    if (Object.op_Equality((Object) CoreEnvBrdfLut.runtimeEnvBrdfLut, (Object) null))
      CoreEnvBrdfLut.runtimeEnvBrdfLut = CoreEnvBrdfLut.Generate(false);
    Shader.SetGlobalTexture("_EnvBrdfLut", (Texture) CoreEnvBrdfLut.runtimeEnvBrdfLut);
  }

  private static void UpdateReflProbe()
  {
    int num = (int) Mathf.Log((float) RenderSettings.get_defaultReflectionResolution(), 2f) - 1;
    if ((double) Shader.GetGlobalFloat("_ReflProbeMaxMip") == (double) num)
      return;
    Shader.SetGlobalFloat("_ReflProbeMaxMip", (float) num);
  }

  public static Texture2D Generate(bool asset = false)
  {
    TextureFormat textureFormat1 = asset ? (TextureFormat) 17 : (TextureFormat) 16;
    TextureFormat textureFormat2 = SystemInfo.SupportsTextureFormat(textureFormat1) ? textureFormat1 : (TextureFormat) 5;
    int num1 = 128;
    int num2 = 32;
    float num3 = 1f / (float) num1;
    float num4 = 1f / (float) num2;
    Texture2D texture2D = new Texture2D(num1, num2, textureFormat2, false, true);
    ((Object) texture2D).set_name("_EnvBrdfLut");
    ((Texture) texture2D).set_wrapMode((TextureWrapMode) 1);
    ((Texture) texture2D).set_filterMode((FilterMode) 1);
    Color[] colorArray = new Color[num1 * num2];
    float num5 = 1f / 128f;
    for (int index = 0; index < num2; ++index)
    {
      double num6 = (double) ((float) index + 0.5f) * (double) num4;
      float num7 = (float) (num6 * num6);
      float num8 = num7 * num7;
      int num9 = 0;
      int num10 = index * num1;
      for (; num9 < num1; ++num9)
      {
        float num11 = ((float) num9 + 0.5f) * num3;
        Vector3 vector3_1;
        ((Vector3) ref vector3_1).\u002Ector(Mathf.Sqrt((float) (1.0 - (double) num11 * (double) num11)), 0.0f, num11);
        float num12 = 0.0f;
        float num13 = 0.0f;
        for (uint Bits = 0; Bits < 128U; ++Bits)
        {
          float num14 = (float) Bits * num5;
          float num15 = (float) CoreEnvBrdfLut.ReverseBits(Bits) / (float) uint.MaxValue;
          float num16 = 6.283185f * num14;
          float num17 = Mathf.Sqrt((float) ((1.0 - (double) num15) / (1.0 + ((double) num8 - 1.0) * (double) num15)));
          float num18 = Mathf.Sqrt((float) (1.0 - (double) num17 * (double) num17));
          Vector3 vector3_2;
          ((Vector3) ref vector3_2).\u002Ector(num18 * Mathf.Cos(num16), num18 * Mathf.Sin(num16), num17);
          float num19 = Mathf.Max((float) Vector3.op_Subtraction(Vector3.op_Multiply(2f * Vector3.Dot(vector3_1, vector3_2), vector3_2), vector3_1).z, 0.0f);
          float num20 = Mathf.Max((float) vector3_2.z, 0.0f);
          float num21 = Mathf.Max(Vector3.Dot(vector3_1, vector3_2), 0.0f);
          if ((double) num19 > 0.0)
          {
            float num22 = (float) (0.5 / ((double) (num19 * (num11 * (1f - num7) + num7)) + (double) (num11 * (num19 * (1f - num7) + num7))));
            float num23 = (float) ((double) num19 * (double) num22 * (4.0 * (double) num21 / (double) num20));
            float num24 = 1f - num21;
            float num25 = num24 * (float) ((double) num24 * (double) num24 * ((double) num24 * (double) num24));
            num12 += num23 * (1f - num25);
            num13 += num23 * num25;
          }
        }
        float num26 = Mathf.Clamp(num12 * num5, 0.0f, 1f);
        float num27 = Mathf.Clamp(num13 * num5, 0.0f, 1f);
        colorArray[num10++] = new Color(num26, num27, 0.0f, 0.0f);
      }
    }
    texture2D.SetPixels(colorArray);
    texture2D.Apply(false, !asset);
    return texture2D;
  }

  private static uint ReverseBits(uint Bits)
  {
    Bits = Bits << 16 | Bits >> 16;
    Bits = (uint) (((int) Bits & 16711935) << 8) | (Bits & 4278255360U) >> 8;
    Bits = (uint) (((int) Bits & 252645135) << 4) | (Bits & 4042322160U) >> 4;
    Bits = (uint) (((int) Bits & 858993459) << 2) | (Bits & 3435973836U) >> 2;
    Bits = (uint) (((int) Bits & 1431655765) << 1) | (Bits & 2863311530U) >> 1;
    return Bits;
  }
}
