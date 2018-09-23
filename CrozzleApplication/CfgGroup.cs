using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrozzleApplication
{
    // Designed to handle groups in configuration 
    class CfgGroup  
    {
        public String CfgTitle { get; set; }
        public List<ConfigurationFileItem> Lines { get; set; }

        public CfgGroup()
        {
            CfgTitle = null;
            Lines = new List<ConfigurationFileItem>();

        }
    }
}
