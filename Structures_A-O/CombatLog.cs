// Decompiled with JetBrains decompiler
// Type: CombatLog
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using System.Collections.Generic;
using UnityEngine;

public class CombatLog
{
  private static Dictionary<ulong, Queue<CombatLog.Event>> players = new Dictionary<ulong, Queue<CombatLog.Event>>();
  private const string selfname = "you";
  private const string noname = "N/A";
  private BasePlayer player;
  private Queue<CombatLog.Event> storage;

  public CombatLog(BasePlayer player)
  {
    this.player = player;
  }

  public void Init()
  {
    this.storage = CombatLog.Get(this.player.userID);
  }

  public void Save()
  {
  }

  public void Log(AttackEntity weapon, string description = null)
  {
    this.Log(weapon, (Projectile) null, description);
  }

  public void Log(AttackEntity weapon, Projectile projectile, string description = null)
  {
    this.Log(new CombatLog.Event()
    {
      time = Time.get_realtimeSinceStartup(),
      attacker_id = !Object.op_Implicit((Object) this.player) || this.player.net == null ? 0U : (uint) (int) this.player.net.ID,
      target_id = 0U,
      attacker = "you",
      target = "N/A",
      weapon = Object.op_Implicit((Object) weapon) ? ((Object) weapon).get_name() : "N/A",
      ammo = Object.op_Implicit((Object) projectile) ? ((Object) projectile).get_name() : "N/A",
      bone = "N/A",
      area = (HitArea) 0,
      distance = 0.0f,
      health_old = 0.0f,
      health_new = 0.0f,
      info = description != null ? description : string.Empty
    });
  }

  public void Log(HitInfo info, string description = null)
  {
    float num = Object.op_Implicit((Object) info.HitEntity) ? info.HitEntity.Health() : 0.0f;
    this.Log(info, num, num, description);
  }

  public void Log(HitInfo info, float health_old, float health_new, string description = null)
  {
    this.Log(new CombatLog.Event()
    {
      time = Time.get_realtimeSinceStartup(),
      attacker_id = !Object.op_Implicit((Object) info.Initiator) || info.Initiator.net == null ? 0U : (uint) (int) info.Initiator.net.ID,
      target_id = !Object.op_Implicit((Object) info.HitEntity) || info.HitEntity.net == null ? 0U : (uint) (int) info.HitEntity.net.ID,
      attacker = Object.op_Equality((Object) this.player, (Object) info.Initiator) ? "you" : (Object.op_Implicit((Object) info.Initiator) ? info.Initiator.ShortPrefabName : "N/A"),
      target = Object.op_Equality((Object) this.player, (Object) info.HitEntity) ? "you" : (Object.op_Implicit((Object) info.HitEntity) ? info.HitEntity.ShortPrefabName : "N/A"),
      weapon = Object.op_Implicit((Object) info.WeaponPrefab) ? ((Object) info.WeaponPrefab).get_name() : "N/A",
      ammo = Object.op_Implicit((Object) info.ProjectilePrefab) ? ((Object) info.ProjectilePrefab).get_name() : "N/A",
      bone = info.boneName,
      area = info.boneArea,
      distance = info.IsProjectile() ? info.ProjectileDistance : Vector3.Distance(info.PointStart, info.HitPositionWorld),
      health_old = health_old,
      health_new = health_new,
      info = description != null ? description : string.Empty
    });
  }

  public void Log(CombatLog.Event val)
  {
    if (this.storage == null)
      return;
    this.storage.Enqueue(val);
    int num = Mathf.Max(0, Server.combatlogsize);
    while (this.storage.Count > num)
      this.storage.Dequeue();
  }

  public string Get(int count)
  {
    if (this.storage == null)
      return string.Empty;
    if (this.storage.Count == 0)
      return "Combat log empty.";
    TextTable textTable = new TextTable();
    textTable.AddColumn("time");
    textTable.AddColumn("attacker");
    textTable.AddColumn("id");
    textTable.AddColumn("target");
    textTable.AddColumn("id");
    textTable.AddColumn("weapon");
    textTable.AddColumn("ammo");
    textTable.AddColumn("area");
    textTable.AddColumn("distance");
    textTable.AddColumn("old_hp");
    textTable.AddColumn("new_hp");
    textTable.AddColumn("info");
    int num1 = this.storage.Count - count;
    int combatlogdelay = Server.combatlogdelay;
    int num2 = 0;
    foreach (CombatLog.Event @event in this.storage)
    {
      if (num1 > 0)
      {
        --num1;
      }
      else
      {
        float num3 = Time.get_realtimeSinceStartup() - @event.time;
        if ((double) num3 >= (double) combatlogdelay)
        {
          string str1 = num3.ToString("0.0s");
          string attacker = @event.attacker;
          uint num4 = @event.attacker_id;
          string str2 = num4.ToString();
          string target = @event.target;
          num4 = @event.target_id;
          string str3 = num4.ToString();
          string weapon = @event.weapon;
          string ammo = @event.ammo;
          string lower = HitAreaUtil.Format(@event.area).ToLower();
          float num5 = @event.distance;
          string str4 = num5.ToString("0.0m");
          num5 = @event.health_old;
          string str5 = num5.ToString("0.0");
          num5 = @event.health_new;
          string str6 = num5.ToString("0.0");
          string info = @event.info;
          textTable.AddRow(new string[12]
          {
            str1,
            attacker,
            str2,
            target,
            str3,
            weapon,
            ammo,
            lower,
            str4,
            str5,
            str6,
            info
          });
        }
        else
          ++num2;
      }
    }
    string str = ((object) textTable).ToString();
    if (num2 > 0)
      str = str + "+ " + (object) num2 + " " + (num2 > 1 ? (object) "events" : (object) "event") + " in the last " + (object) combatlogdelay + " " + (combatlogdelay > 1 ? (object) "seconds" : (object) "second");
    return str;
  }

  public static Queue<CombatLog.Event> Get(ulong id)
  {
    Queue<CombatLog.Event> eventQueue1;
    if (CombatLog.players.TryGetValue(id, out eventQueue1))
      return eventQueue1;
    Queue<CombatLog.Event> eventQueue2 = new Queue<CombatLog.Event>();
    CombatLog.players.Add(id, eventQueue2);
    return eventQueue2;
  }

  public struct Event
  {
    public float time;
    public uint attacker_id;
    public uint target_id;
    public string attacker;
    public string target;
    public string weapon;
    public string ammo;
    public string bone;
    public HitArea area;
    public float distance;
    public float health_old;
    public float health_new;
    public string info;
  }
}
