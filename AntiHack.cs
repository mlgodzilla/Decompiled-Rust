// Decompiled with JetBrains decompiler
// Type: AntiHack
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using EasyAntiCheat.Server.Scout;
using Oxide.Core;
using System.Collections.Generic;
using UnityEngine;

public static class AntiHack
{
  private static Collider[] buffer = new Collider[4];
  private static Dictionary<ulong, int> kicks = new Dictionary<ulong, int>();
  private static Dictionary<ulong, int> bans = new Dictionary<ulong, int>();
  private const int movement_mask = 429990145;
  private const int grounded_mask = 1503731969;
  private const int vehicle_mask = 8192;
  private const int player_mask = 131072;

  public static void ResetTimer(BasePlayer ply)
  {
    ply.lastViolationTime = Time.get_realtimeSinceStartup();
  }

  public static bool ValidateMove(BasePlayer ply, TickInterpolator ticks, float deltaTime)
  {
    using (TimeWarning.New("AntiHack.ValidateMove", 0.1f))
    {
      if (ply.IsFlying)
        ply.lastAdminCheatTime = Time.get_realtimeSinceStartup();
      if (ply.IsAdmin && (ConVar.AntiHack.userlevel < 1 || ConVar.AntiHack.admincheat && ply.UsedAdminCheat(1f)) || ply.IsDeveloper && (ConVar.AntiHack.userlevel < 2 || ConVar.AntiHack.admincheat && ply.UsedAdminCheat(1f)) || (ply.IsSleeping() || ply.IsWounded() || (ply.IsSpectating() || ply.IsDead())))
        return true;
      bool flag = (double) deltaTime > (double) ConVar.AntiHack.maxdeltatime;
      using (TimeWarning.New("IsNoClipping", 0.1f))
      {
        if (AntiHack.IsNoClipping(ply, ticks, deltaTime))
        {
          if (flag)
            return false;
          AntiHack.AddViolation(ply, AntiHackType.NoClip, ConVar.AntiHack.noclip_penalty * ticks.Length);
          if (ConVar.AntiHack.noclip_reject)
            return false;
        }
      }
      using (TimeWarning.New("IsSpeeding", 0.1f))
      {
        if (AntiHack.IsSpeeding(ply, ticks, deltaTime))
        {
          if (flag)
            return false;
          AntiHack.AddViolation(ply, AntiHackType.SpeedHack, ConVar.AntiHack.speedhack_penalty * ticks.Length);
          if (ConVar.AntiHack.speedhack_reject)
            return false;
        }
      }
      using (TimeWarning.New("IsFlying", 0.1f))
      {
        if (AntiHack.IsFlying(ply, ticks, deltaTime))
        {
          if (flag)
            return false;
          AntiHack.AddViolation(ply, AntiHackType.FlyHack, ConVar.AntiHack.flyhack_penalty * ticks.Length);
          if (ConVar.AntiHack.flyhack_reject)
            return false;
        }
      }
      return true;
    }
  }

  public static bool IsNoClipping(BasePlayer ply, TickInterpolator ticks, float deltaTime)
  {
    if (ConVar.AntiHack.noclip_protection <= 0)
      return false;
    ticks.Reset();
    if (!ticks.HasNext())
      return false;
    bool flag = Object.op_Equality((Object) ((Component) ply).get_transform().get_parent(), (Object) null);
    Matrix4x4 matrix4x4 = flag ? Matrix4x4.get_identity() : ((Component) ply).get_transform().get_parent().get_localToWorldMatrix();
    Vector3 oldPos = flag ? ticks.StartPoint : ((Matrix4x4) ref matrix4x4).MultiplyPoint3x4(ticks.StartPoint);
    Vector3 newPos1 = flag ? ticks.EndPoint : ((Matrix4x4) ref matrix4x4).MultiplyPoint3x4(ticks.EndPoint);
    if (ConVar.AntiHack.noclip_protection >= 3)
    {
      float num1 = Mathf.Max(ConVar.AntiHack.noclip_stepsize, 0.1f);
      int num2 = Mathf.Max(ConVar.AntiHack.noclip_maxsteps, 1);
      float distance = Mathf.Max(ticks.Length / (float) num2, num1);
      while (ticks.MoveNext(distance))
      {
        Vector3 newPos2 = flag ? ticks.CurrentPoint : ((Matrix4x4) ref matrix4x4).MultiplyPoint3x4(ticks.CurrentPoint);
        if (AntiHack.TestNoClipping(ply, oldPos, newPos2, true, deltaTime))
          return true;
        oldPos = newPos2;
      }
    }
    else if (ConVar.AntiHack.noclip_protection >= 2)
    {
      if (AntiHack.TestNoClipping(ply, oldPos, newPos1, true, deltaTime))
        return true;
    }
    else if (AntiHack.TestNoClipping(ply, oldPos, newPos1, false, deltaTime))
      return true;
    return false;
  }

