using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBBPAYGExpireSIMS4Batch.Model
{
    
        public static class SAPModel
        {
            //private static List<string> copyList { get; set; }
            public static List<string> copyList
            {
                get { return _copyList; }
                set { _copyList = value; }
            }
            private static List<string> _copyList = new List<string>();

        }

        public static class DesModel
        {
            //private static List<string> copyList { get; set; }
            public static List<string> copyList
            {
                get { return _copyList; }
                set { _copyList = value; }
            }
            private static List<string> _copyList = new List<string>();

        }

    

}
