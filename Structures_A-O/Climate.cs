// Decompiled with JetBrains decompiler
// Type: Climate
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class Climate : SingletonComponent<Climate>
{
  private const float fadeAngle = 20f;
  private const float defaultTemp = 15f;
  private const int weatherDurationHours = 18;
  private const int weatherFadeHours = 6;
  [Range(0.0f, 1f)]
  public float BlendingSpeed;
  [Range(1f, 9f)]
  public float FogMultiplier;
  public float FogDarknessDistance;
  public bool DebugLUTBlending;
  public Climate.WeatherParameters Weather;
  public Climate.ClimateParameters Arid;
  public Climate.ClimateParameters Temperate;
  public Climate.ClimateParameters Tundra;
  public Climate.ClimateParameters Arctic;
  private Climate.ClimateParameters[] climates;
  private Climate.WeatherState state;
  private Climate.WeatherState clamps;
  public Climate.WeatherState Overrides;

  protected void Update()
  {
    if (!Object.op_Implicit((Object) TerrainMeta.BiomeMap) || !Object.op_Implicit((Object) TOD_Sky.get_Instance()))
      return;
    TOD_Sky instance = TOD_Sky.get_Instance();
    long num1 = 36000000000;
    long num2 = (long) World.Seed + ((TOD_CycleParameters) instance.Cycle).get_Ticks();
    long num3 = 18L * num1;
    long num4 = 6L * num1;
    long num5 = num2 / num3;
    float t = Mathf.InverseLerp(0.0f, (float) num4, (float) (num2 % num3));
    this.state = Climate.WeatherState.Fade(this.GetWeatherState((uint) ((ulong) num5 % (ulong) uint.MaxValue)), this.GetWeatherState((uint) ((ulong) (num5 + 1L) % (ulong) uint.MaxValue)), t);
    this.state.Override(this.Overrides);
  }

  public static float GetClouds(Vector3 position)
  {
    if (!Object.op_Implicit((Object) SingletonComponent<Climate>.Instance))
      return 0.0f;
    return Mathf.Max(((Climate) SingletonComponent<Climate>.Instance).clamps.Clouds, ((Climate) SingletonComponent<Climate>.Instance).state.Clouds);
  }

  public static float GetCloudOpacity(Vector3 position)
  {
    if (!Object.op_Implicit((Object) SingletonComponent<Climate>.Instance))
      return 1f;
    return Mathf.InverseLerp(0.9f, 0.8f, Climate.GetFog(position));
  }

  public static float GetFog(Vector3 position)
  {
    if (!Object.op_Implicit((Object) SingletonComponent<Climate>.Instance))
      return 0.0f;
    return Mathf.Max(((Climate) SingletonComponent<Climate>.Instance).clamps.Fog, ((Climate) SingletonComponent<Climate>.Instance).state.Fog);
  }

  public static float GetWind(Vector3 position)
  {
    if (!Object.op_Implicit((Object) SingletonComponent<Climate>.Instance))
      return 0.0f;
    return Mathf.Max(((Climate) SingletonComponent<Climate>.Instance).clamps.Wind, ((Climate) SingletonComponent<Climate>.Instance).state.Wind);
  }

  public static float GetRain(Vector3 position)
  {
    if (!Object.op_Implicit((Object) SingletonComponent<Climate>.Instance))
      return 0.0f;
    float num1 = Object.op_Implicit((Object) TerrainMeta.BiomeMap) ? TerrainMeta.BiomeMap.GetBiome(position, 1) : 0.0f;
    float num2 = Object.op_Implicit((Object) TerrainMeta.BiomeMap) ? TerrainMeta.BiomeMap.GetBiome(position, 8) : 0.0f;
    return (float) ((double) Mathf.Max(((Climate) SingletonComponent<Climate>.Instance).clamps.Rain, ((Climate) SingletonComponent<Climate>.Instance).state.Rain) * (double) Mathf.Lerp(1f, 0.5f, num1) * (1.0 - (double) num2));
  }

  public static float GetSnow(Vector3 position)
  {
    if (!Object.op_Implicit((Object) SingletonComponent<Climate>.Instance))
      return 0.0f;
    float num = Object.op_Implicit((Object) TerrainMeta.BiomeMap) ? TerrainMeta.BiomeMap.GetBiome(position, 8) : 0.0f;
    return Mathf.Max(((Climate) SingletonComponent<Climate>.Instance).clamps.Rain, ((Climate) SingletonComponent<Climate>.Instance).state.Rain) * num;
  }

  public static float GetTemperature(Vector3 position)
  {
    if (!Object.op_Implicit((Object) SingletonComponent<Climate>.Instance) || !Object.op_Implicit((Object) TOD_Sky.get_Instance()))
      return 15f;
    Climate.ClimateParameters src;
    Climate.ClimateParameters dst;
    float blendParameters = ((Climate) SingletonComponent<Climate>.Instance).FindBlendParameters(position, out src, out dst);
    if (src == null || dst == null)
      return 15f;
    float hour = (float) ((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).Hour;
    return Mathf.Lerp(src.Temperature.Evaluate(hour), dst.Temperature.Evaluate(hour), blendParameters);
  }

  private Climate.WeatherState GetWeatherState(uint seed)
  {
    int num1 = (int) SeedRandom.Wanghash(ref seed);
    bool flag1 = (double) SeedRandom.Value(ref seed) < (double) this.Weather.CloudChance;
    bool flag2 = (double) SeedRandom.Value(ref seed) < (double) this.Weather.FogChance;
    bool flag3 = (double) SeedRandom.Value(ref seed) < (double) this.Weather.RainChance;
    int num2 = (double) SeedRandom.Value(ref seed) < (double) this.Weather.StormChance ? 1 : 0;
    float num3 = flag1 ? SeedRandom.Value(ref seed) : 0.0f;
    float num4 = flag2 ? 1f : 0.0f;
    float num5 = flag3 ? 1f : 0.0f;
    float num6 = num2 != 0 ? SeedRandom.Value(ref seed) : 0.0f;
    if ((double) num5 > 0.0)
    {
      num5 = Mathf.Max(num5, 0.5f);
      num4 = Mathf.Max(num4, num5);
      num3 = Mathf.Max(num3, num5);
    }
    return new Climate.WeatherState()
    {
      Clouds = num3,
      Fog = num4,
      Wind = num6,
      Rain = num5
    };
  }

  private float FindBlendParameters(
    Vector3 pos,
    out Climate.ClimateParameters src,
    out Climate.ClimateParameters dst)
  {
    if (this.climates == null)
      this.climates = new Climate.ClimateParameters[4]
      {
        this.Arid,
        this.Temperate,
        this.Tundra,
        this.Arctic
      };
    if (Object.op_Equality((Object) TerrainMeta.BiomeMap, (Object) null))
    {
      src = (Climate.ClimateParameters) null;
      dst = (Climate.ClimateParameters) null;
      return 0.5f;
    }
    int biomeMaxType1 = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, -1);
    int biomeMaxType2 = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, ~biomeMaxType1);
    src = this.climates[TerrainBiome.TypeToIndex(biomeMaxType1)];
    dst = this.climates[TerrainBiome.TypeToIndex(biomeMaxType2)];
    return TerrainMeta.BiomeMap.GetBiome(pos, biomeMaxType2);
  }

  public Climate()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class ClimateParameters
  {
    public AnimationCurve Temperature;
    [Horizontal(4, -1)]
    public Climate.Float4 AerialDensity;
    [Horizontal(4, -1)]
    public Climate.Float4 FogDensity;
    [Horizontal(4, -1)]
    public Climate.Texture2D4 LUT;
  }

  [Serializable]
  public class WeatherParameters
  {
    [Range(0.0f, 1f)]
    public float RainChance = 0.5f;
    [Range(0.0f, 1f)]
    public float FogChance = 0.5f;
    [Range(0.0f, 1f)]
    public float CloudChance = 0.5f;
    [Range(0.0f, 1f)]
    public float StormChance = 0.5f;
  }

  public struct WeatherState
  {
    public float Clouds;
    public float Fog;
    public float Wind;
    public float Rain;

    public static Climate.WeatherState Fade(
      Climate.WeatherState a,
      Climate.WeatherState b,
      float t)
    {
      return new Climate.WeatherState()
      {
        Clouds = Mathf.SmoothStep(a.Clouds, b.Clouds, t),
        Fog = Mathf.SmoothStep(a.Fog, b.Fog, t),
        Wind = Mathf.SmoothStep(a.Wind, b.Wind, t),
        Rain = Mathf.SmoothStep(a.Rain, b.Rain, t)
      };
    }

    public void Override(Climate.WeatherState other)
    {
      if ((double) other.Clouds >= 0.0)
        this.Clouds = Mathf.Clamp01(other.Clouds);
      if ((double) other.Fog >= 0.0)
        this.Fog = Mathf.Clamp01(other.Fog);
      if ((double) other.Wind >= 0.0)
        this.Wind = Mathf.Clamp01(other.Wind);
      if ((double) other.Rain < 0.0)
        return;
      this.Rain = Mathf.Clamp01(other.Rain);
    }

    public void Max(Climate.WeatherState other)
    {
      this.Clouds = Mathf.Max(this.Clouds, other.Clouds);
      this.Fog = Mathf.Max(this.Fog, other.Fog);
      this.Wind = Mathf.Max(this.Wind, other.Wind);
      this.Rain = Mathf.Max(this.Rain, other.Rain);
    }
  }

  public class Value4<T>
  {
    public T Dawn;
    public T Noon;
    public T Dusk;
    public T Night;

    public float FindBlendParameters(TOD_Sky sky, out T src, out T dst)
    {
      double num1 = (double) Mathf.Abs(sky.get_SunriseTime() - (float) ((TOD_CycleParameters) sky.Cycle).Hour);
      float num2 = Mathf.Abs(sky.get_SunsetTime() - (float) ((TOD_CycleParameters) sky.Cycle).Hour);
      float num3 = (float) ((180.0 - (double) sky.get_SunZenith()) / 180.0);
      float num4 = 0.1111111f;
      double num5 = (double) num2;
      if (num1 < num5)
      {
        if ((double) num3 < 0.5)
        {
          src = this.Night;
          dst = this.Dawn;
          return Mathf.InverseLerp(0.5f - num4, 0.5f, num3);
        }
        src = this.Dawn;
        dst = this.Noon;
        return Mathf.InverseLerp(0.5f, 0.5f + num4, num3);
      }
      if ((double) num3 > 0.5)
      {
        src = this.Noon;
        dst = this.Dusk;
        return Mathf.InverseLerp(0.5f + num4, 0.5f, num3);
      }
      src = this.Dusk;
      dst = this.Night;
      return Mathf.InverseLerp(0.5f, 0.5f - num4, num3);
    }
  }

  [Serializable]
  public class Float4 : Climate.Value4<float>
  {
  }

  [Serializable]
  public class Color4 : Climate.Value4<Color>
  {
  }

  [Serializable]
  public class Texture2D4 : Climate.Value4<Texture2D>
  {
  }
}
