// Decompiled with JetBrains decompiler
// Type: SystemInfoGeneralText
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Text;
using UnityEngine;

public class SystemInfoGeneralText : MonoBehaviour
{
  public UnityEngine.UI.Text text;

  public static string currentInfo
  {
    get
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("System");
      stringBuilder.AppendLine();
      stringBuilder.Append("\tName: ");
      stringBuilder.Append(SystemInfo.get_deviceName());
      stringBuilder.AppendLine();
      stringBuilder.Append("\tOS:   ");
      stringBuilder.Append(SystemInfo.get_operatingSystem());
      stringBuilder.AppendLine();
      stringBuilder.AppendLine();
      stringBuilder.Append("CPU");
      stringBuilder.AppendLine();
      stringBuilder.Append("\tModel:  ");
      stringBuilder.Append(SystemInfo.get_processorType());
      stringBuilder.AppendLine();
      stringBuilder.Append("\tCores:  ");
      stringBuilder.Append(SystemInfo.get_processorCount());
      stringBuilder.AppendLine();
      stringBuilder.Append("\tMemory: ");
      stringBuilder.Append(SystemInfo.get_systemMemorySize());
      stringBuilder.Append(" MB");
      stringBuilder.AppendLine();
      stringBuilder.AppendLine();
      stringBuilder.Append("GPU");
      stringBuilder.AppendLine();
      stringBuilder.Append("\tModel:  ");
      stringBuilder.Append(SystemInfo.get_graphicsDeviceName());
      stringBuilder.AppendLine();
      stringBuilder.Append("\tAPI:    ");
      stringBuilder.Append(SystemInfo.get_graphicsDeviceVersion());
      stringBuilder.AppendLine();
      stringBuilder.Append("\tMemory: ");
      stringBuilder.Append(SystemInfo.get_graphicsMemorySize());
      stringBuilder.Append(" MB");
      stringBuilder.AppendLine();
      stringBuilder.Append("\tSM:     ");
      stringBuilder.Append(SystemInfo.get_graphicsShaderLevel());
      stringBuilder.AppendLine();
      stringBuilder.AppendLine();
      stringBuilder.Append("Process");
      stringBuilder.AppendLine();
      stringBuilder.Append("\tMemory:   ");
      stringBuilder.Append(SystemInfoEx.systemMemoryUsed);
      stringBuilder.Append(" MB");
      stringBuilder.AppendLine();
      stringBuilder.AppendLine();
      stringBuilder.Append("Mono");
      stringBuilder.AppendLine();
      stringBuilder.Append("\tCollects: ");
      stringBuilder.Append(GC.CollectionCount(0));
      stringBuilder.AppendLine();
      stringBuilder.Append("\tMemory:   ");
      stringBuilder.Append(SystemInfoGeneralText.MB(GC.GetTotalMemory(false)));
      stringBuilder.Append(" MB");
      stringBuilder.AppendLine();
      stringBuilder.AppendLine();
      if (World.Seed > 0U && World.Size > 0U)
      {
        stringBuilder.Append("World");
        stringBuilder.AppendLine();
        stringBuilder.Append("\tSeed:        ");
        stringBuilder.Append(World.Seed);
        stringBuilder.AppendLine();
        stringBuilder.Append("\tSize:        ");
        stringBuilder.Append(SystemInfoGeneralText.KM2((float) World.Size));
        stringBuilder.Append(" km\x00B2");
        stringBuilder.AppendLine();
        stringBuilder.Append("\tHeightmap:   ");
        stringBuilder.Append(SystemInfoGeneralText.MB(Object.op_Implicit((Object) TerrainMeta.HeightMap) ? TerrainMeta.HeightMap.GetMemoryUsage() : 0L));
        stringBuilder.Append(" MB");
        stringBuilder.AppendLine();
        stringBuilder.Append("\tWatermap:    ");
        stringBuilder.Append(SystemInfoGeneralText.MB(Object.op_Implicit((Object) TerrainMeta.WaterMap) ? TerrainMeta.WaterMap.GetMemoryUsage() : 0L));
        stringBuilder.Append(" MB");
        stringBuilder.AppendLine();
        stringBuilder.Append("\tSplatmap:    ");
        stringBuilder.Append(SystemInfoGeneralText.MB(Object.op_Implicit((Object) TerrainMeta.SplatMap) ? TerrainMeta.SplatMap.GetMemoryUsage() : 0L));
        stringBuilder.Append(" MB");
        stringBuilder.AppendLine();
        stringBuilder.Append("\tBiomemap:    ");
        stringBuilder.Append(SystemInfoGeneralText.MB(Object.op_Implicit((Object) TerrainMeta.BiomeMap) ? TerrainMeta.BiomeMap.GetMemoryUsage() : 0L));
        stringBuilder.Append(" MB");
        stringBuilder.AppendLine();
        stringBuilder.Append("\tTopologymap: ");
        stringBuilder.Append(SystemInfoGeneralText.MB(Object.op_Implicit((Object) TerrainMeta.TopologyMap) ? TerrainMeta.TopologyMap.GetMemoryUsage() : 0L));
        stringBuilder.Append(" MB");
        stringBuilder.AppendLine();
        stringBuilder.Append("\tAlphamap:    ");
        stringBuilder.Append(SystemInfoGeneralText.MB(Object.op_Implicit((Object) TerrainMeta.AlphaMap) ? TerrainMeta.AlphaMap.GetMemoryUsage() : 0L));
        stringBuilder.Append(" MB");
        stringBuilder.AppendLine();
      }
      stringBuilder.AppendLine();
      if (!string.IsNullOrEmpty(World.Checksum))
      {
        stringBuilder.AppendLine("Checksum");
        stringBuilder.Append('\t');
        stringBuilder.AppendLine(World.Checksum);
      }
      return stringBuilder.ToString();
    }
  }

  protected void Update()
  {
    this.text.set_text(SystemInfoGeneralText.currentInfo);
  }

  private static long MB(long bytes)
  {
    return bytes / 1048576L;
  }

  private static long MB(ulong bytes)
  {
    return SystemInfoGeneralText.MB((long) bytes);
  }

  private static int KM2(float meters)
  {
    return Mathf.RoundToInt((float) ((double) meters * (double) meters * 9.99999997475243E-07));
  }

  public SystemInfoGeneralText()
  {
    base.\u002Ector();
  }
}
