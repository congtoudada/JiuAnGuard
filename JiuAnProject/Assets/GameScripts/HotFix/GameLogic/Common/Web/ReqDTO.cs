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
  // /// <summary>
  // /// 实时计数请求
  // /// </summary>
  // [Serializable]
  // public class ReqRuntimeCountDTO
  // {
  //   public long timestamp = -1; //上次响应记录的时间戳，-1表示获取当日全部计数信息
  //   public List<int> candidate; //摄像头候选id，只返回候选id的计数情况
  // }
  
  /// <summary>
  /// 删除记录请求
  /// </summary>
  [Serializable]
  public class ReqDelRecordDTO
  {
    public long key; //主键
    public int pageType; //类型过滤 (1:计数 2:人脸 3:警告)
  }
  
  /// <summary>
  /// 获取记录请求
  /// </summary>
  [Serializable]
  public class ReqRecordListDTO
  {
    public int page; //当前页数
    public int limit; //一页最大数量
    public int pageType; //类型过滤 (1:计数 2:人脸 3:警告)
    public long recordId; //id过滤（可选）
    public string beginTime; //开始时间过滤（可选）
    public string endTime; //结束时间过滤（可选，如果为空表示至今）
    public string pos; //抓拍点过滤（可选）
    public string name; //姓名过滤（可选）
    public int status; //状态 表示方向(1:进 2:出) 表示警告 (1: 打电话 2: 安全帽 3:代刷卡 4:特定区域入侵)
    public List<int> candidate; //摄像头候选id，只返回候选id的计数情况，为空表示选择全部
    public int sort; //排序 (1升序, 2降序)
    public string directory; //目录

    public override string ToString()
    {
      return $"条件查询: {nameof(page)}: {page}, {nameof(limit)}: {limit}, {nameof(pageType)}: {pageType}, " +
             $"{nameof(recordId)}: {recordId}, {nameof(beginTime)}: {beginTime}, {nameof(endTime)}: {endTime}, " +
             $"{nameof(pos)}: {pos}, {nameof(name)}: {name}, {nameof(status)}: {status}, " +
             $"{nameof(candidate)}: {candidate}, {nameof(sort)}: {sort}";
    }
  }
}