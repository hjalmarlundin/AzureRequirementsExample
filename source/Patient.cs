using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace source
{
    public class Patient
    {
        private readonly int vitalSign;

        public Patient(int vitalSign)
        {
            this.vitalSign = vitalSign;
        }

        public bool CanUseDevice()
        {
            return vitalSign > 80;
        }
    }
}