  public static bool TestNoClipping(
    BasePlayer ply,
    Vector3 oldPos,
    Vector3 newPos,
    bool sphereCast,
    float deltaTime = 0.0f)
  {
    ply.vehiclePauseTime = Mathf.Max(0.0f, ply.vehiclePauseTime - deltaTime);
    int num1 = 429990145;
    if ((double) ply.vehiclePauseTime > 0.0)
      num1 &= -8193;
    float noclipBacktracking = ConVar.AntiHack.noclip_backtracking;
    float noclipMargin = ConVar.AntiHack.noclip_margin;
    float radius = ply.GetRadius();
    float height = ply.GetHeight(true);
    Vector3 vector3_1 = Vector3.op_Subtraction(newPos, oldPos);
    Vector3 normalized = ((Vector3) ref vector3_1).get_normalized();
    float num2 = radius - noclipMargin;
    Vector3 vector3_2 = Vector3.op_Subtraction(Vector3.op_Addition(oldPos, new Vector3(0.0f, height - radius, 0.0f)), Vector3.op_Multiply(normalized, noclipBacktracking));
    Vector3 vector3_3 = Vector3.op_Subtraction(Vector3.op_Addition(newPos, new Vector3(0.0f, height - radius, 0.0f)), vector3_2);
    float magnitude = ((Vector3) ref vector3_3).get_magnitude();
    RaycastHit hitInfo = (RaycastHit) null;
    bool flag = Physics.Raycast(new Ray(vector3_2, normalized), ref hitInfo, magnitude + num2, num1, (QueryTriggerInteraction) 1);
    if (!flag & sphereCast)
      flag = Physics.SphereCast(new Ray(vector3_2, normalized), num2, ref hitInfo, magnitude, num1, (QueryTriggerInteraction) 1);
    if (flag)
      return GamePhysics.Verify(hitInfo);
    return false;
  }

  public static bool IsSpeeding(BasePlayer ply, TickInterpolator ticks, float deltaTime)
  {
    ply.speedhackPauseTime = Mathf.Max(0.0f, ply.speedhackPauseTime - deltaTime);
    if ((double) ply.speedhackPauseTime > 0.0 || ConVar.AntiHack.speedhack_protection <= 0)
      return false;
    int num1 = Object.op_Equality((Object) ((Component) ply).get_transform().get_parent(), (Object) null) ? 1 : 0;
    Matrix4x4 matrix4x4 = num1 != 0 ? Matrix4x4.get_identity() : ((Component) ply).get_transform().get_parent().get_localToWorldMatrix();
    Vector3 worldPos = num1 != 0 ? ticks.StartPoint : ((Matrix4x4) ref matrix4x4).MultiplyPoint3x4(ticks.StartPoint);
    Vector3 vector3 = Vector3.op_Subtraction(num1 != 0 ? ticks.EndPoint : ((Matrix4x4) ref matrix4x4).MultiplyPoint3x4(ticks.EndPoint), worldPos);
    float num2 = Vector3Ex.Magnitude2D(vector3);
    if ((double) num2 > 0.0)
    {
      float num3 = Mathf.Max(0.0f, Vector3.Dot(Vector3Ex.XZ3D(Object.op_Implicit((Object) TerrainMeta.HeightMap) ? TerrainMeta.HeightMap.GetNormal(worldPos) : Vector3.get_up()), Vector3Ex.XZ3D(vector3))) * ConVar.AntiHack.speedhack_slopespeed * deltaTime;
      num2 = Mathf.Max(0.0f, num2 - num3);
    }
    float running = 1f;
    float ducking = 0.0f;
    if (ConVar.AntiHack.speedhack_protection >= 2)
    {
      running = ply.IsRunning() ? 1f : 0.0f;
      ducking = ply.IsDucked() || ply.IsSwimming() ? 1f : 0.0f;
    }
    float num4 = Mathf.Max(ConVar.AntiHack.speedhack_forgiveness, 0.1f);
    float num5 = 2f * num4;
    ply.speedhackDistance = Mathf.Clamp((float) ((double) ply.speedhackDistance + (double) num2 - (double) deltaTime * (double) ply.GetSpeed(running, ducking)), -num5, num5);
    return (double) ply.speedhackDistance > (double) num4;
  }

