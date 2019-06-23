using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess
{
    public static class EFConstants
    {
        public static ObfuscateMode USERObfuscateMode = ObfuscateMode.UnMasked;

        public enum ObfuscateMode
        {
            Masked,
            UnMasked
        }
    }
}
