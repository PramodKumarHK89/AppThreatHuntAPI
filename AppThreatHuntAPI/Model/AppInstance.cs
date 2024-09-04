using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppThreatHuntAPI
{
    public class AppInstance
    {
        public string AppId{ get; set; }
        public string AppName{ get; set; }
        public bool IsLatestMSAL { get; set; }
        public bool DetectFlow { get; set; }
        public bool IsROPC { get; set; }    
        public bool IsSecret { get; set; }
        public bool IsNoOwner { get; set; }
        public bool IsIntuneAppProtection { get; set; }
        public bool IsIntuneAppProtectionInScope { get; set; }
        public bool IsMipSdkInScope { get; set; }
        public bool IsMipSdkUsed { get; set; }

    }
}
