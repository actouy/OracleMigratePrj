using System;

namespace Model
{
    [Serializable]
    public partial class GROUPREASON
    {
        public GROUPREASON()
        {
            _pkid = System.Guid.NewGuid().ToString();
        }
        #region Model
        private decimal _regionid;
        private string _reason;
        private decimal _num;
        private string _poorstandard;
        private string _outtype;
        private decimal _pop;
        private string _pkid;
        /// <summary>
        /// 
        /// </summary>
        public decimal REGIONID
        {
            set { _regionid = value; }
            get { return _regionid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string REASON
        {
            set { _reason = value; }
            get { return _reason; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal NUM
        {
            set { _num = value; }
            get { return _num; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string POORSTANDARD
        {
            set { _poorstandard = value; }
            get { return _poorstandard; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string OUTTYPE
        {
            set { _outtype = value; }
            get { return _outtype; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal POP
        {
            set { _pop = value; }
            get { return _pop; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string PKID
        {
            get 
            {
                //_pkid = new Guid().ToString(); 
                return _pkid; 
            }
        }
        #endregion Model

    }
}
