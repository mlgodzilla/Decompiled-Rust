// Decompiled with JetBrains decompiler
// Type: NetworkCryptography
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using System.IO;

public abstract class NetworkCryptography : INetworkCryptocraphy
{
  private MemoryStream buffer = new MemoryStream();

  public MemoryStream EncryptCopy(
    Connection connection,
    MemoryStream stream,
    int offset)
  {
    this.buffer.Position = 0L;
    this.buffer.SetLength(0L);
    this.buffer.Write(stream.GetBuffer(), 0, offset);
    this.EncryptionHandler(connection, stream, offset, this.buffer, offset);
    return this.buffer;
  }

  public MemoryStream DecryptCopy(
    Connection connection,
    MemoryStream stream,
    int offset)
  {
    this.buffer.Position = 0L;
    this.buffer.SetLength(0L);
    this.buffer.Write(stream.GetBuffer(), 0, offset);
    this.DecryptionHandler(connection, stream, offset, this.buffer, offset);
    return this.buffer;
  }

  public void Encrypt(Connection connection, MemoryStream stream, int offset)
  {
    this.EncryptionHandler(connection, stream, offset, stream, offset);
  }

  public void Decrypt(Connection connection, MemoryStream stream, int offset)
  {
    this.DecryptionHandler(connection, stream, offset, stream, offset);
  }

  public bool IsEnabledIncoming(Connection connection)
  {
    if (connection != null && connection.encryptionLevel > 0)
      return (bool) connection.decryptIncoming;
    return false;
  }

  public bool IsEnabledOutgoing(Connection connection)
  {
    if (connection != null && connection.encryptionLevel > 0)
      return (bool) connection.encryptOutgoing;
    return false;
  }

  protected abstract void EncryptionHandler(
    Connection connection,
    MemoryStream src,
    int srcOffset,
    MemoryStream dst,
    int dstOffset);

  protected abstract void DecryptionHandler(
    Connection connection,
    MemoryStream src,
    int srcOffset,
    MemoryStream dst,
    int dstOffset);
}
