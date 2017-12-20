using MWUtility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrentRunner
{
    class DemoClass
    {
        #region Refund
        private string refundCacheKey = "userProxyRefunds";

        private ConcurrentDictionary<string, int> userRefunds = new ConcurrentDictionary<string, int>();
        public void AddRefund(string userId, int refund = 1)
        {
            try
            {
                LogHelper.Debug($"AddRefund: user {userId}, refund to add: {refund}");
                if (userRefunds.ContainsKey(userId))
                {
                    userRefunds[userId] += refund;
                }
                else
                {
                    userRefunds.AddOrUpdate(userId, refund, (u, r) => { return userRefunds[u] + r; });
                }
                WMedis.Instance.Cache(refundCacheKey, userRefunds, true);
            }
            catch (Exception ex)
            {
                LogHelper.Info($"AddRefund error: user {userId}, refund to add: {refund}");
                LogHelper.Error(ex);
            }
        }
        private volatile Dictionary<string, object> userLocks = new Dictionary<string, object>();
        public bool ConsumeRefund(string userId, int consume = 1)
        {
            try
            {
                object _lock = null;
                lock (userLocks)
                {
                    if (!userLocks.ContainsKey(userId))
                    {
                        userLocks.Add(userId, new object());
                    }
                    _lock = userLocks[userId];
                }

                lock (_lock)
                {
                    if (!userRefunds.ContainsKey(userId) || userRefunds[userId] < consume)
                        return false;

                    LogHelper.Debug($"ConsumeRefund: {userId} {consume}/{userRefunds[userId]}");
                    userRefunds[userId] -= consume;
                    WMedis.Instance.Cache(refundCacheKey, userRefunds, true);
                    if (userRefunds[userId] == 0)
                    {
                        int refund = -1;
                        userRefunds.TryRemove(userId, out refund);
                        if (refund > 0)
                        {
                            AddRefund(userId, refund);
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Info($"ConsumeRefund error: user {userId}, consume: {consume}");
                LogHelper.Error(ex);
            }
            return false;
        }

        public int HowManyRefund(string userId)
        {
            try
            {
                if (!userRefunds.ContainsKey(userId))
                    return 0;
                return userRefunds[userId];
            }
            catch (Exception ex)
            {
                LogHelper.Info($"HowManyRefund error: user {userId}");
                LogHelper.Error(ex);
                return 0;
            }
        }

        #endregion

    }
}
