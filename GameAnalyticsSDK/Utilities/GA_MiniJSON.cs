// Decompiled with JetBrains decompiler
// Type: GameAnalyticsSDK.Utilities.GA_MiniJSON
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameAnalyticsSDK.Utilities
{
  public class GA_MiniJSON
  {
    public static object Deserialize(string json)
    {
      if (json == null)
        return (object) null;
      return GA_MiniJSON.Parser.Parse(json);
    }

    public static string Serialize(object obj)
    {
      return GA_MiniJSON.Serializer.Serialize(obj);
    }

    private sealed class Parser : IDisposable
    {
      private const string WORD_BREAK = "{}[],:\"";
      private StringReader json;

      public static bool IsWordBreak(char c)
      {
        if (!char.IsWhiteSpace(c))
          return "{}[],:\"".IndexOf(c) != -1;
        return true;
      }

      private Parser(string jsonString)
      {
        this.json = new StringReader(jsonString);
      }

      public static object Parse(string jsonString)
      {
        using (GA_MiniJSON.Parser parser = new GA_MiniJSON.Parser(jsonString))
          return parser.ParseValue();
      }

      public void Dispose()
      {
        this.json.Dispose();
        this.json = (StringReader) null;
      }

      private Dictionary<string, object> ParseObject()
      {
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        this.json.Read();
        while (true)
        {
          GA_MiniJSON.Parser.TOKEN nextToken;
          do
          {
            nextToken = this.NextToken;
            if (nextToken != GA_MiniJSON.Parser.TOKEN.NONE)
            {
              if (nextToken == GA_MiniJSON.Parser.TOKEN.CURLY_CLOSE)
                goto label_5;
            }
            else
              goto label_4;
          }
          while (nextToken == GA_MiniJSON.Parser.TOKEN.COMMA);
          string index = this.ParseString();
          if (index != null)
          {
            if (this.NextToken == GA_MiniJSON.Parser.TOKEN.COLON)
            {
              this.json.Read();
              dictionary[index] = this.ParseValue();
            }
            else
              goto label_9;
          }
          else
            goto label_7;
        }
label_4:
        return (Dictionary<string, object>) null;
label_5:
        return dictionary;
label_7:
        return (Dictionary<string, object>) null;
label_9:
        return (Dictionary<string, object>) null;
      }

      private List<object> ParseArray()
      {
        List<object> objectList = new List<object>();
        this.json.Read();
        bool flag = true;
        while (flag)
        {
          GA_MiniJSON.Parser.TOKEN nextToken = this.NextToken;
          switch (nextToken)
          {
            case GA_MiniJSON.Parser.TOKEN.NONE:
              return (List<object>) null;
            case GA_MiniJSON.Parser.TOKEN.SQUARED_CLOSE:
              flag = false;
              continue;
            case GA_MiniJSON.Parser.TOKEN.COMMA:
              continue;
            default:
              object byToken = this.ParseByToken(nextToken);
              objectList.Add(byToken);
              continue;
          }
        }
        return objectList;
      }

      private object ParseValue()
      {
        return this.ParseByToken(this.NextToken);
      }

      private object ParseByToken(GA_MiniJSON.Parser.TOKEN token)
      {
        switch (token)
        {
          case GA_MiniJSON.Parser.TOKEN.CURLY_OPEN:
            return (object) this.ParseObject();
          case GA_MiniJSON.Parser.TOKEN.SQUARED_OPEN:
            return (object) this.ParseArray();
          case GA_MiniJSON.Parser.TOKEN.STRING:
            return (object) this.ParseString();
          case GA_MiniJSON.Parser.TOKEN.NUMBER:
            return this.ParseNumber();
          case GA_MiniJSON.Parser.TOKEN.TRUE:
            return (object) true;
          case GA_MiniJSON.Parser.TOKEN.FALSE:
            return (object) false;
          case GA_MiniJSON.Parser.TOKEN.NULL:
            return (object) null;
          default:
            return (object) null;
        }
      }

      private string ParseString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        this.json.Read();
        bool flag = true;
        while (flag)
        {
          if (this.json.Peek() == -1)
            break;
          char nextChar1 = this.NextChar;
          switch (nextChar1)
          {
            case '"':
              flag = false;
              continue;
            case '\\':
              if (this.json.Peek() == -1)
              {
                flag = false;
                continue;
              }
              char nextChar2 = this.NextChar;
              switch (nextChar2)
              {
                case '"':
                case '/':
                case '\\':
                  stringBuilder.Append(nextChar2);
                  continue;
                case 'b':
                  stringBuilder.Append('\b');
                  continue;
                case 'f':
                  stringBuilder.Append('\f');
                  continue;
                case 'n':
                  stringBuilder.Append('\n');
                  continue;
                case 'r':
                  stringBuilder.Append('\r');
                  continue;
                case 't':
                  stringBuilder.Append('\t');
                  continue;
                case 'u':
                  char[] chArray = new char[4];
                  for (int index = 0; index < 4; ++index)
                    chArray[index] = this.NextChar;
                  stringBuilder.Append((char) Convert.ToInt32(new string(chArray), 16));
                  continue;
                default:
                  continue;
              }
            default:
              stringBuilder.Append(nextChar1);
              continue;
          }
        }
        return stringBuilder.ToString();
      }

      private object ParseNumber()
      {
        string nextWord = this.NextWord;
        if (nextWord.IndexOf('.') == -1)
        {
          long result;
          long.TryParse(nextWord, out result);
          return (object) result;
        }
        double result1;
        double.TryParse(nextWord, out result1);
        return (object) result1;
      }

      private void EatWhitespace()
      {
        while (char.IsWhiteSpace(this.PeekChar))
        {
          this.json.Read();
          if (this.json.Peek() == -1)
            break;
        }
      }

      private char PeekChar
      {
        get
        {
          return Convert.ToChar(this.json.Peek());
        }
      }

      private char NextChar
      {
        get
        {
          return Convert.ToChar(this.json.Read());
        }
      }

      private string NextWord
      {
        get
        {
          StringBuilder stringBuilder = new StringBuilder();
          while (!GA_MiniJSON.Parser.IsWordBreak(this.PeekChar))
          {
            stringBuilder.Append(this.NextChar);
            if (this.json.Peek() == -1)
              break;
          }
          return stringBuilder.ToString();
        }
      }

      private GA_MiniJSON.Parser.TOKEN NextToken
      {
        get
        {
          this.EatWhitespace();
          if (this.json.Peek() == -1)
            return GA_MiniJSON.Parser.TOKEN.NONE;
          switch (this.PeekChar)
          {
            case '"':
              return GA_MiniJSON.Parser.TOKEN.STRING;
            case ',':
              this.json.Read();
              return GA_MiniJSON.Parser.TOKEN.COMMA;
            case '-':
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
              return GA_MiniJSON.Parser.TOKEN.NUMBER;
            case ':':
              return GA_MiniJSON.Parser.TOKEN.COLON;
            case '[':
              return GA_MiniJSON.Parser.TOKEN.SQUARED_OPEN;
            case ']':
              this.json.Read();
              return GA_MiniJSON.Parser.TOKEN.SQUARED_CLOSE;
            case '{':
              return GA_MiniJSON.Parser.TOKEN.CURLY_OPEN;
            case '}':
              this.json.Read();
              return GA_MiniJSON.Parser.TOKEN.CURLY_CLOSE;
            default:
              string nextWord = this.NextWord;
              if (nextWord == "false")
                return GA_MiniJSON.Parser.TOKEN.FALSE;
              if (nextWord == "true")
                return GA_MiniJSON.Parser.TOKEN.TRUE;
              return nextWord == "null" ? GA_MiniJSON.Parser.TOKEN.NULL : GA_MiniJSON.Parser.TOKEN.NONE;
          }
        }
      }

      private enum TOKEN
      {
        NONE,
        CURLY_OPEN,
        CURLY_CLOSE,
        SQUARED_OPEN,
        SQUARED_CLOSE,
        COLON,
        COMMA,
        STRING,
        NUMBER,
        TRUE,
        FALSE,
        NULL,
      }
    }

    private sealed class Serializer
    {
      private StringBuilder builder;

      private Serializer()
      {
        this.builder = new StringBuilder();
      }

      public static string Serialize(object obj)
      {
        GA_MiniJSON.Serializer serializer = new GA_MiniJSON.Serializer();
        serializer.SerializeValue(obj);
        return serializer.builder.ToString();
      }

      private void SerializeValue(object value)
      {
        if (value == null)
          this.builder.Append("null");
        else if (value is string str)
          this.SerializeString(str);
        else if (value is bool)
          this.builder.Append((bool) value ? "true" : "false");
        else if (value is IList anArray)
          this.SerializeArray(anArray);
        else if (value is IDictionary dictionary)
          this.SerializeObject(dictionary);
        else if (value is char)
          this.SerializeString(new string((char) value, 1));
        else
          this.SerializeOther(value);
      }

      private void SerializeObject(IDictionary obj)
      {
        bool flag = true;
        this.builder.Append('{');
        foreach (object key in (IEnumerable) obj.Keys)
        {
          if (!flag)
            this.builder.Append(',');
          this.SerializeString(key.ToString());
          this.builder.Append(':');
          this.SerializeValue(obj[key]);
          flag = false;
        }
        this.builder.Append('}');
      }

      private void SerializeArray(IList anArray)
      {
        this.builder.Append('[');
        bool flag = true;
        foreach (object an in (IEnumerable) anArray)
        {
          if (!flag)
            this.builder.Append(',');
          this.SerializeValue(an);
          flag = false;
        }
        this.builder.Append(']');
      }

      private void SerializeString(string str)
      {
        this.builder.Append('"');
        foreach (char ch in str.ToCharArray())
        {
          switch (ch)
          {
            case '\b':
              this.builder.Append("\\b");
              break;
            case '\t':
              this.builder.Append("\\t");
              break;
            case '\n':
              this.builder.Append("\\n");
              break;
            case '\f':
              this.builder.Append("\\f");
              break;
            case '\r':
              this.builder.Append("\\r");
              break;
            case '"':
              this.builder.Append("\\\"");
              break;
            case '\\':
              this.builder.Append("\\\\");
              break;
            default:
              int int32 = Convert.ToInt32(ch);
              if (int32 >= 32 && int32 <= 126)
              {
                this.builder.Append(ch);
                break;
              }
              this.builder.Append("\\u");
              this.builder.Append(int32.ToString("x4"));
              break;
          }
        }
        this.builder.Append('"');
      }

      private void SerializeOther(object value)
      {
        if (value is float)
          this.builder.Append(((float) value).ToString("R"));
        else if (value is int || value is uint || (value is long || value is sbyte) || (value is byte || value is short || (value is ushort || value is ulong)))
          this.builder.Append(value);
        else if (value is double || value is Decimal)
          this.builder.Append(Convert.ToDouble(value).ToString("R"));
        else
          this.SerializeString(value.ToString());
      }
    }
  }
}
