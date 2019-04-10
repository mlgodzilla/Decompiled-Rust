// Decompiled with JetBrains decompiler
// Type: FileEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.IO;
using System.Threading;

public static class FileEx
{
  public static void Backup(DirectoryInfo parent, params string[] names)
  {
    for (int index = 0; index < names.Length; ++index)
      names[index] = Path.Combine(parent.FullName, names[index]);
    FileEx.Backup(names);
  }

  public static bool MoveToSafe(this FileInfo parent, string target, int retries = 10)
  {
    for (int index = 0; index < retries; ++index)
    {
      try
      {
        parent.MoveTo(target);
      }
      catch (Exception ex)
      {
        Thread.Sleep(5);
        continue;
      }
      return true;
    }
    return false;
  }

  public static void Backup(params string[] names)
  {
    for (int index = names.Length - 2; index >= 0; --index)
    {
      FileInfo parent = new FileInfo(names[index]);
      FileInfo fileInfo = new FileInfo(names[index + 1]);
      if (parent.Exists)
      {
        if (fileInfo.Exists)
        {
          if ((DateTime.Now - fileInfo.LastWriteTime).TotalHours >= (index == 0 ? 0.0 : (double) (1 << index - 1)))
          {
            fileInfo.Delete();
            parent.MoveToSafe(fileInfo.FullName, 10);
          }
        }
        else
        {
          if (!fileInfo.Directory.Exists)
            fileInfo.Directory.Create();
          parent.MoveToSafe(fileInfo.FullName, 10);
        }
      }
    }
  }
}
