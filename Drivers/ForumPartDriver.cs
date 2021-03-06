﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NGM.Forum.Models;
using NGM.Forum.Settings;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;

namespace NGM.Forum.Drivers {
    [UsedImplicitly]
    public class ForumPartDriver : ContentPartDriver<ForumPart> {
        protected override string Prefix {
            get { return "ForumPart"; }
        }

        protected override DriverResult Display(ForumPart part, string displayType, dynamic shapeHelper) {
            List<DriverResult> results = new List<DriverResult>();

            if (displayType.Equals("SummaryAdmin", StringComparison.OrdinalIgnoreCase)) {
                results.Add(ContentShape("Parts_Forums_Forum_SummaryAdmin", () => shapeHelper.Parts_Forums_Forum_SummaryAdmin()));
            }

            results.AddRange(new [] { 
                ContentShape("Parts_Forums_Forum_Manage",
                    () => shapeHelper.Parts_Forums_Forum_Manage()),
                ContentShape("Parts_Forums_Forum_Description",
                    () => shapeHelper.Parts_Forums_Forum_Description(Description: part.Description)),
                ContentShape("Parts_Forums_Forum_ForumReplyCount",
                    () => shapeHelper.Parts_Forums_Forum_ForumReplyCount(ReplyCount: part.ReplyCount)),
                ContentShape("Parts_Forums_Forum_ForumThreadCount",
                    () => shapeHelper.Parts_Forums_Forum_ForumThreadCount(ThreadCount: part.ThreadCount)),
                ContentShape("Parts_Forum_Manage",
                    () => shapeHelper.Parts_Forum_Manage())
            });

            return Combined(results.ToArray());
        }

        protected override DriverResult Editor(ForumPart forumPart, dynamic shapeHelper) {

            var results = new List<DriverResult> {
                ContentShape("Parts_Forums_Forum_Fields", () => {
                    if (!forumPart.ContentItem.HasDraft() && !forumPart.ContentItem.HasPublished()) {
                        var settings = forumPart.TypePartDefinition.Settings.GetModel<ForumPartSettings>();
                        forumPart.ThreadedPosts = settings.DefaultThreadedPosts;
                    }
                    return shapeHelper.EditorTemplate(TemplateName: "Parts.Forums.Forum.Fields", Model: forumPart, Prefix: Prefix);
                })
            };


            if (forumPart.Id > 0)
                results.Add(ContentShape("Forum_DeleteButton",
                    deleteButton => deleteButton));

            return Combined(results.ToArray());
        }

        protected override DriverResult Editor(ForumPart forumPart, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(forumPart, Prefix, null, null);
            return Editor(forumPart, shapeHelper);
        }

        protected override void Importing(ForumPart part, ImportContentContext context) {
            var description = context.Attribute(part.PartDefinition.Name, "Description");
            if (description != null) {
                part.Description = description;
            }

            var threadCount = context.Attribute(part.PartDefinition.Name, "ThreadCount");
            if (threadCount != null) {
                part.ThreadCount = Convert.ToInt32(threadCount);
            }

            var postCount = context.Attribute(part.PartDefinition.Name, "PostCount");
            if (postCount != null) {
                part.PostCount = Convert.ToInt32(postCount);
            }

            var threadedPosts = context.Attribute(part.PartDefinition.Name, "ThreadedPosts");
            if (threadedPosts != null) {
                part.ThreadedPosts = Convert.ToBoolean(threadedPosts);
            }

            var weight = context.Attribute(part.PartDefinition.Name, "Weight");
            if (weight != null) {
                part.Weight = Convert.ToInt32(weight);
            }
        }

        protected override void Exporting(ForumPart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Description", part.Description);
            context.Element(part.PartDefinition.Name).SetAttributeValue("ThreadCount", part.ThreadCount);
            context.Element(part.PartDefinition.Name).SetAttributeValue("PostCount", part.PostCount);
            context.Element(part.PartDefinition.Name).SetAttributeValue("ThreadedPosts", part.ThreadedPosts);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Weight", part.Weight);
        }
    }
}