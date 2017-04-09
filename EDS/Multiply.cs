using System.Collections.Generic;

namespace CryptoWizard.Services
{
  public class Multiply
  {
    /// <summary>
    /// This method calculate multiply of points
    /// </summary>
    /// <param name="x">The point</param>
    /// <param name="y">The point</param>
    /// <param name="a">Argument of equation</param>
    /// <param name="p">Mod</param>
    /// <param name="k">Coefficient which show how many multiply</param>
    /// <returns>Return result of addition</returns>
    public IEnumerable<int> MultiplyResult(int x, int y, double a, int p, int k)
    {
      var _x = x;
      var _y = y;
      var point = new int[2];
      point[0] = x;
      point[1] = y;
      for (var i = 1; i < k; i++)
      {
        point = (_x != point[0]) && (_y != point[1]) ? CalculateIfPointsAreNotEqual(_x, _y, point[0], point[1], p) : 
                                                     CalculateIfPointsAreEqual(_x, _y, point[0], point[1], a, p);
      }
      return point;
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
    private int[] CalculateIfPointsAreEqual(int x1, int y1, int x2, int y2, double a, int p)
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
      return GetPoint((int)lambda, x1, x2, y1, p);
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
    private int[] CalculateIfPointsAreNotEqual(int x1, int y1, int x2, int y2, int p)
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
    /// This method calculate the point
    /// </summary>
    /// <param name="lambda">Lambda</param>
    /// <param name="x1">The first point</param>
    /// <param name="x2">The second point</param>
    /// <param name="y1">The first point</param>
    /// <param name="p">Mod</param>
    /// <returns>Return the point</returns>
    private int[] GetPoint(int lambda, int x1, int x2, int y1, int p)
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
      return new int[] { x3, y3 };
    }
  }
}
