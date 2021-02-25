using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using TheMonarchs.Shared;

namespace TheMonarchs.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class MonarchsController : Controller
    {
        private List<Monarch> MonarchData { get; set; }

        public MonarchsController(IMemoryCache cache)
        {
            MonarchData = cache.Get<List<Monarch>>("monarchs");
            
        }

        [HttpGet]
        public ActionResult GetMonarchs()
        {
            return Ok(MonarchData);
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult Firstnames()
        {
            var firstNames = new List<string>();
            foreach (var entry in MonarchData)
            {
                var index = entry.nm.IndexOf(" ", StringComparison.Ordinal);
                if (index == -1)
                {
                    firstNames.Add(entry.nm);
                }
                else
                {
                    firstNames.Add(entry.nm.Substring(0, index));
                }
            }

            var firstNameList = firstNames.GroupBy(first => first)
                .OrderBy(grp => grp.Key)
                .Select(grp => new
                {
                    name = grp.Key, occurence = grp.Count()
                });
            
            return Ok(firstNameList.OrderByDescending(f=>f.occurence).First());
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult Number()
        {
            return Ok(MonarchData.Count.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult LongestRule()
        {
            var rulingYears = AddRulingYearsToData();

            return Ok(rulingYears.OrderByDescending(r=>r.rule).First());
        }
        
        [HttpGet]
        [Route("[action]")]

        public ActionResult LongestRulingHouse()
        {
            var rulingYears = AddRulingYearsToData();
            var houseRuling = rulingYears.GroupBy(house => house.hse)
                .Select(houseGroup => new
                {
                    hse = houseGroup.Key,
                    rule = houseGroup.Sum(x => x.rule)
                });
            return Ok(houseRuling.OrderByDescending(r=>r.rule).First());
        }

        private string CheckYrsValidity(string data)
        {
            var index = data.IndexOf('-');
            var left = data.Substring(0, index);
            var right = data.Substring(index + 1);
            if (right == "")
            {
                right = DateTime.Now.Year.ToString();
            }
            return $"{left}-{right}";
        }

        private List<Monarch> AddRulingYearsToData()
        {
            var rulingYears = new List<Monarch>();
            foreach (var entry in MonarchData)
            {
                if (entry.yrs.Contains('-'))
                {
                    var validatedYears = CheckYrsValidity(entry.yrs);
                    var year = Array.ConvertAll(validatedYears.Split('-'), int.Parse);
                    entry.rule = year[1] - year[0];
                    rulingYears.Add(entry);
                }
                else
                {
                    entry.rule = 0;
                    rulingYears.Add(entry);
                }
            }
            return rulingYears;
        }
    }
}
