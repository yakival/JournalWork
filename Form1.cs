using ExcelDataReader;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ADGV;
using FastReport;

namespace JournalWork
{
    public partial class Form1 : Form
    {
        public DataSet result;
        public List<DataSet> mResult;
        public DataSet resultPlan;
        public List<Subject> dsSubject;
        public List<Students> dsStudents;
        public List<Event_> dsEvent;
        public List<Semestr_> dsSemestr;
        public List<Plan> dsPlan;
        public double Koeff;
        public RootCollection good = new RootCollection();
        public RootCollection subj_ = new RootCollection();
        public List<RootCollection> listLession = new List<RootCollection>();
        public int totJournal = 0, cntJournal = 0, totPlan = 0, cntPlan = 0;

        public Form1()
        {
            InitializeComponent();

            mResult = new List<DataSet>();

            dsSubject = new List<Subject>();
            dsSubject.Clear();
            dsStudents = new List<Students>();
            dsStudents.Clear();
            dsEvent = new List<Event_>();
            dsEvent.Clear();
            dsSemestr = new List<Semestr_>();
            dsSemestr.Clear();
            dsPlan = new List<Plan>();
            dsPlan.Clear();

            shcool.SelectedIndex = 0;

            DataTable tbl = ToDataTable(dsEvent);
            dataGridView1.Columns.Clear();
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = tbl;
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                if (col.Name == "ClassNum") col.Visible = false;
                if (col.Name == "Tutor") col.HeaderText = "Учитель";
                if (col.Name == "Subj") col.HeaderText = "Предмет";
                if (col.Name == "Class_") col.HeaderText = "Класс";
                if (col.Name == "dt") col.HeaderText = "Дата";
                if (col.Name == "event_") col.HeaderText = "Ошибка";
                if (col.Name == "Student") col.HeaderText = "Ученик";
            }

            tbl = ToDataTable(dsPlan);
            planGridView.Columns.Clear();
            planGridView.AutoGenerateColumns = true;
            planGridView.DataSource = tbl;
            foreach (DataGridViewColumn col in planGridView.Columns)
            {
                if (col.Name == "ClassNum") col.Visible = false;
                if (col.Name == "Tutor") col.HeaderText = "Учитель";
                if (col.Name == "Subj") col.HeaderText = "Предмет";
                if (col.Name == "Class_") col.HeaderText = "Класс";
                if (col.Name == "dt") col.HeaderText = "Дата";
                if (col.Name == "event_") col.HeaderText = "Ошибка";
                if (col.Name == "Student") col.HeaderText = "Ученик";
            }

            tbl = ToDataTable(dsSemestr);
            semestrGridView.DataSource = tbl;

        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            dsSubject.Clear();
            dsSemestr.Clear();
            toolStripProgressBar1.Value = 0;
            dsEvent.Clear();
            good.Clear();
            subj_.Clear();

            Koeff = 0.5;// TConvert.ToDouble(txtKoeff.Text);

            // Выбор архива
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Загрузка
                totJournal = 0; cntJournal = 0; totPlan = 0; cntPlan = 0;
                int cnt = 0;
                listLession.Clear();
                mResult.Clear();
                this.Cursor = Cursors.WaitCursor;
                Ionic.Zip.ZipFile zf = new Ionic.Zip.ZipFile(openFileDialog1.FileName);
                foreach (ZipEntry zipEntry in zf)
                {
                    if (zipEntry.FileName.IndexOf(".xls") != -1)
                    {
                        MemoryStream zipMs = new MemoryStream();
                        zipEntry.Extract(zipMs);
                        zipMs.Seek(0, SeekOrigin.Begin);
                        // Загрузка и преобразование Exel к DataSet
                        using (var reader = ExcelReaderFactory.CreateReader(zipMs))
                        {
                            result = reader.AsDataSet();
                            mResult.Add(result);
                            // Разбор по локальным таблицам
                            Bind();
                        }
                        zipMs.Dispose();
                    }
                    cnt++;
                    toolStripProgressBar1.Value = 100 * cnt / zf.Count;
                    Application.DoEvents();
                }
                lblJournal.Text = String.Format("{0}/{1}", cntJournal, totJournal);
                lblPlan.Text = String.Format("{0}/{1}", cntPlan, totPlan);
                this.Cursor = Cursors.Default;
            }

            //DataTable tbl = ToDataTable(dsSubject);
            koeffGridView.DataSource = subj_;

