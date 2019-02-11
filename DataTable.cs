using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JournalWork
{
    public class Subject
    {
        public string Table { get; set; }
        public string Name { get; set; }
        public string Tutor { get; set; }
        public double koeff { get; set; }
        public string FullName { get { return (Name + ((Tutor!="")?", " + Tutor:"")); } }

    }

    public class Students
    {
        public string Name { get; set; }
        public string Class_ { get; set; }

    }

    public class Event_
    {
        public string Tutor { get; set; }
        public string Subj { get; set; }
        public string Class_ { get; set; }
        public int ClassNum { get; set; }
        public DateTime dt { get; set; }
        public string event_ { get; set; }
        public string Student { get; set; }
    }

    public class Semestr_
    {
        public string Name { get; set; }
        public string Tutor { get; set; }
        public string Subj { get; set; }
        public string Class_ { get; set; }
        public int ClassNum { get; set; }
        public string event_ { get; set; }
        public string Student { get; set; }
        public double avg { get; set; }
        public int ball { get; set; }
        public string cnt { get; set; }
        public string skip { get; set; }
    }

    public class Plan
    {
        public string Tutor { get; set; }
        public string Subj { get; set; }
        public string Class_ { get; set; }
        public int ClassNum { get; set; }
        public DateTime dt { get; set; }
        public string event_ { get; set; }
        public string Student { get; set; }
    }

}
