/****************************************************
  文件：UICountGroupWidget_Designer.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年05月08日 23:14:19
  功能：
*****************************************************/

using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TEngine;
using TMPro;

namespace GameLogic
{
	public partial class UICountGroupWidget
	{
		#region 脚本工具生成的代码
		private TMP_Dropdown m_dpGroups;
		private Button m_btnSave;
		private TMP_InputField m_inputNewGroup;
		private Button m_btnCreate;
		private Button m_btnDel;
		private ScrollRect m_scrollRectLeft;
		private ScrollRect m_scrollRectRight;
		private Button m_btnRightMove;
		private Button m_btnLeftMove;
		protected override void ScriptGenerator()
		{
			m_dpGroups = FindChildComponent<TMP_Dropdown>("Top/HorizontalGroup/m_dpGroups");
			m_btnSave = FindChildComponent<Button>("Top/HorizontalGroup/m_btnSave");
			m_inputNewGroup = FindChildComponent<TMP_InputField>("Top/HorizontalGroup/m_inputNewGroup");
			m_btnCreate = FindChildComponent<Button>("Top/HorizontalGroup/m_btnCreate");
			m_btnDel = FindChildComponent<Button>("Top/HorizontalGroup/m_btnDel");
			m_scrollRectLeft = FindChildComponent<ScrollRect>("Main/MainContent/m_scrollRectLeft");
			m_scrollRectRight = FindChildComponent<ScrollRect>("Main/MainContent/m_scrollRectRight");
			m_btnRightMove = FindChildComponent<Button>("Main/m_btnRightMove");
			m_btnLeftMove = FindChildComponent<Button>("Main/m_btnLeftMove");
			m_btnSave.onClick.AddListener(UniTask.UnityAction(OnClickSaveBtn));
			m_btnCreate.onClick.AddListener(UniTask.UnityAction(OnClickCreateBtn));
			m_btnDel.onClick.AddListener(UniTask.UnityAction(OnClickDelBtn));
			m_btnRightMove.onClick.AddListener(UniTask.UnityAction(OnClickRightMoveBtn));
			m_btnLeftMove.onClick.AddListener(UniTask.UnityAction(OnClickLeftMoveBtn));
		}
		#endregion

		#region 事件
		private async UniTaskVoid OnClickSaveBtn()
		{
			UIGlobalDataInstance.Instance.UpdateGroup(
				UIGlobalDataInstance.Instance.CurrentGroupID,
				curWidgets.Select(widget=>widget.Info.id).ToList()
			);
			UIGlobalDataInstance.Instance.Save();
			UISimpleTipWindow.Show("保存成功！");
		}
		private async UniTaskVoid OnClickDelBtn()
		{
			if (m_dpGroups.value == 0)
			{
				UISimpleTipWindow.Show("不可删除默认统计组");
				return;
			}
			m_dpGroups.options.RemoveAt(m_dpGroups.value);
			UIGlobalDataInstance.Instance.RemoveGroup(UIGlobalDataInstance.Instance.CurrentGroupID);
			//设为默认统计组
			m_dpGroups.value = 0;
			UISimpleTipWindow.Show("删除成功！");
		}
		private async UniTaskVoid OnClickCreateBtn()
		{
			string newName = m_inputNewGroup.text;
			if (string.IsNullOrEmpty(newName))
			{
				UISimpleTipWindow.Show("统计组名称不能为空");
				return;
			}
			if (UIGlobalDataInstance.Instance.GroupDict.ContainsKey(newName))
			{
				UISimpleTipWindow.Show("统计组名称不能重复");
				return;
			}
			UIGlobalDataInstance.Instance.UpdateGroup(
				newName,
				curWidgets.Select(widget=>widget.Info.id).ToList()
			);
			UIGlobalDataInstance.Instance.Save();
			RefeshTitle();
			UISimpleTipWindow.Show("新建成功！");
		}
		private async UniTaskVoid OnClickRightMoveBtn()
		{
			for(int i = 0; i < noneWidgets.Count; i++)
			{
				if (noneWidgets[i].IsOn)
				{
					noneWidgets[i].IsOn = false;
					noneWidgets[i].transform.parent = m_scrollRectRight.content;
					curWidgets.Add(noneWidgets[i]);
					noneWidgets.RemoveAt(i);
					i--;
				}
			}
		}
		private async UniTaskVoid OnClickLeftMoveBtn()
		{
			if (m_dpGroups.value == 0)
			{
				UISimpleTipWindow.Show("默认统计组不可修改");
				return;
			}
			for(int i = 0; i < curWidgets.Count; i++)
			{
				if (curWidgets[i].IsOn)
				{
					curWidgets[i].IsOn = false;
					curWidgets[i].transform.SetParent(m_scrollRectLeft.content);
					noneWidgets.Add(curWidgets[i]);
					curWidgets.RemoveAt(i);
					i--;
				}
			}
		}
		#endregion

	}
}
