using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.IO;
using Umbraco.Web.WebApi;
using System.IO;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web.PublishedModels;
using Umbraco.Core.Models.PublishedContent;
using System.Web.Http.Controllers;
using System.Net.Http.Formatting;
using Umbraco.Core.Services;
using System.Net;


/// <summary>
/// Summary description for ImportContent
/// </summary>
public class ImportContentController : UmbracoApiController
{
    private List<String[]> toPrint = new List<String[]>();

    private IContent createContent(string topic, int parentId, string[] split)
    {
        var cs = Services.ContentService;
        long totalChildren;

        var homeNode = Umbraco.ContentAtRoot().First();

        var theme = ""; // Rethink
        var title = "";
        var link = "";
        var type = "";

        theme = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(topic.ToLower());

        title = split[1];
        link = split[4];
        type = split[2];

        var tileType = "";

        if (title.Contains("\\"))
            title = title.Replace("\\", ",");

        if (type.ToLower() == "website" || type.ToLower() == "web link" || type == "" || type == null)
        {
            tileType = "External Link";
        }
        else if (type.ToLower() == "paper")
        {
            tileType = "Academic Paper";
        }
        else
        {
            tileType = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(type.ToLower());
        }

        var filter = SqlContext.Query<IContent>().Where(x => /*x.ContentTypeId == /*ID*/ /*1085 &&*/ x.Name == title);
        var children = cs.GetPagedChildren(id: parentId, pageIndex: 0, pageSize: int.MaxValue, totalRecords: out totalChildren, filter: filter);

        IContent node;

        if (children.Any())
        {
            node = children.First();
            toPrint.Add(new[] { $"found {node.Name}" });
        }
        else
        {
            node = cs.Create(title, parentId, DtContentTile.ModelTypeAlias);
            toPrint.Add(new[] { $"created {node.Name}" });
        }

        node.SetValue("MetaDescription", title);
        node.SetValue("MetaTitle", title);


        string[] metaKeywords = new string[] { title };
        node.AssignTags("MetaKeywords", metaKeywords);

        string[] tileTopics = new string[] { topic };
        node.AssignTags("TileTopics", tileTopics);


        if (tileType.ToLower() == "video")
        {
            toPrint.Add(new[] { "adding: " + link });
            if (link.Contains("youtube") || link.Contains("vimeo") || link.Contains("youtu.be") || (link.Contains("ted.com") && !link.Contains("ed.ted.com")))
            {
                toPrint.Add(new[] { "VIDEO" });
                String rt;
                var uri = UmbracoContext.HttpContext.Request.Url;
                
                WebRequest request = WebRequest.Create($"{uri.Scheme}://{uri.Authority}/umbraco/Api/UnristrictedRteEmbed/GetEmbed?height=240&url={link}&width=360");

                ((System.Net.HttpWebRequest)request).UserAgent =
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)";

                WebResponse response = request.GetResponse();

                Stream dataStream = response.GetResponseStream();

                StreamReader reader = new StreamReader(dataStream);

                rt = reader.ReadToEnd();

                toPrint.Add(new[] { rt });

                var newRt = rt.Replace("\\\"", "'");

                toPrint.Add(new[] { newRt });

                var jsonised = Newtonsoft.Json.JsonConvert.DeserializeObject<TWS.ThinkBlue.Core.MarkupFinder>(newRt);

                var json = $"[{{\"url\": \"{link}\",\"width\": 360,\"height\": 240,\"preview\": \"{jsonised.Markup.ToString()/*.Replace("'", "\\\"")*/}\"}}]";

                toPrint.Add(new[] { json.ToString() });

                node.SetValue("TileExternalVideo", json);

            }
            else
            {

            }
        }

        var originationValue = Newtonsoft.Json.JsonConvert.SerializeObject(new[] { (string)"External" });
        var typeValue = Newtonsoft.Json.JsonConvert.SerializeObject(new[] { (string)tileType });

        #region setGridContent


        string gridContent;
        gridContent = split[3].TrimStart(' ', '"').TrimEnd('"').Trim().Replace("\\", ",");

        toPrint.Add(new[] { $"STRING TO ADD {gridContent}" });
        //node.SetValue("contentGrid", @"{ ""name"": ""1 column layout"", ""sections"": [ { ""grid"": 12, ""rows"": [ { ""name"": ""Full Row"", ""areas"": [ { ""grid"": 12, ""allowAll"": false, ""allowed"": [ ""rte"", ""headline"", ""quote"" ], ""hasConfig"": false, ""controls"": [ { ""value"": """ + gridContent + @""", ""editor"": { ""name"": ""Headline"", ""alias"": ""headline"", ""view"": ""textstring"", ""render"": null, ""icon"": ""icon - coin"", ""config"": { ""style"": ""font - size: 36px; line - height: 45px; font - weight: bold"", ""markup"": "" < h1 >#value#</h1>"" } } } ] } ], ""hasConfig"": false, ""id"": ""894c2705-70c9-4674-a30d-36a07e8c006c"" } ] } ] }");

