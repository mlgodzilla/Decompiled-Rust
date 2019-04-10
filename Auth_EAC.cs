// Decompiled with JetBrains decompiler
// Type: Auth_EAC
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using System.Collections;

public static class Auth_EAC
{
  public static IEnumerator Run(Connection connection)
  {
    if (connection.active != null && connection.rejected == null)
    {
      connection.authStatus = (__Null) string.Empty;
      EACServer.OnJoinGame(connection);
      while (connection.active != null && connection.rejected == null && (string) connection.authStatus == string.Empty)
        yield return (object) null;
    }
  }
}
