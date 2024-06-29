/****************************************************
  文件：DTO.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年05月02日 17:34:22
  功能：
*****************************************************/

using System;
using System.Collections.Generic;

namespace GameLogic
{
    /// <summary>
    /// 响应摄像头信息
    /// </summary>
    // 响应List<CameraInfoDTO>
    [Serializable]
    public class RspCameraInfoDTO
    {
        public int id; //摄像头id
        public string pos; //抓拍点
        public string address; //摄像头ip地址
        public string streamUrl; //摄像头取流地址
        public string cameraType; //摄像头型号
    }
    
    /// <summary>
    /// 响应实时计数信息
    /// </summary>
    [Serializable]
    public class RspRuntimeCountDTO
    {
        // public long timeStamp; //记录当前响应的时间戳
        // public List<int> cameraList; //摄像头id数组，与inStatus和outStatus对应
        // public List<int> inStatus; //随时间降序（从凌晨2点开始的秒数） 7200秒<->2:00
        // public List<int> outStatus; //随时间降序（从凌晨2点开始的秒数）
        public int timestamp; //时间戳（从凌晨2点开始的秒数）
        public int inCount; //进人数
        public int outCount; //出人数
    }

    /// <summary>
    /// 响应记录信息
    /// </summary>
    [Serializable]
    public class RspRecordDTO
    {
        public int page; //当前页
        public int limit; //每页数量
        public int totalCount; //消息总数
        public List<RspRecordItemDTO> list;
    }
    
    /// <summary>
    /// 每条记录内容
    /// </summary>
    [Serializable]
    public class RspRecordItemDTO
    {
        public long recordId; //记录主键
        public string recordTime; //记录时间 yy-mm-dd HH:MM:SS
        public string name; //姓名
        public string pos;  //抓拍点
        public int status;  //方向信息(1进2出) 警告信息(1:打电话 2:安全帽 3:代刷卡 4:特定区域入侵)
        public string shotImg; //抓拍图url
    }
    
    /// <summary>
    /// 实时警告内容
    /// </summary>
    [Serializable]
    public class RspRuntimeWarnDTO
    {
        public string recordTime; //记录时间 yy-mm-dd HH:MM:SS
        public string name; //姓名
        public string pos; //抓拍点
        public int warnType; //警告类型 (1: 打电话 2: 安全帽 3:代刷卡 4:特定区域入侵)
        public string shotImg; //抓拍图url
    }
}