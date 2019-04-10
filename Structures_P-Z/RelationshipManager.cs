// Decompiled with JetBrains decompiler
// Type: RelationshipManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RelationshipManager : BaseEntity
{
  [ServerVar]
  public static int maxTeamSize = 8;
  public Dictionary<ulong, BasePlayer> cachedPlayers = new Dictionary<ulong, BasePlayer>();
  public Dictionary<ulong, RelationshipManager.PlayerTeam> playerTeams = new Dictionary<ulong, RelationshipManager.PlayerTeam>();
  private ulong lastTeamIndex = 1;
  public static RelationshipManager _instance;

  public static RelationshipManager Instance
  {
    get
    {
      return RelationshipManager._instance;
    }
  }

  public void OnEnable()
  {
    if (this.isClient)
      return;
    if (Object.op_Inequality((Object) RelationshipManager._instance, (Object) null))
    {
      Debug.LogError((object) "Major fuckup! RelationshipManager spawned twice, Contact Developers!");
      Object.Destroy((Object) ((Component) this).get_gameObject());
    }
    else
      RelationshipManager._instance = this;
  }

  public void OnDestroy()
  {
    RelationshipManager._instance = (RelationshipManager) null;
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.relationshipManager = (__Null) Pool.Get<RelationshipManager>();
    ((RelationshipManager) info.msg.relationshipManager).maxTeamSize = (__Null) RelationshipManager.maxTeamSize;
    if (!info.forDisk)
      return;
    ((RelationshipManager) info.msg.relationshipManager).lastTeamIndex = (__Null) (long) this.lastTeamIndex;
    ((RelationshipManager) info.msg.relationshipManager).teamList = (__Null) Pool.GetList<ProtoBuf.PlayerTeam>();
    foreach (KeyValuePair<ulong, RelationshipManager.PlayerTeam> playerTeam1 in this.playerTeams)
    {
      RelationshipManager.PlayerTeam playerTeam2 = playerTeam1.Value;
      if (playerTeam2 != null)
      {
        ProtoBuf.PlayerTeam playerTeam3 = (ProtoBuf.PlayerTeam) Pool.Get<ProtoBuf.PlayerTeam>();
        playerTeam3.teamLeader = (__Null) (long) playerTeam2.teamLeader;
        playerTeam3.teamID = (__Null) (long) playerTeam2.teamID;
        playerTeam3.teamName = (__Null) playerTeam2.teamName;
        playerTeam3.members = (__Null) Pool.GetList<ProtoBuf.PlayerTeam.TeamMember>();
        foreach (ulong member in playerTeam2.members)
        {
          ProtoBuf.PlayerTeam.TeamMember teamMember = (ProtoBuf.PlayerTeam.TeamMember) Pool.Get<ProtoBuf.PlayerTeam.TeamMember>();
          BasePlayer byId = RelationshipManager.FindByID(member);
          teamMember.displayName = Object.op_Inequality((Object) byId, (Object) null) ? (__Null) byId.displayName : (__Null) "DEAD";
          teamMember.userID = (__Null) (long) member;
          ((List<ProtoBuf.PlayerTeam.TeamMember>) playerTeam3.members).Add(teamMember);
        }
        ((List<ProtoBuf.PlayerTeam>) ((RelationshipManager) info.msg.relationshipManager).teamList).Add(playerTeam3);
      }
    }
  }

  public void DisbandTeam(RelationshipManager.PlayerTeam teamToDisband)
  {
    if (Interface.CallHook("OnTeamDisband", (object) teamToDisband) != null)
      return;
    this.playerTeams.Remove(teamToDisband.teamID);
    Interface.CallHook("OnTeamDisbanded", (object) teamToDisband);
    // ISSUE: cast to a reference type
    Pool.Free<RelationshipManager.PlayerTeam>((M0&) ref teamToDisband);
  }

  public static BasePlayer FindByID(ulong userID)
  {
    BasePlayer basePlayer1 = (BasePlayer) null;
    if (RelationshipManager.Instance.cachedPlayers.TryGetValue(userID, out basePlayer1))
    {
      if (Object.op_Inequality((Object) basePlayer1, (Object) null))
        return basePlayer1;
      RelationshipManager.Instance.cachedPlayers.Remove(userID);
    }
    BasePlayer basePlayer2 = BasePlayer.activePlayerList.Find((Predicate<BasePlayer>) (x => (long) x.userID == (long) userID));
    if (!Object.op_Implicit((Object) basePlayer2))
      basePlayer2 = BasePlayer.sleepingPlayerList.Find((Predicate<BasePlayer>) (x => (long) x.userID == (long) userID));
    if (Object.op_Inequality((Object) basePlayer2, (Object) null))
      RelationshipManager.Instance.cachedPlayers.Add(userID, basePlayer2);
    return basePlayer2;
  }

  public RelationshipManager.PlayerTeam FindTeam(ulong TeamID)
  {
    if (this.playerTeams.ContainsKey(TeamID))
      return this.playerTeams[TeamID];
    return (RelationshipManager.PlayerTeam) null;
  }

  public RelationshipManager.PlayerTeam CreateTeam()
  {
    RelationshipManager.PlayerTeam playerTeam = (RelationshipManager.PlayerTeam) Pool.Get<RelationshipManager.PlayerTeam>();
    playerTeam.teamID = this.lastTeamIndex;
    this.playerTeams.Add(this.lastTeamIndex, playerTeam);
    ++this.lastTeamIndex;
    return playerTeam;
  }

  [ServerUserVar]
  public static void trycreateteam(ConsoleSystem.Arg arg)
  {
    if (RelationshipManager.maxTeamSize == 0)
    {
      arg.ReplyWith("Teams are disabled on this server");
    }
    else
    {
      BasePlayer player = arg.Player();
      if (player.currentTeam != 0UL || Interface.CallHook("OnTeamCreate", (object) player) != null)
        return;
      RelationshipManager.PlayerTeam team = RelationshipManager.Instance.CreateTeam();
      team.teamLeader = player.userID;
      team.AddPlayer(player);
    }
  }

  [ServerUserVar]
  public static void promote(ConsoleSystem.Arg arg)
  {
    BasePlayer source = arg.Player();
    if (source.currentTeam == 0UL)
      return;
    BasePlayer lookingAtPlayer = RelationshipManager.GetLookingAtPlayer(source);
    if (Object.op_Equality((Object) lookingAtPlayer, (Object) null) || lookingAtPlayer.IsDead() || (Object.op_Equality((Object) lookingAtPlayer, (Object) source) || (long) lookingAtPlayer.currentTeam != (long) source.currentTeam))
      return;
    RelationshipManager.PlayerTeam playerTeam = RelationshipManager.Instance.playerTeams[source.currentTeam];
    if (playerTeam == null || (long) playerTeam.teamLeader != (long) source.userID || Interface.CallHook("OnTeamPromote", (object) playerTeam, (object) lookingAtPlayer) != null)
      return;
    playerTeam.SetTeamLeader(lookingAtPlayer.userID);
  }

  [ServerUserVar]
  public static void leaveteam(ConsoleSystem.Arg arg)
  {
    BasePlayer basePlayer = arg.Player();
    if (Object.op_Equality((Object) basePlayer, (Object) null) || basePlayer.currentTeam == 0UL)
      return;
    RelationshipManager.PlayerTeam team = RelationshipManager.Instance.FindTeam(basePlayer.currentTeam);
    if (team == null || Interface.CallHook("OnTeamLeave", (object) team, (object) basePlayer) != null)
      return;
    team.RemovePlayer(basePlayer.userID);
    basePlayer.ClearTeam();
  }

  [ServerUserVar]
  public static void acceptinvite(ConsoleSystem.Arg arg)
  {
    BasePlayer player = arg.Player();
    if (Object.op_Equality((Object) player, (Object) null) || player.currentTeam != 0UL)
      return;
    RelationshipManager.PlayerTeam team = RelationshipManager.Instance.FindTeam(arg.GetULong(0, 0UL));
    if (team == null)
    {
      player.ClearPendingInvite();
    }
    else
    {
      if (Interface.CallHook("OnTeamAcceptInvite", (object) team, (object) player) != null)
        return;
      team.AcceptInvite(player);
    }
  }

  [ServerUserVar]
  public static void rejectinvite(ConsoleSystem.Arg arg)
  {
    BasePlayer player = arg.Player();
    if (Object.op_Equality((Object) player, (Object) null) || player.currentTeam != 0UL)
      return;
    RelationshipManager.PlayerTeam team = RelationshipManager.Instance.FindTeam(arg.GetULong(0, 0UL));
    if (team == null)
    {
      player.ClearPendingInvite();
    }
    else
    {
      if (Interface.CallHook("OnTeamRejectInvite", (object) player, (object) team) != null)
        return;
      team.RejectInvite(player);
    }
  }

  public static BasePlayer GetLookingAtPlayer(BasePlayer source)
  {
    RaycastHit hit;
    if (Physics.Raycast(source.eyes.position, source.eyes.HeadForward(), ref hit, 5f, 1218652417, (QueryTriggerInteraction) 1))
    {
      BaseEntity entity = hit.GetEntity();
      if (Object.op_Implicit((Object) entity))
        return (BasePlayer) ((Component) entity).GetComponent<BasePlayer>();
    }
    return (BasePlayer) null;
  }

  [ServerVar]
  public static void sleeptoggle(ConsoleSystem.Arg arg)
  {
    BasePlayer basePlayer = arg.Player();
    RaycastHit hit;
    if (Object.op_Equality((Object) basePlayer, (Object) null) || !Physics.Raycast(basePlayer.eyes.position, basePlayer.eyes.HeadForward(), ref hit, 5f, 1218652417, (QueryTriggerInteraction) 1))
      return;
    BaseEntity entity = hit.GetEntity();
    if (!Object.op_Implicit((Object) entity))
      return;
    BasePlayer component = (BasePlayer) ((Component) entity).GetComponent<BasePlayer>();
    if (!Object.op_Implicit((Object) component) || !Object.op_Inequality((Object) component, (Object) basePlayer) || component.IsNpc)
      return;
    if (component.IsSleeping())
      component.EndSleeping();
    else
      component.StartSleeping();
  }

  [ServerUserVar]
  public static void kickmember(ConsoleSystem.Arg arg)
  {
    BasePlayer basePlayer = arg.Player();
    if (Object.op_Equality((Object) basePlayer, (Object) null))
      return;
    RelationshipManager.PlayerTeam team = RelationshipManager.Instance.FindTeam(basePlayer.currentTeam);
    if (team == null || Object.op_Inequality((Object) team.GetLeader(), (Object) basePlayer))
      return;
    ulong playerID = arg.GetULong(0, 0UL);
    if ((long) basePlayer.userID == (long) playerID || Interface.CallHook("OnTeamKick", (object) team, (object) basePlayer) != null)
      return;
    team.RemovePlayer(playerID);
  }

  [ServerUserVar]
  public static void sendinvite(ConsoleSystem.Arg arg)
  {
    BasePlayer basePlayer = arg.Player();
    RelationshipManager.PlayerTeam team = RelationshipManager.Instance.FindTeam(basePlayer.currentTeam);
    RaycastHit hit;
    if (team == null || Object.op_Equality((Object) team.GetLeader(), (Object) null) || (Object.op_Inequality((Object) team.GetLeader(), (Object) basePlayer) || !Physics.Raycast(basePlayer.eyes.position, basePlayer.eyes.HeadForward(), ref hit, 5f, 1218652417, (QueryTriggerInteraction) 1)))
      return;
    BaseEntity entity = hit.GetEntity();
    if (!Object.op_Implicit((Object) entity))
      return;
    BasePlayer component = (BasePlayer) ((Component) entity).GetComponent<BasePlayer>();
    if (!Object.op_Implicit((Object) component) || !Object.op_Inequality((Object) component, (Object) basePlayer) || (component.IsNpc || component.currentTeam != 0UL) || Interface.CallHook("OnTeamInvite", (object) basePlayer, (object) component) != null)
      return;
    team.SendInvite(component);
  }

  [ServerVar]
  public static void fakeinvite(ConsoleSystem.Arg arg)
  {
    BasePlayer player = arg.Player();
    RelationshipManager.PlayerTeam team = RelationshipManager.Instance.FindTeam(arg.GetULong(0, 0UL));
    if (team == null)
      return;
    if (player.currentTeam != 0UL)
      Debug.Log((object) "already in team");
    team.SendInvite(player);
    Debug.Log((object) "sent bot invite");
  }

  [ServerVar]
  public static void addtoteam(ConsoleSystem.Arg arg)
  {
    BasePlayer basePlayer = arg.Player();
    RelationshipManager.PlayerTeam team = RelationshipManager.Instance.FindTeam(basePlayer.currentTeam);
    RaycastHit hit;
    if (team == null || Object.op_Equality((Object) team.GetLeader(), (Object) null) || (Object.op_Inequality((Object) team.GetLeader(), (Object) basePlayer) || !Physics.Raycast(basePlayer.eyes.position, basePlayer.eyes.HeadForward(), ref hit, 5f, 1218652417, (QueryTriggerInteraction) 1)))
      return;
    BaseEntity entity = hit.GetEntity();
    if (!Object.op_Implicit((Object) entity))
      return;
    BasePlayer component = (BasePlayer) ((Component) entity).GetComponent<BasePlayer>();
    if (!Object.op_Implicit((Object) component) || !Object.op_Inequality((Object) component, (Object) basePlayer) || component.IsNpc)
      return;
    team.AddPlayer(component);
  }

  public static bool TeamsEnabled()
  {
    return RelationshipManager.maxTeamSize > 0;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (!info.fromDisk || info.msg.relationshipManager == null)
      return;
    this.lastTeamIndex = (ulong) ((RelationshipManager) info.msg.relationshipManager).lastTeamIndex;
    using (List<ProtoBuf.PlayerTeam>.Enumerator enumerator1 = ((List<ProtoBuf.PlayerTeam>) ((RelationshipManager) info.msg.relationshipManager).teamList).GetEnumerator())
    {
      while (enumerator1.MoveNext())
      {
        ProtoBuf.PlayerTeam current1 = enumerator1.Current;
        RelationshipManager.PlayerTeam playerTeam = (RelationshipManager.PlayerTeam) Pool.Get<RelationshipManager.PlayerTeam>();
        playerTeam.teamLeader = (ulong) current1.teamLeader;
        playerTeam.teamID = (ulong) current1.teamID;
        playerTeam.teamName = (string) current1.teamName;
        playerTeam.members = new List<ulong>();
        using (List<ProtoBuf.PlayerTeam.TeamMember>.Enumerator enumerator2 = ((List<ProtoBuf.PlayerTeam.TeamMember>) current1.members).GetEnumerator())
        {
          while (enumerator2.MoveNext())
          {
            ProtoBuf.PlayerTeam.TeamMember current2 = enumerator2.Current;
            playerTeam.members.Add((ulong) current2.userID);
          }
        }
        this.playerTeams[playerTeam.teamID] = playerTeam;
      }
    }
  }

  public class PlayerTeam
  {
    public List<ulong> members = new List<ulong>();
    public List<ulong> invites = new List<ulong>();
    public ulong teamID;
    public string teamName;
    public ulong teamLeader;

    public BasePlayer GetLeader()
    {
      return RelationshipManager.FindByID(this.teamLeader);
    }

    public void SendInvite(BasePlayer player)
    {
      if (this.invites.Count > 8)
        this.invites.RemoveRange(0, 1);
      BasePlayer byId = RelationshipManager.FindByID(this.teamLeader);
      if (Object.op_Equality((Object) byId, (Object) null))
        return;
      this.invites.Add(player.userID);
      player.ClientRPCPlayer<string, ulong>((Connection) null, player, "CLIENT_PendingInvite", byId.displayName, this.teamID);
    }

    public void AcceptInvite(BasePlayer player)
    {
      if (!this.invites.Contains(player.userID))
        return;
      this.invites.Remove(player.userID);
      this.AddPlayer(player);
      player.ClearPendingInvite();
    }

    public void RejectInvite(BasePlayer player)
    {
      player.ClearPendingInvite();
      this.invites.Remove(player.userID);
    }

    public BasePlayer GetMember(ulong userID)
    {
      return RelationshipManager.FindByID(userID);
    }

    public BasePlayer GetMember(int index)
    {
      if (index >= this.members.Count)
        return (BasePlayer) null;
      return this.GetMember(this.members[index]);
    }

    public void Cleanup()
    {
      for (int index = this.members.Count - 1; index >= 0; --index)
      {
        ulong member = this.members[index];
        if (Object.op_Equality((Object) RelationshipManager.FindByID(member), (Object) null))
          this.members.Remove(member);
      }
    }

    public bool AddPlayer(BasePlayer player)
    {
      ulong userId = player.userID;
      if (this.members.Contains(userId) || player.currentTeam != 0UL || this.members.Count >= RelationshipManager.maxTeamSize)
        return false;
      player.currentTeam = this.teamID;
      this.members.Add(userId);
      this.MarkDirty();
      player.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
      return true;
    }

    public bool RemovePlayer(ulong playerID)
    {
      if (!this.members.Contains(playerID))
        return false;
      this.members.Remove(playerID);
      BasePlayer byId = RelationshipManager.FindByID(playerID);
      if (Object.op_Inequality((Object) byId, (Object) null))
        byId.ClearTeam();
      if ((long) this.teamLeader == (long) playerID)
      {
        if (this.members.Count > 0)
          this.SetTeamLeader(this.members[0]);
        else
          this.Disband();
      }
      this.MarkDirty();
      return true;
    }

    public void SetTeamLeader(ulong newTeamLeader)
    {
      this.teamLeader = newTeamLeader;
      this.MarkDirty();
    }

    public void Disband()
    {
      RelationshipManager.Instance.DisbandTeam(this);
    }

    public void MarkDirty()
    {
      foreach (ulong member in this.members)
      {
        BasePlayer byId = RelationshipManager.FindByID(member);
        if (Object.op_Inequality((Object) byId, (Object) null))
          byId.UpdateTeam(this.teamID);
      }
    }
  }
}
