using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CryptoWizard.Services
{
  /// <summary>
  /// This class implements EDS based on ГОСТ 34.10 - 2001 and use PCLCrypto to calculate SHA1
  /// </summary>
  public class ElectronicDigitalSignature
  {
    /// <summary>
    /// This method generate EDS
    /// </summary>
    /// <param name="x">The point</param>
    /// <param name="y">The point</param>
    /// <param name="message">Message</param>
    /// <param name="privateKey">Private key</param>
    /// <param name="N">Order curve</param>
    /// <param name="k">Random value</param>
    /// <param name="a">Argument of equation</param>
    /// <param name="p">Mod</param>
    /// <returns>Return the pair of value EDS (r and s)</returns>
    public IEnumerable<int> GenerateEDS(int x, int y, byte[] message, int privateKey, int N, int k, double a, int p)
    {
      var n = GreatestPrimeDivisor(N);
      if (n != 0)
      {
        var e = GetHash(message, n);
        var kG = new Multiply().MultiplyResult(x, y, a, p, k);
        var r = kG.ElementAt(0) % n;
        var z = InverseElement(n, k);
        var s = z * (e + privateKey * r) % n;
        return new int[] { r, s };
      }
      return null;
    }
    /// <summary>
    /// This method check EDS
    /// </summary>
    /// <param name="x">The point</param>
    /// <param name="y">The point</param>
    /// <param name="r">The EDS</param>
    /// <param name="s">The EDS</param>
    /// <param name="message">Message</param>
    /// <param name="privateKey">Private key</param>
    /// <param name="N">Order curve</param>
    /// <param name="a">Argument of equation</param>
    /// <param name="p">Mod</param>
    /// <returns>Return true if EDS is authentic else false </returns>
    public bool CheckEDS(int x, int y, int r, int s, byte[] message, int privateKey, int N, double a, int p)
    {
      var n = GreatestPrimeDivisor(N);
      if (n != 0)
      {
        var e = GetHash(message, n);
        var Q = new Multiply().MultiplyResult(x, y, a, p, privateKey);
        if ((r >= 1 && r <= n - 1) && (s >= 1 && s <= n - 1))
        {
          var v = InverseElement(n, s);
          var u1 = (e * v) % n;
          var u2 = (r * v) % n;
          var u1G = new Multiply().MultiplyResult(x, y, a, p, u1);
          var u2Q = new Multiply().MultiplyResult(Q.ElementAt(0), Q.ElementAt(1), a, p, u2);
          var X = new Addition().AdditionResult(u1G.ElementAt(0), u1G.ElementAt(1), u2Q.ElementAt(0), u2Q.ElementAt(1), a, p);
          return (r == X.ElementAt(0) % n) ? true : false;
        }
        else
        {
          return false;
        }
      }
      return false;
    }
    /// <summary>
    /// This method calculate greatest prime divisor
    /// </summary>
    /// <param name="N">Order curve</param>
    /// <returns>Return greatest prime divisor</returns>
    private int GreatestPrimeDivisor(int N)
    {
      var result = 0;
      while (N % 2 == 0)
      {
        N /= 2;
      }
      for (var i = 3; i <= N;)
      {
        if (N % i == 0)
        {
          result = i;
          N /= i;
        }
        else
        {
          i += 2;
        }
      }
      return result;
    }
    /// <summary>
    /// This method calculate inverse element with the help of advanced algorithm by Evklid
    /// </summary>
    /// <param name="p">Mod</param>
    /// <param name="value">Value</param>   
    /// <returns>Return inverse value</returns>
    private int InverseElement(int p, int value)
    {
      int d = 1, x = 0, a = value, b = p, q, y;
      while (a.CompareTo(0) == 1)
      {
        q = b / a;
        y = a;
        a = b % a;
        b = y;
        y = d;
        d = x - (q * d);
        x = y;
      }
      x = x % p;
      if (x.CompareTo(0) == -1)
      {
        x = (x + p) % p;
      }
      return (x < 0) ? (p + x) : x;
    }    
    /// <summary>
    /// This method calculate hash
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="n">Mod</param>
    /// <returns>Return integer value of hash</returns>
    private int GetHash(byte[] message, int n)
    {
      var hash = BitConverter.ToInt32(new SHA1Managed().ComputeHash(message), 0);
      hash = (hash < 0) ? (n - hash) % n : hash % n;
      return (hash == 0) ? 1 : hash;
    }
  }
}
