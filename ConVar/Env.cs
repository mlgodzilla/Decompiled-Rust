// Decompiled with JetBrains decompiler
// Type: ConVar.Env
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("env")]
  public class Env : ConsoleSystem
  {
    [ServerVar]
    public static bool progresstime
    {
      set
      {
        if (Object.op_Equality((Object) TOD_Sky.get_Instance(), (Object) null))
          return;
        TOD_Sky.get_Instance().get_Components().get_Time().ProgressTime = (__Null) (value ? 1 : 0);
      }
      get
      {
        if (Object.op_Equality((Object) TOD_Sky.get_Instance(), (Object) null))
          return false;
        return (bool) TOD_Sky.get_Instance().get_Components().get_Time().ProgressTime;
      }
    }

    [ServerVar]
    public static float time
    {
      set
      {
        if (Object.op_Equality((Object) TOD_Sky.get_Instance(), (Object) null))
          return;
        ((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).Hour = (__Null) (double) value;
      }
      get
      {
        if (Object.op_Equality((Object) TOD_Sky.get_Instance(), (Object) null))
          return 0.0f;
        return (float) ((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).Hour;
      }
    }

    [ServerVar]
    public static int day
    {
      set
      {
        if (Object.op_Equality((Object) TOD_Sky.get_Instance(), (Object) null))
          return;
        ((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).Day = (__Null) value;
      }
      get
      {
        if (Object.op_Equality((Object) TOD_Sky.get_Instance(), (Object) null))
          return 0;
        return (int) ((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).Day;
      }
    }

    [ServerVar]
    public static int month
    {
      set
      {
        if (Object.op_Equality((Object) TOD_Sky.get_Instance(), (Object) null))
          return;
        ((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).Month = (__Null) value;
      }
      get
      {
        if (Object.op_Equality((Object) TOD_Sky.get_Instance(), (Object) null))
          return 0;
        return (int) ((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).Month;
      }
    }

    [ServerVar]
    public static int year
    {
      set
      {
        if (Object.op_Equality((Object) TOD_Sky.get_Instance(), (Object) null))
          return;
        ((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).Year = (__Null) value;
      }
      get
      {
        if (Object.op_Equality((Object) TOD_Sky.get_Instance(), (Object) null))
          return 0;
        return (int) ((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).Year;
      }
    }

    [ServerVar]
    public static void addtime(ConsoleSystem.Arg arg)
    {
      if (Object.op_Equality((Object) TOD_Sky.get_Instance(), (Object) null))
        return;
      DateTime dateTime = ((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).get_DateTime();
      dateTime = dateTime.Add(arg.GetTimeSpan(0));
      ((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).set_DateTime(dateTime);
    }

    public Env()
    {
      base.\u002Ector();
    }
  }
}
