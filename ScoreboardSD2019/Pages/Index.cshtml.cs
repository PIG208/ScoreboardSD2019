using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ScoreboardSD2019.Pages
{
    public class IndexModel : PageModel
    {
        private IConfiguration Configuration { get; set; }
        public IndexModel(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        [BindProperty]
        public List<Singer> Singers { get; set; } = new List<Singer>();
        public void OnGet()
        {
            MySqlIntegration sqlInteg = new MySqlIntegration(Configuration.GetConnectionString("sql"));
            try
            {
                sqlInteg.MySqlSelect("singers", new[] { "*" });
            }
            catch (MySqlException ex)
            {
                ViewData["error"] = JsonConvert.SerializeObject(ex.Message);
            }
            if(sqlInteg.IntegratedResult != null && sqlInteg.IntegratedResult.Count > 0)
            {
                sqlInteg.IntegratedResult.ForEach(element => {
                    Singer singer = null;
                    try
                    {
                        singer = new Singer(element);
                    }
                    catch(NullReferenceException ex)
                    {
                        throw (ex);
                    }
                    if (singer != null)
                        Singers.Add(singer);
                });
            }
        }
        public ActionResult OnGetSingers()
        {
            MySqlIntegration sqlInteg = new MySqlIntegration(Configuration.GetConnectionString("sql"));
            try
            {
                sqlInteg.MySqlSelect("singers", new[] { "*" });
                if(sqlInteg.IntegratedResult.Count > 0)
                {
                    Singers = new List<Singer>();
                    sqlInteg.IntegratedResult.ForEach(element => {
                        Singer singer = null;
                        try
                        {
                            singer = new Singer(element);
                        }
                        catch (NullReferenceException ex)
                        {
                            throw (ex);
                        }
                        if (singer != null)
                            Singers.Add(singer);
                    });
                    return new JsonResult(JsonConvert.SerializeObject(Singers));
                }
            }
            catch(MySqlException ex)
            {
                JsonThrow(ex);
            }
            return new JsonResult("[]");

        }
        public ActionResult OnPostAlterSinger(string StringID, string StringScore, int Round)
        {
            //验证
            if ((StringID != null && int.TryParse(StringID, out int ID)) && (StringScore != null && float.TryParse(StringScore, out float Score)) && Round != -1)
            {
                string[] keys = { string.Format("score{0}", Round.ToString()) };
                string specifier = string.Format("`ID`={0}", ID);
                MySqlIntegration sqlInteg = new MySqlIntegration(Configuration.GetConnectionString("sql"));
                try
                {
                    sqlInteg.MySqlSelect("singers", new[] { "*" }, specifier);
                }
                catch (MySqlException ex)
                {
                    return JsonThrow(ex);
                }
                if(sqlInteg.IntegratedResult.Count > 0)
                {
                    try
                    {
                        sqlInteg.MySqlUpdate("singers", keys, specifier, Score.ToString());
                    }
                    catch (MySqlException ex)
                    {
                        return JsonThrow(ex);
                    }
                }
                return new JsonResult(sqlInteg.IntegratedResult.Count);
            }
            //返回错误
            return Redirect("https://baidu.com");
        }
        private JsonResult JsonThrow(Exception ex)
        {
            return new JsonResult(JsonConvert.SerializeObject(new [] { ex.ToString() ,ex.Message, ex.StackTrace,ex.Source,ex.InnerException.ToString() }));
        }

        [Serializable]
        public class InvalidActionException : Exception
        {
            public InvalidActionException() { }
            public InvalidActionException(string message) : base(message) { }
            public InvalidActionException(string message, Exception inner) : base(message, inner) { }
            protected InvalidActionException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }
    }
}
