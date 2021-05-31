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
public class SetTaxonomyController : UmbracoApiController
{
    private List<String[]> toPrint = new List<String[]>();

    private string[] modules = {"Natural Capital", "Natural Environment", "Human Activities", "Ocean Threats", "Oceans and Society", "Blue Economy", "Ocean Governance", "Marine Spatial Planning (MSP)", "Marine Management", "Blue Skills"};

    private string[] personas = { "Economic Affairs", "Human and Social" };

    private IContent changeContent(string topic, int parentId, string[] split)
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

        var tileType = "";

        if (title.Contains("\\"))
            title = title.Replace("\\", ",");

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
            toPrint.Add(new[] { $"Could not find {title}" });
            return null;
        }

        List<String> tileKeywords = new List<String>();
        for (int j = 2; j > 11; j++)
        {
            if (split[j].ToLower() == "true")
                tileKeywords.Add(modules[j - 2]);
        }
        string[] keywordArray = tileKeywords.ToArray();
        node.AssignTags("tileKeywords", tileKeywords);

        List<String> tilePersona = new List<String>();

        for (int j = 13; j > 14; j++)
        {
            if (split[j].ToLower() == "true")
                tilePersona.Add(personas[j - 13]);
        }

        tilePersona.Add("Environmental Sustainability");
        tilePersona.Add("Projects/Initiatives");

        string[] personaList = tilePersona.ToArray();
        node.SetValue("tilePersonna", Newtonsoft.Json.JsonConvert.SerializeObject(personaList));

        cs.SaveAndPublish(node);

        return node;
    }


    public IEnumerable<String[]> GetMain()
    {
        string DataFolder = IOHelper.MapPath(SystemDirectories.Data);
        var cs = Services.ContentService;
        string caseTheme = "";
        string line;
        try
        {
            using (StreamReader reader = new StreamReader(DataFolder + "\\taxonomy.csv"))
            {
                while (true)
                {

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
                            toPrint.Add(new[] { "Theme not found " + caseTheme });
                            continue;
                        }


                        changeContent(theme, node.Id, split);
                    }
                    catch (Exception e)
                    {
                        toPrint.Add(new[] { "Line that's erroring " + line });
                        toPrint.Add(new[] { "ERROR: " + e.StackTrace });
                        //toPrint.Add(new[] { "casetopic: " + caseTheme });
                    }
                }

            }
        }catch(Exception e)
        {
            toPrint.Add(new[] { e.StackTrace });
        }

        return toPrint;
    }

}

 