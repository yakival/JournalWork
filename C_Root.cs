using System;
using System.IO;
using System.Collections;
using System.Data;
using System.Net;
using System.Web;
using System.Configuration;


namespace JournalWork
{
    interface IRootCollection
	{
	}
	/// <summary>
	/// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// //ОПИСАНИЕ УНИФИЦИРОВАННОГО СПРАВОЧНИКА
	/// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// </summary>
    [Serializable]
    public class RootCollection : System.Collections.CollectionBase, IRootCollection
	{
        public class RootSorter : IComparer
        {
            public int Compare(Object x, Object y)
            {
                ItemRoot p1 = (ItemRoot)x;
                IComparable ic1 = (IComparable)String.Format("{0} {1}", p1.Name, p1.FullName);

                ItemRoot p2 = (ItemRoot)y;
                IComparable ic2 = (IComparable)String.Format("{0} {1}", p2.Name, p2.FullName);

                return ic1.CompareTo(ic2);
            }
        }
        public class RootSorterStat : IComparer
        {
            public int Compare(Object x, Object y)
            {
                ItemRoot p1 = (ItemRoot)x;
                IComparable ic1 = (IComparable)String.Format("{0}", p1.FieldStat);

                ItemRoot p2 = (ItemRoot)y;
                IComparable ic2 = (IComparable)String.Format("{0}", p2.FieldStat);

                return ic1.CompareTo(ic2);
            }
        }
        
        public RootCollection()
		{
		}

		public void Add(ItemRoot aItem)
		{
			List.Add(aItem);
		}
		public void Remove(int index)
		{
			if (index > Count - 1 || index < 0)
			{
			}
			else
			{
				List.RemoveAt(index); 
			}
		}
		public ItemRoot this[int Index]
		{
			get
			{
				if (Index > List.Count - 1 || Index < 0)
				{
					return null;
				}
				else
				{
					return (ItemRoot) List[Index];
				}
			}
		}
        public int GetIndex(string ID)
        {
            for (int i = 0; i < List.Count; i++)
            {
                if (((ItemRoot)List[i]).ID == ID)
                {
                    return i;
                }
            }
            return -1;
        }
        //Выборка информации по справочнику
		public string getListID()
		{
			string ret="";
			foreach(ItemRoot ie in List)
			{
				ret+=ie.ID.ToString()+",";
			}
			ret=ret.Substring(0,ret.Length-1);
			return ret;
		}
		public ItemRoot getByID(string aID)
		{
			ItemRoot ret=null;
			foreach( ItemRoot r in List )
			{
				if(r.ID==aID)
				{ 
					ret=(ItemRoot) r; 
					return ret;
				}
			}
			return ret;
		}
        public ItemRoot getByName(string aName)
        {
            ItemRoot ret = null;
            foreach (ItemRoot r in List)
            {
                if (r.Name == aName)
                {
                    ret = (ItemRoot)r;
                    return ret;
                }
            }
            return ret;
        }
        public void Sort()
        {
            IComparer NameSorter = new RootSorter();
            InnerList.Sort(NameSorter);
        }
        public void SortStat()
        {
            IComparer NameSorter = new RootSorterStat();
            InnerList.Sort(NameSorter);
        }
    }



    [Serializable]
    public class ItemRoot
    {
        private string FID;
        private string FName;
        private string FFName;

        public string ID { get { return FID; } set { FID = value; } }
        public string Name { get { return FName; } set { FName = value; } }
        public string FullName { get { return FFName; } set { FFName = value; } }
        public string Comment { get; set; }
        public string FieldStat { get; set; }
        public string def { get; set; }
        public int posResults { get; set; }
        public int posTables { get; set; }
        public int lineBegin { get; set; }
        public string Subj { get; set; }
        public double Koeff { get; set; }

        public ItemRoot()
        {
            FName = "";
            FFName = "";
            Comment = "";
            FieldStat = "";
            def = "";
            posResults = 0;
            posTables = 0;
            lineBegin = 0;
            Koeff = 0;
        }

        public bool Fill(string aID, string aName, string aFullName)
        {
            if (aID == "") return false;
            ID = aID;
            Name = aName;
            FullName = aFullName;
            return true;
        }

        /*
        public bool Fill( long aID )
        {
            if( aID==0 ) return false;
            DataSet ds = dbproxy.ExecuteDataSet( dbproxy.db, "SELECT * FROM tDic WHERE idDic= "+aID.ToString() );
            DataRow[] dr = ds.Tables[0].Select("","");
            foreach( DataRow r in dr )
            {
                ID=TConvert.ToLong(r["idDic"]);
                Name=r["dicName"].ToString();
                FullName=r["DicExt"].ToString();
                Hidden=(bool) (TConvert.ToLong(r["H"])>0);
                ReadOnly=(bool) (TConvert.ToLong(r["R"])>0);
                Sys=(bool) (TConvert.ToLong(r["S"])>0);
            }
            return true;
        }
         */

    }



}