  public static bool IsFlying(BasePlayer ply, TickInterpolator ticks, float deltaTime)
  {
    ply.flyhackPauseTime = Mathf.Max(0.0f, ply.flyhackPauseTime - deltaTime);
    if ((double) ply.flyhackPauseTime > 0.0 || ConVar.AntiHack.flyhack_protection <= 0)
      return false;
    ticks.Reset();
    if (!ticks.HasNext())
      return false;
    bool flag = Object.op_Equality((Object) ((Component) ply).get_transform().get_parent(), (Object) null);
    Matrix4x4 matrix4x4 = flag ? Matrix4x4.get_identity() : ((Component) ply).get_transform().get_parent().get_localToWorldMatrix();
    Vector3 oldPos = flag ? ticks.StartPoint : ((Matrix4x4) ref matrix4x4).MultiplyPoint3x4(ticks.StartPoint);
    Vector3 newPos1 = flag ? ticks.EndPoint : ((Matrix4x4) ref matrix4x4).MultiplyPoint3x4(ticks.EndPoint);
    if (ConVar.AntiHack.flyhack_protection >= 3)
    {
      float num1 = Mathf.Max(ConVar.AntiHack.flyhack_stepsize, 0.1f);
      int num2 = Mathf.Max(ConVar.AntiHack.flyhack_maxsteps, 1);
      float distance = Mathf.Max(ticks.Length / (float) num2, num1);
      while (ticks.MoveNext(distance))
      {
        Vector3 newPos2 = flag ? ticks.CurrentPoint : ((Matrix4x4) ref matrix4x4).MultiplyPoint3x4(ticks.CurrentPoint);
        if (AntiHack.TestFlying(ply, oldPos, newPos2, true))
          return true;
        oldPos = newPos2;
      }
    }
    else if (ConVar.AntiHack.flyhack_protection >= 2)
    {
      if (AntiHack.TestFlying(ply, oldPos, newPos1, true))
        return true;
    }
    else if (AntiHack.TestFlying(ply, oldPos, newPos1, false))
      return true;
    return false;
  }

  public static bool TestFlying(
    BasePlayer ply,
    Vector3 oldPos,
    Vector3 newPos,
    bool verifyGrounded)
  {
    ply.isInAir = false;
    ply.isOnPlayer = false;
    if (verifyGrounded)
    {
      float flyhackExtrusion = ConVar.AntiHack.flyhack_extrusion;
      Vector3 pos = Vector3.op_Multiply(Vector3.op_Addition(oldPos, newPos), 0.5f);
      if (!ply.OnLadder() && !WaterLevel.Test(Vector3.op_Subtraction(pos, new Vector3(0.0f, flyhackExtrusion, 0.0f))) && (EnvironmentManager.Get(pos) & EnvironmentType.Elevator) == (EnvironmentType) 0)
      {
        float flyhackMargin = ConVar.AntiHack.flyhack_margin;
        float radius = ply.GetRadius();
        float height = ply.GetHeight(false);
        Vector3 vector3_1 = Vector3.op_Addition(pos, new Vector3(0.0f, radius - flyhackExtrusion, 0.0f));
        Vector3 vector3_2 = Vector3.op_Addition(pos, new Vector3(0.0f, height - radius, 0.0f));
        float num1 = radius - flyhackMargin;
        ply.isInAir = !Physics.CheckCapsule(vector3_1, vector3_2, num1, 1503731969, (QueryTriggerInteraction) 1);
        if (ply.isInAir)
        {
          int num2 = Physics.OverlapCapsuleNonAlloc(vector3_1, vector3_2, num1, AntiHack.buffer, 131072, (QueryTriggerInteraction) 1);
          for (int index = 0; index < num2; ++index)
          {
            BasePlayer baseEntity = ((Component) AntiHack.buffer[index]).get_gameObject().ToBaseEntity() as BasePlayer;
            if (!Object.op_Equality((Object) baseEntity, (Object) null) && !Object.op_Equality((Object) baseEntity, (Object) ply) && (!baseEntity.isInAir && !baseEntity.isOnPlayer) && (!baseEntity.TriggeredAntiHack(1f, float.PositiveInfinity) && !baseEntity.IsSleeping()))
            {
              ply.isOnPlayer = true;
              ply.isInAir = false;
              break;
            }
          }
          for (int index = 0; index < AntiHack.buffer.Length; ++index)
            AntiHack.buffer[index] = (Collider) null;
        }
      }
    }
    else
      ply.isInAir = !ply.OnLadder() && !ply.IsSwimming() && !ply.IsOnGround();
    if (ply.isInAir)
    {
      bool flag = false;
      Vector3 vector3 = Vector3.op_Subtraction(newPos, oldPos);
      double num1 = (double) Mathf.Abs((float) vector3.y);
      float num2 = Vector3Ex.Magnitude2D(vector3);
      if (vector3.y >= 0.0)
      {
        ply.flyhackDistanceVertical += (float) vector3.y;
        flag = true;
      }
      double num3 = (double) num2;
      if (num1 < num3)
      {
        ply.flyhackDistanceHorizontal += num2;
        flag = true;
      }
      if (flag)
      {
        float num4 = ply.GetJumpHeight() + ConVar.AntiHack.flyhack_forgiveness_vertical;
        if ((double) ply.flyhackDistanceVertical > (double) num4)
          return true;
        float num5 = 5f + ConVar.AntiHack.flyhack_forgiveness_horizontal;
        if ((double) ply.flyhackDistanceHorizontal > (double) num5)
          return true;
      }
    }
    else
    {
      ply.flyhackDistanceVertical = 0.0f;
      ply.flyhackDistanceHorizontal = 0.0f;
    }
    return false;
  }

