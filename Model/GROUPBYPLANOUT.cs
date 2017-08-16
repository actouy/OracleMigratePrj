/**  版本信息模板在安装目录下，可自行修改。
* GROUPBYPLANOUT.cs
*
* 功 能： N/A
* 类 名： GROUPBYPLANOUT
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2016/12/27 16:09:45   N/A    初版
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
	/// GROUPBYPLANOUT:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class GROUPBYPLANOUT
	{
		public GROUPBYPLANOUT()
		{}
		#region Model
		private decimal _paa001;
		private decimal _aaa001;
		private decimal _pad003;
		private decimal _nad003;
		private decimal _rid;
		private string _aad105;
		/// <summary>
		/// 
		/// </summary>
		public decimal PAA001
		{
			set{ _paa001=value;}
			get{return _paa001;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal AAA001
		{
			set{ _aaa001=value;}
			get{return _aaa001;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal PAD003
		{
			set{ _pad003=value;}
			get{return _pad003;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal NAD003
		{
			set{ _nad003=value;}
			get{return _nad003;}
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

