using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web;
using System.Threading;
using System.Security.Cryptography;
using Octopus.Entity;
using WCFDemo.Entities;
using System.IO;
using System.Runtime.Serialization;

namespace WCFDemo
{
    class aaa
    {
        static void Main(params string[] paras)
        {
            short hour = 0;
            short.TryParse("20" ?? string.Empty, out hour);
            var scheduleOnceTime = new DateTime(DateTime.Now.Ticks - DateTime.Now.Ticks % TimeSpan.TicksPerDay).AddHours(hour);
            Console.WriteLine(scheduleOnceTime );
        }
    }
    [DataContract]
    public class UserPrize
    {
        /// <summary>
        /// 获奖记录的ID
        /// </summary>
        [DataMember]
        public int LogID { get; set; }
        /// <summary>
        /// 奖品名称
        /// </summary>
        [DataMember]
        public string PrizeName { get; set; }
        /// <summary>
        /// 兑换ID
        /// </summary>
        [DataMember]
        public int ExchangeID { get; set; }
        /// <summary>
        /// 兑换类型
        /// </summary>
        [DataMember]
        public int ExchangeType { get; set; }
        /// <summary>
        /// 中奖日期
        /// </summary>
        [DataMember]
        public DateTime LuckyTime { get; set; }
        /// <summary>
        /// 奖品有效期起始日期
        /// </summary>
        [DataMember]
        public DateTime BeginValidDate { get; set; }
        /// <summary>
        /// 奖品有效期截止日期
        /// </summary>
        [DataMember]
        public DateTime EndValidDate { get; set; }
        /// <summary>
        /// 奖品内容
        /// </summary>
        [DataMember]
        public string PrizeContent { get; set; }
    }


    /// <summary>
    /// 抽奖奖品基本属性
    /// </summary>
    [DataContract]
    public class Prize
    {
        /// <summary>
        /// 奖品ID，主键自增
        /// </summary>
        [DataMember]
        public int PrizeID { get; set; }
        /// <summary>
        /// 奖品索引位置
        /// </summary>
        [DataMember]
        public int PrizeIndex { get; set; }
        /// <summary>
        /// 奖品名称
        /// </summary>
        [DataMember]
        public string PrizeName { get; set; }
        /// <summary>
        /// 奖品描述
        /// </summary>
        [DataMember]
        public string PrizeDescription { get; set; }
    }

    /// <summary>
    /// 用户抽奖日志
    /// </summary>
    [DataContract]
    public class UserPrizeLog
    {
        /// <summary>
        /// 中奖用户昵称
        /// </summary>
        [DataMember]
        public string UserName { get; set; }
        /// <summary>
        /// 奖品ID
        /// </summary>
        [DataMember]
        public int PrizeID { get; set; }
        /// <summary>
        /// 奖品名称
        /// </summary>
        [DataMember]
        public string PrizeName { get; set; }
        /// <summary>
        /// 中奖时间
        /// </summary>
        [DataMember]
        public string LuckyTime { get; set; }
    }
}
