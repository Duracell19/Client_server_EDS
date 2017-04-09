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
    public IEnumerable<int> InverseResult(int x, int y, int p)
    {
      return new int[] { x, p - y };
    }
  }
}
