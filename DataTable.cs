using System;

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
    public class Tutor
    {
        public string Name { get; set; }
        public string Class_ { get; set; }
        public int ClassNum { get; set; }

    }

    public class Event_ 
    {
        private string _FullEvent = "";
        public string Tutor { get; set; }
        public string Subj { get; set; }
        public string Class_ { get; set; }
        public int ClassNum { get; set; }
        public DateTime dt { get; set; }
        public string event_ { get; set; }
        public int kod { get; set; }
        public string Student { get; set; }
        public string FullEvent { get {
                _FullEvent = "";
                if (kod == 1) _FullEvent = "";
                if (kod == 2) _FullEvent = "«Низкая накопляемость оценок» (менее трех оценок за урок)";
                if (kod == 3) _FullEvent = "Нет ни одной оценки за дату - «Несвоевременное выставление оценки»";
                if (kod == 4) _FullEvent = "Необходимо отследить случай, когда за один урок стоит и «Н» и оценка ";
                if (kod == 5) _FullEvent = "Оценка «2» не исправлена за 3 занятия";
                if (kod == 6) _FullEvent = "Оценка «2» на первом или последнем уроке в триместре или полугодии";
                if (kod == 7) _FullEvent = "Оценка «2» после «Н»";
                return (_FullEvent);
            } }
    }

    public class Semestr_
    {
        public string Name { get; set; }
        public string Tutor { get; set; }
        public string Subj { get; set; }
        public string Class_ { get; set; }
        public int ClassNum { get; set; }
        public string event_ { get; set; }
        public int kod { get; set; }
        public string Student { get; set; }
        public double avg { get; set; }
        public string avg_str { get { return(avg.ToString("N2")); } }
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
        public int kod { get; set; }
        public string Student { get; set; }
    }

    public class Params
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Num { get; set; }
        public DateTime dt1 { get; set; }
        public DateTime dt2 { get; set; }

    }

    // АКТИВНОСТЬ
    public class actClass
    {
        public string Name { get; set; }
        public int ClassNum { get; set; }
        public int inputTutor { get; set; }
        public int inputStudent { get; set; }
        public int inputFamily { get; set; }
        public int countTutor { get; set; }
        public int countStudent { get; set; }
        public int countFamily { get; set; }
        public int countTutorFree { get; set; }
        public int countStudentFree { get; set; }
        public int countFamilyFree { get; set; }
        public int tot { get; set; }
    }
    public class actStudents
    {
        public string Name { get; set; }
        public string Class_ { get; set; }
        public int ClassNum { get; set; }
        public int Count { get; set; }
    }
    public class actTutor
    {
        public string Name { get; set; }
        public string Class_ { get; set; }
        public int ClassNum { get; set; }
        public int Count { get; set; }
    }
    public class actFamily
    {
        public string Name { get; set; }
        public string Class_ { get; set; }
        public int ClassNum { get; set; }
        public int Count { get; set; }
    }
    public class actStudentT0
    {
        public string Name { get; set; }
        public int ClassNum { get; set; }
        public int Count { get; set; }
        public double Pr { get; set; }
        public int Tot { get; set; }
    }

}
