using MinecraftLaunch.Modules.Models.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace MinecraftLaunch.Modules.Utils {
    public class McNewsUtil {
        private readonly static string ImageBaseUrl = "https://www.minecraft.net";

        private readonly static string McVersionUpdateAPI = "https://www.minecraft.net/content/minecraft-net/_jcr_content.articles.grid?tileselection=auto&pageSize=50&tagsPath=minecraft:stockholm/minecraft";

        public static async ValueTask<List<ArticleJsonEntity>> GetMcVersionUpdatesAsync() {
            using var httpResponse = await HttpUtil.HttpSimulateBrowserGetAsync(McVersionUpdateAPI);

            using var stream = await httpResponse.Content.ReadAsStreamAsync();
            var json = StringUtil.ConvertGzipStreamToString(stream);
            var model = json.ToJsonEntity<McVersionUpdateJsonEntity>();

            return model.Articles;
        }

        public static async ValueTask<ArticleJsonEntity> GetNewImageUrlAsync(ArticleJsonEntity article) {
            using var httpResponse = await HttpUtil.HttpSimulateBrowserGetAsync($"{ImageBaseUrl}{article.NewsUrl}");
            using var stream = await httpResponse.Content.ReadAsStreamAsync();

            var htmlStrs = StringUtil.ConvertGzipStreamToList(stream);
            foreach (var item in htmlStrs.AsParallel()) {
                if (item.Contains("og:image")) {
                    article.ImageUrl = StringUtil.GetPropertyFromHtmlText(item, "meta", "content");
                }
            }

            return article;
        }
    }
}
