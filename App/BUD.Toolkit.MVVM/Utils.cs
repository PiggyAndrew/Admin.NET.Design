using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Toolkit.MVVM;

public class Utils
{
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string psz1, string psz2);

    public static int CompareStrings(string str1, string str2)
    {
        return StrCmpLogicalW(str1,str2);
    }
}
