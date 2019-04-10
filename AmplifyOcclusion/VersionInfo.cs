// Decompiled with JetBrains decompiler
// Type: AmplifyOcclusion.VersionInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace AmplifyOcclusion
{
  [Serializable]
  public class VersionInfo
  {
    private static string StageSuffix = "_dev002";
    public const byte Major = 2;
    public const byte Minor = 0;
    public const byte Release = 0;
    [SerializeField]
    private int m_major;
    [SerializeField]
    private int m_minor;
    [SerializeField]
    private int m_release;

    public static string StaticToString()
    {
      return string.Format("{0}.{1}.{2}", (object) (byte) 2, (object) (byte) 0, (object) (byte) 0) + VersionInfo.StageSuffix;
    }

    public override string ToString()
    {
      return string.Format("{0}.{1}.{2}", (object) this.m_major, (object) this.m_minor, (object) this.m_release) + VersionInfo.StageSuffix;
    }

    public int Number
    {
      get
      {
        return this.m_major * 100 + this.m_minor * 10 + this.m_release;
      }
    }

    private VersionInfo()
    {
      this.m_major = 2;
      this.m_minor = 0;
      this.m_release = 0;
    }

    private VersionInfo(byte major, byte minor, byte release)
    {
      this.m_major = (int) major;
      this.m_minor = (int) minor;
      this.m_release = (int) release;
    }

    public static VersionInfo Current()
    {
      return new VersionInfo((byte) 2, (byte) 0, (byte) 0);
    }

    public static bool Matches(VersionInfo version)
    {
      if (2 == version.m_major && version.m_minor == 0)
        return version.m_release == 0;
      return false;
    }
  }
}
