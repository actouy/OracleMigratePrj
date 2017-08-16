/**  版本信息模板在安装目录下，可自行修改。
* GROUPBYFITNESS.cs
*
* 功 能： N/A
* 类 名： GROUPBYFITNESS
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2016/12/27 16:09:50   N/A    初版
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
	/// GROUPBYFITNESS:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class GROUPBYFITNESS
	{
		public GROUPBYFITNESS()
		{}
		#region Model
		private decimal _allpopulation;
		private decimal _goodhealth;
		private decimal _chronicailment;
		private decimal _seriousillness;
		private decimal _disability;
		private decimal _rid;
		private string _aad105;
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
		public decimal GOODHEALTH
		{
			set{ _goodhealth=value;}
			get{return _goodhealth;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal CHRONICAILMENT
		{
			set{ _chronicailment=value;}
			get{return _chronicailment;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal SERIOUSILLNESS
		{
			set{ _seriousillness=value;}
			get{return _seriousillness;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal DISABILITY
		{
			set{ _disability=value;}
			get{return _disability;}
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

