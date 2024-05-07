/****************************************************
  文件：ReqDTO.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年05月02日 17:41:18
  功能：
*****************************************************/

using System;
using System.Collections.Generic;

namespace GameLogic
{
  [Serializable]
  public class ReqRecordList
  {
    public int page; //当前页数
    public int limit; //数量
    public PageType type; //类型过滤
    public long recordId; //id过滤（可选）
    public string beginTime; //开始时间过滤（可选）
    public string endTime; //结束时间过滤（可选）
    public string pos; //抓拍点过滤（可选）
    public int direction; //方向过滤（可选，1进2出）
    public string name; //姓名过滤（可选）
    public int sort; //排序 (1升序, 2降序)
  }
}