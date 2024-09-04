using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppThreatHuntAPI
{
    public class Policy
    {
        public bool DetectLatestMSAL { get; set; }
        public bool DetectFlow { get; set; }
        public bool DetectROPC{ get; set; }
        public bool DetectSecret { get; set; }
        public bool DetectNoOwner { get; set; }
        public bool DetectIntuneAppProtection { get; set; }
        public bool DetectMipSdk { get; set; }

    }
}
