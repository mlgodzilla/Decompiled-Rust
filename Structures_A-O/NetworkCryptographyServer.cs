// Decompiled with JetBrains decompiler
// Type: NetworkCryptographyServer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using System.IO;

public class NetworkCryptographyServer : NetworkCryptography
{
  protected override void EncryptionHandler(
    Connection connection,
    MemoryStream src,
    int srcOffset,
    MemoryStream dst,
    int dstOffset)
  {
    if (connection.encryptionLevel <= 1)
      Craptography.XOR(2161U, src, srcOffset, dst, dstOffset);
    else
      EACServer.Encrypt(connection, src, srcOffset, dst, dstOffset);
  }

  protected override void DecryptionHandler(
    Connection connection,
    MemoryStream src,
    int srcOffset,
    MemoryStream dst,
    int dstOffset)
  {
    if (connection.encryptionLevel <= 1)
      Craptography.XOR(2161U, src, srcOffset, dst, dstOffset);
    else
      EACServer.Decrypt(connection, src, srcOffset, dst, dstOffset);
  }
}
