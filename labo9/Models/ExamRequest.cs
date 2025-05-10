using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace labo9.Models
{
    public class ExamRequest
    {
        public int Id { get; set; }
        public string StudentId { get; set; }
        public string FullName { get; set; }
        public string SemesterYear { get; set; }
        public string Major { get; set; }
        public string Subject { get; set; }
        public int ECTSPoints { get; set; }
        public string Instructor { get; set; }
        public string Justification { get; set; }
        public string StudentSignature { get; set; }
        public string CommissionMember1 { get; set; }
        public string CommissionMember2 { get; set; }
        public string CommissionMember3 { get; set; }
        public string DeanDecision { get; set; }
        public DateTime DecisionDate { get; internal set; }
        public DateTime SubmissionDate { get; internal set; }
    }
}
