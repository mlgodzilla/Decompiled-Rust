﻿// Decompiled with JetBrains decompiler
// Type: MusicUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class MusicUtil
{
  public const float OneSixteenth = 0.0625f;

  public static double BeatsToSeconds(float tempo, float beats)
  {
    return 60.0 / (double) tempo * (double) beats;
  }

  public static double BarsToSeconds(float tempo, float bars)
  {
    return MusicUtil.BeatsToSeconds(tempo, bars * 4f);
  }

  public static int SecondsToSamples(double seconds)
  {
    return MusicUtil.SecondsToSamples(seconds, AudioSettings.get_outputSampleRate());
  }

  public static int SecondsToSamples(double seconds, int sampleRate)
  {
    return (int) ((double) sampleRate * seconds);
  }

  public static int SecondsToSamples(float seconds)
  {
    return MusicUtil.SecondsToSamples(seconds, AudioSettings.get_outputSampleRate());
  }

  public static int SecondsToSamples(float seconds, int sampleRate)
  {
    return (int) ((double) sampleRate * (double) seconds);
  }

  public static int BarsToSamples(float tempo, float bars, int sampleRate)
  {
    return MusicUtil.SecondsToSamples(MusicUtil.BarsToSeconds(tempo, bars), sampleRate);
  }

  public static int BarsToSamples(float tempo, float bars)
  {
    return MusicUtil.SecondsToSamples(MusicUtil.BarsToSeconds(tempo, bars));
  }

  public static int BeatsToSamples(float tempo, float beats)
  {
    return MusicUtil.SecondsToSamples(MusicUtil.BeatsToSeconds(tempo, beats));
  }

  public static float SecondsToBeats(float tempo, double seconds)
  {
    return tempo / 60f * (float) seconds;
  }

  public static float SecondsToBars(float tempo, double seconds)
  {
    return MusicUtil.SecondsToBeats(tempo, seconds) / 4f;
  }

  public static float Quantize(float position, float gridSize)
  {
    return Mathf.Round(position / gridSize) * gridSize;
  }

  public static float FlooredQuantize(float position, float gridSize)
  {
    return Mathf.Floor(position / gridSize) * gridSize;
  }
}
