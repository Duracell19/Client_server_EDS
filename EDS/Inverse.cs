using System.Collections.Generic;

namespace CryptoWizard.Services
{
  public class Inverse
  {
    /// <summary>
    /// This method inverse the point
    /// </summary>
    /// <param name="x">The first point</param>
    /// <param name="y">The first point</param>
    /// <param name="p">Mod</param>
    /// <returns>Return the inverse point</returns>
    public IEnumerable<long> InverseResult(long x, long y, long p)
    {
      return new long[] { x, p - y };
    }
  }
}
