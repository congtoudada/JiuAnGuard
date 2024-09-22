/****************************************************
  文件：RecordItemWidgetMgr.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月30日 17:38:10
  功能：
*****************************************************/

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using TEngine;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace GameLogic
{
    /// <summary>
    /// 控制RecordItem
    /// </summary>
    public class RecordItemWidgetMgr : UIWidget
    {
        public const int PAGE_MAX = 20; //一页最多20条记录
        private int add_idx = 0; //赋值item的索引
        private List<long> selectedList = new List<long>();
        private PageType _pageType = PageType.Count;
        private PageWidget _pageWidget;

        public PageType PageType
        {
            get => _pageType;
            set => _pageType = value;
        }

        public void Init(PageType pageType)
        {
            _pageType = pageType;
        }

        public void Clear()
        {
            add_idx = 0;
            ClearAllSelected();
            for (int i = 0; i < _itemList.Count; i++)
            {
                _itemList[i].Visible = false;
            }
        }
        
        #region 增删RecordItem

        private List<RecordItemWidget> _itemList;
        protected override void BindMemberProperty()
        {
            var mScrollRectRecord = FindChildComponent<ScrollRect>("Main/m_scrollRectRecord");
            var recordItemSlot = mScrollRectRecord.content.transform;
            var pageWidgetSlot = this.transform.Find("Main/PageWidget").gameObject;
            _pageWidget = CreateWidget<PageWidget>(pageWidgetSlot);
            _itemList = new List<RecordItemWidget>(PAGE_MAX);
            //清理测试数据
            foreach (Transform trans in recordItemSlot)
            {
                Object.Destroy(trans.gameObject);
            }
            //预初始化PAGE_MAX
            for (int i = 0; i < PAGE_MAX; i++)
            {
                _itemList.Add(CreateWidgetByPath<RecordItemWidget>(recordItemSlot, nameof(RecordItemWidget), false));
            }

            _pageWidget.allSelectEvent += ViewSelectAll;
            _pageWidget.allCancelEvent += ViewCancelAll;
            _pageWidget.allSelectDelEvent += DelAllRecordItem;
        }

        private void ViewSelectAll()
        {
            selectedList.Clear();
            for (int i = 0; i < _itemList.Count; i++)
            {
                if (_itemList[i].Visible)
                {
                    _itemList[i].SelectThis(true);
                }
                else
                {
                    break;
                }
            }
        }

        private void ViewCancelAll()
        {
            for (int i = 0; i < _itemList.Count; i++)
            {
                if (_itemList[i].Visible)
                {
                    _itemList[i].SelectThis(false);
                }
                else
                {
                    break;
                }
            }
        }

        public void SetRecordItem(long key, string recordTime, string name, 
            string pos, string status, string img_url, float warnScore, RecordItemWidget.RenderTypeEnum renderType)
        {
            if (add_idx >= PAGE_MAX) return;
            _itemList[add_idx].Update(this, key, recordTime, name, pos, status, img_url, warnScore, renderType);
            _itemList[add_idx].Visible = true;
            add_idx++;
        }

        public void DelRecordItem(long key)
        {
            RemoveSelected(key);
            for (int i = 0; i < _itemList.Count; i++)
            {
                var recordItem = _itemList[i];
                if (recordItem.Key == key) //找到待删除字段
                {
                    //把最后一个元素赋值给当前元素
                    if (add_idx > 0 && i != add_idx - 1)
                    {
                        var last = _itemList[add_idx - 1];
                        recordItem.Update(last);
                        last.Visible = false;
                        add_idx--;
                    }
                    else
                    {
                        recordItem.Visible = false;
                    }
                }
            }
        }

        private void DelAllRecordItem()
        {
            for (int i = 0; i < selectedList.Count; i++)
            {
                //前端删除
                DelRecordItem(_itemList[i].Key);
                //后端删除
                WWWForm form = new WWWForm();
                ReqDelRecordDTO dto = new ReqDelRecordDTO();
                dto.key = _itemList[i].Key;
                dto.pageType = (int) PageType;
                form.AddField("DTO", JsonConvert.SerializeObject(dto));
                Utility.Http.Post(WebURL.GetFullURL("record_delete"), form).Forget();
            }
        }
        #endregion
        
        #region 选择
        /// <summary>
        /// 添加已选项
        /// </summary>
        /// <param name="key"></param>
        public void AddSelected(long key)
        {
            if (!selectedList.Contains(key))
            {
                selectedList.Add(key);
                _pageWidget.RefreshByUser(selectedList.Count);
            }
        }
        
        /// <summary>
        /// 移除已选
        /// </summary>
        /// <param name="key"></param>
        public void RemoveSelected(long key)
        {
            if (selectedList.Contains(key))
            {
                selectedList.Remove(key);
                _pageWidget.RefreshByUser(selectedList.Count);
            }
        }
        
        /// <summary>
        /// 清空所有已选
        /// </summary>
        public void ClearAllSelected()
        {
            selectedList.Clear();
            _pageWidget.RefreshByUser(selectedList.Count);
        }
        #endregion

        /// <summary>
        /// 更新翻页控件
        /// </summary>
        /// <param name="totalCount"></param>
        /// <param name="currentPage"></param>
        public void RefreshPageWidget(int totalCount, int currentPage)
        {
            _pageWidget.RefreshByServer(totalCount, currentPage, PAGE_MAX);
        }

        public void RegisterTurnPageEvent(Action<int> turnPageCallback)
        {
            _pageWidget.turnPageEvent += turnPageCallback;
        }
        
        public void UnRegisterTurnPageEvent(Action<int> turnPageCallback)
        {
            _pageWidget.turnPageEvent -= turnPageCallback;
        }
    }
}