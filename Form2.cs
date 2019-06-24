using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FastReport;

namespace JournalWork
{
    public partial class Form2 : Form
    {
        public DataSet result;
        public List<actClass> dsClass;
        public List<actTutor> dsTutor;
        public List<actStudents> dsStudents;
        public List<actFamily> dsFamily;
        public RootCollection listClass;
        public DateTime dtRep1;
        public DateTime dtRep2;

        public Form2()
        {
            InitializeComponent();

            dsClass = new List<actClass>();
            dsClass.Clear();
            dsTutor = new List<actTutor>();
            dsTutor.Clear();
            dsStudents = new List<actStudents>();
            dsStudents.Clear();
            dsFamily = new List<actFamily>();
            dsFamily.Clear();

            listClass = new RootCollection();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            lFileData1.Text = "";
            lFileData2.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            lFileData1.Text = openFileDialog1.FileName;
            btnOK.Enabled = true;
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            lFileData2.Text = openFileDialog2.FileName;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

            // Загрузка и преобразование Exel к DataSet
            using (var reader = ExcelReaderFactory.CreateReader(new FileStream(lFileData1.Text, FileMode.Open)))
            {
                result = reader.AsDataSet();
                Bind();
            }

        }
        private void Bind()
        {
            this.Cursor = Cursors.WaitCursor;

            string str0 = "";
            int totTutor = 0;
            int totStudent = 0;
            int totFamily = 0;
            int totTutorFree = 0;
            int totStudentFree = 0;
            int totFamilyFree = 0;
            int totTutorInp = 0;
            int totStudentInp = 0;
            int totFamilyInp = 0;

            int workDays = 0;

            foreach (DataTable tbl in result.Tables)
            {
                DataRow[] dr = tbl.Select();
                string tblName = tbl.TableName;

                int pos = 0;
                while (pos < dr.Length)
                {
                    if ((TConvert.ToString(dr[pos][0]) != "") && (TConvert.ToString(dr[pos][1]) == ""))
                    {
                        // Читаем заголовок
                        while (str0 == "")
                        {
                            string str = TConvert.ToString(dr[pos][0]);
                            if (str.ToUpper().IndexOf("ДАННЫЕ") != -1)
                            {
                                str0 = str;
                                string[] separator = { "с", "по" };
                                string[] mas = str.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                                dtRep1 = TConvert.ToDateTime(mas[1]);
                                dtRep2 = TConvert.ToDateTime(mas[2]);
                                workDays = getBusinessDates(dtRep1, dtRep2);
                            }
                            pos++;
                            if (pos == dr.Length) break;
                        }
                    }
                    if ((str0 != "") && (TConvert.ToString(dr[pos][0]).IndexOf("Пользователь") != -1))
                    {
                        pos++;
                        break;
                    }
                    pos++;
                }
                if (pos >= dr.Length)
                    continue;

                while (pos < dr.Length)
                {
                    DataRow r0 = dr[pos];
                    string sName = TConvert.ToString(r0[0]);
                    string sRole = TConvert.ToString(r0[1]).ToUpper();
                    string sClass = TConvert.ToString(r0[2]);
                    string[] mClass = sClass.Split(',');
                    int cntInput = TConvert.ToInt(r0[4].ToString().Replace("-", ""));
                    if (sClass != "")
                    {
                        // ФОРМИРУЕМ КЛАССЫ
                        for (int i = 0; i < mClass.Length; i++)
                        {
                            int ClassNum = TConvert.ToInt(Regex.Replace(mClass[i], "[^0-9]", ""));

                            ItemRoot ir = listClass.getByID(mClass[i]);
                            if (ir == null)
                            {
                                ir = new ItemRoot { ID = mClass[i], ClassNum = ClassNum };
                                listClass.Add(ir);
                            }

                            if (sRole.IndexOf("ПЕДАГОГ") != -1)
                            {
                                dsTutor.Add(new actTutor
                                {
                                    Class_ = mClass[i],
                                    Name = sName,
                                    Count = cntInput
                                });
                                ir.inputTutor += cntInput;
                                if (cntInput > 0) { totTutorInp++; ir.countTutor++;  } else { totTutorFree++; ir.countTutorFree++; }
                                ir.tot++;
                                totTutor++;
                            }
                            if (sRole.IndexOf("ОБУЧАЮ") != -1)
                            {
                                dsStudents.Add(new actStudents
                                {
                                    Class_ = mClass[i],
                                    Name = sName,
                                    Count = cntInput
                                });
                                ir.inputStudent += cntInput;
                                if (cntInput > 0) { totStudentInp++; ir.countStudent++; } else { totStudentFree++; ir.countStudentFree++; }
                                ir.tot++;
                                totStudent++;
                            }
                            if (sRole.IndexOf("РОДИТ") != -1)
                            {
                                dsFamily.Add(new actFamily
                                {
                                    Class_ = mClass[i],
                                    Name = sName,
                                    Count = cntInput
                                });
                                ir.inputFamily += cntInput;
                                if (cntInput > 0) { totFamilyInp++; ir.countFamily++; } else { totFamilyFree++; ir.countFamilyFree++; }
                                ir.tot++;
                                totFamily++;
                            }
                        }
                    }
                    pos++;
                }
            }

            // ФИКСИРУЕМ РАСЧЁТЫ ПО КЛАССАМ
            foreach (ItemRoot ir in listClass)
            {
                dsClass.Add(new actClass
                {
                    Name=ir.ID,
                    inputFamily = ir.inputFamily,
                    inputStudent = ir.inputStudent,
                    inputTutor = ir.inputTutor,
                    countFamily = ir.countFamily,
                    countStudent = ir.countStudent,
                    countTutor = ir.countTutor,
                    countFamilyFree = ir.countFamilyFree,
                    countStudentFree = ir.countStudentFree,
                    countTutorFree = ir.countTutorFree,
                    tot = ir.tot,
                    ClassNum = ir.ClassNum
                });
            }

            ////// ФОРМИРУЕМ ОТЧЁТ
            Report report = new Report();
            using (MemoryStream stream = new MemoryStream(Properties.Resources.spravka2))
            {
                report.Load(stream);
            }
            //report.Load("../../spravka2.frx");

            report.SetParameterValue("dt1", dtRep1.ToString("dd.MM.yyyy"));
            report.SetParameterValue("dt1_day", dtRep1.ToString("dd"));
            report.SetParameterValue("dt1_month", dtRep1.ToString("MM"));
            report.SetParameterValue("dt1_year", dtRep1.ToString("yyyy"));
            report.SetParameterValue("dt1_day", dtRep1.ToString("dd"));
            report.SetParameterValue("dt2", dtRep2.ToString("dd.MM.yyyy"));
            report.SetParameterValue("dt2_day", dtRep2.ToString("dd"));
            report.SetParameterValue("dt2_month", dtRep2.ToString("MM"));
            report.SetParameterValue("dt2_smonth", dtRep2.ToString("MMMM"));
            report.SetParameterValue("dt2_year", dtRep2.ToString("yyyy"));
            report.SetParameterValue("totTutor", totTutor);
            report.SetParameterValue("totStudent", totStudent);
            report.SetParameterValue("totFamily", totFamily);
            report.SetParameterValue("totTutorInp", totTutorInp);
            report.SetParameterValue("totStudentInp", totStudentInp);
            report.SetParameterValue("totFamilyInp", totFamilyInp);
            report.SetParameterValue("totTutorFree", totTutorFree);
            report.SetParameterValue("totStudentFree", totStudentFree);
            report.SetParameterValue("totFamilyFree", totFamilyFree);

            DataSet ds = new DataSet();

            // ГРУППИРУЕМ ПО КЛАССАМ И СЧИТАЕМ УЧЕНИКОВ БЕЗ ВХОДОВ
            List<actStudentT0> ds0 = new List<actStudentT0>();
            var query = dsStudents.Where(ev => ((ev.Count == 0))).
                GroupBy(rec => rec.Class_, rec => rec.Count, (keys, args) => new { Class_ = keys, Count = args.Count() }).
                OrderBy(rec => rec.Class_ ).
                ToList();
            foreach (var result in query)
            {
                int tot = listClass.getByID(result.Class_).tot;
                ds0.Add(new actStudentT0 { Name = result.Class_, ClassNum = listClass.getByID(result.Class_).ClassNum, Count = result.Count, Pr = TConvert.ToDouble(result.Count) * 100 / TConvert.ToDouble(tot),  Tot= TConvert.ToInt(tot) });
            }
            DataTable tbl_ = ToDataTable(ds0);
            tbl_.TableName = "StudentT0";
            ds.Tables.Add(tbl_);

            // ФОРМИРУЕМ СПИСОК НАИБОЛЕЕ АКТИВНЫХ КЛАССОВ
            string strGoodClass = "";
            ds0 = new List<actStudentT0>();
            query = dsStudents.Where(ev => ((ev.Count > 0))).
                GroupBy(rec => rec.Class_, rec => rec.Count, (keys, args) => new { Class_ = keys, Count = args.Count(),  }).
                OrderByDescending(rec => rec.Count).
                ToList();
            foreach (var result in query)
            {
                int tot = listClass.getByID(result.Class_).tot;
                ds0.Add(new actStudentT0 { Name = result.Class_, Count = result.Count, Pr = TConvert.ToDouble(result.Count) * 100 / TConvert.ToDouble(tot), Tot = TConvert.ToInt(tot) });
            }
            ds0 = ds0.Where(ev => ((ev.Pr > 95))).
                OrderByDescending(rec => rec.Pr).
                ToList();
            foreach (actStudentT0 result in ds0)
            {
                strGoodClass += ((strGoodClass != "") ? ", " : "") + result.Name;
            }
            report.SetParameterValue("strGoodClass", strGoodClass);

            // ЕЖЕДНЕВНОЕ КОЛИЧЕСТВО УЧЕНИКОВ
            int countDay = 0;
            query = dsStudents.Where(ev => ((ev.Count >= workDays))).
                GroupBy(rec => rec.Class_, rec => rec.Count, (keys, args) => new { Class_ = keys, Count = args.Count(), }).
                OrderByDescending(rec => rec.Count).
                ToList();
            foreach (var result in query)
            {
                countDay += result.Count;
            }
            report.SetParameterValue("countDayStudent", countDay);

            // ЕЖЕДНЕВНОЕ КОЛИЧЕСТВО РОДИТЕЛЕЙ
            countDay = 0;
            query = dsFamily.Where(ev => ((ev.Count >= workDays))).
                GroupBy(rec => rec.Class_, rec => rec.Count, (keys, args) => new { Class_ = keys, Count = args.Count(), }).
                OrderByDescending(rec => rec.Count).
                ToList();
            foreach (var result in query)
            {
                countDay += result.Count;
            }
            report.SetParameterValue("countDayFamily", countDay);

            // НАИБОЛЕЕ АКТИВНЫЕ РОДИТЕЛИ
            tbl_ = ToDataTable(dsFamily);
            tbl_.TableName = "Family";
            ds.Tables.Add(tbl_);

            // СПИСОК УЧИТЕЛЕЙ
            dsTutor = dsTutor.OrderBy(rec => rec.Name).ToList();
            tbl_ = ToDataTable(dsTutor);
            tbl_.TableName = "Tutor";
            ds.Tables.Add(tbl_);

            // ПОЛНЫЙ РАСКЛАД ПО КЛАССАМ
            dsClass = dsClass.OrderBy(rec => rec.ClassNum).ToList();
            tbl_ = ToDataTable(dsClass);
            tbl_.TableName = "Class_";
            ds.Tables.Add(tbl_);

            // Регистрируем источники в отчёте
            report.RegisterData(ds);
            report.GetDataSource("StudentT0").Enabled = true;
            report.GetDataSource("Family").Enabled = true;
            report.GetDataSource("Tutor").Enabled = true;
            report.GetDataSource("Class_").Enabled = true;
            report.AutoFillDataSet = true;


            this.Cursor = Cursors.Default;
            report.Prepare();
            report.ShowPrepared();
            //report.Design();

            report.Dispose();
            ds.Dispose();
            this.Close();
        }

