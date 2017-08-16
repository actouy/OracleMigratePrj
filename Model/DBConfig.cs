using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class DBConfig
    {
        private string p1;
        private string p2;
        private string p3;
        private string p4;

        public DBConfig(string p1, string p2, string p3, string p4)
        {
            // TODO: Complete member initialization
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.p4 = p4;
        }

        public DBConfig()
        {
            // TODO: Complete member initialization
        }
        public string DBServerIP
        {
            get { return p1; }
            set { p1 = value; }
        }
        public string OracleSID
        {
            get { return p2; }
            set { p2 = value; }
        }
        public string DBUserName
        {
            get { return p3; }
            set { p3 = value; }
        }
        public string DBPassword
        {
            get { return p4; }
            set { p4 = value; }
        }
    }

}