  public static void NoteAdminHack(BasePlayer ply)
  {
    AntiHack.Ban(ply, "Cheat Detected!");
  }

  public static void FadeViolations(BasePlayer ply, float deltaTime)
  {
    if ((double) Time.get_realtimeSinceStartup() - (double) ply.lastViolationTime <= (double) ConVar.AntiHack.relaxationpause)
      return;
    ply.violationLevel = Mathf.Max(0.0f, ply.violationLevel - ConVar.AntiHack.relaxationrate * deltaTime);
  }

  public static void EnforceViolations(BasePlayer ply)
  {
    if (ConVar.AntiHack.enforcementlevel <= 0 || (double) ply.violationLevel <= (double) ConVar.AntiHack.maxviolation)
      return;
    if (ConVar.AntiHack.debuglevel >= 1)
      AntiHack.LogToConsole(ply, ply.lastViolationType, "Enforcing (violation of " + (object) ply.violationLevel + ")");
    string reason = ((int) ply.lastViolationType).ToString() + " Violation Level " + (object) ply.violationLevel;
    if (ConVar.AntiHack.enforcementlevel > 1)
      AntiHack.Kick(ply, reason);
    else
      AntiHack.Kick(ply, reason);
  }

  public static void Log(BasePlayer ply, AntiHackType type, string message)
  {
    if (ConVar.AntiHack.debuglevel > 1)
      AntiHack.LogToConsole(ply, type, message);
    AntiHack.LogToEAC(ply, type, message);
  }

  private static void LogToConsole(BasePlayer ply, AntiHackType type, string message)
  {
    Debug.LogWarning((object) (ply.ToString() + " " + (object) type + ": " + message));
  }

  private static void LogToEAC(BasePlayer ply, AntiHackType type, string message)
  {
    if (!ConVar.AntiHack.reporting || EACServer.eacScout == null)
      return;
    EACServer.eacScout.SendInvalidPlayerStateReport(ply.UserIDString, (InvalidPlayerStateReportCategory) 2, ((int) type).ToString() + ": " + message);
  }

  public static void AddViolation(BasePlayer ply, AntiHackType type, float amount)
  {
    if (Interface.CallHook("OnPlayerViolation", (object) ply, (object) type, (object) amount) != null)
      return;
    ply.lastViolationType = type;
    ply.lastViolationTime = Time.get_realtimeSinceStartup();
    ply.violationLevel += amount;
    if (ConVar.AntiHack.debuglevel >= 2 && (double) amount > 0.0 || ConVar.AntiHack.debuglevel >= 3)
      AntiHack.LogToConsole(ply, type, "Added violation of " + (object) amount + " in frame " + (object) Time.get_frameCount() + " (now has " + (object) ply.violationLevel + ")");
    AntiHack.EnforceViolations(ply);
  }

  public static void Kick(BasePlayer ply, string reason)
  {
    if (EACServer.eacScout != null)
      EACServer.eacScout.SendKickReport(ply.userID.ToString(), reason, (KickReasonCategory) 4);
    AntiHack.AddRecord(ply, AntiHack.kicks);
    ConsoleSystem.Run(ConsoleSystem.Option.get_Server(), "kick", new object[2]
    {
      (object) ply.userID,
      (object) reason
    });
  }

  public static void Ban(BasePlayer ply, string reason)
  {
    if (EACServer.eacScout != null)
      EACServer.eacScout.SendKickReport(ply.userID.ToString(), reason, (KickReasonCategory) 1);
    AntiHack.AddRecord(ply, AntiHack.bans);
    ConsoleSystem.Run(ConsoleSystem.Option.get_Server(), "ban", new object[2]
    {
      (object) ply.userID,
      (object) reason
    });
  }

  private static void AddRecord(BasePlayer ply, Dictionary<ulong, int> records)
  {
    if (records.ContainsKey(ply.userID))
      ++records[ply.userID];
    else
      records.Add(ply.userID, 1);
  }

  public static int GetKickRecord(BasePlayer ply)
  {
    return AntiHack.GetRecord(ply, AntiHack.kicks);
  }

  public static int GetBanRecord(BasePlayer ply)
  {
    return AntiHack.GetRecord(ply, AntiHack.bans);
  }

  private static int GetRecord(BasePlayer ply, Dictionary<ulong, int> records)
  {
    if (!records.ContainsKey(ply.userID))
      return 0;
    return records[ply.userID];
  }
}
