﻿using Newtonsoft.Json;
using Reddit.NET.Exceptions;
using Reddit.NET.Controllers.EventArgs;
using Reddit.NET.Controllers.Structures;
using RedditThings = Reddit.NET.Models.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace Reddit.NET.Controllers.Internal
{
    public class Validators
    {
        public Exception BuildException(Exception ex, List<List<string>> errors)
        {
            ex.Data.Add("errors", errors);

            return ex;
        }

        protected void CheckErrors(List<List<string>> errors)
        {
            if (errors != null)
            {
                foreach (List<string> errorList in errors)
                {
                    CheckErrors(errorList);
                }
            }
        }

        // Exception thrown will be on the first error in the list.  --Kris
        protected void CheckErrors(List<string> errors)
        {
            if (errors != null)
            {
                foreach (string error in errors)
                {
                    switch (error)
                    {
                        default:
                            throw (RedditControllerException)BuildException(new RedditControllerException("Reddit API returned errors."), new List<List<string>> { errors });
                        case "RATELIMIT":
                            throw (RedditRateLimitException)BuildException(new RedditRateLimitException("Reddit ratelimit exceeded."), new List<List<string>> { errors });
                        case "SUBREDDIT_EXISTS":
                            throw (RedditSubredditExistsException)BuildException(new RedditSubredditExistsException("That subreddit already exists."),
                                new List<List<string>> { errors });
                        case "INVALID_PERMISSION_TYPE":
                            throw (RedditInvalidPermissionTypeException)BuildException(new RedditInvalidPermissionTypeException(errors[1]),
                                new List<List<string>> { errors });
                    }
                }
            }
        }

        protected void CheckNull(object res, string msg = "Reddit API returned null response.")
        {
            if (res == null)
            {
                throw new RedditControllerException(msg);
            }
        }

        public dynamic Validate(dynamic obj)
        {
            CheckNull(obj);

            return obj;
        }

        public RedditThings.GenericContainer Validate(RedditThings.GenericContainer genericContainer)
        {
            CheckNull(genericContainer);

            Validate(genericContainer.JSON);

            return genericContainer;
        }

        public RedditThings.Generic Validate(RedditThings.Generic generic)
        {
            CheckNull(generic, "Reddit API returned empty response container.");

            CheckErrors(generic.Errors);

            return generic;
        }

        public RedditThings.DynamicShortListingContainer Validate(RedditThings.DynamicShortListingContainer dynamicShortListingContainer)
        {
            CheckNull(dynamicShortListingContainer);

            Validate(dynamicShortListingContainer.Data);

            return dynamicShortListingContainer;
        }

        public RedditThings.DynamicShortListingData Validate(RedditThings.DynamicShortListingData dynamicShortListingData)
        {
            CheckNull(dynamicShortListingData, "Reddit API returned empty response container.");

            return dynamicShortListingData;
        }

        public RedditThings.ImageUploadResult Validate(RedditThings.ImageUploadResult imageUploadResult)
        {
            CheckNull(imageUploadResult);

            CheckErrors(imageUploadResult.Errors);

            return imageUploadResult;
        }

        public RedditThings.LiveUpdateEventContainer Validate(RedditThings.LiveUpdateEventContainer liveUpdateEventContainer)
        {
            CheckNull(liveUpdateEventContainer);
            CheckNull(liveUpdateEventContainer.Data);

            return liveUpdateEventContainer;
        }

        public RedditThings.LiveThreadCreateResultContainer Validate(RedditThings.LiveThreadCreateResultContainer liveThreadCreateResultContainer)
        {
            CheckNull(liveThreadCreateResultContainer);
            CheckNull(liveThreadCreateResultContainer.JSON);
            CheckErrors(liveThreadCreateResultContainer.JSON.Errors);
            CheckNull(liveThreadCreateResultContainer.JSON.Data);
            CheckNull(liveThreadCreateResultContainer.JSON.Data.Id);

            return liveThreadCreateResultContainer;
        }

        public RedditThings.LiveUpdateContainer Validate(RedditThings.LiveUpdateContainer liveUpdateContainer, int? minChildren = null)
        {
            CheckNull(liveUpdateContainer);
            CheckNull(liveUpdateContainer.Data);
            if (minChildren.HasValue)
            {
                CheckNull(liveUpdateContainer.Data.Children);
                if (liveUpdateContainer.Data.Children.Count < minChildren.Value)
                {
                    throw new RedditControllerException("Expected number of results not returned.");
                }
            }

            return liveUpdateContainer;
        }

        public RedditThings.SubredditSettingsContainer Validate(RedditThings.SubredditSettingsContainer subredditSettingsContainer)
        {
            CheckNull(subredditSettingsContainer);

            Validate(subredditSettingsContainer.Data);

            return subredditSettingsContainer;
        }

        public RedditThings.SubredditSettings Validate(RedditThings.SubredditSettings subredditSettings)
        {
            CheckNull(subredditSettings, "Reddit API returned empty response container.");

            return subredditSettings;
        }

        public List<RedditThings.ActionResult> Validate(List<RedditThings.ActionResult> actionResults)
        {
            CheckNull(actionResults);

            foreach (RedditThings.ActionResult actionResult in actionResults)
            {
                Validate(actionResult);
            }

            return actionResults;
        }

        public RedditThings.ActionResult Validate(RedditThings.ActionResult actionResult)
        {
            CheckNull(actionResult);

            if (!actionResult.Ok)
            {
                RedditControllerException ex = new RedditControllerException("Reddit API returned non-Ok response.");

                ex.Data.Add("actionResult", actionResult);

                throw ex;
            }

            return actionResult;
        }

        public RedditThings.FlairListResultContainer Validate(RedditThings.FlairListResultContainer flairListResultContainer)
        {
            CheckNull(flairListResultContainer);
            CheckNull(flairListResultContainer.Users, "Reddit API returned empty response container.");

            foreach (RedditThings.FlairListResult flairListResult in flairListResultContainer.Users)
            {
                Validate(flairListResult);
            }

            return flairListResultContainer;
        }

        public RedditThings.Flair Validate(RedditThings.Flair flair)
        {
            CheckNull(flair);
            CheckNull(flair.Id, "Reddit API returned flair object with no Id.");

            return flair;
        }

        public RedditThings.FlairV2 Validate(RedditThings.FlairV2 flairV2)
        {
            CheckNull(flairV2);
            CheckNull(flairV2.Id, "Reddit API returned flair object with no Id.");

            return flairV2;
        }

        public RedditThings.ModActionContainer Validate(RedditThings.ModActionContainer modActionContainer)
        {
            CheckNull(modActionContainer);

            Validate(modActionContainer.Data);

            return modActionContainer;
        }

        public RedditThings.ModActionData Validate(RedditThings.ModActionData modActionData)
        {
            CheckNull(modActionData, "Reddit API returned empty response object.");
            CheckNull(modActionData.Children, "Reddit API returned response with null children.");

            return modActionData;
        }

        public RedditThings.WikiPageRevisionContainer Validate(RedditThings.WikiPageRevisionContainer wikiPageRevisionContainer)
        {
            CheckNull(wikiPageRevisionContainer);
            CheckNull(wikiPageRevisionContainer.Data, "Reddit API returned empty response object.");

            return wikiPageRevisionContainer;
        }

        public RedditThings.WikiPageRevisionData Validate(RedditThings.WikiPageRevisionData wikiPageRevisionData)
        {
            CheckNull(wikiPageRevisionData);

            return wikiPageRevisionData;
        }

        public RedditThings.WikiPageSettingsContainer Validate(RedditThings.WikiPageSettingsContainer wikiPageSettingsContainer)
        {
            CheckNull(wikiPageSettingsContainer);
            CheckNull(wikiPageSettingsContainer.Data, "Reddit API returned empty response object.");

            return wikiPageSettingsContainer;
        }

        public RedditThings.WikiPageSettings Validate(RedditThings.WikiPageSettings wikiPageSettings)
        {
            CheckNull(wikiPageSettings);

            return wikiPageSettings;
        }

        public List<RedditThings.UserPrefsContainer> Validate(List<RedditThings.UserPrefsContainer> userPrefsContainers)
        {
            CheckNull(userPrefsContainers);

            foreach (RedditThings.UserPrefsContainer userPrefsContainer in userPrefsContainers)
            {
                CheckNull(userPrefsContainer, "Reddit API returned a list with at least one null entry.");
                CheckNull(userPrefsContainer.Data, "Reddit API returned a list with at least one entry that contains null data.");
            }

            return userPrefsContainers;
        }

        public RedditThings.UserPrefsContainer Validate(RedditThings.UserPrefsContainer userPrefsContainer)
        {
            CheckNull(userPrefsContainer);
            CheckNull(userPrefsContainer.Data, "Reddit API returned empty response object.");

            return userPrefsContainer;
        }

        public RedditThings.UserPrefsData Validate(RedditThings.UserPrefsData userPrefsData)
        {
            CheckNull(userPrefsData);

            return userPrefsData;
        }

        public RedditThings.PostResultShortContainer Validate(RedditThings.PostResultShortContainer postResultShortContainer)
        {
            CheckNull(postResultShortContainer);
            CheckNull(postResultShortContainer.JSON, "Reddit API returned an empty response object.");
            CheckErrors(postResultShortContainer.JSON.Errors);
            CheckNull(postResultShortContainer.JSON.Data, "Reddit API returned a response object with null data.");

            return postResultShortContainer;
        }

        public RedditThings.PostResultShort Validate(RedditThings.PostResultShort postResultShort)
        {
            CheckNull(postResultShort);
            CheckErrors(postResultShort.Errors);
            CheckNull(postResultShort.Data, "Reddit API returned an empty response object.");

            return postResultShort;
        }

        public RedditThings.PostResultContainer Validate(RedditThings.PostResultContainer postResultContainer)
        {
            CheckNull(postResultContainer);
            CheckNull(postResultContainer.JSON, "Reddit API returned an empty response object.");
            CheckErrors(postResultContainer.JSON.Errors);
            CheckNull(postResultContainer.JSON.Data, "Reddit API returned a response object with null data.");
            CheckNull(postResultContainer.JSON.Data.Things, "Reddit API returned a response object with empty data.");

            if (postResultContainer.JSON.Data.Things.Count == 0)
            {
                throw new RedditControllerException("Reddit API returned a PostResultContainer with an empty result list.");
            }

            return postResultContainer;
        }

        public RedditThings.PostResult Validate(RedditThings.PostResult postResult)
        {
            CheckNull(postResult);
            CheckErrors(postResult.Errors);
            CheckNull(postResult.Data, "Reddit API returned an empty response object.");
            CheckNull(postResult.Data.Things, "Reddit API returned a response object with empty data.");

            if (postResult.Data.Things.Count == 0)
            {
                throw new RedditControllerException("Reddit API returned a PostResult with an empty result list.");
            }

            return postResult;
        }

        public RedditThings.JQueryReturn Validate(RedditThings.JQueryReturn jQueryReturn)
        {
            CheckNull(jQueryReturn);

            if (!jQueryReturn.Success)
            {
                throw new RedditControllerException("Reddit API returned a non-success response.");
            }

            return jQueryReturn;
        }

        public List<(RedditThings.PostContainer, RedditThings.CommentContainer)> Validate(List<(RedditThings.PostContainer, RedditThings.CommentContainer)> ps)
        {
            CheckNull(ps);

            if (ps.Count == 0)
            {
                throw new RedditControllerException("Empty list returned.");
            }

            CheckNull(ps[0].Item1);
            CheckNull(ps[0].Item2);

            return ps;
        }

        public RedditThings.CommentResultContainer Validate(RedditThings.CommentResultContainer commentResultContainer)
        {
            CheckNull(commentResultContainer);
            CheckNull(commentResultContainer.JSON, "Reddit API returned empty response object.");

            CheckErrors(commentResultContainer.JSON.Errors);

            CheckNull(commentResultContainer.JSON.Data, "Reddit API returned response object with empty JSON.");
            CheckNull(commentResultContainer.JSON.Data.Things, "Reddit API returned response object with empty data.");

            if (commentResultContainer.JSON.Data.Things.Count == 0)
            {
                throw new RedditControllerException("JSON data contains empty comments list.");
            }

            CheckNull(commentResultContainer.JSON.Data.Things[0].Data, "Reddit API returned response object with null comment data.");

            return commentResultContainer;
        }

        public RedditThings.SubredditContainer Validate(RedditThings.SubredditContainer subredditContainer)
        {
            CheckNull(subredditContainer);
            CheckNull(subredditContainer.Data, "Reddit API returned empty response object.");
            CheckNull(subredditContainer.Data.Children, "Reddit API returned a response object with null children.");

            return subredditContainer;
        }
    }
}
