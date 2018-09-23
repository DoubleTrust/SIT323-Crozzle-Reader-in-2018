using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrozzleApplication
{
    // Designed to connect each group's title and inner sequence
    public class TitleSequence  
    {
        public String SequenceLine { get; set; }
        public String Title { get; set; }

        public TitleSequence(String identifier, string originalData)
        {
            Title = identifier;
            SequenceLine = originalData;
        }

    }
}
