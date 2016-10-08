using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;

namespace GitHubTracker
{
    [Export(typeof(IVersionControlClassifier))]
    internal class FullGitHubClassifier : IVersionControlClassifier
    {
        private static readonly Regex s_regex = new Regex(@"GitHub\W+(\w+)/(\w+)\W+(\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly IGitHubClient _client;

        [ImportingConstructor]
        public FullGitHubClassifier(IGitHubClient client)
        {
            _client = client;
        }

        public IEnumerable<ITagSpan<IVersionControlTag>> GetTags(string text, SnapshotSpan snapShot)
        {
            var matches = s_regex.Matches(text);

            if (matches.Count == 0)
            {
                yield break;
            }

            foreach (Match match in matches)
            {
                yield return new TagSpan<GitHubTag>(
                    new SnapshotSpan(snapShot.Start + match.Index, match.Value.Length),
                    new GitHubTag(match.Groups[1].Value, match.Groups[2].Value, Convert.ToInt32(match.Groups[3].Value), _client));
            }
        }
    }
}