        public DataTable ToDataTable<T>(List<T> iList)
        {
            DataTable dataTable = new DataTable();
            PropertyDescriptorCollection propertyDescriptorCollection =
                TypeDescriptor.GetProperties(typeof(T));
            for (int i = 0; i < propertyDescriptorCollection.Count; i++)
            {
                PropertyDescriptor propertyDescriptor = propertyDescriptorCollection[i];
                Type type = propertyDescriptor.PropertyType;

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    type = Nullable.GetUnderlyingType(type);


                dataTable.Columns.Add(propertyDescriptor.Name, type);
            }
            object[] values = new object[propertyDescriptorCollection.Count];
            foreach (T iListItem in iList)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = propertyDescriptorCollection[i].GetValue(iListItem);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        public enum NonWorkingDays { SaturdaySunday = 0, FridaySaturday = 1 };
        public int getBusinessDates(DateTime dateSt, DateTime dateNd, NonWorkingDays nonWorkingDays = NonWorkingDays.SaturdaySunday)
        {
            List<DateTime> datelist = new List<DateTime>();
            while (dateSt.Date < dateNd.Date)
            {
                datelist.Add((dateSt = dateSt.AddDays(1)));
            }
            if (nonWorkingDays == NonWorkingDays.SaturdaySunday)
            {
                return datelist.Count(d => d.DayOfWeek != DayOfWeek.Saturday &&
                       d.DayOfWeek != DayOfWeek.Friday);
            }
            else
            {
                return datelist.Count(d => d.DayOfWeek != DayOfWeek.Friday &&
                       d.DayOfWeek != DayOfWeek.Saturday);
            }
        }

        public static DateTime AddWorkingDays(DateTime date, int daysToAdd)
        {
            while (daysToAdd > 0)
            {
                date = date.AddDays(1);

                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday) daysToAdd -= 1;
            }

            return date;
        }

    }
}
