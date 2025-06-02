using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientQuickAccess.Classes
{
    public class NewLogonRequest
    {
        public string HashedUsername { get; set; }
        public string HashedPassword { get; set; }
    }
}
