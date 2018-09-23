using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrozzleApplication
{   // Designed to handle groups in Crozzle 
    class CzlGroup  
    {
        public String CzlTitle { get; set; }
        public List<CrozzleFileItem> Lines { get; set; }

        public CzlGroup()
        {
            CzlTitle = null;
            Lines = new List<CrozzleFileItem>();
        }

        //public void AddNewItem(ConfigurationFileItem Line)

        //public override string ToString()
    }
}
