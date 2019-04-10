﻿// Decompiled with JetBrains decompiler
// Type: NetworkWriteEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using SilentOrbit.ProtocolBuffers;
using System.IO;
using UnityEngine;

public static class NetworkWriteEx
{
  public static void WriteObject<T>(this Write write, T obj)
  {
    if (typeof (T) == typeof (Vector3))
      write.Vector3(GenericsUtil.Cast<T, Vector3>(obj));
    else if (typeof (T) == typeof (Ray))
      write.Ray(GenericsUtil.Cast<T, Ray>(obj));
    else if (typeof (T) == typeof (float))
      write.Float(GenericsUtil.Cast<T, float>(obj));
    else if (typeof (T) == typeof (short))
      write.Int16(GenericsUtil.Cast<T, short>(obj));
    else if (typeof (T) == typeof (ushort))
      write.UInt16(GenericsUtil.Cast<T, ushort>(obj));
    else if (typeof (T) == typeof (int))
      write.Int32(GenericsUtil.Cast<T, int>(obj));
    else if (typeof (T) == typeof (uint))
      write.UInt32(GenericsUtil.Cast<T, uint>(obj));
    else if (typeof (T) == typeof (byte[]))
      write.Bytes(GenericsUtil.Cast<T, byte[]>(obj));
    else if (typeof (T) == typeof (long))
      write.Int64(GenericsUtil.Cast<T, long>(obj));
    else if (typeof (T) == typeof (ulong))
      write.UInt64(GenericsUtil.Cast<T, ulong>(obj));
    else if (typeof (T) == typeof (string))
      write.String(GenericsUtil.Cast<T, string>(obj));
    else if (typeof (T) == typeof (sbyte))
      write.Int8(GenericsUtil.Cast<T, sbyte>(obj));
    else if (typeof (T) == typeof (byte))
      write.UInt8(GenericsUtil.Cast<T, byte>(obj));
    else if (typeof (T) == typeof (bool))
      write.Bool(GenericsUtil.Cast<T, bool>(obj));
    else if ((object) obj is IProto)
      ((IProto) (object) obj).WriteToStream((Stream) write);
    else
      Debug.LogError((object) ("NetworkData.Write - no handler to write " + (object) obj + " -> " + (object) obj.GetType()));
  }
}
