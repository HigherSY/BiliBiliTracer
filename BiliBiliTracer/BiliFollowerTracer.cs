using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using BiliBiliTracer.Model;
using BiliBiliTracer.Service;

namespace BiliBiliTracer
{
    public class BiliFollowerTracer : PeriodWorkerService
    {
        private readonly ILogger<BiliFollowerTracer> _logger;
        private readonly DbConnection _db;
        private readonly BiliService _bili;
        private readonly IConfiguration _cfg;

        protected override TimeSpan Delay { get; set; }

        public BiliFollowerTracer(ILogger<BiliFollowerTracer> logger, DbConnection db, BiliService biliSvc, IConfiguration configuration)
        {
            _logger = logger;
            _db = db;
            _bili = biliSvc;
            _cfg = configuration;

            Delay = TimeSpan.FromSeconds(_cfg.GetValue<int>("BiliService:Interval:Follower"));
        }

        protected override async Task DoWorkAsync()
        {
            _logger.LogInformation("Running at: {time}", DateTimeOffset.Now);
            int[] vmids = _cfg.GetSection("BiliService:vmids").Get<int[]>();
            try
            {
                using var conn = _db.New();
                foreach (var vmid in vmids)
                {
                    int vmidRowAffected = 0;
                    for (int i = 1; i <= 5; i++)
                    {
                        var data = await _bili.GetJsonData<Followers>($"/x/relation/followers?vmid={vmid}&pn={i}&ps=50&order=desc");

                        var values = data.list.Select(item => new { followerData = item.GetRawText(), vmid }).ToArray();
                        int rowAffected = await conn.ExecuteAsync(@"INSERT INTO bili.Relation (followerData, vmid) VALUES (@followerData, @vmid)", values);
                        vmidRowAffected += rowAffected;
                        if (rowAffected < 50 || i >= 5)
                        {
                            _logger.LogInformation("vmid: {} page: {} total: {} rowAffected: {}", vmid, i, data.total, vmidRowAffected);
                            await conn.ExecuteAsync(@"INSERT INTO bili.TotalFollower (vmid, total) VALUES(@vmid, @total)", new { vmid, data.total });
                            break;
                        }
                    }
                    await Task.Delay(250);
                };

                int userMerged = await conn.ExecuteAsync(@"MERGE INTO bili.UserData as T USING (SELECT DISTINCT(mid) FROM bili.Relation) as S ON T.mid = S.mid WHEN NOT MATCHED THEN INSERT (mid) VALUES(S.mid);");
                _logger.LogInformation("userMerged: {}", userMerged);
            }
            catch (BiliApiException ex)
            {
                _logger.LogWarning(ex, "Remote API error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unknown error");
            }
        }
    }
}
