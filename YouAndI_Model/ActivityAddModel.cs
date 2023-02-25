using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouAndI_Model
{
    public class ActivityAddModel
    {
        public string title { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public int type { get; set; }
        public string location { get; set; }
        public string introduction { get; set; }
        public int maxnumber { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
        public PaymentModel? Payment { get; set; }
        public List<ActivityTagModel> tags { get; set; }
    }
}
