using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using BiliBiliTracer.Service;

namespace BiliBiliTracer
{
    public class BiliDetailTracer : PeriodWorkerService
    {
        private readonly ILogger<BiliDetailTracer> _logger;
        private readonly DbConnection _db;
        private readonly BiliService _bili;
        private readonly IConfiguration _cfg;

        protected override TimeSpan Delay { get; set; }

        public BiliDetailTracer(ILogger<BiliDetailTracer> logger, DbConnection db, BiliService biliSvc, IConfiguration configuration)
        {
            _logger = logger;
            _db = db;
            _bili = biliSvc;
            _cfg = configuration;

            Delay = TimeSpan.FromSeconds(_cfg.GetValue<int>("BiliService:Interval:UserDetail"));
        }

        protected override async Task DoWorkAsync()
        {
            _logger.LogInformation("Running at: {time}", DateTimeOffset.Now);
            using var conn = _db.New();
            var mids = await conn.QueryAsync<int>(@"UPDATE Top(200) bili.UserData SET working = 1 OUTPUT INSERTED.mid WHERE working = 0 AND userCardData IS NULL");
            foreach (var mid in mids)
            {
                try
                {
                    var userCardData = await _bili.GetJsonData($"/x/web-interface/card?mid={mid}");
                    // var stat = await _bili.GetJsonData($"/x/relation/stat?vmid={mid}");
                    // var navnum = await _bili.GetJsonData($"/x/space/navnum?mid={mid}");
                    // await conn.ExecuteAsync(@"UPDATE bili.UserData SET working = 0, userData = @userData, stat = @stat, navnum = @navnum WHERE mid = @mid", new { userData = userData.GetRawText(), stat = stat.GetRawText(), navnum = navnum.GetRawText(), mid });
                    await conn.ExecuteAsync(@"UPDATE bili.UserData SET working = 0, userCardData = @userCardData WHERE mid = @mid", new { userCardData = userCardData.GetRawText(), mid });
                }
                catch (BiliApiException ex)
                {
                    // 2 ÒâÎ¶×Å´íÎó
                    await conn.ExecuteAsync(@"UPDATE bili.UserData SET working = 2 WHERE mid = @mid", new { mid });
                    _logger.LogWarning(ex, "Remote API error on user {}", mid);
                }
                catch (Exception ex)
                {
                    await conn.ExecuteAsync(@"UPDATE bili.UserData SET working = 0 WHERE mid = @mid", new { mid });
                    _logger.LogError(ex, "Unknown erroron user {}", mid);
                }
            }
            _logger.LogInformation("Updated users detail: {}", mids);
        }
    }
}
