using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GithubExtension
{
    public static class Helpers
    {
        public static int IndexOfDivergence(this string mainString, string divergentString)
        {
            if (mainString.Length == 0 || divergentString.Length == 0)
                return -1;

            for (int i = 0; i < mainString.Length; i++)
            {
                if (i >= divergentString.Length)
                    return i;
                if (mainString.Substring(i, 1) != divergentString.Substring(i, 1))
                    return i;
            }
            return -1;
        }
    }
}
