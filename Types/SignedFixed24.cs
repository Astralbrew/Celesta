using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Types
{
    internal class SignedFixed24
    {
        public short IntegerPart { get; set; } = 0;
        public byte FractionalPart { get; set; } = 0;

        public double Value => IntegerPart + FractionalPart / 256d;

        public SignedFixed24(double f)
        {
            IntegerPart = (short)Math.Floor(f);
            FractionalPart = (byte)((f - IntegerPart) * 256);
        }

        public static bool TryParse(string input, out SignedFixed24 result)
        {
            double fres;
            if (!double.TryParse(input, out fres))
            {
                result = new SignedFixed24(0);
                return false;
            }
            result = new SignedFixed24(fres);
            return true;    
        }

        public override string ToString()
        {
            return $"{Value} ({IntegerPart}:{FractionalPart})";
        }
    }
}