            DataTable tbl = ToDataTable(dsSemestr);
            semestrGridView.DataSource = tbl;

            goodGridView.DataSource = good;

            tbl = ToDataTable(dsEvent);
            tbl.DefaultView.Sort = "Class_, Subj, dt";
            dataGridView1.Columns.Clear();
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = tbl;
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                if (col.Name == "ClassNum") col.Visible = false;
                if (col.Name == "Tutor") col.HeaderText = "Учитель";
                if (col.Name == "Subj") col.HeaderText = "Предмет";
                if (col.Name == "Class_") col.HeaderText = "Класс";
                if (col.Name == "dt") col.HeaderText = "Дата";
                if (col.Name == "event_") col.HeaderText = "Ошибка";
                if (col.Name == "Student") col.HeaderText = "Ученик";
            }

            toolStripProgressBar1.Value = 0;
        }

        private void shcool_SelectedIndexChanged(object sender, EventArgs e)
        {
            string filter = "";
            if (shcool.SelectedIndex > 0)
            {
                if (shcool.SelectedIndex == 1) filter += " ClassNum < 5 ";
                if (shcool.SelectedIndex == 2) filter += " ((ClassNum > 4) AND (ClassNum < 10)) ";
                if (shcool.SelectedIndex == 3) filter += " ClassNum > 9 ";
            }

            DataTable tbl = ToDataTable(dsEvent);
            tbl.DefaultView.Sort = "Class_, Subj, dt";
            tbl.DefaultView.RowFilter = filter;
            dataGridView1.Columns.Clear();
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = tbl;
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                if (col.Name == "ClassNum") col.Visible = false;
                if (col.Name == "Tutor") col.HeaderText = "Учитель";
                if (col.Name == "Subj") col.HeaderText = "Предмет";
                if (col.Name == "Class_") col.HeaderText = "Класс";
                if (col.Name == "dt") col.HeaderText = "Дата";
                if (col.Name == "event_") col.HeaderText = "Ошибка";
                if (col.Name == "Student") col.HeaderText = "Ученик";
            }
            dataGridView1.Refresh();

            tbl = ToDataTable(dsPlan);
            tbl.DefaultView.Sort = "Class_, Subj, dt";
            tbl.DefaultView.RowFilter = filter;
            planGridView.Columns.Clear();
            planGridView.AutoGenerateColumns = true;
            planGridView.DataSource = tbl;
            foreach (DataGridViewColumn col in planGridView.Columns)
            {
                if (col.Name == "ClassNum") col.Visible = false;
                if (col.Name == "Tutor") col.HeaderText = "Учитель";
                if (col.Name == "Subj") col.HeaderText = "Предмет";
                if (col.Name == "Class_") col.HeaderText = "Класс";
                if (col.Name == "dt") col.HeaderText = "Дата";
                if (col.Name == "event_") col.HeaderText = "Ошибка";
                if (col.Name == "Student") col.HeaderText = "Ученик";
            }
            planGridView.Refresh();

            AdvancedDataGridView gv = (AdvancedDataGridView)semestrGridView;
            DataTable tb = (DataTable)gv.DataSource;
            tb.DefaultView.RowFilter = filter;
            gv.Refresh();

        }
        public bool getFilter(Event_ obj)
        {
            bool ret = true;

            if (shcool.SelectedIndex > 0)
            {
                if (shcool.SelectedIndex == 1) ret = obj.ClassNum < 5;
                if (shcool.SelectedIndex == 2) ret = (obj.ClassNum > 4) && (obj.ClassNum < 10);
                if (shcool.SelectedIndex == 3) ret = obj.ClassNum > 9;
            }

            return (ret);
        }

        private void Bind()
        {
            int posTable = -1;
            foreach (DataTable tbl in result.Tables)
            {
                DataRow[] dr = tbl.Select();
                string tblName = tbl.TableName;
                DateTime currDT = TConvert.ToDateTime(DateTime.Now.ToString("dd.MM.yyyy"));
                DateTime dt2 = DateTime.Now;

                int pos = 0;
                int ClassNum = 0;
                int firstTrimestr = -1;
                int sumTrimestr = -1;
                int cntTrimestr = 0;
                int cntTrimestrNone = 0;    // Количество пропущенных уроков
                int cntTrimestrAll = 0;     // Общее количество уроков
                bool isErrors = false;
                string str0 = "", str1 = "", str2 = "", str3 = "", class_ = "", subj = "", tutorName = "", year = "";
                string[] mTutor = null;
                List<string> mTutor_ = new List<string>();

                totJournal++;
                posTable++;
                while (pos < dr.Length)
                {
                    if ((TConvert.ToString(dr[pos][0]) != "") && (TConvert.ToString(dr[pos][1]) == ""))
                    {
                        // Читаем заголовок
                        while ((str0 == "") || (str1 == "") || (str2 == "") || (str3 == ""))
                        {
                            string str = TConvert.ToString(dr[pos][0]);
                            if (str.ToUpper().IndexOf("КЛАСС") != -1)
                            {
                                str0 = str.Split(':')[1];
                                class_ = str0.Split('(')[0];
                                ClassNum = TConvert.ToInt(Regex.Replace(class_, "[^0-9]", ""));
                                //if (ClassNum < 2) return;
                            }
                            if (str.ToUpper().IndexOf("ГОД") != -1)
                            {
                                str1 = dr[1][0].ToString().Split(':')[1].Split('/')[0];
                                year = str1;
                            }
                            if (str.ToUpper().IndexOf("ПРЕДМЕТ") != -1)
                            {
                                subj = str.Split(':')[1];
                                str2 = subj;
                            }
                            if (str.ToUpper().IndexOf("ФАМИЛИЯ") != -1)
                            {
                                tutorName = str.Split(':')[1];
                                str3 = tutorName;
                                mTutor = tutorName.Trim().Split(',');
                                tutorName = "";
                                foreach (string s in mTutor)
                                {
                                    string[] mFio = s.Trim().Split(' ');
                                    string one = mFio[2] + " " + mFio[0].Substring(0, 1) + "." + mFio[1].Substring(0, 1) + ".";
                                    mTutor_.Add(one);
                                    tutorName += ((tutorName != "") ? ", " : "") + one;
                                }
                            }
                            pos++;
                            if (pos == dr.Length) break;
                        }
                    }
                    if ((str0 != "") && (str1 != "") && (str2 != "") && (str3 != "") && (TConvert.ToString(dr[pos][0]).IndexOf("№") != -1)) break;
                    pos++;
                }
                if (pos >= dr.Length)
                    continue;
                dsSubject.Add(new Subject { Name = subj, Tutor = tutorName, Table = tbl.TableName, koeff = 0.5 });
                // Добавляем список преподавателей
                foreach (string s in mTutor_)
                {
                    if (good.getByID(s) == null) good.Add(new ItemRoot { ID = s });
                }
                // Добавляем предметы
                if (subj_.getByID(subj) == null)
                {
                    subj_.Add(new ItemRoot { ID = subj, Koeff = 0.5 });
                }

                if (ClassNum > 1)
                {
                    cntJournal++;
                    //Определяем даты занятий 
                    RootCollection rc = new RootCollection();
                    RootCollection rct = new RootCollection();
                    DataRow r0 = dr[pos];
                    int currMonth = -1;
                    int month_ = -1;
                    bool isSemestr = false;
                    int oldDay = 0;
                    int cntDay = 0;
                    string sdt = "";
                    for (int i = 2; i < r0.ItemArray.Length; i++)
                    {
                        if (TConvert.ToString(r0[i]) != "")
                        {
                            month_ = getMonth(TConvert.ToString(r0[i]));
                            if ((TConvert.ToString(r0[i]).ToUpper().IndexOf("ТР") != -1) || (TConvert.ToString(r0[i]).ToUpper().IndexOf("СЕМ") != -1))
                            {
                                rct.Add(new ItemRoot { ID = i.ToString(), def = TConvert.ToString(r0[i]).ToUpper(), FullName = str0, Comment = tblName, FieldStat = str3 });
                                rc[rc.Count - 1].FullName = "1";
                                isSemestr = true;
                                firstTrimestr = -1;
                            }
                            else
                            {
                                if ((month_ < currMonth) && (month_ != -1))
                                {
                                    year = (TConvert.ToInt(year) + 1).ToString();
                                }
                                if (month_ != -1)
                                {
                                    if (firstTrimestr == -1) firstTrimestr = i;
                                }
                                currMonth = month_;
                                isSemestr = false;
                            }
                            month_ = getMonth(TConvert.ToString(r0[i]));
                        }
                        if ((currMonth != -1)&&(!isSemestr))
                        {
                            int newDay = TConvert.ToInt(dr[7][i]);
                            if ((oldDay == 0)) oldDay = newDay;
                            if (oldDay != newDay)
                            {
                                if ((cntDay > 0) && (cntDay < 3))
                                {
                                    dsEvent.Add(new Event_ { Tutor = tutorName, Subj = subj, Class_ = class_, ClassNum = ClassNum, dt = TConvert.ToDateTime(sdt), event_ = "Низкая накопляемость оценок" });
                                    isErrors = true;
                                }
                                if (cntDay == 0)
                                {
                                    dsEvent.Add(new Event_ { Tutor = tutorName, Subj = subj, Class_ = class_, ClassNum = ClassNum, dt = TConvert.ToDateTime(String.Format("{0}.{1}.{2}", dr[7][i].ToString(), (currMonth + 1).ToString(), year)), event_ = "Несвоевременное выставление оценки" });
                                    isErrors = true;
                                }
                                cntDay = 0;
                            }

                            sdt = String.Format("{0}.{1}.{2}", newDay, currMonth + 1, TConvert.ToInt(year));
                            if ((year == currDT.Year.ToString()) && (currMonth + 1 == currDT.Month))
                                if (TConvert.ToDateTime(sdt) > currDT)
                                    break;
                            rc.Add(new ItemRoot
                            {
                                ID = sdt,
                                def = i.ToString(),
                                Name = (firstTrimestr > 0) ? "1" : "",
                                FullName = str0,
                                Comment = tblName,
                                FieldStat = tutorName,
                                Subj = str2,
                                posResults = mResult.Count - 1,
                                posTables = posTable,
                                lineBegin = pos + 3
                            });
                            firstTrimestr = -2;

                            ////////////////////////////////////////////////////////////////
                            // Проверяем 3 отценки за занятие
                            ////////////////////////////////////////////////////////////////
                            int cnt = 0;
                            for (int j = 8; j < dr.Length; j++)
                            {
                                string cell = dr[j][i].ToString();
                                if (cell.ToUpper().IndexOf("Н") == -1)
                                {
                                    foreach (string s in dr[j][i].ToString().Split('/'))
                                    {
                                        if (TConvert.ToInt(s) > 0)
                                        {
                                            cnt++;
                                            cntDay++;
                                        }
                                    }
                                }
                                else
                                {
                                    if (cell.Length > 1)
                                    {
                                        dsEvent.Add(new Event_ { Tutor = tutorName, Subj = subj, Class_ = class_, ClassNum = ClassNum, dt = TConvert.ToDateTime(sdt), event_ = "Не правильное оформление отсутствия на уроке" });
                                        isErrors = true;
                                    }
                                }
                            }

                        }
                    }
                    listLession.Add(rc);

                    for (int j = pos+3; j < dr.Length; j++)
                    {
                        bool isN = false, is2 = false;
                        int cnt2 = 0;

                        for (int i = 0; i < rc.Count; i++)
                        {
                            string cell = dr[j][TConvert.ToInt(rc[i].def)].ToString();
                            ////////////////////////////////////////////////////////////////
                            // Считаем баллы за триместр/семестр
                            ////////////////////////////////////////////////////////////////
                            if (rc[i].Name == "1")
                            {
                                sumTrimestr = 0;
                                cntTrimestr = 0;
                                cntTrimestrNone = 0;
                                cntTrimestrAll = 0;
                            }
                            cntTrimestrAll++;
                            if ((cell.ToUpper().IndexOf("Н") != -1) || (cell.ToUpper().IndexOf("Б") != -1) || (cell.ToUpper().IndexOf("П") != -1) || (cell.ToUpper().IndexOf("О") != -1)) cntTrimestrNone++;
                            if (cell.ToUpper().IndexOf("Н") != -1) isN = true;
                            else
                            {
                                ////////////////////////////////////////////////////////////////
                                // Считаем баллы за триместр/семестр
                                ////////////////////////////////////////////////////////////////
                                string[] mBalls = cell.Split('/');
                                foreach (string s in mBalls)
                                {
                                    int val = TConvert.ToInt(Regex.Replace(s, "[^0-9]", ""));
                                    if (val > 0)
                                    {
                                        sumTrimestr += val;
                                        cntTrimestr++;
                                    }
                                }
                                ////////////////////////////////////////////////////////////////
                                // Проверяем 2-ку после "н"
                                ////////////////////////////////////////////////////////////////
                                if (isN && (cell.IndexOf("2") != -1))
                                {
                                    dsEvent.Add(new Event_ { Tutor = tutorName, Subj = subj, Class_ = class_, ClassNum = ClassNum, dt = TConvert.ToDateTime(rc[i].ID), event_ = "После отсутствия на занятии поставлена оценка 2", Student = dr[j][1].ToString() });
                                    isErrors = true;
                                }
                                isN = false;
                                ////////////////////////////////////////////////////////////////
                                // Проверяем 2-ку в начале и в конце
                                ////////////////////////////////////////////////////////////////
                                if (cell.IndexOf("2") != -1)
                                {
                                    if (rc[i].Name == "1")
                                    {
                                        dsEvent.Add(new Event_ { Tutor = tutorName, Subj = subj, Class_ = class_, ClassNum = ClassNum, dt = TConvert.ToDateTime(rc[i].ID), event_ = "Оценка 2 в начале триместра(семестра)", Student = dr[j][1].ToString() });
                                        isErrors = true;
                                    }
                                    if (rc[i].FullName == "1")
                                    {
                                        dsEvent.Add(new Event_ { Tutor = tutorName, Subj = subj, Class_ = class_, ClassNum = ClassNum, dt = TConvert.ToDateTime(rc[i].ID), event_ = "Оценка 2 в конце триместра(семестра)", Student = dr[j][1].ToString() });
                                        isErrors = true;
                                    }
                                }
                                ////////////////////////////////////////////////////////////////
                                // Проверяем через сколько исправили 2-ку
                                ////////////////////////////////////////////////////////////////
                                if (is2)
                                {
                                    if (cell == "") cnt2++;
                                    else
                                    {
                                        is2 = false;
                                        cnt2 = 0;
                                    }
                                    if (cnt2 == 3)
                                    {
                                        dsEvent.Add(new Event_ { Tutor = tutorName, Subj = subj, Class_ = class_, ClassNum = ClassNum, dt = dt2, event_ = "Оценка 2 не исправлена за 3 занятия", Student = dr[j][1].ToString() });
                                        isErrors = true;
                                        is2 = false;
                                    }
                                }
                                if (cell.IndexOf("2") != -1) { is2 = true; cnt2 = 0; dt2 = TConvert.ToDateTime(rc[i].ID); }
                            }
                            ////////////////////////////////////////////////////////////////
                            // Считаем баллы за триместр/семестр
                            ////////////////////////////////////////////////////////////////
                            if (rc[i].FullName == "1")
                            {
                                int valitog = TConvert.ToInt(Regex.Replace(TConvert.ToString(dr[j][TConvert.ToInt(rc[i].def) + 1]), "[^0-9]", ""));
                                if (valitog > 0)
                                {
                                    ItemRoot ir = rct.getByID((TConvert.ToInt(rc[i].def) + 1).ToString());
                                    ////////////////////////////////////////////////////////////////
                                    // Проверяем средний балл за семестр/триместр
                                    ////////////////////////////////////////////////////////////////
                                    if (sumTrimestr > 0)
                                    {
                                        double calc = (cntTrimestr != 0) ? TConvert.ToDouble(sumTrimestr) / TConvert.ToDouble(cntTrimestr) : 0;
                                        // Округляем
                                        int num = (int)calc;
                                        double fract = calc - num;
                                        if (fract >= Koeff) num++;
                                        if (num != valitog)
                                        {
                                            dsSemestr.Add(new Semestr_ { Tutor = tutorName, Subj = subj, Class_ = class_, ClassNum = ClassNum, Name = ir.def, event_ = "Не верный средний бал", Student = dr[j][1].ToString(), avg = calc, ball = valitog });
                                            isErrors = true;
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////
                                    // Проверяем количество оценок за семестр/триместр
                                    ////////////////////////////////////////////////////////////////
                                    if (((cntTrimestr < 3) && (ir.def.ToUpper().IndexOf("ТР") != -1)) || ((cntTrimestr < 5) && (ir.def.ToUpper().IndexOf("СЕМ") != -1)))
                                    {
                                        double calc = (cntTrimestr != 0) ? TConvert.ToDouble(sumTrimestr) / TConvert.ToDouble(cntTrimestr) : 0;
                                        dsSemestr.Add(new Semestr_ { Tutor = tutorName, Subj = subj, Class_ = class_, ClassNum = ClassNum, Name = ir.def, event_ = "Итоговая оценка выставлена менее, чем порог(3ТР, 5СЕМ)", Student = dr[j][1].ToString(), avg = calc, ball = valitog, cnt = cntTrimestr.ToString() });
                                        isErrors = true;
                                    }
                                    ////////////////////////////////////////////////////////////////
                                    // Проверяем пропущено/было фактически за семестр/триместр (2/3)
                                    ////////////////////////////////////////////////////////////////
                                    if (cntTrimestrNone > (cntTrimestrAll * 2 / 3))
                                    {
                                        dsSemestr.Add(new Semestr_ { Tutor = tutorName, Subj = subj, Class_ = class_, ClassNum = ClassNum, Name = ir.def, event_ = "Количество пропусков уроков за триместр/семестр превышает 2/3 ", Student = dr[j][1].ToString(), skip = cntTrimestrNone.ToString() + "/" + cntTrimestrAll.ToString() });
                                        isErrors = true;
                                    }
                                }

                            }

                        }
                    }

                    if (isErrors)
                    {
                        foreach (string s in mTutor)
                        {
                            string[] mFio = s.Trim().Split(' ');
                            string one = mFio[2] + " " + mFio[0].Substring(0, 1) + "." + mFio[1].Substring(0, 1) + ".";
                            ItemRoot ir = good.getByID(one);
                            if (ir != null)
                            {
                                good.Remove(good.GetIndex(ir.ID));
                            }
                        }
                    }
                }
                else listLession.Add(new RootCollection());
            }
        }
        private int getMonth(string month)
        {
            if (month.ToUpper().IndexOf("ЯНВ") != -1) return 0;
            if (month.ToUpper().IndexOf("ФЕВ") != -1) return 1;
            if (month.ToUpper().IndexOf("МАР") != -1) return 2;
            if (month.ToUpper().IndexOf("АПР") != -1) return 3;
            if (month.ToUpper().IndexOf("МАЙ") != -1) return 4;
            if (month.ToUpper().IndexOf("ИЮН") != -1) return 5;
            if (month.ToUpper().IndexOf("ИЮЛ") != -1) return 6;
            if (month.ToUpper().IndexOf("АВГ") != -1) return 7;
            if (month.ToUpper().IndexOf("СЕН") != -1) return 8;
            if (month.ToUpper().IndexOf("ОКТ") != -1) return 9;
            if (month.ToUpper().IndexOf("НОЯ") != -1) return 10;
            if (month.ToUpper().IndexOf("ДЕК") != -1) return 11;
            return -1;
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

        private void Form1_Shown(object sender, EventArgs e)
        {
            openToolStripMenuItem1_Click(null, null);

            //AskForm form = new AskForm();
            //form.ShowDialog();

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsNumber(e.KeyChar) && e.KeyChar != (Char)Keys.Back)
            {
                e.Handled = true;

            }
        }

        private void koeffGridView_SortStringChanged(object sender, EventArgs e)
        {
            AdvancedDataGridView gv = (AdvancedDataGridView)sender;
            DataTable tbl = (DataTable)gv.DataSource;
            tbl.DefaultView.Sort = gv.SortString;
            gv.Refresh();
        }

        private void koeffGridView_FilterStringChanged(object sender, EventArgs e)
        {
            AdvancedDataGridView gv = (AdvancedDataGridView)sender;
            DataTable tbl = (DataTable)gv.DataSource;
            tbl.DefaultView.RowFilter = gv.FilterString;
            gv.Refresh();
        }

        /// /////////////////////////////////////////////////////////////////
        // ПЛАНИРОВАНИЕ
        /////////////////////////////////////////////////////////////////////
        private void itmOpenPlan_Click(object sender, EventArgs e)
        {
            // Выбор архива
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                cntPlan = 0;
                totPlan = 0;
                dsPlan.Clear();
                toolStripProgressBar1.Value = 0;
                // Загрузка
                this.Cursor = Cursors.WaitCursor;
                int cnt = 0;
                this.Cursor = Cursors.WaitCursor;
                Ionic.Zip.ZipFile zf = new Ionic.Zip.ZipFile(openFileDialog1.FileName);
                foreach (ZipEntry zipEntry in zf)
                {
                    if (zipEntry.FileName.IndexOf(".xls") != -1)
                    {
                        MemoryStream zipMs = new MemoryStream();
                        zipEntry.Extract(zipMs);
                        zipMs.Seek(0, SeekOrigin.Begin);
                        // Загрузка и преобразование Exel к DataSet
                        using (var reader = ExcelReaderFactory.CreateReader(zipMs))
                        {
                            resultPlan = reader.AsDataSet();
                            // Разбор по локальным таблицам
                            BindPlan();
                        }
                        zipMs.Dispose();

                        cnt++;
                        toolStripProgressBar1.Value = 100 * cnt / zf.Count;
                        Application.DoEvents();
                    }
                }
            }
            this.Cursor = Cursors.Default;
            toolStripProgressBar1.Value = 0;
            lblPlan.Text = String.Format("{0}/{1}", cntPlan, totPlan);

            shcool_SelectedIndexChanged(null, null);
            goodGridView.DataSource = good;
            goodGridView.Refresh();

        }
        private void BindPlan()
        {
            DateTime currDT = DateTime.Now;
            DateTime sdt;
            bool isErrors = false;
            bool isFirst = false;
            string[] mTutor = { };
            foreach (DataTable tbl in resultPlan.Tables)
            {
                DataRow[] dr = tbl.Select();
                int pos = 0, poslist = 0;
                RootCollection currItem = null;
                DataRow[] currTable = null;
                totPlan++;
                while (pos < dr.Length)
                {
                    if ((TConvert.ToString(dr[pos][0]) != "") && (TConvert.ToString(dr[pos][1]) == ""))
                    {
                        string str0 = "", str2 = "", str3 = "", class_ = "", subj = "", tutorName = "";
                        int ClassNum = 0;
                        currItem = null;
                        while ((str0=="")|| (str2 == "")|| (str3 == ""))
                        {
                            string str = TConvert.ToString(dr[pos][0]);
                            if (str.ToUpper().IndexOf("КЛАСС") != -1)
                            {
                                // Читаем заголовок
                                str0 = str.Split(':')[1];
                                class_ = str0.Split('(')[0];
                                ClassNum = TConvert.ToInt(Regex.Replace(class_, "[^0-9]", ""));
                                if (ClassNum < 2) return;
                            }
                            if (str.ToUpper().IndexOf("ПРЕДМЕТ") != -1)
                            {
                                subj = str.Split(':')[1];
                                str2 = subj;
                            }
                            if (str.ToUpper().IndexOf("ФИО") != -1)
                            {
                                tutorName = str.Split(':')[1];
                                str3 = tutorName;
                                mTutor = tutorName.Trim().Split(',');
                                tutorName = "";
                                foreach (string s in mTutor)
                                {
                                    string[] mFio = s.Trim().Split(' ');
                                    string one = mFio[2] + " " + mFio[0].Substring(0, 1) + "." + mFio[1].Substring(0, 1) + ".";
                                    tutorName += ((tutorName != "") ? ", " : "") + one;
                                }
                            }
                            pos++;
                            if (pos == dr.Length) break;
                        }
                        foreach (RootCollection rc in listLession)
                        {
                            if (rc.Count > 0)
                                if ((str0.IndexOf(rc[0].FullName.Trim()) != -1) && (rc[0].Comment.IndexOf(str2.Trim()) != -1)) // && (rc[0].FieldStat.Trim() == str3.Trim()))
                                {
                                    cntPlan++;
                                    currItem = rc;
                                    poslist = listLession.IndexOf(currItem);
                                    currTable = mResult[rc[0].posResults].Tables[rc[0].posTables].Select();
                                    isFirst = true;
                                    break;
                                }
                        }
                        continue;
                    }
                    if (TConvert.ToString(dr[pos][0]) != "")
                    {
                        if (currItem != null)
                        {
                            string s = TConvert.ToString(dr[pos][0]);
                            if((s.Substring(2,1)==".")&& (s.Substring(5, 1) == "."))
                            {
                                sdt = TConvert.ToDateTime(dr[pos][0]);
                                if (sdt <= currDT)
                                {
                                    ItemRoot ir = currItem.getByID(String.Format("{0}.{1}.{2}", sdt.Day, sdt.Month, sdt.Year));

                                    string class_ = currItem[0].FullName.Split('(')[0];
                                    int ClassNum = TConvert.ToInt(Regex.Replace(class_, "[^0-9]", ""));

                                    string str = TConvert.ToString(dr[pos][2]);
                                    if (str.Length <= 3)
                                    {
                                        dsPlan.Add(new Plan
                                        { 
                                            Tutor = currItem[0].FieldStat,
                                            Subj = currItem[0].Subj,
                                            Class_ = class_,
                                            ClassNum = ClassNum,
                                            dt = sdt,
                                            event_ = "Нет домашнего задания"
                                        });
                                        isErrors = true;
                                    }
                                    str = TConvert.ToString(dr[pos][1]).ToLower();
                                    if (str.Length <= 3)
                                    {
                                        dsPlan.Add(new Plan
                                        {
                                            Tutor = currItem[0].FieldStat,
                                            Subj = currItem[0].Subj,
                                            Class_ = class_,
                                            ClassNum = ClassNum,
                                            dt = sdt,
                                            event_ = "Нет темы занятия"
                                        });
                                        isErrors = true;
                                    }
                                    if (isFirst)
                                    {
                                        if ((str.IndexOf("инструктаж") == -1) && (str.IndexOf("охрана труда") == -1) && (str.IndexOf("по охране труда") == -1))
                                        {
                                            dsPlan.Add(new Plan
                                            {
                                                Tutor = currItem[0].FieldStat,
                                                Subj = currItem[0].Subj,
                                                Class_ = class_,
                                                ClassNum = ClassNum,
                                                dt = sdt,
                                                event_ = "Нет отметки инструктажа"
                                            });
                                            isErrors = true;
                                        }
                                        isFirst = false;
                                    }else
                                    {
                                        if (TConvert.ToDateTime(dr[pos - 1][0]).Year < sdt.Year)
                                        {
                                            if ((str.IndexOf("инструктаж") == -1) && (str.IndexOf("охрана труда") == -1) && (str.IndexOf("по охране труда") == -1))
                                            {
                                                dsPlan.Add(new Plan
                                                {
                                                    Tutor = currItem[0].FieldStat,
                                                    Subj = currItem[0].Subj,
                                                    Class_ = class_,
                                                    ClassNum = ClassNum,
                                                    dt = sdt,
                                                    event_ = "Нет отметки инструктажа"
                                                });
                                                isErrors = true;
                                            }
                                        }
                                    }
                                    for (int i= currItem[0].lineBegin; i<currTable.Length; i++)
                                    {
                                        if ((str.IndexOf("диктант") != -1) || (str.IndexOf("контрольная") != -1) || (str.IndexOf("лабораторная") != -1) || (str.IndexOf("проверочная") != -1) ||
                                            (str.IndexOf("практическая") != -1) || (str.IndexOf("самостоятельная") != -1) || (str.IndexOf("тест") != -1) || (str.IndexOf("изложение") != -1) || (str.IndexOf("сочинение") != -1))
                                        {
                                            str = TConvert.ToString(currTable[i][TConvert.ToInt(ir.def)]);
                                            if (str == "")
                                            {
                                                dsPlan.Add(new Plan
                                                {
                                                    Tutor = currItem[0].FieldStat,
                                                    Subj = currItem[0].Subj,
                                                    Class_ = class_,
                                                    ClassNum = ClassNum,
                                                    dt = sdt,
                                                    event_ = "Нет оценки за контрольное занятие",
                                                    Student = TConvert.ToString(currTable[i][1])
                                                });
                                                isErrors = true;
                                            }
                                        }
                                    }

                                    if (isErrors)
                                    {
                                        foreach (string s_ in mTutor)
                                        {
                                            string[] mFio = s_.Trim().Split(' ');
                                            ItemRoot ir_ = good.getByID(currItem[0].FieldStat);
                                            if (ir != null)
                                            {
                                                good.Remove(good.GetIndex(ir.ID));
                                            }
                                        }
                                        isErrors = false;
                                    }

                                }
                            }
                        }
                    }
                    pos++;
                }
            }
        }
        private void настроитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Report report = new Report();
            //report.Load()

            DataSet ds = new DataSet();

            DataTable tbl = (DataTable)dataGridView1.DataSource;
            tbl.TableName = "Journal";
            ds.Tables.Add(tbl);

            tbl = (DataTable)semestrGridView.DataSource;
            tbl.TableName = "Semestr";
            ds.Tables.Add(tbl);

            tbl = (DataTable)planGridView.DataSource;
            tbl.TableName = "Plan";
            ds.Tables.Add(tbl);

            report.RegisterData(ds);
            report.GetDataSource("Journal").Enabled = true;
            report.GetDataSource("Semestr").Enabled = true;
            report.GetDataSource("Plan").Enabled = true;
            report.AutoFillDataSet = true;

            report.SetParameterValue("Number", "1");

            ReportPage page = new ReportPage();

            report.Pages.Add(page);

            page.CreateUniqueName();

            report.Design();

            report.Dispose();
            ds.Dispose();
        }

        private void показатьToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

    }
}
