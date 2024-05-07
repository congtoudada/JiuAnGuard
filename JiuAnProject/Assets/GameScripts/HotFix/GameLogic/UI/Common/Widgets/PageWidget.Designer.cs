/****************************************************
  文件：PageWidget_Designer.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年05月06日 18:58:36
  功能：
*****************************************************/

using System;
using Cysharp.Threading.Tasks;
using TEngine;
using TMPro;
using UnityEngine.UI;

namespace GameLogic
{
    public partial class PageWidget
    {
	    private bool isAllSelecting = false;
        #region 脚本工具生成的代码
		private TextMeshProUGUI m_textSelectCount;
		private TextMeshProUGUI m_textTotal;
		private Button m_btnLast;
		private Button m_btnFirst;
		private Button m_btnSecond;
		private Button m_btnThird;
		private Button m_btnFourth;
		private Button m_btnFifth;
		private Button m_btnNext;
		private TMP_InputField m_inputPage;
		private Button m_btnJump;
		private Button m_btnAllSelect;
		private Button m_btnDelSelect;
		protected override void ScriptGenerator()
		{
			m_textSelectCount = FindChildComponent<TextMeshProUGUI>("m_textSelectCount");
			m_textTotal = FindChildComponent<TextMeshProUGUI>("m_textTotal");
			m_btnLast = FindChildComponent<Button>("m_btnLast");
			m_btnFirst = FindChildComponent<Button>("Horizontal/m_btnFirst");
			m_btnSecond = FindChildComponent<Button>("Horizontal/m_btnSecond");
			m_btnThird = FindChildComponent<Button>("Horizontal/m_btnThird");
			m_btnFourth = FindChildComponent<Button>("Horizontal/m_btnFourth");
			m_btnFifth = FindChildComponent<Button>("Horizontal/m_btnFifth");
			m_btnNext = FindChildComponent<Button>("m_btnNext");
			m_inputPage = FindChildComponent<TMP_InputField>("m_inputPage");
			m_btnJump = FindChildComponent<Button>("m_btnJump");
			m_btnAllSelect = FindChildComponent<Button>("m_btnAllSelect");
			m_btnDelSelect = FindChildComponent<Button>("m_btnDelSelect");
			m_btnLast.onClick.AddListener(UniTask.UnityAction(OnClickLastBtn));
			m_btnNext.onClick.AddListener(UniTask.UnityAction(OnClickNextBtn));
			m_btnJump.onClick.AddListener(UniTask.UnityAction(OnClickJumpBtn));
			m_btnAllSelect.onClick.AddListener(UniTask.UnityAction(OnClickAllSelectBtn));
			m_btnDelSelect.onClick.AddListener(UniTask.UnityAction(OnClickDelSelectBtn));
		}
		#endregion

		#region 事件
		private async UniTaskVoid OnClickLastBtn()
		{
			//如果在页首，直接返回
			if (currentPage <= 1)
			{
				UISimpleTipWindow.Show("已经处于首页");
			}
			else
			{
				turnPageEvent(currentPage - 1);
			}
		}

		private async UniTaskVoid OnClickNextBtn()
		{
			//如果在页尾，直接返回
			if (currentPage >= totalPage)
			{
				UISimpleTipWindow.Show("已经处于尾页");
			}
			else
			{
				turnPageEvent(currentPage + 1);
			}
		}
		private async UniTaskVoid OnClickJumpBtn()
		{
			if (string.IsNullOrEmpty(m_inputPage.text))
			{
                UISimpleTipWindow.Show("页码为空");
				return;
			}
			int jumpPage = Convert.ToInt32(m_inputPage.text);
			if (jumpPage < 1 || jumpPage > totalPage)
			{
				UISimpleTipWindow.Show("页码不合法");
				return;
			}
			turnPageEvent(jumpPage);
		}
		private async UniTaskVoid OnClickAllSelectBtn()
		{
			if (!isAllSelecting)
			{
				if (allSelectEvent != null) allSelectEvent();
				isAllSelecting = true;

			}
			else
			{
				if (allCancelEvent != null) allCancelEvent();
				isAllSelecting = false;
			}
			UpdateSelectAllBtnText();
		}
		
		private async UniTaskVoid OnClickDelSelectBtn()
		{
			if (selectedCount <= 0)
			{
				UISimpleTipWindow.Show("没有选中任何数据");
				return;
			}
			UITipWindow.Show(title: "删除所选", main_text: $"当前选中{selectedCount}条数据，确定删除？", 
				confirmCallback: () =>
				{
					if (allSelectDelEvent != null) allSelectDelEvent();
					GameModule.UI.HideUI<UITipWindow>();
				});
		}
		#endregion

		public void UpdateSelectAllBtnText()
		{
			string txt = null;
			if (isAllSelecting)
			{
				txt = "取消全选";
			}
			else
			{
				txt = "全选当前";
			}
			m_btnAllSelect.transform.FindChildComponent<TextMeshProUGUI>("Text (TMP)").text = txt;
		}
    }
}