/**  版本信息模板在安装目录下，可自行修改。
* GROUPBYPROPERTY.cs
*
* 功 能： N/A
* 类 名： GROUPBYPROPERTY
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2016/12/27 16:09:44   N/A    初版
*
* Copyright (c) 2012 Maticsoft Corporation. All rights reserved.
*┌──────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
*│　版权所有：动软卓越（北京）科技有限公司　　　　　　　　　　　　　　│
*└──────────────────────────────────┘
*/
using System;
namespace Model
{
	/// <summary>
	/// GROUPBYPROPERTY:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class GROUPBYPROPERTY
	{
		public GROUPBYPROPERTY()
		{}
		#region Model
		private decimal _allhouseholds;
		private decimal _allpopulation;
		private decimal _poorhouseholds;
		private decimal _poorpopulation;
		private decimal _poorlowhouseholds;
		private decimal _poorlowpopulation;
		private decimal _insuredhouseholds;
		private decimal _insuredpopulation;
		private decimal _fivepoorhouseholds;
		private decimal _fivepoorpopulation;
		private decimal _rid;
		private string _aad105;
		/// <summary>
		/// 
		/// </summary>
		public decimal ALLHOUSEHOLDS
		{
			set{ _allhouseholds=value;}
			get{return _allhouseholds;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal ALLPOPULATION
		{
			set{ _allpopulation=value;}
			get{return _allpopulation;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal POORHOUSEHOLDS
		{
			set{ _poorhouseholds=value;}
			get{return _poorhouseholds;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal POORPOPULATION
		{
			set{ _poorpopulation=value;}
			get{return _poorpopulation;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal POORLOWHOUSEHOLDS
		{
			set{ _poorlowhouseholds=value;}
			get{return _poorlowhouseholds;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal POORLOWPOPULATION
		{
			set{ _poorlowpopulation=value;}
			get{return _poorlowpopulation;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal INSUREDHOUSEHOLDS
		{
			set{ _insuredhouseholds=value;}
			get{return _insuredhouseholds;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal INSUREDPOPULATION
		{
			set{ _insuredpopulation=value;}
			get{return _insuredpopulation;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal FIVEPOORHOUSEHOLDS
		{
			set{ _fivepoorhouseholds=value;}
			get{return _fivepoorhouseholds;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal FIVEPOORPOPULATION
		{
			set{ _fivepoorpopulation=value;}
			get{return _fivepoorpopulation;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal RID
		{
			set{ _rid=value;}
			get{return _rid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string AAD105
		{
			set{ _aad105=value;}
			get{return _aad105;}
		}
		#endregion Model

	}
}

