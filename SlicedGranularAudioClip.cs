// Decompiled with JetBrains decompiler
// Type: SlicedGranularAudioClip
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class SlicedGranularAudioClip : MonoBehaviour, IClientComponent
{
  public AudioClip sourceClip;
  public AudioClip granularClip;
  public int sampleRate;
  public float grainAttack;
  public float grainSustain;
  public float grainRelease;
  public float grainFrequency;
  public int grainAttackSamples;
  public int grainSustainSamples;
  public int grainReleaseSamples;
  public int grainFrequencySamples;
  public int samplesUntilNextGrain;
  public List<SlicedGranularAudioClip.Grain> grains;
  public List<int> startPositions;
  public int lastStartPositionIdx;

  public SlicedGranularAudioClip()
  {
    base.\u002Ector();
  }

  public class Grain
  {
    private float[] sourceData;
    private int startSample;
    private int currentSample;
    private int attackTimeSamples;
    private int sustainTimeSamples;
    private int releaseTimeSamples;
    private float gain;
    private float gainPerSampleAttack;
    private float gainPerSampleRelease;
    private int attackEndSample;
    private int releaseStartSample;
    private int endSample;

    public bool finished
    {
      get
      {
        return this.currentSample >= this.endSample;
      }
    }

    public void Init(float[] source, int start, int attack, int sustain, int release)
    {
      this.sourceData = source;
      this.startSample = start;
      this.currentSample = start;
      this.attackTimeSamples = attack;
      this.sustainTimeSamples = sustain;
      this.releaseTimeSamples = release;
      this.gainPerSampleAttack = 0.5f / (float) this.attackTimeSamples;
      this.gainPerSampleRelease = -0.5f / (float) this.releaseTimeSamples;
      this.attackEndSample = this.startSample + this.attackTimeSamples;
      this.releaseStartSample = this.attackEndSample + this.sustainTimeSamples;
      this.endSample = this.releaseStartSample + this.releaseTimeSamples;
      this.gain = 0.0f;
    }

    public float GetSample()
    {
      if (this.currentSample >= this.sourceData.Length)
        return 0.0f;
      double num = (double) this.sourceData[this.currentSample];
      if (this.currentSample <= this.attackEndSample)
      {
        this.gain += this.gainPerSampleAttack;
        if ((double) this.gain > 0.5)
          this.gain = 0.5f;
      }
      else if (this.currentSample >= this.releaseStartSample)
      {
        this.gain += this.gainPerSampleRelease;
        if ((double) this.gain < 0.0)
          this.gain = 0.0f;
      }
      ++this.currentSample;
      double gain = (double) this.gain;
      return (float) (num * gain);
    }

    public void FadeOut()
    {
      this.releaseStartSample = this.currentSample;
      this.endSample = this.releaseStartSample + this.releaseTimeSamples;
    }
  }
}
