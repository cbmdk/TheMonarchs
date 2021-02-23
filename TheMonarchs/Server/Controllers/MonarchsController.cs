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
        private IMemoryCache _cache;

        public MonarchsController(IMemoryCache cache)
        {
            _cache = cache;
        }

        [HttpGet]
        public ActionResult GetMonarchs()
        {
            var data = _cache.Get<List<Monarch>>("monarchs");
            return Ok(data);
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult Firstnames()
        {
            var firstnames = new List<char>();
            var data = _cache.Get<List<Monarch>>("monarchs");
            firstnames = data.SelectMany(x => x.nm).ToList();
            return Ok(firstnames);
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult Number()
        {
            var data = _cache.Get<List<Monarch>>("monarchs");
            return Ok(data.Count.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult LongestRule()
        {
            var data = _cache.Get<List<Monarch>>("monarchs");
            var rulingYears = new List<Monarch>();
            foreach (var entry in data)
            {
                int years = 0;
                if (entry.yrs.Contains('-'))
                {
                    var validatedYears = CheckYrsValidity(entry.yrs);
                    int[] year = Array.ConvertAll(validatedYears.Split('-'), int.Parse);
                    var item = new Monarch();
                    item = entry;
                    item.rule = year[1] - year[0];
                    rulingYears.Add(item);
                }
                else
                {
                    var item = entry;
                    item.rule = 0;
                    rulingYears.Add(item);
                }
            }
            
            return Ok(rulingYears.OrderByDescending(r=>r.rule).First());
        }

        [HttpGet]
        [Route("[action]")]

        public ActionResult LongestRulingHouse()
        {
            var data = _cache.Get<List<Monarch>>("monarchs");
            var houses = data.GroupBy(house => house.hse);
            var rulingHouses = new List<House>();
            var monarchs = new List<Monarch>();
            foreach (var house in houses)
            {
                foreach (var entry in house)
                {
                    int years = 0;
                    if (entry.yrs.Contains('-'))
                    {
                        var validatedYears = CheckYrsValidity(entry.yrs);
                        int[] year = Array.ConvertAll(validatedYears.Split('-'), int.Parse);
                        var item = new Monarch();
                        item = entry;
                        item.rule = item.rule = year[1] - year[0];
                        monarchs.Add(item);
                    }
                }
                rulingHouses.Add(new House
                {
                    HouseName = house.Key,
                    Monarchs = monarchs
                });
            }
            return Ok(houses);
        }

        private string CheckYrsValidity(string data)
        {
            var index = data.IndexOf('-');
            var left = data.Substring(0, index);
            var right = data.Substring(index + 1);
            if (right == "")
            {
                right = DateTime.Now.Year.ToString();
                return $"{left}-{right}";
            }
            return data;
        }

    }
}
