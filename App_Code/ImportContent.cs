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
        link = split[2];
        type = split[3];

        var tileType = "";

        if (type.ToLower() == "website" || type == "" || type == null)
        {
            tileType = "External Link";
        }
        else
        {
            tileType = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(type.ToLower());
        }

        var filter = SqlContext.Query<IContent>().Where(x => /*x.ContentTypeId == /*ID*/ /*1085 &&*/ x.Name == title);
        var children = cs.GetPagedChildren(id: parentId, pageIndex: 0, pageSize: int.MaxValue, totalRecords: out totalChildren, filter: filter);

        IContent node;

        if (title.Contains("\\"))
            title = title.Replace("\\", ",");

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

        var originationValue = Newtonsoft.Json.JsonConvert.SerializeObject(new[] { (string)"External" });
        var typeValue = Newtonsoft.Json.JsonConvert.SerializeObject(new[] { (string)tileType });

        node.SetValue("TileType", typeValue);
        node.SetValue("TileContentOrigination", originationValue);

        node.SetValue("ContentTitle", title);

        node.SetValue("TileExternalUrl", link);

        cs.SaveAndPublish(node);

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

                    var node = Umbraco.ContentSingleAtXPath("//dtListPage[@nodeName='" + caseTheme + "']");


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

                    
                    createContent(theme, Umbraco.ContentSingleAtXPath("//dtListPage[@nodeName='" + caseTheme + "']").Id, split);
                }
                catch (Exception e)
                {
                    toPrint.Add(new[] { "ERROR: " + e.StackTrace });
                    toPrint.Add(new[] { "casetopic: " + caseTheme });
                }
            }

        }

        return toPrint;
    }

}