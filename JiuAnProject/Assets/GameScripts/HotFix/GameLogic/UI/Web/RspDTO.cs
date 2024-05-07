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
    // 响应List<CameraInfoDTO>
    [Serializable]
    public class RspCameraInfoDTO
    {
        public int id;
        public string pos;
    }
    
    // 响应RspRecordDTO
    [Serializable]
    public class RspRecordDTO
    {
        public int page;
        public int limit; //每页数量
        public int totalCount;
        public List<RspRecordItemDTO> list;
    }
    
    [Serializable]
    public class RspRecordItemDTO
    {
        public long key;
        public string recordTime;
        public string name;
        public string pos;
        public int status; //1进2出
        public string shot_img; //抓拍图url
    }
}