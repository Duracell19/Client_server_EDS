using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CryptoWizard.Services
{
  public class Addition 
  {
    private int[] _w; //Weights
    /// <summary>
    /// This method calculate addition of points
    /// </summary>
    /// <param name="x1">The first point</param>
    /// <param name="y1">The first point</param>
    /// <param name="x2">The second point</param>
    /// <param name="y2">The second point</param>
    /// <param name="a">Argument of equation</param>
    /// <param name="p">Mod</param>
    /// <returns>Return result of addition</returns>
    public IEnumerable<long> AdditionResult(long x1, long y1, long x2, long y2, double a, long p)
    {
      return (x1 != x2) && (y1 != y2) ? CalculateIfPointsAreNotEqual(x1, y1, x2, y2, p) : CalculateIfPointsAreEqual(x1, y1, x2, y2, a, p);
    }
    /// <summary>
    /// This method calculate the point if current points are equal
    /// </summary>
    /// <param name="x1">The first point</param>
    /// <param name="y1">The first point</param>
    /// <param name="x2">The second point</param>
    /// <param name="y2">The second point</param>
    /// <param name="a">Argument of equation</param>
    /// <param name="p">Mod</param>
    /// <returns>Return the point</returns>
    private IEnumerable<long> CalculateIfPointsAreEqual(long x1, long y1, long x2, long y2, double a, long p)
    {
      var lambda1 = 3 * x1 * x1 + a;
      var lambda2 = 2 * y1;
      if (lambda2 < 0)
      {
        lambda2 = (-1) * InverseElement(p, (-1) * lambda2);
      }
      else
      {
        lambda2 = InverseElement(p, lambda2);
      }
      var lambda = lambda1 * lambda2;
      lambda = (lambda < 0) ? (p + lambda) % p : lambda % p;
      return GetPoint((long)lambda, x1, x2, y1, p);
    }
    /// <summary>
    /// This method calculate the point if current points are not equal
    /// </summary>
    /// <param name="x1">The first point</param>
    /// <param name="y1">The first point</param>
    /// <param name="x2">The second point</param>
    /// <param name="y2">The second point</param>
    /// <param name="p">Mod</param>
    /// <returns>Return the point</returns>
    private IEnumerable<long> CalculateIfPointsAreNotEqual(long x1, long y1, long x2, long y2, long p)
    {
      var lambda1 = y2 - y1;
      var lambda2 = x2 - x1;
      if (lambda2 < 0)
      {
        lambda2 = (-1) * InverseElement(p, (-1) * lambda2);
      }
      else
      {
        lambda2 = InverseElement(p, lambda2);
      }
      var lambda = lambda1 * lambda2;
      lambda = (lambda < 0) ? (p + lambda) % p : lambda % p;
      return GetPoint(lambda, x1, x2, y1, p);
    }
    /// <summary>
    /// This method calculate inverse element with the help of advanced algorithm by Evklid
    /// </summary>
    /// <param name="p">Mod</param>
    /// <param name="value">Value</param>   
    /// <returns>Return inverse value</returns>
    private long InverseElement(long p, long value)
    {
      long d = 1, x = 0, a = value, b = p, q, y;
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
    /// This method calculate the point
    /// </summary>
    /// <param name="lambda">Lambda</param>
    /// <param name="x1">The first point</param>
    /// <param name="x2">The second point</param>
    /// <param name="y1">The first point</param>
    /// <param name="p">Mod</param>
    /// <returns>Return the point</returns>
    private IEnumerable<long> GetPoint(long lambda, long x1, long x2, long y1, long p)
    {
      var x3 = (lambda * lambda - x1 - x2) % p;
      if (x3 < 0)
      {
        x3 = (p + x3) % p;
      }
      var y3 = (lambda * (x1 - x3) - y1) % p;
      if (y3 < 0)
      {
        y3 = (p + y3) % p;
      }
      return new long[] { x3, y3 };
    }

    public IEnumerable<int> AdditionNeuralNetworkResult(int x1, int y1, int x2, int y2, double a, int p)
    {
      return (x1 != x2) && (y1 != y2) ? CalculateIfPointsAreNotEqualNeuralNetwork(x1, y1, x2, y2, p) :
                                        CalculateIfPointsAreEqualNeuralNetwork(x1, y1, x2, y2, a, p);
    }

    private IEnumerable<int> CalculateIfPointsAreEqualNeuralNetwork(int x1, int y1, int x2, int y2, double a, int p)
    {
      var max = new int[] { x1, y1, x2, y2 }.Max();
      var countRepeat = new BitArray(new int[] { max }).Cast<bool>().Select(bit => bit ? 1 : 0).Reverse().SkipWhile(x => x == 0).Count();
      var countW = countRepeat * 2;
      var w = new string[countW];
      _w = new int[countW];
      var j = 1;
      w[0] = "1";
      _w[0] = Convert.ToInt32(w[0], 2);
      var count = 1;
      for (var i = 1; i < countW; i++)
      {
        j *= 10;
        w[i] = j.ToString();
        _w[i] = Convert.ToInt32(w[i], 2);
        if (count == countRepeat)
        {
          j = 1;
          w[i] = j.ToString();
          _w[i] = Convert.ToInt32(w[i], 2);
          count = 0;
        }
        count++;
      }
      var lambda1 = 3 * x1 * x1 + a;
      var lambda2 = 2 * y1;
      if (lambda2 < 0)
      {
        lambda2 = (int)((-1) * InverseElement(p, (-1) * lambda2));
      }
      else
      {
        lambda2 = (int)(InverseElement(p, lambda2));
      }
      var lambda = (int)lambda1 * lambda2;
      lambda = (lambda < 0) ? (p + lambda) % p : lambda % p;
      var k_ = new BitArray(new int[] { lambda * lambda }).Cast<bool>().Select(bit => bit ? 1 : 0);
      int lambda_ = 0;
      for (int i = 0, i1 = countW - 1; i < countW; i++, i1--)
      {
        lambda_ += k_.ElementAt(i) * _w[i1];
      }
      var mod = new BitArray(new int[] { p });
      var x3 = mod.Xor(new BitArray(new int[] { lambda_ }).Xor(new BitArray(new int[] { x1 })).Xor(new BitArray(new int[] { x2 })));
      var x3_ = x3.Cast<bool>().Select(bit => bit ? 1 : 0).Reverse().SkipWhile(x => x == 0);
      var x_ = Convert.ToInt32(string.Join("", x3_), 2) % p;
      var yy = new BitArray(new int[] { x1 }).Xor(x3);
      var yyy = yy.Cast<bool>().Select(bit => bit ? 1 : 0).Reverse().SkipWhile(x => x == 0);
      var yyyy = Convert.ToInt32(string.Join("", yyy.DefaultIfEmpty()), 2);
      var y3_ = new BitArray(new int[] { yyyy * lambda }).Cast<bool>().Select(bit => bit ? 1 : 0);
      lambda_ = 0;
      for (int i = 0, i1 = countW - 1; i < countW; i++, i1--)
      {
        lambda_ += y3_.ElementAt(i) * _w[i1];
      }
      var y = new BitArray(new int[] { lambda_ }).Xor(new BitArray(new int[] { y1 }));
      var y_ = Convert.ToInt32(string.Join("", y.Cast<bool>().Select(bit => bit ? 1 : 0).Reverse().SkipWhile(x => x == 0)), 2) % p;
      return new int[] { x_, y_ };
    }

    private IEnumerable<int> CalculateIfPointsAreNotEqualNeuralNetwork(int x1, int y1, int x2, int y2, int p)
    {
      //TODO избавить от W можно сразу умножать десятичные числа на 2
      var max = new int[] { x1, y1, x2, y2 }.Max();
      var countRepeat = new BitArray(new int[] { max }).Cast<bool>().Select(bit => bit ? 1 : 0).Reverse().SkipWhile(x => x == 0).Count();
      var countW = countRepeat * 2;
      var w = new string[countW];
      _w = new int[countW];
      var j = 1;
      w[0] = "1";
      _w[0] = Convert.ToInt32(w[0], 2);
      var count = 1;
      for (var i = 1; i < countW; i++)
      {
        j *= 10;
        w[i] = j.ToString();
        _w[i] = Convert.ToInt32(w[i], 2);
        if (count == countRepeat)
        {
          j = 1;
          w[i] = j.ToString();
          _w[i] = Convert.ToInt32(w[i], 2);
          count = 0;
        }
        count++;
      }
      var mod = new BitArray(new int[] { p });
      var lambda1 = y2 - y1;
      var lambda2 = x2 - x1;
      if (lambda2 < 0)
      {
        lambda2 = (int)((-1) * InverseElement(p, (-1) * lambda2));
      }
      else
      {
        lambda2 = (int)(InverseElement(p, lambda2));
      }
      var lambda = lambda1 * lambda2;
      lambda = (lambda < 0) ? (p + lambda) % p : lambda % p;
      var k_ = new BitArray(new int[] { lambda * lambda }).Cast<bool>().Select(bit => bit ? 1 : 0); 
      int lambda_ = 0;
      for (int i = 0, i1 = countW - 1; i < countW; i++, i1--)
      {
        lambda_ += k_.ElementAt(i) * _w[i1];
      }
      var x3 = mod.Xor(new BitArray(new int[] { lambda_ }).Xor(new BitArray(new int[] { x1 })).Xor(new BitArray(new int[] { x2 })));
      var x3_ = x3.Cast<bool>().Select(bit => bit ? 1 : 0).Reverse().SkipWhile(x => x == 0).DefaultIfEmpty();
      var x_ = Convert.ToInt32(string.Join("", x3_), 2) % p;
      var yy = new BitArray(new int[] { x1 }).Xor(x3);
      var yyy = yy.Cast<bool>().Select(bit => bit ? 1 : 0).Reverse().SkipWhile(x => x == 0);
      var yyyy = Convert.ToInt32(string.Join("", yyy), 2);
      var y3_ = new BitArray(new int[] { yyyy * lambda }).Cast<bool>().Select(bit => bit ? 1 : 0); 
      lambda_ = 0;
      for (int i = 0, i1 = countW - 1; i < countW; i++, i1--)
      {
        lambda_ += y3_.ElementAt(i) * _w[i1];
      }
      var y = new BitArray(new int[] { lambda_ }).Xor(new BitArray(new int[] { y1 }));
      var str = y.Cast<bool>().Select(bit => bit ? 1 : 0).Reverse().SkipWhile(x => x == 0).DefaultIfEmpty();
      var y_ = Convert.ToInt32(string.Join("", str), 2) % p;
      return new int[] { x_ , y_ };
    }
  }
}
