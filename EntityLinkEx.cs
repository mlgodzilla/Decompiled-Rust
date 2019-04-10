// Decompiled with JetBrains decompiler
// Type: EntityLinkEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System.Collections.Generic;

public static class EntityLinkEx
{
  public static void FreeLinks(this List<EntityLink> links)
  {
    for (int index = 0; index < links.Count; ++index)
    {
      EntityLink link = links[index];
      link.Clear();
      // ISSUE: cast to a reference type
      Pool.Free<EntityLink>((M0&) ref link);
    }
    links.Clear();
  }

  public static void ClearLinks(this List<EntityLink> links)
  {
    for (int index = 0; index < links.Count; ++index)
      links[index].Clear();
  }

  public static void AddLinks(
    this List<EntityLink> links,
    BaseEntity entity,
    Socket_Base[] sockets)
  {
    for (int index = 0; index < sockets.Length; ++index)
    {
      Socket_Base socket = sockets[index];
      EntityLink entityLink = (EntityLink) Pool.Get<EntityLink>();
      entityLink.Setup(entity, socket);
      links.Add(entityLink);
    }
  }
}
