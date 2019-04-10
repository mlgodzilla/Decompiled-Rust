// Decompiled with JetBrains decompiler
// Type: CommunityEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;

public class CommunityEntity : PointEntity
{
  public static CommunityEntity ServerInstance;
  public static CommunityEntity ClientInstance;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("CommunityEntity.OnRpcMessage", 0.1f))
      ;
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override void InitShared()
  {
    if (this.isServer)
      CommunityEntity.ServerInstance = this;
    else
      CommunityEntity.ClientInstance = this;
    base.InitShared();
  }

  public override void DestroyShared()
  {
    base.DestroyShared();
    if (this.isServer)
      CommunityEntity.ServerInstance = (CommunityEntity) null;
    else
      CommunityEntity.ClientInstance = (CommunityEntity) null;
  }
}
