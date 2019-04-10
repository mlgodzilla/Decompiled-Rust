// Decompiled with JetBrains decompiler
// Type: DirectoryEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.IO;
using System.Threading;

public static class DirectoryEx
{
  public static void Backup(DirectoryInfo parent, params string[] names)
  {
    for (int index = 0; index < names.Length; ++index)
      names[index] = Path.Combine(parent.FullName, names[index]);
    DirectoryEx.Backup(names);
  }

  public static bool MoveToSafe(this DirectoryInfo parent, string target, int retries = 10)
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
      DirectoryInfo parent = new DirectoryInfo(names[index]);
      DirectoryInfo directoryInfo = new DirectoryInfo(names[index + 1]);
      if (parent.Exists)
      {
        if (directoryInfo.Exists)
        {
          if ((DateTime.Now - directoryInfo.LastWriteTime).TotalHours >= (index == 0 ? 0.0 : (double) (1 << index - 1)))
          {
            directoryInfo.Delete(true);
            parent.MoveToSafe(directoryInfo.FullName, 10);
          }
        }
        else
        {
          if (!directoryInfo.Parent.Exists)
            directoryInfo.Parent.Create();
          parent.MoveToSafe(directoryInfo.FullName, 10);
        }
      }
    }
  }

  public static void CopyAll(string sourceDirectory, string targetDirectory)
  {
    DirectoryEx.CopyAll(new DirectoryInfo(sourceDirectory), new DirectoryInfo(targetDirectory));
  }

  public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
  {
    if (source.FullName.ToLower() == target.FullName.ToLower() || !source.Exists)
      return;
    if (!target.Exists)
      target.Create();
    foreach (FileInfo file in source.GetFiles())
    {
      FileInfo fileInfo = new FileInfo(Path.Combine(target.FullName, file.Name));
      file.CopyTo(fileInfo.FullName, true);
      fileInfo.CreationTime = file.CreationTime;
      fileInfo.LastAccessTime = file.LastAccessTime;
      fileInfo.LastWriteTime = file.LastWriteTime;
    }
    foreach (DirectoryInfo directory in source.GetDirectories())
    {
      DirectoryInfo subdirectory = target.CreateSubdirectory(directory.Name);
      DirectoryEx.CopyAll(directory, subdirectory);
      subdirectory.CreationTime = directory.CreationTime;
      subdirectory.LastAccessTime = directory.LastAccessTime;
      subdirectory.LastWriteTime = directory.LastWriteTime;
    }
  }
}
