// Decompiled with JetBrains decompiler
// Type: ConVar.Data
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.IO;
using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("data")]
  public class Data : ConsoleSystem
  {
    [ServerVar]
    [ClientVar]
    public static void export(ConsoleSystem.Arg args)
    {
      string str = args.GetString(0, "none");
      string path = Path.Combine(Application.get_persistentDataPath(), str + ".raw");
      if (!(str == "splatmap"))
      {
        if (!(str == "heightmap"))
        {
          if (!(str == "biomemap"))
          {
            if (!(str == "topologymap"))
            {
              if (!(str == "alphamap"))
              {
                if (str == "watermap")
                {
                  if (Object.op_Implicit((Object) TerrainMeta.WaterMap))
                    RawWriter.Write(TerrainMeta.WaterMap.ToEnumerable(), path);
                }
                else
                {
                  args.ReplyWith("Unknown export source: " + str);
                  return;
                }
              }
              else if (Object.op_Implicit((Object) TerrainMeta.AlphaMap))
                RawWriter.Write(TerrainMeta.AlphaMap.ToEnumerable(), path);
            }
            else if (Object.op_Implicit((Object) TerrainMeta.TopologyMap))
              RawWriter.Write(TerrainMeta.TopologyMap.ToEnumerable(), path);
          }
          else if (Object.op_Implicit((Object) TerrainMeta.BiomeMap))
            RawWriter.Write(TerrainMeta.BiomeMap.ToEnumerable(), path);
        }
        else if (Object.op_Implicit((Object) TerrainMeta.HeightMap))
          RawWriter.Write(TerrainMeta.HeightMap.ToEnumerable(), path);
      }
      else if (Object.op_Implicit((Object) TerrainMeta.SplatMap))
        RawWriter.Write(TerrainMeta.SplatMap.ToEnumerable(), path);
      args.ReplyWith("Export written to " + path);
    }

    public Data()
    {
      base.\u002Ector();
    }
  }
}
