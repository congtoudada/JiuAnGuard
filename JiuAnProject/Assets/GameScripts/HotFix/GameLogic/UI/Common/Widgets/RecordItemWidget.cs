/****************************************************
  文件：RecordItemWidget.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 21:51:39
  功能：
*****************************************************/

using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using TEngine;
using TMPro;

namespace GameLogic
{
	public class RecordItemWidget : UIWidget
	{
		public enum RenderTypeEnum
		{
			Red = 0,
			Green
		}
		
		private long key;
		private string recordTime;
		private string name;
		private string pos;
		private string status;
		private string img_url;
		private float warnScore;
		private RenderTypeEnum renderType; //0:红 1:绿

		public RenderTypeEnum RenderType
		{
			get => renderType;
			set => renderType = value;
		}

		public string ImgURL
		{
			get => img_url;
			set => img_url = value;
		}

		public long Key
		{
			get => key;
			set => key = value;
		}

		public string RecordTime
		{
			get => recordTime;
			set => recordTime = value;
		}

		public string Name
		{
			get => name;
			set => name = value;
		}

		public string Pos
		{
			get => pos;
			set => pos = value;
		}

		public string Status
		{
			get => status;
			set => status = value;
		}
		
		public float WarnScore
		{
			get => warnScore;
			set => warnScore = value;
		}

		private RecordItemWidgetMgr mgr;
		
		#region 脚本工具生成的代码
		private Toggle m_toggleSelect;
		private TextMeshProUGUI m_textID;
		private TextMeshProUGUI m_textRecordTime;
		private TextMeshProUGUI m_textName;
		private TextMeshProUGUI m_textPos;
		private TextMeshProUGUI m_textStatus;
		private Button m_btnDetail;
		private Button m_btnDel;
		protected override void ScriptGenerator()
		{
			m_toggleSelect = FindChildComponent<Toggle>("Horizontal/m_toggleSelect");
			m_textID = FindChildComponent<TextMeshProUGUI>("Horizontal/m_textID");
			m_textRecordTime = FindChildComponent<TextMeshProUGUI>("Horizontal/m_textRecordTime");
			m_textName = FindChildComponent<TextMeshProUGUI>("Horizontal/m_textName");
			m_textPos = FindChildComponent<TextMeshProUGUI>("Horizontal/m_textPos");
			m_textStatus = FindChildComponent<TextMeshProUGUI>("Horizontal/m_textStatus");
			m_btnDetail = FindChildComponent<Button>("m_btnDetail");
			m_btnDel = FindChildComponent<Button>("m_btnDel");
			m_toggleSelect.onValueChanged.AddListener(OnToggleSelectChange);
			m_btnDetail.onClick.AddListener(UniTask.UnityAction(OnClickDetailBtn));
			m_btnDel.onClick.AddListener(UniTask.UnityAction(OnClickDelBtn));
		}
		#endregion
		
		#region 事件
		private void OnToggleSelectChange(bool isOn)
		{
			if (isOn)
			{
				mgr.AddSelected(key);
			}
			else
			{
				mgr.RemoveSelected(key);
			}
		}
		private async UniTaskVoid OnClickDetailBtn()
		{
			UITipWindow.Show(title: GetTitleString(mgr.PageType), sub_text: GetSubString(), img_url: ImgURL);
		}
		private async UniTaskVoid OnClickDelBtn()
		{
			UITipWindow.Show(title: "删除" + GetTitleString(mgr.PageType), main_text: "<color=red>你真的要删除该记录吗？</color>", 
				sub_text: GetSubString(), img_url: ImgURL,
				confirmCallback: async () =>
				{
					WWWForm form = new WWWForm();
					ReqDelRecordDTO dto = new ReqDelRecordDTO();
					dto.key = Key;
					dto.pageType = (int) mgr.PageType;
					form.AddField("DTO", JsonConvert.SerializeObject(dto));
					string ret = await Utility.Http.Post(WebURL.GetFullURL("record_delete"), form);
					bool result = JsonConvert.DeserializeObject<bool>(ret);
					if (result)
					{
						mgr.DelRecordItem(key);
						UISimpleTipWindow.Show("删除成功");
					}
					else
					{
						UISimpleTipWindow.Show("删除失败");
					}
				});
		}
		#endregion
		
		public void Update(RecordItemWidget item)
		{
			if (item != this)
				Update(item.mgr, item.Key, item.RecordTime, item.Name, item.Pos, 
					item.Status, item.ImgURL, item.warnScore, item.renderType);
		}
		
		public void Update(RecordItemWidgetMgr mgr, long key, string recordTime, string name, 
			string pos, string status, string img_url, float warnScore, RenderTypeEnum renderType)
		{
			this.mgr = mgr;
			Key = key;
			var recordTimeArr = recordTime.Split("_");
			recordTimeArr[1] = recordTimeArr[1].Replace("-", ":");
			recordTime = recordTimeArr[0] + " " + recordTimeArr[1];
			RecordTime = recordTime;
			Name = name;
			Pos = pos;
			Status = status;
			ImgURL = img_url;
			WarnScore = warnScore;
			RenderType = renderType;
			Refresh();
		}

		private void Refresh()
		{
			m_textID.text = key.ToString();
			m_textRecordTime.text = recordTime;
			if (string.IsNullOrEmpty(name))
			{
				m_textName.gameObject.SetActive(false);
			}
			else
			{
				m_textName.text = name;
				m_textName.gameObject.SetActive(true);
			}
			m_textPos.text = pos;
			m_textStatus.text = status;
			m_toggleSelect.isOn = false;
			switch (renderType)
			{
				case RenderTypeEnum.Red:
					transform.GetComponent<Image>().SetSprite("main_right_recordItem");
					break;
				case RenderTypeEnum.Green:
					transform.GetComponent<Image>().SetSprite("main_right_recordItem2");
					break;
			}
		}

		private string GetTitleString(PageType pageType)
		{
			switch (pageType)
			{
				case PageType.Count:
					return "计数记录";
				case PageType.Face:
					return "人脸记录";
				case PageType.Warn:
					return "警告记录";
			}

			return "";
		}

		private static StringBuilder _builder = new StringBuilder();
		private string GetSubString()
		{
			_builder.Clear();
			_builder.Append("ID: ").Append(Key).AppendLine();
			_builder.Append("时间: ").Append(RecordTime).AppendLine();
			if (!string.IsNullOrEmpty(Name))
			{
				_builder.Append("姓名: ").Append(Name).AppendLine();
			}
			_builder.Append("抓拍点: ").Append(Pos).AppendLine();
			_builder.Append("进出状态: ").Append(Status).AppendLine();
			if (warnScore > 0)
			{
				_builder.Append("报警置信度: ").Append(WarnScore.ToString("0.00")).AppendLine();
			}
			return _builder.ToString();
		}

		public void SelectThis(bool isOn)
		{
			m_toggleSelect.isOn = isOn;
		}
	}
}
