using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Data.Fields;
using Sitecore.Pipelines.InsertRenderings.Processors;
using Sitecore.Pipelines.InsertRenderings;
using Sitecore.Web.UI;
using System.Collections.Specialized;
using System.Web.UI;
using Sitecore.Reflection;
using Sitecore.Web.UI.WebControls;
using Sitecore.Data;
using Sitecore.Events;
using Sitecore.SecurityModel;
using Sitecore.Pipelines;
using Sitecore.Pipelines.Save;
using Sitecore.Xml;
using Sitecore.Publishing.Pipelines.Publish;
namespace Sitecore.Support.Pipelines.Save

{

    public class ReplaceSpecSymbols
    {
        public void Process(SaveArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.IsNotNull(args.Items, "args.Items");
            SaveArgs.SaveItem[] items = args.Items;
            for (int i = 0; i < items.Length; i++)
            {
                SaveArgs.SaveItem saveItem = items[i];
                Item item = Client.ContentDatabase.Items[saveItem.ID, saveItem.Language, saveItem.Version];
                if (item != null)
                {
                    SaveArgs.SaveField[] fields = saveItem.Fields;
                    for (int j = 0; j < fields.Length; j++)
                    {
                        SaveArgs.SaveField saveField = fields[j];
                        Field field = item.Fields[saveField.ID];
                        if (!field.IsBlobField && field.Type == "Layout")
                        {
                           
                            if (!string.IsNullOrEmpty(saveField.Value))
                            {
                                if (saveField.Value.Contains("?"))
                                {
                                    saveField.Value = saveField.Value.Replace("?", "%3F");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
namespace Sitecore.Support
{
    public class SymbolsReplacer
    {
        protected void OnPublishBegin(object sender, System.EventArgs args)
        {
            Item rootItem = Database.GetDatabase("master").GetRootItem();
            if (rootItem == null) return;
            ChangeItemsRecursive(rootItem.Children["Content"]);
            
           
        }
        private void ChangeItemsRecursive(Item root)
        {
            Field finRenderings = root.Fields["__Final Renderings"];
            if (finRenderings != null || !String.IsNullOrEmpty(finRenderings.Value))
            {
                if (finRenderings.Value.Contains("?"))
                {
                    root.Editing.BeginEdit();
                    root.Fields["__Final Renderings"].SetValue(finRenderings.Value.Replace("?", "%3F"), true);
                    root.Editing.EndEdit(false);
                }
            }
            if (root.Children == null || root.Children.Count == 0) return;
            foreach (Item child in root.Children)
            {
                ChangeItemsRecursive(child);
            }
        }
    }

    
}
