// Decompiled with JetBrains decompiler
// Type: EffectNetwork
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using Network.Visibility;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class EffectNetwork
{
  public static void Send(Effect effect)
  {
    if (Net.sv == null || !((Server) Net.sv).IsConnected())
      return;
    using (TimeWarning.New("EffectNetwork.Send", 0.1f))
    {
      if (!string.IsNullOrEmpty(effect.pooledString))
        effect.pooledstringid = (__Null) (int) StringPool.Get(effect.pooledString);
      if (effect.pooledstringid == null)
        Debug.Log((object) ("String ID is 0 - unknown effect " + effect.pooledString));
      else if (effect.broadcast)
      {
        if (!((Write) ((NetworkPeer) Net.sv).write).Start())
          return;
        ((Write) ((NetworkPeer) Net.sv).write).PacketID((Message.Type) 13);
        effect.WriteToStream((Stream) ((NetworkPeer) Net.sv).write);
        ((Write) ((NetworkPeer) Net.sv).write).Send(new SendInfo((IEnumerable<Connection>) BaseNetworkable.GlobalNetworkGroup.subscribers));
      }
      else
      {
        Group group;
        if (effect.entity > 0)
        {
          BaseEntity ent = BaseNetworkable.serverEntities.Find((uint) effect.entity) as BaseEntity;
          if (!ent.IsValid())
            return;
          group = (Group) ent.net.group;
        }
        else
          group = ((Manager) ((Server) Net.sv).visibility).GetGroup(effect.worldPos);
        if (group == null)
          return;
        ((Write) ((NetworkPeer) Net.sv).write).Start();
        ((Write) ((NetworkPeer) Net.sv).write).PacketID((Message.Type) 13);
        effect.WriteToStream((Stream) ((NetworkPeer) Net.sv).write);
        ((Write) ((NetworkPeer) Net.sv).write).Send(new SendInfo((IEnumerable<Connection>) group.subscribers));
      }
    }
  }

  public static void Send(Effect effect, Connection target)
  {
    effect.pooledstringid = (__Null) (int) StringPool.Get(effect.pooledString);
    if (effect.pooledstringid == null)
    {
      Debug.LogWarning((object) ("EffectNetwork.Send - unpooled effect name: " + effect.pooledString));
    }
    else
    {
      ((Write) ((NetworkPeer) Net.sv).write).Start();
      ((Write) ((NetworkPeer) Net.sv).write).PacketID((Message.Type) 13);
      effect.WriteToStream((Stream) ((NetworkPeer) Net.sv).write);
      ((Write) ((NetworkPeer) Net.sv).write).Send(new SendInfo(target));
    }
  }
}
