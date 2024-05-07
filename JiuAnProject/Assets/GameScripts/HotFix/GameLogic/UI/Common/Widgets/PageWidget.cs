/****************************************************
  文件：PageWidget.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年05月02日 15:47:15
  功能：
*****************************************************/

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TEngine;
using TMPro;

namespace GameLogic
{
	public partial class PageWidget : UIWidget
	{
		private List<Button> _switchBtnList = new List<Button>(5);
		public event Action<int> turnPageEvent; //翻页
		public event Action allSelectEvent; //全选
		public event Action allCancelEvent; //取消全选
		public event Action allSelectDelEvent; //所选删除
		private int totalPage = 0;
		private int curSelectedIdx = -1; //上次选中按钮
		private int currentPage = -1; //当前选中页
		private int selectedCount = 0; //选中数量
		private int totalCount = 0; //总消息数量
		private int limit = 0; //页容量
		private Color selectedColor = new Color(231, 85, 85);
		protected override void BindMemberProperty()
		{
			_switchBtnList.Add(m_btnFirst);
			_switchBtnList.Add(m_btnSecond);
			_switchBtnList.Add(m_btnThird);
			_switchBtnList.Add(m_btnFourth);
			_switchBtnList.Add(m_btnFifth);
		}

		public void RefreshByUser(int selectedCount)
		{
			this.selectedCount = selectedCount;
			m_textSelectCount.text = $"当前选中{selectedCount}条";
		}

		public void RefreshByServer(int totalCount, int currentPage, int limit)
		{
			this.totalCount = totalCount;
			this.currentPage = currentPage;
			this.limit = limit;
			m_textTotal.text = $"共{totalCount}条（{limit}条/页）";
			if (totalCount == 0)
			{
				foreach (var item in _switchBtnList)
				{
					item.gameObject.SetActive(false);
				}
				return;
			}
			totalPage = (totalCount - 1) / limit + 1; //总页数
			int startPage = (currentPage - 1) / _switchBtnList.Count; //1~5->0 6~10->1
			startPage = startPage * _switchBtnList.Count + 1; //1~5->1 6~10->6
			for (int i = 0; i < _switchBtnList.Count; i++)
			{
				int showPage = startPage + i; //控件展示的页数
				if (showPage > totalPage)
				{
					SetBtnText(i, totalPage.ToString());
					_switchBtnList[i].gameObject.SetActive(false);
				}
				else
				{
					_switchBtnList[i].gameObject.SetActive(true);
					SetBtnText(i, showPage.ToString());
					_switchBtnList[i].onClick.RemoveAllListeners();
					_switchBtnList[i].onClick.AddListener(() =>
					{
						if (turnPageEvent != null) turnPageEvent(showPage);
					});
					if (showPage == currentPage)
					{
						var cg = _switchBtnList[i].transform.FindChildComponent<CanvasGroup>("Mask");
						DOTween.To(() => cg.alpha, x => cg.alpha = x, 1.0f, 0.25f);
						if (curSelectedIdx != -1 && i != curSelectedIdx)
						{
							var lastCg = _switchBtnList[curSelectedIdx].transform
								.FindChildComponent<CanvasGroup>("Mask");
							DOTween.To(() => lastCg.alpha, x => lastCg.alpha = x, 0, 0.25f);
						}
						curSelectedIdx = i;
					}
				}
			}
		}

		private void SetBtnText(int idx, string content)
		{
			if (idx < 0 || idx >= _switchBtnList.Count) return;
			_switchBtnList[idx].transform.FindChildComponent<TextMeshProUGUI>("Text (TMP)").text = content;
		}

		private int GetBtnText(int idx)
		{
			if (idx < 0 || idx >= _switchBtnList.Count) return -1;
			return Convert.ToInt32(_switchBtnList[idx].transform.FindChildComponent<TextMeshProUGUI>("Text (TMP)").text);
		}
	}
}