        node.SetValue("contentGrid", @"{ ""name"": ""1 column layout"", ""sections"": [ { ""grid"": 12, ""rows"": [ { ""name"": ""Full Row"", ""areas"": [ { ""grid"": 12, ""allowAll"": false, ""allowed"": [ ""rte"", ""headline"", ""quote"" ], ""hasConfig"": false, ""controls"": [ { ""value"": "" " + gridContent + @""", ""editor"": { ""name"": ""Rich text editor"", ""alias"": ""rte"", ""view"": ""rte"", ""render"": null, ""icon"": ""icon - article"", ""config"": {} } } ] } ], ""label"": ""Modal Content Editorial"", ""hasConfig"": false, ""id"": ""a120f627-e08d-850d-5497-2b307e17310e"" } ] } ] }");


        #endregion

        node.SetValue("TileType", typeValue);
        node.SetValue("TileContentOrigination", originationValue);

        node.SetValue("ContentTitle", ""); // repurposed for CTA

        node.SetValue("TileExternalUrl", link);

        if (!String.IsNullOrWhiteSpace(gridContent))
        {
            cs.SaveAndPublish(node);
        }
        else
        {
            cs.Save(node);
        }

        return node;
    }


    public IEnumerable<String[]> GetMain()
    {
        string DataFolder = IOHelper.MapPath(SystemDirectories.Data);
        var cs = Services.ContentService;
        string caseTheme = "";
        string line;
        using (StreamReader reader = new StreamReader(DataFolder + "\\content.csv"))
        {
            while (true)
            {
                // s

                line = reader.ReadLine();
                if (line == null || line == "")
                {
                    break;
                }
                else if (line.StartsWith(";"))
                {
                    continue;
                }
                try
                {
                    String[] split = line.Split(',');

                    string theme = split[0];

                    caseTheme = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(theme);

                    long totalChildren = 0;

                    var filter = SqlContext.Query<IContent>().Where(x => /*x.ContentTypeId == /*ID*/ /*1085 &&*/ x.Name == caseTheme);
                    var children = cs.GetPagedChildren(id: cs.GetById(new Guid("702f7185-50cb-49ff-9df0-b123d93b2874")).Id, pageIndex: 0, pageSize: int.MaxValue, totalRecords: out totalChildren, filter: filter);


                    var node = children.FirstOrDefault();
                    //var node = Umbraco.ContentSingleAtXPath("//dtListPage[@nodeName='" + caseTheme + "']");


                    if (node == null)
                    {
                        var newParent = cs.Create(caseTheme, Umbraco.ContentSingleAtXPath("//dtListPage[@nodeName='Tiles']").Id
                            , DtListPage.ModelTypeAlias);

                        newParent.SetValue("HideFromNavigation", true);
                        newParent.SetValue("HideFromSearch", true);

                        newParent.SetValue("MetaTitle", caseTheme);
                        newParent.SetValue("MetaDescription", caseTheme);

                        string[] keywords = new string[] { caseTheme };
                        newParent.AssignTags("MetaKeywords", keywords);

                        cs.SaveAndPublish(newParent);

                    }


                    createContent(theme, node.Id, split);
                    //createContent(theme, Umbraco.ContentSingleAtXPath("//dtListPage[@nodeName='" + caseTheme + "']").Id, split);
                }
                catch (Exception e)
                {
                    toPrint.Add(new[] { "Line that's erroring " + line });
                    toPrint.Add(new[] { "ERROR: " + e.StackTrace });
                    //toPrint.Add(new[] { "casetopic: " + caseTheme });
                }
            }

        }

        return toPrint;
    }

}

public class DeleteContentController : UmbracoApiController
{
    private List<String[]> toPrint = new List<String[]>();
    public IEnumerable<String[]> GetDelete(string a)
    {
        //702f7185-50cb-49ff-9df0-b123d93b2874
        var tilesParent = Umbraco.Content(a);

        if (tilesParent != null)
        {
            long totalChildren = 0;
            var cs = Services.ContentService;
            var ct = Services.ContentTypeService;
            var dtContentTileId = ct.Get("dtContentTile").Id;

            var filter = SqlContext.Query<IContent>().Where(x => x.ContentTypeId == dtContentTileId);
            var tiles = cs.GetPagedDescendants(id: cs.GetById(new Guid(a)).Id, pageIndex: 0, pageSize: int.MaxValue, totalRecords: out totalChildren, filter: filter);

            foreach (var item in tiles.Where(x => x.GetValue<string>("tileContentOrigination") == "[\"External\"]"))
            {
                toPrint.Add(new[] { $"{item.ContentType.Name} -- {item.Name} :: {item.GetValue<string>("tileContentOrigination")}" });
                // remove without recycling
                cs.Delete(item);
            }
        }
        else
        {
            toPrint.Add(new[] { "guid incorrect" });
            var uri = UmbracoContext.HttpContext.Request.Url;
            toPrint.Add(new[] { $"{uri.Scheme}://{uri.Authority}" });

        }

        return toPrint;
    }
}