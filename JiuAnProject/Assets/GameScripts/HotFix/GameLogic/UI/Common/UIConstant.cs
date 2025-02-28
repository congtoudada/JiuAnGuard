/****************************************************
  文件：UIConstant.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年09月22日 16:35:21
  功能：
*****************************************************/

namespace GameLogic
{
    public static class UIConstant
    {
        public const float WarnThreshold = 0.68f; //报警分数阈值（在该分数两侧的item会有不同的渲染方式）
        public const int MaxWarnCount = 9999; //最多存储9999条实时报警数据
    }
}