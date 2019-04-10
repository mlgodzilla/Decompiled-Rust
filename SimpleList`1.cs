// Decompiled with JetBrains decompiler
// Type: SimpleList`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class SimpleList<T>
{
  private static readonly T[] emptyArray = new T[0];
  private const int defaultCapacity = 16;
  public T[] array;
  public int count;

  public T[] Array
  {
    get
    {
      return this.array;
    }
  }

  public int Count
  {
    get
    {
      return this.count;
    }
  }

  public int Capacity
  {
    get
    {
      return this.array.Length;
    }
    set
    {
      if (value == this.array.Length)
        return;
      if (value > 0)
      {
        T[] objArray = new T[value];
        if (this.count > 0)
          System.Array.Copy((System.Array) this.array, 0, (System.Array) objArray, 0, this.count);
        this.array = objArray;
      }
      else
        this.array = SimpleList<T>.emptyArray;
    }
  }

  public T this[int index]
  {
    get
    {
      return this.array[index];
    }
    set
    {
      this.array[index] = value;
    }
  }

  public SimpleList()
  {
    this.array = SimpleList<T>.emptyArray;
  }

  public SimpleList(int capacity)
  {
    this.array = capacity == 0 ? SimpleList<T>.emptyArray : new T[capacity];
  }

  public void Add(T item)
  {
    if (this.count == this.array.Length)
      this.EnsureCapacity(this.count + 1);
    this.array[this.count++] = item;
  }

  public void Clear()
  {
    if (this.count <= 0)
      return;
    System.Array.Clear((System.Array) this.array, 0, this.count);
    this.count = 0;
  }

  public bool Contains(T item)
  {
    for (int index = 0; index < this.count; ++index)
    {
      if (this.array[index].Equals((object) item))
        return true;
    }
    return false;
  }

  public void CopyTo(T[] array)
  {
    System.Array.Copy((System.Array) this.array, 0, (System.Array) array, 0, this.count);
  }

  public void EnsureCapacity(int min)
  {
    if (this.array.Length >= min)
      return;
    int num = this.array.Length == 0 ? 16 : this.array.Length * 2;
    this.Capacity = num < min ? min : num;
  }
}
